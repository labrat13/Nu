using System;
using System.Data.OleDb;
using System.IO;
using System.Globalization;
using System.Collections.Generic;
using System.Data;

namespace UAMX_2
{
    /// <summary>
    /// Представляет шаблонный класс доступа к локальной Access БД Хранилища
    /// Должен быть функционально расширен в производном классе.
    /// 25012020 - добавлены базовые функции адаптера БД из моей библиотеки классов.
    ///  Теперь этот класс реально содержит общий набор функций для Хранилищ.
    /// </summary>
    public class DbAdapterBase
    {
        #region Fields
        /// <summary>
        /// Имя файла базы данных
        /// </summary>
        public const string DatabaseFileName = "db.mdb";
        /// <summary>
        /// database connection string
        /// </summary>
        protected String m_connectionString;
        /// <summary>
        /// database connection
        /// </summary>
        protected OleDbConnection m_connection;
        /// <summary>
        /// Transaction for current connection
        /// </summary>
        protected OleDbTransaction m_transaction;
        /// <summary>
        /// Timeout value for DB command, in seconds
        /// </summary>
        protected int m_Timeout;
        /// <summary>
        /// Database is read-only
        /// </summary>
        protected bool m_ReadOnly;
        ///// <summary>
        ///// Константа название таблицы БД - для функций адаптера
        ///// </summary>
        //internal const string Table_Category = "CategoryTable";

        //все объекты команд сбрасываются в нуль при отключении соединения с БД
        //TODO: Новые команды внести в ClearCommands()
        /// <summary>
        /// Команда без параметров, используемая во множестве функций
        /// </summary>
        protected OleDbCommand m_cmdWithoutArguments;


        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        public DbAdapterBase()
        {
            m_connection = null;
            m_connectionString = String.Empty;
            m_Timeout = 60;
            m_transaction = null;
            m_ReadOnly = false;
            ClearCommands();
            return;
        }

        /// <summary>
        /// NT-Конструктор
        /// </summary>
        /// <param name="dbFilePath">Путь к файлу БД</param>
        /// <param name="readOnly">Флаг Открыть БД только для чтения</param>
        public DbAdapterBase(String dbFilePath, bool readOnly)
        {
            m_connectionString = createConnectionString(dbFilePath, readOnly);
            m_Timeout = 60;
            m_transaction = null;            
            m_connection = new OleDbConnection(m_connectionString);
            m_ReadOnly = readOnly;

            ClearCommands();
            return;
        }

        /// <summary>
        /// NT-Close and dispose connection
        /// </summary>
        ~DbAdapterBase()
        {
            this.Close();
        }

        #region Properties
        /// <summary>
        /// Database is read-only
        /// </summary>
        public bool ReadOnly
        {
            get { return m_ReadOnly; }
            //set { dbReadOnly = value; }
        }

        /// <summary>
        /// Get Set timeout value for all  new execute command
        /// </summary>
        public int Timeout
        {
            get
            {
                return m_Timeout;
            }
            set
            {
                m_Timeout = value;
            }
        }

        /// <summary>
        /// Get or Set connection string
        /// </summary>
        public string ConnectionString
        {
            get
            {
                return m_connectionString;
            }
            set
            {
                m_connectionString = value;
                this.m_connection = new OleDbConnection(m_connectionString);
            }
        }

        /// <summary>
        /// Is connection opened?
        /// </summary>
        public bool isConnectionActive
        {
            get { return ((this.m_connection != null) && (this.m_connection.State == ConnectionState.Open)); }
        }
        /// <summary>
        /// Is transaction active?
        /// </summary>
        public bool isTransactionActive
        {
            get { return (this.m_transaction != null); }
        }

        #endregion

        #region Service functions

        /// <summary>
        /// NT-все объекты команд класса сбросить в нуль
        /// </summary>
        protected virtual void ClearCommands()
        {
            m_cmdWithoutArguments = null;

            return;
        }

        /// <summary>
        /// NT-Создать соединение с БД по сохраненной строке соединения
        /// </summary>
        public virtual void Open()
        {
            //close existing connection
            this.Close();
            //open new connection and set as primary
            this.m_connection = new OleDbConnection(m_connectionString);
            this.m_connection.Open();

            return;
        }

        /// <summary>
        /// NT-Close connection if not closed
        /// </summary>
        /// <exception cref="SqlException">Error in connection</exception>
        /// <remarks>Без лога, или проверять его существование!</remarks>
        public virtual void Close()
        {
            if (m_connection != null)
            {
                if (m_connection.State != ConnectionState.Closed)
                    m_connection.Close();
                m_connection = null;
            }

            //все объекты команд сбросить в нуль при отключении соединения с БД, чтобы ссылка на объект соединения при следующем подключении не оказалась устаревшей
            ClearCommands();

            return;
        }

        /// <summary>
        /// NT-Создать строку соединения с БД
        /// </summary>
        /// <param name="dbFile">Путь к файлу БД</param>
        public static string createConnectionString(string dbFile, bool readOnly)
        {
            //Provider=Microsoft.Jet.OLEDB.4.0;Data Source="C:\Documents and Settings\salomatin\Мои документы\Visual Studio 2008\Projects\RadioBase\радиодетали.mdb"
            OleDbConnectionStringBuilder b = new OleDbConnectionStringBuilder();
            b.Provider = "Microsoft.Jet.OLEDB.4.0";
            b.DataSource = dbFile;
            //это только для БД на незаписываемых дисках
            if (readOnly)
            {
                b.Add("Mode", "Share Deny Write");
            }
            //user id and password can specify here
            return b.ConnectionString;
        }


        #endregion


        #region Transaction functions
        /// <summary>
        /// NT-Начать транзакцию. 
        /// </summary>
        public void TransactionBegin()
        {
            m_transaction = m_connection.BeginTransaction();
            //сбросить в нуль все объекты команд, чтобы они были пересозданы для новой транзакции
            ClearCommands();
        }
        /// <summary>
        /// NT-Подтвердить транзакцию Нужно закрыть соединение после этого!
        /// </summary>
        public void TransactionCommit()
        {
            m_transaction.Commit();
            //сбросить в нуль все объекты команд, чтобы они были пересозданы для новой транзакции
            ClearCommands();
            m_transaction = null;
        }
        /// <summary>
        /// NT-Отменить транзакцию. Нужно закрыть соединение после этого!
        /// </summary>
        public void TransactionRollback()
        {
            m_transaction.Rollback();
            //сбросить в нуль все объекты команд, чтобы они были пересозданы для новой транзакции
            ClearCommands();
            m_transaction = null;
        }

        #endregion


        #region Database functions

        /// <summary>
        /// NT-Исполнить командный запрос SQL
        /// Например, создать таблицу или индекс.
        /// </summary>
        /// <param name="query">Текст запроса</param>
        /// <param name="timeout">Таймаут команды в секундах</param>
        /// <returns></returns>
        public int ExecuteNonQuery(string query, int timeout)
        {
            OleDbCommand cmd = new OleDbCommand(query, this.m_connection, this.m_transaction);
            cmd.CommandTimeout = timeout;
            return cmd.ExecuteNonQuery();
        }

        /// <summary>
        /// NT- Исполнить запрос с целочисленным результатом.
        /// Например, MAX() или COUNT()
        /// </summary>
        /// <param name="query">Текст запроса</param>
        /// <param name="timeout">Таймаут команды в секундах</param>
        /// <returns>Возвращает результат - целое число, или -1 при ошибке.</returns>
        public int ExecuteScalar(string query, int timeout)
        {
            OleDbCommand cmd = new OleDbCommand(query, this.m_connection, this.m_transaction);
            cmd.CommandTimeout = timeout;
            Object ob = cmd.ExecuteScalar(); //Тут могут быть исключения из-за другого типа данных
            String s = ob.ToString();
            if (String.IsNullOrEmpty(s))
                return -1;
            else return Int32.Parse(s);
        }

        /// <summary>
        /// NT- собирает список ИД в строку для WHERE
        /// </summary>
        /// <param name="postIdList">Список ИД</param>
        /// <param name="columnName">Название столбца запроса</param>
        /// <returns></returns>
        /// <example>
        /// Это пример функции которая использует данную функцию для выборки нескольких строк за один запрос
        /// Такой прием ускоряет выборку множества строк из большой БД в 6..10 раз
        /// public List<PostsObj> getPostsByIds(List<int> idList)
        ///{
        ///    List<PostsObj> result = new List<PostsObj>();
        ///    int len = idList.Count;
        ///    if (len == 0) return result;//сразу выйти если входной список пустой
        ///    //1 разделить массив ид на порции по 8 элементов, если там есть столько
        ///    List<List<Int32>> lar = SplitListInt32(idList, 8);
        ///    //для каждой порции:
        ///    foreach (List<Int32> li in lar)
        ///    {
        ///        //2 вызвать функцию выборки из БД по массиву из 8 ид
        ///        //3 собрать все в один выходной список 
        ///        result.AddRange(this.getPostsByIds_sub(li));
        ///    }
        ///    return result;
        ///}
        /// </example>
        private string makeWhereText(List<int> postIdList, string columnName)
        {
            //returns (Id = 0) OR (Id = 1) OR (Id = 3)

            int cnt = postIdList.Count;
            String[] sar = new string[cnt];
            //
            for (int i = 0; i < cnt; i++)
                sar[i] = String.Format("({0} = {1})", columnName, postIdList[i]);
            //
            return String.Join(" OR ", sar);
        }

        /// <summary>
        /// RT-разделить список на части по N элементов или менее
        /// </summary>
        /// <param name="idList">Исходный список</param>
        /// <param name="n">Размер каждой из частей, больше 0</param>
        /// <returns>Возвращает список списков, каждый из которых содержит части входного списка.</returns>
        public static List<List<int>> SplitListInt32(List<int> idList, int n)
        {
            //проверка аргументов
            if (n <= 0)
                throw new ArgumentException("Argument N must be greather than 0!", "n");

            List<List<Int32>> result = new List<List<int>>();
            int cnt = idList.Count;
            if (cnt == 0) return result;
            //если там меньше N, то весь список возвращаем как единственный элемент 
            if (cnt <= n)
            {
                result.Add(idList);
                return result;
            }
            //иначе
            int c = cnt / n; //полных кусков по n элементов
            int cs = cnt % n; //остаточная длина куска
            //целые куски добавим
            for (int i = 0; i < c; i++)
                result.Add(idList.GetRange(i * n, n));
            //остаток
            if (cs > 0)
                result.Add(idList.GetRange(c * n, cs));

            return result;
        }
        /// <summary>
        /// NT-Извлечь файл шаблона базы данных из ресурсов сборки
        /// </summary>
        /// <remarks>
        /// Файл БД должен быть помещен в ресурсы сборки в VisualStudio2008.
        /// Окно Свойства проекта - вкладка Ресурсы - кнопка-комбо Добавить ресурс - Добавить существующий файл. Там выбрать файл БД.
        /// При этом он помещается также в дерево файлов проекта, и при компиляции берется оттуда и помещается в сборку как двоичный массив байт.
        /// После этого можно изменять этот файл проекта, изменения в ресурс сборки будут внесены после компиляции
        /// Эта функция извлекает файл БД в указанный путь файла.
        /// </remarks>
        /// <param name="filepath">Путь к итоговому файлу *.mdb</param>
        /// <param name="resourceStream">содержимое ресурса</param>
        /// <example>
        /// dbAdapter.extractDbFile("C:\\db.mdb", Properties.Resources.db);
        /// </example>
        public static void extractDbFile(string filepath, byte[] resourceBytes)
        {
            FileStream fs = new FileStream(filepath, FileMode.Create);
            fs.Write(resourceBytes, 0, resourceBytes.Length);
            fs.Close();

            return;
        }

        /// <summary>
        /// NT-Получить строку текста из ридера таблицы или пустую строку
        /// </summary>
        /// <param name="rdr">Объект ридера таблицы бд</param>
        /// <param name="p">Номер столбца в таблице и ридере</param>
        /// <returns>Возвращает строку текста из поля или пустую строку если в поле хранится значение DbNull</returns>
        public static string getDbString(OleDbDataReader rdr, int p)
        {
            if (rdr.IsDBNull(p))
                return String.Empty;
            else return rdr.GetString(p).Trim();
        }

        /// <summary>
        /// NT- Получить максимальное значение ИД для столбца таблицы
        /// Обычно применяется для столбца первичного ключа, но можно и для других целочисленных столбцов.
        /// </summary>
        /// <param name="table">Название таблицы</param>
        /// <param name="column">Название столбца первичного ключа</param>
        /// <returns>Returns max value or -1 if no results</returns>
        public int getTableMaxInt32(string table, string column)
        {
            //SELECT MAX(id) FROM table;
            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
                m_cmdWithoutArguments.CommandTimeout = m_Timeout;
            }
            //execute command
            string query = String.Format(CultureInfo.InvariantCulture, "SELECT MAX({0}) FROM {1};", column, table);
            m_cmdWithoutArguments.CommandText = query;
            Object ob = m_cmdWithoutArguments.ExecuteScalar(); //Тут могут быть исключения из-за другого типа данных
            String s = ob.ToString();
            if (String.IsNullOrEmpty(s))
                return -1;
            else return Int32.Parse(s);
        }

        /// <summary>
        /// NT-Получить минимальное значение ИД для столбца таблицы
        /// Обычно применяется для столбца первичного ключа, но можно и для других целочисленных столбцов.
        /// </summary>
        /// <param name="table">Название таблицы</param>
        /// <param name="column">Название столбца первичного ключа</param>
        /// <returns>Returns min value or -1 if no results</returns>
        public int getTableMinInt32(string table, string column)
        {
            //SELECT MIN(id) FROM table;
            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
                m_cmdWithoutArguments.CommandTimeout = m_Timeout;
            }
            //execute command
            string query = String.Format(CultureInfo.InvariantCulture, "SELECT MIN({0}) FROM {1};", column, table);
            m_cmdWithoutArguments.CommandText = query;
            Object ob = m_cmdWithoutArguments.ExecuteScalar(); //Тут могут быть исключения из-за другого типа данных
            String s = ob.ToString();
            if (String.IsNullOrEmpty(s))
                return -1;
            else return Int32.Parse(s);
        }

        /// <summary>
        /// NT-Получить число записей в таблице
        /// </summary>
        /// <param name="table">Название таблицы</param>
        /// <param name="column">Название столбца первичного ключа</param>
        /// <returns>Returns row count or -1 if no results</returns>
        public int GetRowCount(string table, string column)
        {
            //SELECT COUNT(id) FROM table;
            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
                m_cmdWithoutArguments.CommandTimeout = m_Timeout;
            }
            //execute command
            string query = String.Format(CultureInfo.InvariantCulture, "SELECT COUNT({0}) FROM {1};", column, table);
            m_cmdWithoutArguments.CommandText = query;
            Object ob = m_cmdWithoutArguments.ExecuteScalar(); //Тут могут быть исключения из-за другого типа данных
            String s = ob.ToString();
            if (String.IsNullOrEmpty(s))
                return -1;
            else return Int32.Parse(s);
        }

        /// <summary>
        /// NT-Получить число записей в таблице, с указанным числовым значением.
        /// </summary>
        /// <remarks>Применяется для столбца первичного ключа, проверяет что запись с этим ключом существует.
        /// Но может применяться и в других случаях.
        /// </remarks>
        /// <param name="table">Название таблицы</param>
        /// <param name="column">Название столбца</param>
        /// <param name="val">Числовое значение в столбце</param>
        /// <returns>Возвращает число записей с таким значением этого столбца, или -1 при ошибке.</returns>
        public int GetRowCount(string table, string column, int val)
        {
            //SELECT column FROM table WHERE (column = value);
            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
                m_cmdWithoutArguments.CommandTimeout = m_Timeout;
            }
            //execute command
            string query = String.Format(CultureInfo.InvariantCulture, "SELECT COUNT({0}) FROM {1} WHERE ({0} = {2});", column, table, val);
            m_cmdWithoutArguments.CommandText = query;
            Object ob = m_cmdWithoutArguments.ExecuteScalar(); //Тут могут быть исключения из-за другого типа данных
            String s = ob.ToString();
            if (String.IsNullOrEmpty(s))
                return -1;
            else return Int32.Parse(s);
        }

        /// <summary>
        /// NT-Проверить существование записи с указанным Id номером
        /// </summary>
        /// <param name="tablename">Название таблицы</param>
        /// <param name="column">Название столбца идентификатора</param>
        /// <param name="idValue">Значение идентификатора записи</param>
        /// <returns>Возвращает True если запись существует, иначе возвращает False.</returns>
        public bool IsRowExists(String tablename, string column, Int32 idValue, Int32 timeout)
        {
            String query = String.Format("SELECT {0} FROM {1} WHERE ({0} = {2})", column, tablename, idValue);
            if (this.m_cmdWithoutArguments == null)
            {
                //create command
                this.m_cmdWithoutArguments = new OleDbCommand(query, m_connection, m_transaction);
                this.m_cmdWithoutArguments.CommandTimeout = timeout;
            }
            this.m_cmdWithoutArguments.CommandText = query;
            //get result
            OleDbDataReader rdr = this.m_cmdWithoutArguments.ExecuteReader();
            bool result = rdr.HasRows;
            rdr.Close();
            return result;
        }

        /// <summary>
        /// NT-Удалить запись(и) из таблицы по значению поля в столбце
        /// </summary>
        /// <remarks>Удаляет все строки с указанным значением параметра.
        /// </remarks>
        /// <param name="table">Название таблицы</param>
        /// <param name="column">Название столбца</param>
        /// <param name="val">Значение столбца</param>
        /// <returns>Возвращает число затронутых (удаленных) строк таблицы.</returns>
        public int DeleteRow(string table, string column, int val)
        {
            //DELETE FROM table WHERE (column = value);
            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
                m_cmdWithoutArguments.CommandTimeout = m_Timeout;
            }
            //execute command
            string query = String.Format(CultureInfo.InvariantCulture, "DELETE FROM {0} WHERE ({0}.{1} = {2});", table, column, val);
            m_cmdWithoutArguments.CommandText = query;
            return m_cmdWithoutArguments.ExecuteNonQuery(); //Тут могут быть исключения из-за другого типа данных
        }

        /// <summary>
        /// NT-получить значение автоинкремента для последней измененной таблицы в текущем сеансе БД
        /// </summary>
        /// <returns></returns>
        public int GetLastAutonumber()
        {
            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
                m_cmdWithoutArguments.CommandTimeout = m_Timeout;
            }
            //execute command
            m_cmdWithoutArguments.CommandText = "SELECT @@IDENTITY;";
            return (int)m_cmdWithoutArguments.ExecuteScalar();
        }

        /// <summary>
        /// NT-Получить список ид записей таблицы
        /// </summary>
        /// <param name="tablename">Название таблицы</param>
        /// <param name="column">Название столбца ИД типа Int32</param>
        /// <param name="where">текст условия отбора. Если отбор не требуется, то передать пустую строку или null. Пример: id > 100 </param>
        /// <returns>Возвращает список идентификаторов записей таблицы</returns>
        public List<Int32> getListOfIds(String tablename, string column, string where)
        {
            String query;
            //create query text
            if (String.IsNullOrEmpty(where))
                query = String.Format("SELECT {1} FROM {0}", tablename, column);
            else
                query = String.Format("SELECT {1} FROM {0} WHERE ({2})", tablename, column, where);
            //create command
            OleDbCommand cmd = new OleDbCommand(query, m_connection, m_transaction);
            cmd.CommandTimeout = m_Timeout;
            //get result
            OleDbDataReader rdr = cmd.ExecuteReader();
            List<int> li = new List<int>();
            if (rdr.HasRows == true)
            {
                while (rdr.Read())
                {
                    li.Add(rdr.GetInt32(0));
                }
            }
            rdr.Close();

            return li;
        }


        /// <summary>
        /// NT-Удалить все строки из указанной таблицы.
        /// Счетчик первичного ключа не сбрасывается - его отдельно надо сбрасывать.
        /// </summary>
        /// <param name="table">Название таблицы</param>
        /// <param name="timeout">Таймаут операции в секундах</param>
        public void TableClear(string table, int timeout)
        {
            //DELETE FROM table;
            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
                m_cmdWithoutArguments.CommandTimeout = timeout;
            }
            //execute command
            string query = String.Format(CultureInfo.InvariantCulture, "DELETE FROM {0};", table);
            m_cmdWithoutArguments.CommandText = query;
            m_cmdWithoutArguments.ExecuteNonQuery();

            return;
        }

        //TODO: Если нужна функция очистки всей БД, раскомментируйте код и измените его, вписав правильные имена таблиц.
        ///// <summary>
        ///// NFT-Очистить БД 
        ///// </summary>
        ///// <returns>True if Success, False otherwise</returns>
        //internal bool ClearDb()
        //{
        //    bool result = false;
        //    try
        //    {
        //        this.TransactionBegin();
        //        this.TableClear(DbAdapter.ContentTableName);
        //        this.TableClear(DbAdapter.DocumentTableName);
        //        this.TableClear(DbAdapter.PictureTableName);
        //        this.TransactionCommit();
        //        result = true;
        //    }
        //    catch (Exception)
        //    {
        //        this.TransactionRollback();
        //        result = false;
        //    }
        //    return result;
        //}

        #endregion

        #region *** Для таблицы свойств ключ-значение ***
        /// <summary>
        /// NT-Получить значения свойств из таблицы БД
        /// </summary>
        /// <remarks>
        /// Это функция для таблицы Ключ-Значения. 
        /// Структура таблицы:
        /// - id counter, primary key - первичный ключ, не читается.
        /// - p text - название параметра, ключ (строка), должно быть уникальным.
        /// - d text - значение параметра, значение (строка), допускаются повторы и пустые строки.
        /// </remarks>
        /// <param name="table">Название таблицы</param>
        /// <returns>Словарь, содержащий все пары ключ-значение из таблицы БД</returns>
        public Dictionary<String, String> KeyValueReadDictionary(String table)
        {
            Dictionary<String, string> dict = new Dictionary<string, string>();
            //create command
            String query = String.Format("SELECT * FROM {0};", table);
            OleDbCommand cmd = new OleDbCommand(query, this.m_connection, this.m_transaction);
            cmd.CommandTimeout = m_Timeout;
            //execute command
            OleDbDataReader rdr = cmd.ExecuteReader();
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    //int id = rdr.GetInt32(0); //id not used
                    String param = rdr.GetString(1);
                    String val = rdr.GetString(2);
                    //store to dictionary 
                    dict.Add(param, val);
                }
            }
            //close reader
            rdr.Close();

            return dict;
        }



        /// <summary>
        /// NT - Перезаписать значения свойств в таблице БД.
        /// Все записи из таблицы будут удалены и заново вставлены.
        /// </summary>
        /// <remarks>
        /// Это функция для таблицы Ключ-Значения. 
        /// Структура таблицы:
        /// - id counter, primary key - первичный ключ, не читается.
        /// - p text - название параметра, ключ (строка), должно быть уникальным.
        /// - d text - значение параметра, значение (строка), допускаются повторы и пустые строки.
        /// </remarks>
        /// <param name="table">Название таблицы</param>
        /// <param name="dic">Словарь, содержащий пары ключ-значение</param>
        public void KeyValueStoreDictionary(String table, Dictionary<string, string> dic)
        {
            //1 - очистить таблицу
            String query = String.Format("DELETE * FROM {0};", table);
            OleDbCommand cmd = new OleDbCommand(query, this.m_connection, this.m_transaction);
            cmd.CommandTimeout = m_Timeout;
            cmd.ExecuteNonQuery();
            //2 - записать новые значения
            query = String.Format("INSERT INTO {0} (param, val) VALUES (?, ?);", table);
            cmd.CommandText = query;
            cmd.Parameters.Add(new OleDbParameter("@p0", OleDbType.VarWChar));
            cmd.Parameters.Add(new OleDbParameter("@p1", OleDbType.VarWChar));
            //execute commands
            foreach (KeyValuePair<String, String> kvp in dic)
            {
                cmd.Parameters[0].Value = kvp.Key;
                cmd.Parameters[1].Value = kvp.Value;
                cmd.ExecuteNonQuery();
            }

            return;
        }

        /// <summary>
        /// NT-Получить один из параметров, не загружая весь набор
        /// </summary>
        /// <remarks>
        /// Это функция для таблицы Ключ-Значения. Ыункция универсальная, поэтому надо указывать имена таблиц и столбцов. 
        /// Структура таблицы:
        /// - id counter, primary key - первичный ключ, не читается.
        /// - p text - название параметра, ключ (строка), должно быть уникальным.
        /// - d text - значение параметра, значение (строка), допускаются повторы и пустые строки.
        /// </remarks>
        /// <param name="table">Название таблицы</param>
        /// <param name="columnName">Название столбца ключа</param>
        /// <param name="paramName">Название параметра (ключ)</param>
        /// <returns>Возвращает строку значения параметра</returns>
        public string KeyValueGetParameter(String table, String columnName, String paramName)
        {
            //create command
            String query = String.Format("SELECT * FROM {0} WHERE ({1} = '{2}' );", table, columnName, paramName);
            OleDbCommand cmd = new OleDbCommand(query, this.m_connection, this.m_transaction);
            cmd.CommandTimeout = m_Timeout;
            //execute command
            OleDbDataReader rdr = cmd.ExecuteReader();
            String result = String.Empty;
            if (rdr.HasRows)
            {
                while (rdr.Read())
                {
                    //int id = rdr.GetInt32(0); //id not used
                    //String param = rdr.GetString(1);//param not used
                    result = rdr.GetString(2);
                }
            }
            //close reader
            rdr.Close();
            return result;
        }

        #endregion


        #region Функции пользовательские

        //TODO: Добавить код новых функций здесь, каждый комплект функций для таблицы поместить в отдельный region
        //новые команды для них обязательно внести в ClearCommands(), иначе транзакции будут работать неправильно. 

        /// <summary>
        /// NT-Получить максимальный ИдентификаторЭлемента, существующий в БД
        /// </summary>
        /// <returns>Возвращает 0, если никаких элементов нет в БД.</returns>
        /// <remarks>
        /// Эта функция вызывается из объекта UAMX-2.ElementIdManager.
        /// И она должна быть определена в производном классе алаптера БД.
        /// Поэтому она должна быть public, а не internal. Хотя более нигде она не используется.
        /// </remarks>
        public virtual int getMaxElementId()
        {
            throw new NotImplementedException();
            //TODO: переопределить в производном классе
            //поскольку идентификатор элемента уникальный, собрать его из всех таблиц элементов - непросто!
        }
        
        #endregion


    }
}
