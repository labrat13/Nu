using System;
using System.Collections.Generic;
using System.Text;
using System.Data.OleDb;
using System.Globalization;
using System.IO;


namespace PMEngine
{
    /// <summary>
    /// Представляет локальную БД.
    /// </summary>
    public class DbAdapter
    {

        #region Fields
        /// <summary>
        /// Имя файла базы данных
        /// </summary>
        public const string DatabaseFileName = "db.mdb";
        /// <summary>
        /// database connection string
        /// </summary>
        public static String ConnectionString;
        /// <summary>
        /// database connection
        /// </summary>
        private OleDbConnection m_connection;
        /// <summary>
        /// Transaction for current connection
        /// </summary>
        private OleDbTransaction m_transaction;
        /// <summary>
        /// Database is read-only
        /// </summary>
        private bool dbReadOnly;
        /// <summary>
        /// Константа название таблицы БД - для функций адаптера
        /// </summary>
        internal const string Table_Category = "CategoryTable";
        /// <summary>
        /// Константа название таблицы БД - для функций адаптера
        /// </summary>
        internal const string Table_ProjectInfo = "ProjectInfoTable";
        /// <summary>
        /// Константа название таблицы БД - для функций адаптера
        /// </summary>
        internal const string Table_TaskClass = "TaskClassTable";
        /// <summary>
        /// Константа название таблицы БД - для функций адаптера
        /// </summary>
        internal const string Table_TaskInfo = "TaskInfoTable";

        //все объекты команд сбрасываются в нуль при отключении соединения с БД
        //TODO: Новые команды внести в ClearCommands()
        /// <summary>
        /// Команда без параметров, используемая во множестве функций
        /// </summary>
        private OleDbCommand m_cmdWithoutArguments;
        private OleDbCommand m_cmdGetElementIDByWebTitle;
        private OleDbCommand m_cmdInsertКатегорияЗадачи;
        private OleDbCommand m_cmdUpdateКатегорияЗадачи;
        private OleDbCommand m_cmdInsertКлассЗадачи;
        private OleDbCommand m_cmdUpdateКлассЗадачи;
        private OleDbCommand m_cmdInsertTaskInfo;
        private OleDbCommand m_cmdUpdateTaskInfo;
        private OleDbCommand m_cmdInsertProjectInfo;
        private OleDbCommand m_cmdUpdateProjectInfo;

        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        public DbAdapter()
        {

        }
        /// <summary>
        /// Database is read-only
        /// </summary>
        public bool ReadOnly
        {
            get { return dbReadOnly; }
        }

        #region Service functions

        /// <summary>
        /// NT-все объекты команд класса сбросить в нуль
        /// </summary>
        protected void ClearCommands()
        {
            m_cmdWithoutArguments = null;
            m_cmdGetElementIDByWebTitle = null;
            m_cmdInsertКатегорияЗадачи = null;
            m_cmdUpdateКатегорияЗадачи = null;
            m_cmdInsertКлассЗадачи = null;
            m_cmdUpdateКлассЗадачи = null;
            m_cmdInsertTaskInfo = null;
            m_cmdUpdateTaskInfo = null;
            m_cmdInsertProjectInfo = null;
            m_cmdUpdateProjectInfo = null;
            //m_cmd4 = null;
            //m_cmd5 = null;
            //m_cmd6 = null;
            return;
        }

        /// <summary>
        /// NT-Создать и инициализировать объект адаптера БД
        /// </summary>
        /// <param name="dbFile">Путь к файлу БД</param>
        /// <param name="ReadOnly">Флаг Открыть БД только для чтения</param>
        /// <returns>Возвращает инициализированный объект адаптера БД</returns>
        public static DbAdapter SetupDbAdapter(String dbFile, bool ReadOnly)
        {
            //теперь создать новый интерфейс БД
            DbAdapter dblayer = new DbAdapter();
            dblayer.dbReadOnly = ReadOnly;
            String constr = createConnectionString(dbFile, ReadOnly);
            //connect to database
            dblayer.Connect(constr);
            return dblayer;
        }


        /// <summary>
        /// NT-Открыть соединение с БД
        /// </summary>
        /// <param name="connectionString">Строка соединения с БД</param>
        public void Connect(String connectionString)
        {
            //create connection
            OleDbConnection con = new OleDbConnection(connectionString);
            //try open connection
            con.Open();
            con.Close();
            //close existing connection
            Disconnect();
            //open new connection and set as primary
            this.m_connection = con;
            ConnectionString = connectionString;
            this.m_connection.Open();

            return;
        }


        /// <summary>
        /// NT-Закрыть соединение с БД
        /// </summary>
        public void Disconnect()
        {
            if (m_connection != null)
            {
                if (m_connection.State == System.Data.ConnectionState.Open)
                    m_connection.Close();
                m_connection = null;
            }

            //все объекты команд сбросить в нуль при отключении соединения с БД, чтобы ссылка на объект соединения при следующем подключении не оказалась устаревшей
            ClearCommands();

            return;
        }



        /// <summary>
        /// NT-Начать транзакцию. 
        /// </summary>
        public void TransactionBegin()
        {
            m_transaction = m_connection.BeginTransaction();
            //сбросить в нуль все объекты команд, чтобы они были пересозданы для новой транзакции
            ClearCommands();
        }
        //NT-Подтвердить транзакцию Нужно закрыть соединение после этого!
        public void TransactionCommit()
        {
            m_transaction.Commit();
            //сбросить в нуль все объекты команд, чтобы они были пересозданы для новой транзакции
            ClearCommands(); //надо ли сбросить m_transactions = null?
            m_transaction = null;

        }
        //NT-Отменить транзакцию. Нужно закрыть соединение после этого!
        public void TransactionRollback()
        {
            m_transaction.Rollback();
            //сбросить в нуль все объекты команд, чтобы они были пересозданы для новой транзакции
            ClearCommands();
            m_transaction = null;
        }

        /// <summary>
        /// NT-Создать строку соединения с БД
        /// </summary>
        /// <param name="dbFile">Путь к файлу БД</param>
        public static string createConnectionString(string dbFile, bool ReadOnly)
        {
            //Provider=Microsoft.Jet.OLEDB.4.0;Data Source="C:\Documents and Settings\salomatin\Мои документы\Visual Studio 2008\Projects\RadioBase\радиодетали.mdb"
            OleDbConnectionStringBuilder b = new OleDbConnectionStringBuilder();
            b.Provider = "Microsoft.Jet.OLEDB.4.0";
            b.DataSource = dbFile;
            //это только для БД на незаписываемых дисках
            if (ReadOnly)
            {
                b.Add("Mode", "Share Deny Write");
            }
            //user id and password can specify here
            return b.ConnectionString;
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
        public static void extractDbFile(string filepath)
        {
            FileStream fs = new FileStream(filepath, FileMode.Create);
            byte[] content = Properties.Resources.dbase;//Укажите здесь имя ресурса - шаблона БД
            fs.Write(content, 0, content.Length);
            fs.Close();
        }

        #endregion


        #region *** Для всех таблиц ***
        /// <summary>
        /// NT- Получить максимальное значение ИД для столбца таблицы
        /// Обычно применяется для столбца первичного ключа, но можно и для других целочисленных столбцов.
        /// </summary>
        /// <param name="table">Название таблицы</param>
        /// <param name="column">Название столбца первичного ключа</param>
        /// <returns>Returns max value or -1 if no results</returns>
        public int getTableMaxId(string table, string column)
        {
            //SELECT MAX(id) FROM table;
            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
                m_cmdWithoutArguments.CommandTimeout = 60;
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
        public int getTableMinId(string table, string column)
        {
            //SELECT MIN(id) FROM table;
            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
                m_cmdWithoutArguments.CommandTimeout = 60;
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
                m_cmdWithoutArguments.CommandTimeout = 60;
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
                m_cmdWithoutArguments.CommandTimeout = 120;
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
        /// NT-Удалить запись(и) из таблицы по значению поля в столбце
        /// </summary>
        /// <remarks>Удаляет все строки с указанным значением параметра.
        /// </remarks>
        /// <param name="table">Название таблицы</param>
        /// <param name="column">Название столбца</param>
        /// <param name="val">Значение столбца</param>
        /// <returns>Возвращает число затронутых (удаленных) строк таблицы.</returns>
        private int DeleteRow(string table, string column, int val)
        {
            //DELETE FROM table WHERE (column = value);
            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
                m_cmdWithoutArguments.CommandTimeout = 120;
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
        internal int GetLastAutonumber()
        {
            //SELECT COUNT(id) FROM table;
            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
                m_cmdWithoutArguments.CommandTimeout = 60;
            }
            //execute command
            m_cmdWithoutArguments.CommandText = "SELECT @@IDENTITY;";
            return (int)m_cmdWithoutArguments.ExecuteScalar();
        }

        //TODO: Если нужна функция очистки всей БД, раскомментируйте код и измените его, вписав правильные имена таблиц.
        /// <summary>
        /// NFT-Очистить БД Хранилища
        /// </summary>
        /// <returns>True if Success, False otherwise</returns>
        internal bool ClearDb()
        {
            bool result = false;
            try
            {
                this.TransactionBegin();
                this.ClearTable(DbAdapter.Table_Category);
                this.ClearTable(DbAdapter.Table_ProjectInfo);
                this.ClearTable(DbAdapter.Table_TaskClass);
                this.ClearTable(DbAdapter.Table_TaskInfo);
                this.TransactionCommit();
                result = true;
            }
            catch (Exception)
            {
                this.TransactionRollback();
                result = false;
            }
            return result;
        }

        /// <summary>
        /// RT-Удалить все строки из указанной таблицы.
        /// Счетчик первичного ключа не сбрасывается - его отдельно надо сбрасывать.
        /// </summary>
        /// <param name="table">Название таблицы</param>
        public void ClearTable(string table)
        {
            //DELETE FROM table;
            if (m_cmdWithoutArguments == null)
            {
                m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
                m_cmdWithoutArguments.CommandTimeout = 600;
            }
            //execute command
            string query = String.Format(CultureInfo.InvariantCulture, "DELETE FROM {0};", table);
            m_cmdWithoutArguments.CommandText = query;
            m_cmdWithoutArguments.ExecuteNonQuery();

            return;
        }

        /// <summary>
        /// Получить строку текста из ридера таблицы или пустую строку
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

        #endregion


        //TODO: Добавить код новых функций здесь, каждый комплект функций для таблицы поместить в отдельный region
        //новые команды для них обязательно внести в ClearCommands(), иначе транзакции будут работать неправильно.

        //Эти функции закомментированы и оставлены только для примера. (Хотя их тут излишне много...)

        ///// <summary>
        ///// NT-Получить максимальный ИдентификаторЭлемента, существующий в БД
        ///// </summary>
        ///// <returns>Возвращает 0, если никаких элементов нет в БД.</returns>
        //public Int32 getMaxElementId()
        //{
        //    //получить максимальный ид элементов из всех таблиц БД
        //    //тут все равно просматривать все таблицы БД, ведь неизвестно, где будет элемент с наибольшим идентификатором.
        //    Int32 id1 = getTableMaxId(DbAdapter.Table_Category, "eid");
        //    Int32 id2 = getTableMaxId(DbAdapter.Table_ProjectInfo, "eid");
        //    Int32 id3 = getTableMaxId(DbAdapter.Table_TaskClass, "eid");
        //    Int32 id4 = getTableMaxId(DbAdapter.Table_TaskInfo, "eid");
        //    //вычисляем собственно максимум
        //    Int32 result = Math.Max(Math.Max(id1, id2), Math.Max(id3, id4));
        //    //обрабатываем вариант -1
        //    if (result < 0) result = 0;

        //    return result;
        //}


        //#region *** WebName functions ***
        ////функции отменены, так как теперь веб-имя не уникальное и не нуждается в проверке в БД
        ///// <summary>
        ///// NT-Получить идентификатор элемента по веб-имени
        ///// </summary>
        ///// <param name="table">Название таблицы</param>
        ///// <param name="webname">Веб-имя элемента</param>
        ///// <returns>Идентификатор элемента или -1 если не найден</returns>
        //private int getElementIdByWebName(String table, string webname)
        //{
        //    //SELECT id FROM Items WHERE (webname = ?)
        //    //create command
        //    if (m_cmdGetElementIDByWebTitle == null)
        //    {
        //        m_cmdGetElementIDByWebTitle = new OleDbCommand("", this.m_connection, m_transaction);
        //        m_cmdGetElementIDByWebTitle.CommandTimeout = 60;
        //        m_cmdGetElementIDByWebTitle.Parameters.Add("@webname", OleDbType.WChar);
        //    }
        //    //execute command
        //    String query = String.Format("SELECT eid FROM {0} WHERE (webname = ?);", table);
        //    m_cmdGetElementIDByWebTitle.CommandText = query;
        //    m_cmdGetElementIDByWebTitle.Parameters[0].Value = webname;
        //    //read results
        //    OleDbDataReader rdr = m_cmdGetElementIDByWebTitle.ExecuteReader();
        //    int result = -1;
        //    if (rdr.HasRows)
        //    {
        //        rdr.Read(); //only first row
        //        result = rdr.GetInt32(0);
        //    }
        //    //close reader
        //    rdr.Close();

        //    return result;
        //}
        ///// <summary>
        ///// NT-Проверить, что веб-имя присутствует в таблицах элементов 
        ///// </summary>
        ///// <param name="name">веб-имя</param>
        ///// <returns>true  если имя уже присутствует, false в противном случае</returns>
        //public bool IsDbContainsWebName(string name)
        //{
        //    //Это надо делать быстро
        //    Int32 result = 0;
        //    //просматриваем таблицы, начиная с самой нагруженной
        //    result = this.getElementIdByWebName(DbAdapter.Table_TaskInfo, name);
        //    if (result >= 0) return true;
        //    result = this.getElementIdByWebName(DbAdapter.Table_ProjectInfo, name);
        //    if (result >= 0) return true;
        //    result = this.getElementIdByWebName(DbAdapter.Table_Category, name);
        //    if (result >= 0) return true;
        //    result = this.getElementIdByWebName(DbAdapter.Table_TaskClass, name);
        //    if (result >= 0) return true;

        //    //ничего не нашли
        //    return false;
        //}

        /////// <summary>
        /////// NT-Создать незанятое веб-имя из названия элемента - отменена, так как теперь веб-имя не уникальное и не нуждается в проверке в БД
        /////// </summary>
        /////// <param name="title">Название элемента как основа для веб-имени</param>
        /////// <param name="parentTitle">Название родительского элемента, используется если название элемента непригодно.</param>
        /////// <returns>Возвращает строку незанятого веб-имени.</returns>
        ////public String WebNameCreateFromTitle(string title, string parentTitle)
        ////{
        ////    //сгенерировать веб-имя для категории
        ////    String text = title;
        ////    //если имя пустое, взять до 8 символов из имени родительской категории
        ////    if (title.Length < 3)
        ////    {
        ////        text = parentTitle;
        ////        if (text.Length > 8) text = text.Substring(0, 8);
        ////    }

        ////    String webname = СсылкаНаЭлемент.createWebnameFromTitle(text);
        ////    //проверить, что имя не занято, и если занято, то изменить
        ////    //путем добавления в конец числа-счетчика попыток, обрезая имя так, чтобы оно не превышало максимума
        ////    String wn = String.Copy(webname);
        ////    Int32 counter = 0;
        ////    while (this.IsDbContainsWebName(wn))
        ////    {
        ////        wn = String.Copy(webname);
        ////        //получить строку счетчика
        ////        string counterstring = counter.ToString(CultureInfo.InvariantCulture);
        ////        //вычислить длину имени, чтобы поместилось в максимум вместе с текстом счетчика и разделителем
        ////        Int32 len = 64 - (counterstring.Length + 1);
        ////        //обрезать имя так, чтобы вошел счетчик и символ подчеркивания
        ////        if (wn.Length > len)
        ////            wn = wn.Substring(0, len);
        ////        //имя сформировать и потом проверить
        ////        wn = wn + "_" + counterstring;
        ////        counter++;//увеличить счетчик на следующий цикл
        ////    }
        ////    //тут wn должно содержать свободное веб-имя
        ////    return wn;
        ////}

        /////// <summary>
        /////// NT-Проверка, что веб-имя соответствует правилам - отменена, так как теперь веб-имя не уникальное и не нуждается в проверке в БД
        /////// </summary>
        /////// <param name="newname">Новое веб-имя элемента</param>
        /////// <param name="oldname">Старое веб-имя элемента</param>
        /////// <returns>true or false</returns>
        ////public bool WebNameCheck(string newname, string oldname)
        ////{
        ////    //if oldname is valid and exists, and same as newname, return true
        ////    if ((!СсылкаНаЭлемент.IsInvalidWebName(oldname)) && (this.IsDbContainsWebName(oldname)) && (oldname.Equals(newname)))
        ////        return true;
        ////    //check newname
        ////    if (СсылкаНаЭлемент.IsInvalidWebName(newname)) return false;//длина от 3 до 64, не должно содержать неподходящих символов
        ////    if (this.IsDbContainsWebName(newname)) return false; //должно быть уникальным в системе - это долгая операция, поэтому последняя
        ////    else return true;
        ////}

        //#endregion

        //#region ** КатегорияЗадачи functions **

        ///// <summary>
        ///// NT-Получить все категории
        ///// </summary>
        ///// <returns></returns>
        //public List<UAMX_Element> getAllCategories()
        //{
        //    //SELECT * FROM Items;
        //    if (m_cmdWithoutArguments == null)
        //    {
        //        m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
        //        m_cmdWithoutArguments.CommandTimeout = 60;
        //    }
        //    //create command
        //    string query = String.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0};", DbAdapter.Table_Category);
        //    m_cmdWithoutArguments.CommandText = query;
        //    //execute command
        //    //read rows results
        //    OleDbDataReader rdr = m_cmdWithoutArguments.ExecuteReader();
        //    КатегорияЗадачи c = null;
        //    List<UAMX_Element> result = new List<UAMX_Element>();
        //    if (rdr.HasRows)
        //    {
        //        while (rdr.Read())
        //        {
        //            c = readRowКатегорияЗадачи(rdr);
        //            result.Add(c);
        //        }
        //    }
        //    //close reader
        //    rdr.Close();
        //    return result;
        //}

        ///// <summary>
        ///// NT-Получить объект по ИД элемента
        ///// </summary>
        ///// <param name="id">Идентификатор элемента</param>
        ///// <returns>Возвращает объект или null</returns>
        //public КатегорияЗадачи getCategoryByElementId(ИдентификаторЭлемента id)
        //{
        //    //SELECT * FROM Items WHERE (eid = ?)
        //    if (m_cmdWithoutArguments == null)
        //    {
        //        m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
        //        m_cmdWithoutArguments.CommandTimeout = 60;
        //    }
        //    //create command
        //    string query = String.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0} WHERE (eid = {1});", DbAdapter.Table_Category, id.Id.ToString());
        //    m_cmdWithoutArguments.CommandText = query;
        //    //execute command
        //    //read single row results
        //    OleDbDataReader rdr = m_cmdWithoutArguments.ExecuteReader();
        //    КатегорияЗадачи c = null;
        //    if (rdr.HasRows)
        //    {
        //        rdr.Read(); //only first row
        //        c = readRowКатегорияЗадачи(rdr);
        //    }
        //    //close reader
        //    rdr.Close();
        //    return c;
        //}
        ///// <summary>
        ///// NT-Прочитать данные элемента
        ///// </summary>
        ///// <param name="rdr">Ридер с данными</param>
        ///// <returns>Возвращает объект данных элемента</returns>
        //private КатегорияЗадачи readRowКатегорияЗадачи(OleDbDataReader rdr)
        //{
        //    КатегорияЗадачи c = new КатегорияЗадачи();
        //    //UAMX поля
        //    c.TableId = rdr.GetInt32(0);
        //    c.ElementId = new ИдентификаторЭлемента((Int32)rdr.GetInt32(1));
        //    c.IsActive = rdr.GetBoolean(2);
        //    c.ВебИмя = rdr.GetString(3);
        //    c.ДатаСоздания = rdr.GetDateTime(4);
        //    c.Название = rdr.GetString(5);
        //    c.Описание = rdr.GetString(6);
        //    c.Замечания = rdr.GetString(7);
        //    c.РодительскийЭлемент = new СсылкаНаЭлемент(rdr.GetString(8));
        //    //Собственные поля - нет

        //    return c;
        //}
        ///// <summary>
        ///// NT-Вставить элемент в таблицу
        ///// </summary>
        ///// <param name="s">Объект категории задачи</param>
        ///// <returns>Возвращает число затронутых записей.</returns>
        //public int InsertКатегорияЗадачи(КатегорияЗадачи s)
        //{
        //    //Вставить предмет в таблицу и вписать в объект идентификатор первичного ключа
        //    String query = "INSERT INTO " + DbAdapter.Table_Category + " (eid, active, webname, creadate, title, descr, remarks, parent) VALUES (?, ?, ?, ?, ?, ?, ?, ?);";
        //    OleDbCommand cmd = this.m_cmdInsertКатегорияЗадачи;
        //    //create command
        //    if (cmd == null)
        //    {
        //        cmd = new OleDbCommand(query, this.m_connection, m_transaction);
        //        cmd.CommandTimeout = 60;
        //        //UAMX fields
        //        cmd.Parameters.Add("@eid", OleDbType.Integer);
        //        cmd.Parameters.Add("@active", OleDbType.Boolean);
        //        cmd.Parameters.Add("@webname", OleDbType.WChar);
        //        cmd.Parameters.Add("@creadate", OleDbType.Date);
        //        cmd.Parameters.Add("@title", OleDbType.WChar);
        //        cmd.Parameters.Add("@descr", OleDbType.WChar);
        //        cmd.Parameters.Add("@remarks", OleDbType.WChar);
        //        cmd.Parameters.Add("@parent", OleDbType.WChar);
        //        //store back
        //        this.m_cmdInsertКатегорияЗадачи = cmd;
        //    }
        //    //set arguments UAMX
        //    cmd.Parameters[0].Value = (Int32)s.ElementId.Id;
        //    cmd.Parameters[1].Value = s.IsActive;
        //    cmd.Parameters[2].Value = s.ВебИмя;
        //    cmd.Parameters[3].Value = s.ДатаСоздания;
        //    cmd.Parameters[4].Value = s.Название;
        //    cmd.Parameters[5].Value = s.Описание;
        //    cmd.Parameters[6].Value = s.Замечания;
        //    cmd.Parameters[7].Value = s.РодительскийЭлемент.SerializeToDataString();
        //    //execute
        //    int result = cmd.ExecuteNonQuery();
        //    //TODO: тут надо бы выяснить и вписать в объект новый tableId
        //    //чтобы отличать несохраненный объект от уже сохраненного в БД.

        //    return result;
        //}
        ///// <summary>
        ///// NT-Обновить элемент
        ///// </summary>
        ///// <param name="s">Элемент</param>
        ///// <returns>Возвращает число затронутых записей.</returns>
        //public int UpdateКатегорияЗадачи(КатегорияЗадачи s)
        //{
        //    //Обновить запись элемента в таблице
        //    String query = "UPDATE " + DbAdapter.Table_Category + " SET active=?, webname=?, creadate=?, title=?, descr=?, remarks=?, parent=? WHERE (eid = ?)";
        //    OleDbCommand cmd = this.m_cmdUpdateКатегорияЗадачи;
        //    //create command
        //    if (cmd == null)
        //    {
        //        cmd = new OleDbCommand(query, this.m_connection, m_transaction);
        //        cmd.CommandTimeout = 60;
        //        //UAMX fields
        //        cmd.Parameters.Add("@active", OleDbType.Boolean);
        //        cmd.Parameters.Add("@webname", OleDbType.WChar);
        //        cmd.Parameters.Add("@creadate", OleDbType.Date);
        //        cmd.Parameters.Add("@title", OleDbType.WChar);
        //        cmd.Parameters.Add("@descr", OleDbType.WChar);
        //        cmd.Parameters.Add("@remarks", OleDbType.WChar);
        //        cmd.Parameters.Add("@parent", OleDbType.WChar);
        //        cmd.Parameters.Add("@eid", OleDbType.Integer);
        //        //store back
        //        this.m_cmdUpdateКатегорияЗадачи = cmd;
        //    }
        //    //execute command
        //    cmd.Parameters[0].Value = s.IsActive;
        //    cmd.Parameters[1].Value = s.ВебИмя;
        //    cmd.Parameters[2].Value = s.ДатаСоздания;
        //    cmd.Parameters[3].Value = s.Название;
        //    cmd.Parameters[4].Value = s.Описание;
        //    cmd.Parameters[5].Value = s.Замечания;
        //    cmd.Parameters[6].Value = s.РодительскийЭлемент.SerializeToDataString();
        //    cmd.Parameters[7].Value = (Int32)s.ElementId.Id;
        //    //execute
        //    int result = cmd.ExecuteNonQuery();

        //    return result;


        //}

        //#endregion

        //#region ** КлассЗадачи functions **



        ///// <summary>
        ///// NT-Вставить элемент в таблицу
        ///// </summary>
        ///// <param name="s">Объект класса задачи</param>
        ///// <returns>Возвращает число затронутых записей.</returns>
        //public int InsertКлассЗадачи(КлассЗадачи s)
        //{
        //    //Вставить предмет в таблицу и вписать в объект идентификатор первичного ключа
        //    String query = "INSERT INTO " + DbAdapter.Table_TaskClass + " (eid, active, webname, creadate, title, descr, remarks, parent, metod) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?);";
        //    OleDbCommand cmd = this.m_cmdInsertКлассЗадачи;
        //    //create command
        //    if (cmd == null)
        //    {
        //        cmd = new OleDbCommand(query, this.m_connection, m_transaction);
        //        cmd.CommandTimeout = 60;
        //        //UAMX fields
        //        cmd.Parameters.Add("@eid", OleDbType.Integer);
        //        cmd.Parameters.Add("@active", OleDbType.Boolean);
        //        cmd.Parameters.Add("@webname", OleDbType.WChar);
        //        cmd.Parameters.Add("@creadate", OleDbType.Date);
        //        cmd.Parameters.Add("@title", OleDbType.WChar);
        //        cmd.Parameters.Add("@descr", OleDbType.WChar);
        //        cmd.Parameters.Add("@remarks", OleDbType.WChar);
        //        cmd.Parameters.Add("@parent", OleDbType.WChar);
        //        cmd.Parameters.Add("@metod", OleDbType.WChar);
        //        //store back
        //        this.m_cmdInsertКлассЗадачи = cmd;
        //    }
        //    //set arguments UAMX
        //    cmd.Parameters[0].Value = (Int32)s.ElementId.Id;
        //    cmd.Parameters[1].Value = s.IsActive;
        //    cmd.Parameters[2].Value = s.ВебИмя;
        //    cmd.Parameters[3].Value = s.ДатаСоздания;
        //    cmd.Parameters[4].Value = s.Название;
        //    cmd.Parameters[5].Value = s.Описание;
        //    cmd.Parameters[6].Value = s.Замечания;
        //    cmd.Parameters[7].Value = s.РодительскийЭлемент.SerializeToDataString();
        //    cmd.Parameters[8].Value = s.МетодикаЗадачи.SerializeToDataString();
        //    //execute
        //    int result = cmd.ExecuteNonQuery();
        //    //TODO: тут надо бы выяснить и вписать в объект новый tableId
        //    //чтобы отличать несохраненный объект от уже сохраненного в БД.
        //    //а можно сначала вызывать UPDATE, а если он вернет 0 а не 1, то INSERT
        //    //но это конечно не поможет выявить, сохранен объект или нет.

        //    return result;
        //}
        ///// <summary>
        ///// NT-Обновить элемент
        ///// </summary>
        ///// <param name="s">Элемент</param>
        ///// <returns>Возвращает число затронутых записей.</returns>
        //public int UpdateКлассЗадачи(КлассЗадачи s)
        //{
        //    //Обновить запись элемента в таблице
        //    String query = "UPDATE " + DbAdapter.Table_TaskClass + " SET active=?, webname=?, creadate=?, title=?, descr=?, remarks=?, parent=?, metod=? WHERE (eid = ?)";
        //    OleDbCommand cmd = this.m_cmdUpdateКлассЗадачи;
        //    //create command
        //    if (cmd == null)
        //    {
        //        cmd = new OleDbCommand(query, this.m_connection, m_transaction);
        //        cmd.CommandTimeout = 60;
        //        //UAMX fields
        //        cmd.Parameters.Add("@active", OleDbType.Boolean);
        //        cmd.Parameters.Add("@webname", OleDbType.WChar);
        //        cmd.Parameters.Add("@creadate", OleDbType.Date);
        //        cmd.Parameters.Add("@title", OleDbType.WChar);
        //        cmd.Parameters.Add("@descr", OleDbType.WChar);
        //        cmd.Parameters.Add("@remarks", OleDbType.WChar);
        //        cmd.Parameters.Add("@parent", OleDbType.WChar);
        //        //
        //        cmd.Parameters.Add("@metod", OleDbType.WChar);
        //        cmd.Parameters.Add("@eid", OleDbType.Integer);
        //        //store back
        //        this.m_cmdUpdateКлассЗадачи = cmd;
        //    }
        //    //execute command
        //    cmd.Parameters[0].Value = s.IsActive;
        //    cmd.Parameters[1].Value = s.ВебИмя;
        //    cmd.Parameters[2].Value = s.ДатаСоздания;
        //    cmd.Parameters[3].Value = s.Название;
        //    cmd.Parameters[4].Value = s.Описание;
        //    cmd.Parameters[5].Value = s.Замечания;
        //    cmd.Parameters[6].Value = s.РодительскийЭлемент.SerializeToDataString();
        //    cmd.Parameters[7].Value = s.МетодикаЗадачи.SerializeToDataString();
        //    cmd.Parameters[8].Value = (Int32)s.ElementId.Id;
        //    //execute
        //    int result = cmd.ExecuteNonQuery();

        //    return result;


        //}
        ///// <summary>
        ///// NT-Получить все классы задач
        ///// </summary>
        ///// <returns></returns>
        //public List<UAMX_Element> getAllClassTasks()
        //{
        //    //SELECT * FROM Items;
        //    if (m_cmdWithoutArguments == null)
        //    {
        //        m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
        //        m_cmdWithoutArguments.CommandTimeout = 60;
        //    }
        //    //create command
        //    string query = String.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0};", DbAdapter.Table_TaskClass);
        //    m_cmdWithoutArguments.CommandText = query;
        //    //execute command
        //    //read single row results
        //    OleDbDataReader rdr = m_cmdWithoutArguments.ExecuteReader();
        //    КлассЗадачи c = null;
        //    List<UAMX_Element> result = new List<UAMX_Element>(); 
        //    if (rdr.HasRows)
        //    {
        //        while (rdr.Read())
        //        {
        //            c = readRowКлассЗадачи(rdr);
        //            result.Add(c);
        //        }
        //    }
        //    //close reader
        //    rdr.Close();
        //    return result;
        //}

        ///// <summary>
        ///// NT-Получить объект по ИД элемента
        ///// </summary>
        ///// <param name="id">Идентификатор элемента</param>
        ///// <returns>Возвращает объект или null</returns>
        //public КлассЗадачи getClassTaskByElementId(ИдентификаторЭлемента id)
        //{
        //    //SELECT * FROM Items WHERE (eid = ?)
        //    if (m_cmdWithoutArguments == null)
        //    {
        //        m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
        //        m_cmdWithoutArguments.CommandTimeout = 60;
        //    }
        //    //create command
        //    string query = String.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0} WHERE (eid = {1});", DbAdapter.Table_TaskClass, id.Id.ToString());
        //    m_cmdWithoutArguments.CommandText = query;
        //    //execute command
        //    //read single row results
        //    OleDbDataReader rdr = m_cmdWithoutArguments.ExecuteReader();
        //    КлассЗадачи c = null;
        //    if (rdr.HasRows)
        //    {
        //        rdr.Read(); //only first row
        //        c = readRowКлассЗадачи(rdr);
        //    }
        //    //close reader
        //    rdr.Close();
        //    return c;
        //}
        ///// <summary>
        ///// NT-Прочитать данные элемента
        ///// </summary>
        ///// <param name="rdr">Ридер с данными</param>
        ///// <returns>Возвращает объект данных элемента</returns>
        //private КлассЗадачи readRowКлассЗадачи(OleDbDataReader rdr)
        //{
        //    КлассЗадачи c = new КлассЗадачи();
        //    //UAMX поля
        //    c.TableId = rdr.GetInt32(0);
        //    c.ElementId = new ИдентификаторЭлемента((Int32)rdr.GetInt32(1));
        //    c.IsActive = rdr.GetBoolean(2);
        //    c.ВебИмя = rdr.GetString(3);
        //    c.ДатаСоздания = rdr.GetDateTime(4);
        //    c.Название = rdr.GetString(5);
        //    c.Описание = rdr.GetString(6);
        //    c.Замечания = rdr.GetString(7);
        //    c.РодительскийЭлемент = new СсылкаНаЭлемент(rdr.GetString(8));
        //    //Собственные поля - нет
        //    c.МетодикаЗадачи = new МетодикаЗадачи(rdr.GetString(9));

        //    return c;
        //}
        //#endregion

        //#region ** TaskInfo functions **

        ///// <summary>
        ///// NT-Вставить элемент в таблицу
        ///// </summary>
        ///// <param name="s">Объект класса задачи</param>
        ///// <returns>Возвращает число затронутых записей.</returns>
        //public int InsertTaskInfo(TaskInfo s)
        //{
        //    //Вставить предмет в таблицу и вписать в объект идентификатор первичного ключа
        //    String query = "INSERT INTO " + DbAdapter.Table_TaskInfo + " (eid, active, webname, creadate, title, descr, remarks, parent, log, efold, result, plan, algo, tst, fin, begdate, findate, repeat, trud, warn, soder, src, stasks, ltasks, cls, cat, place, stoppit, wmode) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);";
        //    OleDbCommand cmd = this.m_cmdInsertTaskInfo;
        //    //create command
        //    if (cmd == null)
        //    {
        //        cmd = new OleDbCommand(query, this.m_connection, m_transaction);
        //        cmd.CommandTimeout = 60;
        //        //UAMX fields
        //        cmd.Parameters.Add("@eid", OleDbType.Integer);
        //        cmd.Parameters.Add("@active", OleDbType.Boolean);
        //        cmd.Parameters.Add("@webname", OleDbType.WChar);
        //        cmd.Parameters.Add("@creadate", OleDbType.Date);
        //        cmd.Parameters.Add("@title", OleDbType.WChar);
        //        cmd.Parameters.Add("@descr", OleDbType.WChar);
        //        cmd.Parameters.Add("@remarks", OleDbType.WChar);
        //        cmd.Parameters.Add("@parent", OleDbType.WChar);
        //        //TaskBase fields
        //        cmd.Parameters.Add("@log", OleDbType.WChar);
        //        cmd.Parameters.Add("@efold", OleDbType.WChar);
        //        cmd.Parameters.Add("@result", OleDbType.WChar);
        //        cmd.Parameters.Add("@plan", OleDbType.WChar);
        //        cmd.Parameters.Add("@algo", OleDbType.WChar);
        //        cmd.Parameters.Add("@tst", OleDbType.WChar);
        //        cmd.Parameters.Add("@fin", OleDbType.Integer);
        //        cmd.Parameters.Add("@begdate", OleDbType.Date);
        //        cmd.Parameters.Add("@findate", OleDbType.Date);
        //        cmd.Parameters.Add("@repeat", OleDbType.WChar);
        //        cmd.Parameters.Add("@trud", OleDbType.WChar);
        //        cmd.Parameters.Add("@warn", OleDbType.Integer);
        //        cmd.Parameters.Add("@soder", OleDbType.WChar);
        //        cmd.Parameters.Add("@src", OleDbType.WChar);
        //        cmd.Parameters.Add("@stasks", OleDbType.WChar);
        //        cmd.Parameters.Add("@ltasks", OleDbType.WChar);
        //        cmd.Parameters.Add("@cls", OleDbType.WChar);
        //        cmd.Parameters.Add("@cat", OleDbType.WChar);
        //        //TaskInfo fields
        //        cmd.Parameters.Add("@place", OleDbType.WChar);
        //        cmd.Parameters.Add("@stoppit", OleDbType.WChar);
        //        cmd.Parameters.Add("@wmode", OleDbType.WChar);
        //        //store back
        //        this.m_cmdInsertTaskInfo = cmd;
        //    }
        //    //set arguments UAMX
        //    cmd.Parameters[0].Value = (Int32)s.ElementId.Id;
        //    cmd.Parameters[1].Value = s.IsActive;
        //    cmd.Parameters[2].Value = s.ВебИмя;
        //    cmd.Parameters[3].Value = s.ДатаСоздания;
        //    cmd.Parameters[4].Value = s.Название;
        //    cmd.Parameters[5].Value = s.Описание;
        //    cmd.Parameters[6].Value = s.Замечания;
        //    cmd.Parameters[7].Value = s.РодительскийЭлемент.SerializeToDataString();
        //    //TaskBase fields
        //    cmd.Parameters[8].Value = s.Log.SerializeToDataString();
        //    cmd.Parameters[9].Value = s.ElementFolder;
        //    cmd.Parameters[10].Value = s.КонечныйРезультат;
        //    cmd.Parameters[11].Value = s.ПланРаботы;
        //    cmd.Parameters[12].Value = s.Алгоритм;
        //    cmd.Parameters[13].Value = s.ТекущееСостояниеЗадачи.SerializeToDataString();
        //    cmd.Parameters[14].Value = s.ПроцентЗавершенностиРаботы;//@fin
        //    cmd.Parameters[15].Value = s.ДатаНачала;
        //    cmd.Parameters[16].Value = s.ДатаОкончания;
        //    cmd.Parameters[17].Value = s.РежимПовтора.SerializeToDataString();
        //    cmd.Parameters[18].Value = s.Трудоемкость.SerializeToDataString();
        //    cmd.Parameters[19].Value = (Int32)s.Важность;//@warn
        //    cmd.Parameters[20].Value = s.СодержаниеРаботы;
        //    cmd.Parameters[21].Value = s.ИсточникЗадачи.SerializeToDataString();
        //    cmd.Parameters[22].Value = s.СписокПодчиненныхЗадач.SerializeToDataString();
        //    cmd.Parameters[23].Value = s.СписокСвязанныхЗадач.SerializeToDataString();
        //    cmd.Parameters[24].Value = s.СсылкаКлассЗадачи.SerializeToDataString();
        //    cmd.Parameters[25].Value = s.СсылкаКатегорияЗадачи.SerializeToDataString();
        //    //TaskInfo fields
        //    cmd.Parameters[26].Value = s.РабочееМестоЗадачи;
        //    cmd.Parameters[27].Value = s.МестоОстановкиЗадачи;
        //    cmd.Parameters[28].Value = s.РежимНапоминанияЗадачи.SerializeToDataString();
        //    //execute
        //    int result = cmd.ExecuteNonQuery();
        //    //TODO: тут надо бы выяснить и вписать в объект новый tableId
        //    //чтобы отличать несохраненный объект от уже сохраненного в БД.
        //    //а можно сначала вызывать UPDATE, а если он вернет 0 а не 1, то INSERT
        //    //но это конечно не поможет выявить, сохранен объект или нет.

        //    return result;
        //}
        ///// <summary>
        ///// NT-Обновить элемент
        ///// </summary>
        ///// <param name="s">Элемент</param>
        ///// <returns>Возвращает число затронутых записей.</returns>
        //public int UpdateTaskInfo(TaskInfo s)
        //{
        //    //Обновить запись элемента в таблице
        //    String query = "UPDATE " + DbAdapter.Table_TaskInfo + " SET active=?, webname=?, creadate=?, title=?, descr=?, remarks=?, parent=?, log=?, efold=?, result=?, plan=?, algo=?, tst=?, fin=?, begdate=?, findate=?, repeat=?, trud=?, warn=?, soder=?, src=?, stasks=?, ltasks=?, cls=?, cat=?, place=?, stoppit=?, wmode=? WHERE (eid = ?)";
        //    OleDbCommand cmd = this.m_cmdUpdateTaskInfo;
        //    //create command
        //    if (cmd == null)
        //    {
        //        cmd = new OleDbCommand(query, this.m_connection, m_transaction);
        //        cmd.CommandTimeout = 60;
        //        //UAMX fields
        //        cmd.Parameters.Add("@active", OleDbType.Boolean);
        //        cmd.Parameters.Add("@webname", OleDbType.WChar);
        //        cmd.Parameters.Add("@creadate", OleDbType.Date);
        //        cmd.Parameters.Add("@title", OleDbType.WChar);
        //        cmd.Parameters.Add("@descr", OleDbType.WChar);
        //        cmd.Parameters.Add("@remarks", OleDbType.WChar);
        //        cmd.Parameters.Add("@parent", OleDbType.WChar);
        //        //TaskBase fields
        //        cmd.Parameters.Add("@log", OleDbType.WChar);
        //        cmd.Parameters.Add("@efold", OleDbType.WChar);
        //        cmd.Parameters.Add("@result", OleDbType.WChar);
        //        cmd.Parameters.Add("@plan", OleDbType.WChar);
        //        cmd.Parameters.Add("@algo", OleDbType.WChar);
        //        cmd.Parameters.Add("@tst", OleDbType.WChar);
        //        cmd.Parameters.Add("@fin", OleDbType.Integer);
        //        cmd.Parameters.Add("@begdate", OleDbType.Date);
        //        cmd.Parameters.Add("@findate", OleDbType.Date);
        //        cmd.Parameters.Add("@repeat", OleDbType.WChar);
        //        cmd.Parameters.Add("@trud", OleDbType.WChar);
        //        cmd.Parameters.Add("@warn", OleDbType.Integer);
        //        cmd.Parameters.Add("@soder", OleDbType.WChar);
        //        cmd.Parameters.Add("@src", OleDbType.WChar);
        //        cmd.Parameters.Add("@stasks", OleDbType.WChar);
        //        cmd.Parameters.Add("@ltasks", OleDbType.WChar);
        //        cmd.Parameters.Add("@cls", OleDbType.WChar);
        //        cmd.Parameters.Add("@cat", OleDbType.WChar);
        //        //TaskInfo fields
        //        cmd.Parameters.Add("@place", OleDbType.WChar);
        //        cmd.Parameters.Add("@stoppit", OleDbType.WChar);
        //        cmd.Parameters.Add("@wmode", OleDbType.WChar);
        //        //
        //        cmd.Parameters.Add("@eid", OleDbType.Integer);
        //        //store back
        //        this.m_cmdUpdateTaskInfo = cmd;
        //    }
        //    //execute command
        //    cmd.Parameters[0].Value = s.IsActive;
        //    cmd.Parameters[1].Value = s.ВебИмя;
        //    cmd.Parameters[2].Value = s.ДатаСоздания;
        //    cmd.Parameters[3].Value = s.Название;
        //    cmd.Parameters[4].Value = s.Описание;
        //    cmd.Parameters[5].Value = s.Замечания;
        //    cmd.Parameters[6].Value = s.РодительскийЭлемент.SerializeToDataString();
        //    //TaskBase fields
        //    cmd.Parameters[7].Value = s.Log.SerializeToDataString();
        //    cmd.Parameters[8].Value = s.ElementFolder;
        //    cmd.Parameters[9].Value = s.КонечныйРезультат;
        //    cmd.Parameters[10].Value = s.ПланРаботы;
        //    cmd.Parameters[11].Value = s.Алгоритм;
        //    cmd.Parameters[12].Value = s.ТекущееСостояниеЗадачи.SerializeToDataString();
        //    cmd.Parameters[13].Value = s.ПроцентЗавершенностиРаботы;//@fin
        //    cmd.Parameters[14].Value = s.ДатаНачала;
        //    cmd.Parameters[15].Value = s.ДатаОкончания;
        //    cmd.Parameters[16].Value = s.РежимПовтора.SerializeToDataString();
        //    cmd.Parameters[17].Value = s.Трудоемкость.SerializeToDataString();
        //    cmd.Parameters[18].Value = (Int32)s.Важность;//@warn
        //    cmd.Parameters[19].Value = s.СодержаниеРаботы;
        //    cmd.Parameters[20].Value = s.ИсточникЗадачи.SerializeToDataString();
        //    cmd.Parameters[21].Value = s.СписокПодчиненныхЗадач.SerializeToDataString();
        //    cmd.Parameters[22].Value = s.СписокСвязанныхЗадач.SerializeToDataString();
        //    cmd.Parameters[23].Value = s.СсылкаКлассЗадачи.SerializeToDataString();
        //    cmd.Parameters[24].Value = s.СсылкаКатегорияЗадачи.SerializeToDataString();
        //    //TaskInfo fields
        //    cmd.Parameters[25].Value = s.РабочееМестоЗадачи;
        //    cmd.Parameters[26].Value = s.МестоОстановкиЗадачи;
        //    cmd.Parameters[27].Value = s.РежимНапоминанияЗадачи.SerializeToDataString();
        //    //
        //    cmd.Parameters[28].Value = (Int32)s.ElementId.Id;
        //    //execute
        //    int result = cmd.ExecuteNonQuery();

        //    return result;
        //}

        ///// <summary>
        ///// NT-Получить объект по ИД элемента
        ///// </summary>
        ///// <param name="id">Идентификатор элемента</param>
        ///// <returns>Возвращает объект или null</returns>
        //public TaskInfo getTaskInfoByElementId(ИдентификаторЭлемента id)
        //{
        //    //SELECT * FROM Items WHERE (eid = ?)
        //    if (m_cmdWithoutArguments == null)
        //    {
        //        m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
        //        m_cmdWithoutArguments.CommandTimeout = 60;
        //    }
        //    //create command
        //    string query = String.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0) WHERE (eid = {1});", DbAdapter.Table_TaskInfo, id.Id.ToString());
        //    m_cmdWithoutArguments.CommandText = query;
        //    //execute command
        //    //read single row results
        //    OleDbDataReader rdr = m_cmdWithoutArguments.ExecuteReader();
        //    TaskInfo c = null;
        //    if (rdr.HasRows)
        //    {
        //        rdr.Read(); //only first row
        //        c = readRowTaskInfo(rdr);
        //    }
        //    //close reader
        //    rdr.Close();
        //    return c;
        //}
        ///// <summary>
        ///// NT-Прочитать данные элемента
        ///// </summary>
        ///// <param name="rdr">Ридер с данными</param>
        ///// <returns>Возвращает объект данных элемента</returns>
        //private TaskInfo readRowTaskInfo(OleDbDataReader rdr)
        //{
        //    TaskInfo c = new TaskInfo();
        //    //UAMX поля
        //    c.TableId = rdr.GetInt32(0);
        //    c.ElementId = new ИдентификаторЭлемента((Int32)rdr.GetInt32(1));
        //    c.IsActive = rdr.GetBoolean(2);
        //    c.ВебИмя = rdr.GetString(3);
        //    c.ДатаСоздания = rdr.GetDateTime(4);
        //    c.Название = rdr.GetString(5);
        //    c.Описание = rdr.GetString(6);
        //    c.Замечания = rdr.GetString(7);
        //    c.РодительскийЭлемент = new СсылкаНаЭлемент(rdr.GetString(8));
        //    //TaskBase поля 
        //    c.Log = new ЛогЗадачи(rdr.GetString(9));
        //    c.ElementFolder = rdr.GetString(10);
        //    c.КонечныйРезультат = rdr.GetString(11);
        //    c.ПланРаботы = rdr.GetString(12);
        //    c.Алгоритм = rdr.GetString(13);
        //    c.ТекущееСостояниеЗадачи = new КлассСостоянияЗадачи(rdr.GetString(14));
        //    c.ПроцентЗавершенностиРаботы = rdr.GetInt32(15);
        //    c.ДатаНачала = rdr.GetDateTime(16);
        //    c.ДатаОкончания = rdr.GetDateTime(17);
        //    c.РежимПовтора = new КлассРежимПовтораЗадачи(rdr.GetString(18));
        //    c.Трудоемкость = new КлассТрудоемкостьЗадачи(rdr.GetString(19));
        //    c.Важность = (ЕнумВажностьЗадачи)rdr.GetInt32(20);
        //    c.СодержаниеРаботы = rdr.GetString(21);
        //    c.ИсточникЗадачи = new КлассИсточникЗадачи(rdr.GetString(22));
        //    c.СписокПодчиненныхЗадач = new КлассСписокСсылокНаЭлементы(rdr.GetString(23));
        //    c.СписокСвязанныхЗадач = new КлассСписокСсылокНаЭлементы(rdr.GetString(24));
        //    c.СсылкаКлассЗадачи = new СсылкаНаЭлемент(rdr.GetString(25));
        //    c.СсылкаКатегорияЗадачи = new СсылкаНаЭлемент(rdr.GetString(26));
        //    //Поля для TaskInfo
        //    c.РабочееМестоЗадачи = rdr.GetString(27);
        //    c.МестоОстановкиЗадачи = rdr.GetString(28);
        //    c.РежимНапоминанияЗадачи = new КлассРежимНапоминанияЗадачи(rdr.GetString(29));

        //    return c;
        //}
        //#endregion

        //#region ** ProjectInfo functions **

        ///// <summary>
        ///// NT-Вставить элемент в таблицу
        ///// </summary>
        ///// <param name="s">Объект класса задачи</param>
        ///// <returns>Возвращает число затронутых записей.</returns>
        //public int InsertProjectInfo(ProjectInfo s)
        //{
        //    //Вставить предмет в таблицу и вписать в объект идентификатор первичного ключа - 30 столбцов
        //    String query = "INSERT INTO " + DbAdapter.Table_ProjectInfo + " (eid, active, webname, creadate, title, descr, remarks, parent, log, efold, result, plan, algo, tst, fin, begdate, findate, repeat, trud, warn, soder, src, stasks, ltasks, cls, cat, ver, prich, usein, sprjs) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);";
        //    OleDbCommand cmd = this.m_cmdInsertProjectInfo;
        //    //create command
        //    if (cmd == null)
        //    {
        //        cmd = new OleDbCommand(query, this.m_connection, m_transaction);
        //        cmd.CommandTimeout = 60;
        //        //UAMX fields
        //        cmd.Parameters.Add("@eid", OleDbType.Integer);
        //        cmd.Parameters.Add("@active", OleDbType.Boolean);
        //        cmd.Parameters.Add("@webname", OleDbType.WChar);
        //        cmd.Parameters.Add("@creadate", OleDbType.Date);
        //        cmd.Parameters.Add("@title", OleDbType.WChar);
        //        cmd.Parameters.Add("@descr", OleDbType.WChar);
        //        cmd.Parameters.Add("@remarks", OleDbType.WChar);
        //        cmd.Parameters.Add("@parent", OleDbType.WChar);
        //        //TaskBase fields
        //        cmd.Parameters.Add("@log", OleDbType.WChar);
        //        cmd.Parameters.Add("@efold", OleDbType.WChar);
        //        cmd.Parameters.Add("@result", OleDbType.WChar);
        //        cmd.Parameters.Add("@plan", OleDbType.WChar);
        //        cmd.Parameters.Add("@algo", OleDbType.WChar);
        //        cmd.Parameters.Add("@tst", OleDbType.WChar);
        //        cmd.Parameters.Add("@fin", OleDbType.Integer);
        //        cmd.Parameters.Add("@begdate", OleDbType.Date);
        //        cmd.Parameters.Add("@findate", OleDbType.Date);
        //        cmd.Parameters.Add("@repeat", OleDbType.WChar);
        //        cmd.Parameters.Add("@trud", OleDbType.WChar);
        //        cmd.Parameters.Add("@warn", OleDbType.Integer);
        //        cmd.Parameters.Add("@soder", OleDbType.WChar);
        //        cmd.Parameters.Add("@src", OleDbType.WChar);
        //        cmd.Parameters.Add("@stasks", OleDbType.WChar);
        //        cmd.Parameters.Add("@ltasks", OleDbType.WChar);
        //        cmd.Parameters.Add("@cls", OleDbType.WChar);
        //        cmd.Parameters.Add("@cat", OleDbType.WChar);
        //        //ProjectInfo fields
        //        cmd.Parameters.Add("@ver", OleDbType.WChar);
        //        cmd.Parameters.Add("@prich", OleDbType.WChar);
        //        cmd.Parameters.Add("@usein", OleDbType.WChar);
        //        cmd.Parameters.Add("@sprjs", OleDbType.WChar);
        //        //store back
        //        this.m_cmdInsertProjectInfo = cmd;
        //    }
        //    //set arguments UAMX
        //    cmd.Parameters[0].Value = (Int32)s.ElementId.Id;
        //    cmd.Parameters[1].Value = s.IsActive;
        //    cmd.Parameters[2].Value = s.ВебИмя;
        //    cmd.Parameters[3].Value = s.ДатаСоздания;
        //    cmd.Parameters[4].Value = s.Название;
        //    cmd.Parameters[5].Value = s.Описание;
        //    cmd.Parameters[6].Value = s.Замечания;
        //    cmd.Parameters[7].Value = s.РодительскийЭлемент.SerializeToDataString();
        //    //TaskBase fields
        //    cmd.Parameters[8].Value = s.Log.SerializeToDataString();
        //    cmd.Parameters[9].Value = s.ElementFolder;
        //    cmd.Parameters[10].Value = s.КонечныйРезультат;
        //    cmd.Parameters[11].Value = s.ПланРаботы;
        //    cmd.Parameters[12].Value = s.Алгоритм;
        //    cmd.Parameters[13].Value = s.ТекущееСостояниеЗадачи.SerializeToDataString();
        //    cmd.Parameters[14].Value = s.ПроцентЗавершенностиРаботы;//@fin
        //    cmd.Parameters[15].Value = s.ДатаНачала;
        //    cmd.Parameters[16].Value = s.ДатаОкончания;
        //    cmd.Parameters[17].Value = s.РежимПовтора.SerializeToDataString();
        //    cmd.Parameters[18].Value = s.Трудоемкость.SerializeToDataString();
        //    cmd.Parameters[19].Value = (Int32)s.Важность;//@warn
        //    cmd.Parameters[20].Value = s.СодержаниеРаботы;
        //    cmd.Parameters[21].Value = s.ИсточникЗадачи.SerializeToDataString();
        //    cmd.Parameters[22].Value = s.СписокПодчиненныхЗадач.SerializeToDataString();
        //    cmd.Parameters[23].Value = s.СписокСвязанныхЗадач.SerializeToDataString();
        //    cmd.Parameters[24].Value = s.СсылкаКлассЗадачи.SerializeToDataString();
        //    cmd.Parameters[25].Value = s.СсылкаКатегорияЗадачи.SerializeToDataString();
        //    //ProjectInfo fields
        //    cmd.Parameters[26].Value = s.ВерсияПроекта.ToString();
        //    cmd.Parameters[27].Value = s.ПричинаСозданияПроекта;
        //    cmd.Parameters[28].Value = s.СитуацияИспользованияПроекта;
        //    cmd.Parameters[29].Value = s.СписокПодчиненныхПроектов.SerializeToDataString();
        //    //execute
        //    int result = cmd.ExecuteNonQuery();
        //    //TODO: тут надо бы выяснить и вписать в объект новый tableId
        //    //чтобы отличать несохраненный объект от уже сохраненного в БД.
        //    //а можно сначала вызывать UPDATE, а если он вернет 0 а не 1, то INSERT
        //    //но это конечно не поможет выявить, сохранен объект или нет.

        //    return result;
        //}
        ///// <summary>
        ///// NT-Обновить элемент
        ///// </summary>
        ///// <param name="s">Элемент</param>
        ///// <returns>Возвращает число затронутых записей.</returns>
        //public int UpdateProjectInfo(ProjectInfo s)
        //{
        //    //Обновить запись элемента в таблице
        //    String query = "UPDATE " + DbAdapter.Table_ProjectInfo + " SET active=?, webname=?, creadate=?, title=?, descr=?, remarks=?, parent=?, log=?, efold=?, result=?, plan=?, algo=?, tst=?, fin=?, begdate=?, findate=?, repeat=?, trud=?, warn=?, soder=?, src=?, stasks=?, ltasks=?, cls=?, cat=?, ver=?, prich=?, usein=?, sprjs=? WHERE (eid = ?)";
        //    OleDbCommand cmd = this.m_cmdUpdateProjectInfo;
        //    //create command
        //    if (cmd == null)
        //    {
        //        cmd = new OleDbCommand(query, this.m_connection, m_transaction);
        //        cmd.CommandTimeout = 60;
        //        //UAMX fields
        //        cmd.Parameters.Add("@active", OleDbType.Boolean);
        //        cmd.Parameters.Add("@webname", OleDbType.WChar);
        //        cmd.Parameters.Add("@creadate", OleDbType.Date);
        //        cmd.Parameters.Add("@title", OleDbType.WChar);
        //        cmd.Parameters.Add("@descr", OleDbType.WChar);
        //        cmd.Parameters.Add("@remarks", OleDbType.WChar);
        //        cmd.Parameters.Add("@parent", OleDbType.WChar);
        //        //TaskBase fields
        //        cmd.Parameters.Add("@log", OleDbType.WChar);
        //        cmd.Parameters.Add("@efold", OleDbType.WChar);
        //        cmd.Parameters.Add("@result", OleDbType.WChar);
        //        cmd.Parameters.Add("@plan", OleDbType.WChar);
        //        cmd.Parameters.Add("@algo", OleDbType.WChar);
        //        cmd.Parameters.Add("@tst", OleDbType.WChar);
        //        cmd.Parameters.Add("@fin", OleDbType.Integer);
        //        cmd.Parameters.Add("@begdate", OleDbType.Date);
        //        cmd.Parameters.Add("@findate", OleDbType.Date);
        //        cmd.Parameters.Add("@repeat", OleDbType.WChar);
        //        cmd.Parameters.Add("@trud", OleDbType.WChar);
        //        cmd.Parameters.Add("@warn", OleDbType.Integer);
        //        cmd.Parameters.Add("@soder", OleDbType.WChar);
        //        cmd.Parameters.Add("@src", OleDbType.WChar);
        //        cmd.Parameters.Add("@stasks", OleDbType.WChar);
        //        cmd.Parameters.Add("@ltasks", OleDbType.WChar);
        //        cmd.Parameters.Add("@cls", OleDbType.WChar);
        //        cmd.Parameters.Add("@cat", OleDbType.WChar);
        //        //ProjectInfo fields
        //        cmd.Parameters.Add("@ver", OleDbType.WChar);
        //        cmd.Parameters.Add("@prich", OleDbType.WChar);
        //        cmd.Parameters.Add("@usein", OleDbType.WChar);
        //        cmd.Parameters.Add("@sprjs", OleDbType.WChar);
        //        //
        //        cmd.Parameters.Add("@eid", OleDbType.Integer);
        //        //store back
        //        this.m_cmdUpdateProjectInfo = cmd;
        //    }
        //    //execute command
        //    cmd.Parameters[0].Value = s.IsActive;
        //    cmd.Parameters[1].Value = s.ВебИмя;
        //    cmd.Parameters[2].Value = s.ДатаСоздания;
        //    cmd.Parameters[3].Value = s.Название;
        //    cmd.Parameters[4].Value = s.Описание;
        //    cmd.Parameters[5].Value = s.Замечания;
        //    cmd.Parameters[6].Value = s.РодительскийЭлемент.SerializeToDataString();
        //    //TaskBase fields
        //    cmd.Parameters[7].Value = s.Log.SerializeToDataString();
        //    cmd.Parameters[8].Value = s.ElementFolder;
        //    cmd.Parameters[9].Value = s.КонечныйРезультат;
        //    cmd.Parameters[10].Value = s.ПланРаботы;
        //    cmd.Parameters[11].Value = s.Алгоритм;
        //    cmd.Parameters[12].Value = s.ТекущееСостояниеЗадачи.SerializeToDataString();
        //    cmd.Parameters[13].Value = s.ПроцентЗавершенностиРаботы;//@fin
        //    cmd.Parameters[14].Value = s.ДатаНачала;
        //    cmd.Parameters[15].Value = s.ДатаОкончания;
        //    cmd.Parameters[16].Value = s.РежимПовтора.SerializeToDataString();
        //    cmd.Parameters[17].Value = s.Трудоемкость.SerializeToDataString();
        //    cmd.Parameters[18].Value = (Int32)s.Важность;//@warn
        //    cmd.Parameters[19].Value = s.СодержаниеРаботы;
        //    cmd.Parameters[20].Value = s.ИсточникЗадачи.SerializeToDataString();
        //    cmd.Parameters[21].Value = s.СписокПодчиненныхЗадач.SerializeToDataString();
        //    cmd.Parameters[22].Value = s.СписокСвязанныхЗадач.SerializeToDataString();
        //    cmd.Parameters[23].Value = s.СсылкаКлассЗадачи.SerializeToDataString();
        //    cmd.Parameters[24].Value = s.СсылкаКатегорияЗадачи.SerializeToDataString();
        //    //ProjectInfo fields
        //    cmd.Parameters[25].Value = s.ВерсияПроекта.ToString();
        //    cmd.Parameters[26].Value = s.ПричинаСозданияПроекта;
        //    cmd.Parameters[27].Value = s.СитуацияИспользованияПроекта;
        //    cmd.Parameters[28].Value = s.СписокПодчиненныхПроектов.SerializeToDataString();
        //    //
        //    cmd.Parameters[29].Value = (Int32)s.ElementId.Id;
        //    //execute
        //    int result = cmd.ExecuteNonQuery();

        //    return result;
        //}

        ///// <summary>
        ///// NT-Получить объект по ИД элемента
        ///// </summary>
        ///// <param name="id">Идентификатор элемента</param>
        ///// <returns>Возвращает объект или null</returns>
        //public ProjectInfo getProjectInfoByElementId(ИдентификаторЭлемента id)
        //{
        //    //SELECT * FROM Items WHERE (eid = ?)
        //    if (m_cmdWithoutArguments == null)
        //    {
        //        m_cmdWithoutArguments = new OleDbCommand(String.Empty, this.m_connection, m_transaction);
        //        m_cmdWithoutArguments.CommandTimeout = 60;
        //    }
        //    //create command
        //    string query = String.Format(CultureInfo.InvariantCulture, "SELECT * FROM {0) WHERE (eid = {1});", DbAdapter.Table_ProjectInfo, id.Id.ToString());
        //    m_cmdWithoutArguments.CommandText = query;
        //    //execute command
        //    //read single row results
        //    OleDbDataReader rdr = m_cmdWithoutArguments.ExecuteReader();
        //    ProjectInfo c = null;
        //    if (rdr.HasRows)
        //    {
        //        rdr.Read(); //only first row
        //        c = readRowProjectInfo(rdr);
        //    }
        //    //close reader
        //    rdr.Close();
        //    return c;
        //}
        ///// <summary>
        ///// NT-Прочитать данные элемента
        ///// </summary>
        ///// <param name="rdr">Ридер с данными</param>
        ///// <returns>Возвращает объект данных элемента</returns>
        //private ProjectInfo readRowProjectInfo(OleDbDataReader rdr)
        //{
        //    ProjectInfo c = new ProjectInfo();
        //    //UAMX поля
        //    c.TableId = rdr.GetInt32(0);
        //    c.ElementId = new ИдентификаторЭлемента((Int32)rdr.GetInt32(1));
        //    c.IsActive = rdr.GetBoolean(2);
        //    c.ВебИмя = rdr.GetString(3);
        //    c.ДатаСоздания = rdr.GetDateTime(4);
        //    c.Название = rdr.GetString(5);
        //    c.Описание = rdr.GetString(6);
        //    c.Замечания = rdr.GetString(7);
        //    c.РодительскийЭлемент = new СсылкаНаЭлемент(rdr.GetString(8));
        //    //TaskBase поля 
        //    c.Log = new ЛогЗадачи(rdr.GetString(9));
        //    c.ElementFolder = rdr.GetString(10);
        //    c.КонечныйРезультат = rdr.GetString(11);
        //    c.ПланРаботы = rdr.GetString(12);
        //    c.Алгоритм = rdr.GetString(13);
        //    c.ТекущееСостояниеЗадачи = new КлассСостоянияЗадачи(rdr.GetString(14));
        //    c.ПроцентЗавершенностиРаботы = rdr.GetInt32(15);
        //    c.ДатаНачала = rdr.GetDateTime(16);
        //    c.ДатаОкончания = rdr.GetDateTime(17);
        //    c.РежимПовтора = new КлассРежимПовтораЗадачи(rdr.GetString(18));
        //    c.Трудоемкость = new КлассТрудоемкостьЗадачи(rdr.GetString(19));
        //    c.Важность = (ЕнумВажностьЗадачи)rdr.GetInt32(20);
        //    c.СодержаниеРаботы = rdr.GetString(21);
        //    c.ИсточникЗадачи = new КлассИсточникЗадачи(rdr.GetString(22));
        //    c.СписокПодчиненныхЗадач = new КлассСписокСсылокНаЭлементы(rdr.GetString(23));
        //    c.СписокСвязанныхЗадач = new КлассСписокСсылокНаЭлементы(rdr.GetString(24));
        //    c.СсылкаКлассЗадачи = new СсылкаНаЭлемент(rdr.GetString(25));
        //    c.СсылкаКатегорияЗадачи = new СсылкаНаЭлемент(rdr.GetString(26));
        //    //ProjectInfo поля
        //    c.ВерсияПроекта = new Version(rdr.GetString(27));
        //    c.ПричинаСозданияПроекта = rdr.GetString(28);
        //    c.СитуацияИспользованияПроекта = rdr.GetString(29);
        //    c.СписокПодчиненныхПроектов = new КлассСписокСсылокНаЭлементы(rdr.GetString(30));

        //    return c;
        //}
        //#endregion

        ///// <summary>
        ///// NT-Удалить элемент по его идентификатору элемента
        ///// </summary>
        ///// <param name="id">Идентификатор элемента</param>
        ///// <returns>Возвращает число удаленных записей.</returns>
        //public int DeleteElement(ИдентификаторЭлемента id)
        //{
        //    //поскольку неизвестно, в какой таблице этот элемент, пробуем удалить поочередно из каждой таблицы.
        //    //и если откуда-то удалился, то выходим.

        //    //удаление по веб-имени нельзя сделать так же, поскольку нельзя веб-имя просто подставить в строку запроса
            
        //    int idd = (Int32)id.Id;
        //    int r1 = this.DeleteRow(DbAdapter.Table_TaskInfo, "eid", idd);
        //    if (r1 > 0) return r1;
        //    //else
        //    r1 = this.DeleteRow(DbAdapter.Table_ProjectInfo, "eid", idd);
        //    if (r1 > 0) return r1;
            
        //    r1 = this.DeleteRow(DbAdapter.Table_TaskClass, "eid", idd);
        //    if (r1 > 0) return r1; 
            
        //    r1 = this.DeleteRow(DbAdapter.Table_Category, "eid", idd);
        //    if (r1 > 0) return r1;

        //    return 0;//запись не найдена
        //}













    }
}
