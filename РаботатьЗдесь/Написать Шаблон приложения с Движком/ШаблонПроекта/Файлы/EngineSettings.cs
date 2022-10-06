using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Globalization;
using System.IO;
using System.Threading;

namespace PMEngine
{
    /// <summary>
    /// NT-Это класс, управляющий файлом настроек проекта. 
    /// </summary>
    /// <remarks>
    /// Содержит словарь, в котором хранятся все значения настроек, 
    /// функции для работы с файлом настроек проекта и проперти для доступа к настройкам.
    /// Читает или создает файл настроек проекта.
    /// TODO: класс функционально недопроектирован. Надо его перепроектировать в конце разработки движка.
    /// </remarks>
    public class EngineSettings : RecordBase
    {
        //для добавления нового члена в файл настроек:
        //1 добавить в константы новый тег для этого члена настроек.
        //2 создать проперти для доступа к этому члену настроек, по образцу.
        //3 добавить в конструкторе класса значение по умолчанию для этого члена.
        //4 добавить в функцию StoreToFile вывод этого члена в правильном месте.
        //5 добавить в функцию  LoadFromFile чтение  этого члена в правильном месте.
        //* если автоматически выводить настройки в файл, то они будут там вперемешку и файл неудобно будет читать.
        //   поэтому я их тут вручную вывожу в файл настроек в правильном порядке.

        #region Константы названий тегов словаря
        //названия свойств писать только латинскими буквами!
        internal const string tagStoragePath = "StoragePath";
        internal const string tagReadOnly = "ReadOnly";
        internal const string tagEngineVersion = "EngineVersion";
        internal const string tagEngineClass = "EngineClass";//класс движка - надо же как-то различать файлы настроек разных движков.
        internal const string tagCreator = "Creator";
        internal const string tagTitle = "Title";        
        internal const string tagDescription = "Description";
        
        //всего ХЗ штук
        #endregion
        /// <summary>
        /// Стандартное название файла настроек проекта
        /// </summary>
        internal const String DescriptionFileName = "settings.xml";

        /// <summary>
        /// NR-Конструктор
        /// </summary>
        public EngineSettings()
        {
            //fill dictionary with default items
            m_dictionary.Clear();
            m_dictionary.Add(tagStoragePath, String.Empty);
            m_dictionary.Add(tagReadOnly, Boolean.FalseString);
            m_dictionary.Add(tagEngineVersion, Engine.getEngineVersionString());
            m_dictionary.Add(tagCreator, "Павел Селяков");
            m_dictionary.Add(tagTitle, String.Empty);
            m_dictionary.Add(tagDescription, String.Empty);
            m_dictionary.Add(tagEngineClass, Engine.getEngineClass());
            //total ХЗ items
            return;
        }

        #region *** Properties ***
        /// <summary>
        /// Путь к корневому каталогу данных проекта
        /// </summary>
        public string StoragePath
        {
            get
            {
                return this.m_dictionary[tagStoragePath];
            }
            set
            {
                m_dictionary[tagStoragePath] = value;
            }
        }

        /// <summary>
        /// read-only flag
        /// </summary>
        /// <remarks>По умолчанию значение false.</remarks>
        public bool ReadOnly
        {
            get
            {
                return this.getValueAsBoolean(tagReadOnly);
            }
            internal set
            {
                this.setValue(tagReadOnly, value);
            }
        }

        /// <summary>
        /// Версия движка
        /// </summary>
        public string EngineVersion
        {
            get
            {
                return this.m_dictionary[tagEngineVersion];
            }
        }

        /// <summary>
        /// Класс движка
        /// </summary>
        public string EngineClass
        {
            get
            {
                return this.m_dictionary[tagEngineClass];
            }
        }

        /// <summary>
        /// Создатель Хранилища
        /// </summary>
        public string Creator
        {
            get
            {
                return this.m_dictionary[tagCreator];
            }
            set
            {
                m_dictionary[tagCreator] = value;
            }
        }

        /// <summary>
        /// Название Проекта краткое
        /// </summary>
        public string Title
        {
            get
            {
                return this.m_dictionary[tagTitle];
            }
            set
            {
                m_dictionary[tagTitle] = value;
            }
        }

        /// <summary>
        /// Текст описания Хранилища
        /// </summary>
        public string Description
        {
            get
            {
                return this.m_dictionary[tagDescription];
            }
            set
            {
                m_dictionary[tagDescription] = value;
            }
        }


        //эти оставлены для примера использования
        ///// <summary>
        ///// Размер архивов документов
        ///// </summary>
        //public Int64 DocsSize
        //{
        //    get
        //    {
        //        return Int64.Parse(this.m_dictionary[tagDocsSize], CultureInfo.InvariantCulture);
        //    }
        //    internal set
        //    {
        //        m_dictionary[tagDocsSize] = value.ToString(CultureInfo.InvariantCulture);
        //    }
        //}

        ///// <summary>
        ///// Число файлов в архивах документов
        ///// </summary>
        //public int DocsCount
        //{
        //    get
        //    {
        //        return Int32.Parse(this.m_dictionary[tagDocsCount], CultureInfo.InvariantCulture);
        //    }
        //    internal set
        //    {
        //        m_dictionary[tagDocsCount] = value.ToString(CultureInfo.InvariantCulture);
        //    }
        //}

        #endregion

        #region *** Functions ***
        /// <summary>
        /// NT-Загрузить файл настроек движка,
        /// проверить значения настроек на допустимость.
        /// </summary>
        /// <param name="storagePath">Корневой каталог проекта данных движка</param>
        /// <returns>Возвращает объект информации о хранилище.</returns>
        public static EngineSettings Load(String storagePath)
        {
            String filePath = Path.Combine(storagePath, EngineSettings.DescriptionFileName);
            return LoadFile(filePath, true);
        }
        /// <summary>
        /// NT-Загрузить файл настроек движка
        /// </summary>
        /// <param name="settingsFilePath">Путь к файлу настроек</param>
        /// <param name="validate">Проверить значения на допустимость</param>
        /// <returns>Возвращает объект информации о хранилище.</returns>
        public static EngineSettings LoadFile(String settingsFilePath, bool validate)
        {
            EngineSettings si = new EngineSettings();
            si.LoadFromFile(settingsFilePath);
            if(validate) si.Validate();
            return si;
        }

        /// <summary>
        /// NT-проверить что файл настроек читается и правильный.
        /// Исключений не выбрасывает.
        /// </summary>
        /// <param name="filePath">Путь к файлу настроек</param>
        /// <returns>Возвращает объект информации о хранилище или null</returns>
        public static EngineSettings TryLoad(string filePath)
        {
            EngineSettings result = null;
            try
            {
                result = LoadFile(filePath, true);
            }
            catch (Exception)
            {
                result = null;
            }
            return result;
        }

        /// <summary>
        /// NT-Сохранить настройки по текущему пути проекта
        /// </summary>
        /// <returns>Возвращает True если сохранение удалось. Иначе возвращает False.</returns>
        public bool Store()
        {
            //1 create new file or replace existing file
            string file = Path.Combine(this.StoragePath, EngineSettings.DescriptionFileName);
            return this.Store(file);
        }
        /// <summary>
        /// NT-Сохранить настройки по указанному пути
        /// </summary>
        /// <param name="file">Путь к файлу настроек</param>
        /// <returns>Возвращает True если сохранение удалось. Иначе возвращает False.</returns>
        public bool Store(string file)
        {
            //TODO: Тут надо быстро подменить файл если он не заблокирован.
            //несколько раз пытаемся его удалить, делая перерывы по 100мс.
            //если все же не удалось, то возвращаем неудачу.
            //но не исключение. - все же это не критическая ошибка.
            //если кто-то занял файл так надолго, то он его и обновит наверно тогда.

            //это одинаковый алгоритм для первого создания файла и для обновления файла
            for (int i = 0; i < 5; i++)
            {
                if (!File.Exists(file))
                    break;
                else
                {
                    try
                    {
                        File.Delete(file);
                        break;
                    }
                    catch (Exception)
                    {
                        ;
                    }
                    Thread.Sleep(100);
                }
            }
            //тут если удаление удалось, то надо перезаписать файл.
            if (!File.Exists(file))
            {
                //create file
                this.StoreToFile(file);
                return true;
            }
            else
                return false;
        }
       
        /// <summary>
        /// NFT-Записать данные в файл описания хранилища
        /// </summary>
        /// <param name="filepath"></param>
        private void StoreToFile(String filepath)
        {

            //1 write info to string builder first 
            StringBuilder sb = new StringBuilder();
            XmlWriterSettings s = new XmlWriterSettings();
            s.Encoding = Encoding.Unicode;
            s.Indent = true;
            XmlWriter wr = XmlWriter.Create(sb, s);
            wr.WriteStartElement("Settings");
            //Записать предупреждение пользователю о бесполезности редактирования файла
            wr.WriteElementString("DoNotEdit", "Не редактировать! Этот файл автоматически перезаписывается.");// - прочие свойства заменим на более простой синтаксис ниже
            //вывести остальные свойства хранилища
            _writeElement(wr, tagEngineClass);//вывести класс движка первым членом.
            _writeElement(wr, tagEngineVersion);//вывести версию движка вторым членом, по ней потом можно разобрать файл правильно.
            //остальные поля выводить в порядке, удобном для чтения
            _writeElement(wr, tagCreator);
            _writeElement(wr, tagTitle);
            _writeElement(wr, tagDescription);
            _writeElement(wr, tagStoragePath);
            _writeElement(wr, tagReadOnly);
            //TODO: добавить новые члены настроек здесь

            wr.WriteEndElement();
            wr.Flush();
            //2 get file and store to it fast
            //TODO: тут нужен стойкий многопроцессный код с учетом возможных блокировок файла.
            //а пока тут простое решение для отладки тестов.
            StreamWriter sw = new StreamWriter(filepath, false, Encoding.Unicode);//пишем в юникоде а то все жалуются.
            sw.Write(sb.ToString());
            sw.Close();

            return;
        }
        /// <summary>
        /// RT-вывести свойство в xml-поток. Служебная функция для более простого синтаксиса.
        /// </summary>
        /// <param name="wr">Xml-писатель</param>
        /// <param name="p">Название элемента словаря и свойства</param>
        private void _writeElement(XmlWriter wr, string p)
        {
            wr.WriteElementString(p, m_dictionary[p]);
        }


        /// <summary>
        /// RT-Загрузить данные из файла описания хранилища
        /// </summary>
        /// <param name="filepath">Путь к файлу описания хранилища</param>
        private void LoadFromFile(String filepath)
        {
            //1 - read file to string buffer
            StreamReader sr = new StreamReader(filepath, Encoding.Unicode);
            String xml = sr.ReadToEnd();
            sr.Close();
            //2 - parse xml to data
            XmlReaderSettings s = new XmlReaderSettings();
            s.CloseInput = true;
            s.IgnoreWhitespace = true;
            XmlReader rd = XmlReader.Create(new StringReader(xml), s);
            rd.Read();
            rd.ReadStartElement("Settings");

            //read file version info - Облом. XmlReader атрибуты не читает.

            //skip message for user
            rd.ReadStartElement("DoNotEdit");
            rd.ReadString();
            rd.ReadEndElement();
            //read list of properties
            _readElement(rd, tagEngineClass);//всегда первым членом настроек
            _readElement(rd, tagEngineVersion);//всегда вторым членом настроек, по нему можно определить дальнейший порядок полей файла.
            _readElement(rd, tagCreator);
            _readElement(rd, tagTitle);
            _readElement(rd, tagDescription);
            _readElement(rd, tagStoragePath);       
            _readElement(rd, tagReadOnly);
            //TODO: добавить новые члены настроек здесь

            //end of properties list
            rd.ReadEndElement();
            rd.Close();

            return;
        }

        /// <summary>
        /// NT-Проверить значения настроек и версию движка
        /// </summary>
        private void Validate()
        {
            //выбросить исключение, если:
            //-обязательные значения отсутствуют
            //-значения находятся вне допустимых пределов

            //check engine class
            if (!String.Equals(this.EngineClass, Engine.getEngineClass(), StringComparison.Ordinal))
                throw new Exception(String.Format("Несовместимый EngineClass: {0}", this.EngineClass));
            //check project engine version
            if(!Utility.isCompatibleEngineVersion(this.EngineVersion))
                throw new Exception(String.Format("Несовместимый EngineVersion: {0}", this.EngineVersion));
            //check engine data folder
            if(String.IsNullOrEmpty(this.StoragePath))
                throw new Exception("Недопустимый StoragePath");
            if(!Directory.Exists(this.StoragePath))
                throw new Exception(String.Format("Каталог StoragePath не найден: {0}", this.StoragePath));
            //check engine data project title
            if (String.IsNullOrEmpty(this.Title.Trim()))
                throw new Exception("Недопустимое название проекта");
            //TODO: Добавить проверки для новых членов файла настроек движка
            
            return;
        }


        /// <summary>
        /// NT-извлечь свойство из xml-потока. Служебная функция для более простого синтаксиса.
        /// </summary>
        /// <param name="rd"></param>
        /// <param name="p"></param>
        private void _readElement(XmlReader rd, string p)
        {
            rd.ReadStartElement(p);
            String s = rd.ReadString();
            m_dictionary[p] = s;
            rd.ReadEndElement();
            return;
        }


        /// <summary>
        /// NR-получить строковое представление объекта для отладчика
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return base.ToString();//TODO: add code here
        }
        #endregion




    }

    /// <summary>
    /// RT-Базовый класс переноса данных Хранилища
    /// </summary>
    public class RecordBase
    {
        /// <summary>
        /// Словарь для хранения данных в системе ключ-значение
        /// </summary>
        protected Dictionary<String, String> m_dictionary;

        public RecordBase()
        {
            m_dictionary = new Dictionary<string, string>(16);
        }

        ~RecordBase()
        {
            m_dictionary.Clear();
        }

        /// <summary>
        /// Получить внутренний словарь для прямого чтения
        /// </summary>
        /// <returns></returns>
        internal Dictionary<String, String> getBaseDictionary()
        {
            return m_dictionary;
        }

        protected void setValue(string name, long val)
        {
            this.m_dictionary[name] = val.ToString(CultureInfo.InvariantCulture);
        }

        protected void setValue(string name, int val)
        {
            this.m_dictionary[name] = val.ToString(CultureInfo.InvariantCulture);
        }

        protected void setValue(string name, UInt32 val)
        {
            this.m_dictionary[name] = val.ToString(CultureInfo.InvariantCulture);
        }

        internal void setValue(string name, string val)
        {
            this.m_dictionary[name] = val;
        }

        protected void setValue(string name, bool val)
        {
            this.m_dictionary[name] = val.ToString();
        }


        protected UInt32 getValueAsUInt32(string name)
        {
            return UInt32.Parse(this.m_dictionary[name], CultureInfo.InvariantCulture);
        }
        protected Int64 getValueAsInt64(string name)
        {
            return Int64.Parse(this.m_dictionary[name], CultureInfo.InvariantCulture);
        }
        protected Int32 getValueAsInt32(string name)
        {
            return Int32.Parse(this.m_dictionary[name], CultureInfo.InvariantCulture);
        }
        protected string getValueAsString(string name)
        {
            return this.m_dictionary[name];
        }
        protected bool getValueAsBoolean(string name)
        {
            return Boolean.Parse(this.m_dictionary[name]);
        }
    }


}
