using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Xml;
using System.Threading;
using System.Reflection;

namespace UAMX_2
{
    /// <summary>
    /// NT-Это класс, управляющий файлом настроек Хранилища. 
    /// </summary>
    /// <remarks>
    /// Содержит словарь, в котором хранятся все значения настроек, 
    /// функции для работы с файлом настроек Хранилища и проперти для доступа к настройкам.
    /// Читает или создает файл настроек Хранилища.
    /// TODO: класс функционально недопроектирован. Надо его перепроектировать в конце разработки движка.
    /// 
    /// Применение:
    /// - Создать производный класс
    /// - добавить в него Константы названий тегов словаря для новых полей настроек Хранилища.
    /// - Добавить в него Проперти для этих новых полей настроек Хранилища.
    /// - Добавить в конструкторе класса  значения по умолчанию для этих новых полей настроек Хранилища.
    /// - переопределить в нем функции  loadUserData, storeUserData, validateUserData, чтобы выводить и загружать и проверять новые поля настроек Хранилища, в правильном порядке. 
    /// * если автоматически выводить настройки в файл, то они будут там вперемешку и файл неудобно будет читать.
    ///   Поэтому я их тут вручную вывожу в файл настроек в правильном порядке.
    /// </remarks>
    public class EngineSettingsBase: RecordBase
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
        public const string tagStoragePath = "StoragePath";
        public const string tagReadOnly = "ReadOnly";
        public const string tagEngineVersion = "EngineVersion";
        /// <summary>
        /// Класс движка - чтобы различать Хранилища разных движков.
        /// </summary>
        public const string tagEngineClass = "EngineClass";//
        public const string tagCreator = "Creator";
        public const string tagTitle = "Title";
        public const string tagDescription = "Description";
        /// <summary>
        /// Префикс ссылки для СсылкаНаЭлемент и ИдентификаторЭлемента
        /// </summary>
        public const string tagLinkPrefix = "LinkPrefix";

        //всего ХЗ штук
        #endregion

        /// <summary>
        /// Стандартное название файла настроек проекта
        /// </summary>
        public const String DescriptionFileName = "settings.xml";

        /// <summary>
        /// NR-Конструктор
        /// </summary>
        public EngineSettingsBase(): base()
        {
            //fill dictionary with default items
            m_dictionary.Clear();
            m_dictionary.Add(tagStoragePath, String.Empty);
            m_dictionary.Add(tagReadOnly, Boolean.FalseString);
            m_dictionary.Add(tagEngineVersion, this.getCurrentEngineVersionString());
            m_dictionary.Add(tagCreator, "Павел Селяков");
            m_dictionary.Add(tagTitle, String.Empty);
            m_dictionary.Add(tagDescription, String.Empty);
            m_dictionary.Add(tagEngineClass, this.getEngineClass());
            m_dictionary.Add(tagLinkPrefix, "link");
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
            set
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

        /// <summary>
        /// Префикс ссылки на Элемент Хранилища
        /// </summary>
        public string LinkPrefix
        {
            get
            {
                return this.m_dictionary[tagLinkPrefix];
            }
            set
            {
                m_dictionary[tagLinkPrefix] = value;
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

        

        /// <summary>
        /// NT-получить строковое представление объекта для отладчика
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return base.ToString();//показывает число записей в словаре
        }

#region Парадные функции загрузки и выгрузки файла
        /// <summary>
        /// NT-Загрузить файл настроек движка,
        /// проверить значения настроек на допустимость.
        /// </summary>
        /// <param name="storagePath">Корневой каталог проекта данных движка</param>
        /// <returns>Возвращает объект информации о хранилище.</returns>
        public static new EngineSettingsBase Load(String storagePath)
        {
            String filePath = Path.Combine(storagePath, EngineSettingsBase.DescriptionFileName);
            return LoadFile(filePath, true);
        }
        /// <summary>
        /// NT-Загрузить файл настроек движка
        /// </summary>
        /// <param name="settingsFilePath">Путь к файлу настроек</param>
        /// <param name="validate">Проверить значения на допустимость</param>
        /// <returns>Возвращает объект информации о хранилище.</returns>
        public static EngineSettingsBase LoadFile(String settingsFilePath, bool validate)
        {
            EngineSettingsBase si = new EngineSettingsBase();
            si.LoadFromFile(settingsFilePath);
            if (validate) si.Validate();
            return si;
        }

        /// <summary>
        /// NT-проверить что файл настроек читается и правильный.
        /// Исключений не выбрасывает.
        /// </summary>
        /// <param name="filePath">Путь к файлу настроек</param>
        /// <returns>Возвращает объект информации о хранилище или null</returns>
        public static EngineSettingsBase TryLoad(string filePath)
        {
            EngineSettingsBase result = null;
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
            string file = Path.Combine(this.StoragePath, EngineSettingsBase.DescriptionFileName);
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

#endregion

        #region Служебные функции загрузки выгрузки и валидации файла
        /// <summary>
        /// RT-Загрузить данные из файла описания хранилища
        /// </summary>
        /// <param name="filepath">Путь к файлу описания хранилища</param>
        protected void LoadFromFile(String filepath)
        {
            //1 - read file to string buffer - так как файл настроек, как правило, небольшой, памяти на него должно хватить.
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

            //TODO: read file version info - Облом. 
            //XmlReader атрибуты читает, но я не умею его заставить сейчас
            //Хотя в проекте StackOverflow - удалось. Надо оттуда сюда перетащить идею.

            //skip message for user
            rd.ReadStartElement("DoNotEdit");
            rd.ReadString();
            rd.ReadEndElement();
            //read list of properties 
            //TODO: проверить набор и порядок полей здесь
            _readElement(rd, tagEngineClass);//всегда первым членом настроек
            _readElement(rd, tagEngineVersion);//всегда вторым членом настроек, по нему можно определить дальнейший порядок полей файла.
            _readElement(rd, tagCreator);
            _readElement(rd, tagTitle);
            _readElement(rd, tagDescription);
            _readElement(rd, tagStoragePath);
            _readElement(rd, tagReadOnly);
            _readElement(rd, tagLinkPrefix);
            //дополнительные поля производных классов загружать в переопределенной функции
            loadUserData(rd);
            //end of properties list
            rd.ReadEndElement();
            rd.Close();

            return;
        }

        /// <summary>
        /// NT-извлечь свойство из xml-потока. Служебная функция для более простого синтаксиса.
        /// </summary>
        /// <param name="rd">Объект ридера</param>
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
        /// NFT-Записать данные в файл описания хранилища
        /// </summary>
        /// <param name="filepath"></param>
        protected void StoreToFile(String filepath)
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
            //TODO: проверить набор и порядок полей здесь
            _writeElement(wr, tagCreator);
            _writeElement(wr, tagTitle);
            _writeElement(wr, tagDescription);
            _writeElement(wr, tagStoragePath);
            _writeElement(wr, tagReadOnly);
            _writeElement(wr, tagLinkPrefix);
            //TODO: добавить новые члены настроек здесь
            storeUserData(wr);
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
        /// NT-Проверить значения настроек и версию движка
        /// </summary>
        protected void Validate()
        {
            //выбросить исключение, если:
            //-обязательные значения отсутствуют
            //-значения находятся вне допустимых пределов

            //check engine class
            if (!String.Equals(this.EngineClass, this.getEngineClass(), StringComparison.Ordinal))
                throw new Exception(String.Format("Несовместимый EngineClass: {0}", this.EngineClass));
            //check project engine version
            if (!this.isCompatibleEngineVersion(this.EngineVersion))
                throw new Exception(String.Format("Несовместимый EngineVersion: {0}", this.EngineVersion));
            //check engine data folder
            if (String.IsNullOrEmpty(this.StoragePath))
                throw new Exception("Недопустимый StoragePath");
            if (!Directory.Exists(this.StoragePath))
                throw new Exception(String.Format("Каталог StoragePath не найден: {0}", this.StoragePath));
            //check engine data project title
            if (String.IsNullOrEmpty(this.Title.Trim()))
                throw new Exception("Недопустимое название проекта");
            //TODO: Добавить проверки для новых членов файла настроек движка
            //TODO: проверить набор и порядок полей здесь

            //проверки для пользовательских полей, определенных в производном классе.
            validateUserData();

            return;
        }


        #endregion

        #region Функции, которые надо переопределить в производном классе для поддержки добавленных полей
        /// <summary>
        /// NT-Переопределите функцию в производном классе для загрузки дополнительных свойств Хранилища.
        /// </summary>
        /// <param name="rd"></param>
        protected virtual void loadUserData(XmlReader rd)
        {
            return; //тут ничего не делаем
        }
        /// <summary>
        /// NT-Переопределите функцию в производном классе для выгрузки дополнительных свойств Хранилища.
        /// </summary>
        /// <param name="wr"></param>
        protected virtual void storeUserData(XmlWriter wr)
        {
            return; //тут ничего не делаем
        }

        /// <summary>
        /// NT-Переопределите функцию в производном классе для проверки значений дополнительных свойств Хранилища.
        /// </summary>
        protected virtual void validateUserData()
        {
            return; //тут ничего не делаем
        }
        #endregion


        #region Функции работы с Версиями и КлассДвижка
        //TODO: Эти функции не дадут тут нужную версию движка. 
        //Они должны запускаться ТОЛЬКО из сборки Движка, а не из этой сборки
        //Сейчас они тут только для примера кода!

        /// <summary>
        /// NT-Получить класс Сущности Движка.
        /// Он нужен, чтобы раличать движки и их Каталог Проекта между собой.
        /// </summary>
        /// <returns>Возвращает строковый идентификатор класса Сущности Движка</returns>
        public virtual string getEngineClass()
        {
            return Assembly.GetExecutingAssembly().GetName().Name;
            //TODO: он будет возвращать версию сборки УАМХ, а не Движка. Эту функцию надо переопределить в производном классе Движка.
        }

        /// <summary>
        /// NT-Получить текущую версию  сборки Движка.
        /// </summary>
        /// <returns></returns>
        public virtual Version getCurrentEngineVersion()
        {
            //return Assembly.GetExecutingAssembly().GetName().Version;
            throw new NotImplementedException();
            //TODO: он будет возвращать версию сборки УАМХ, а не Движка. Эту функцию надо переопределить в производном классе Движка.
        }

        /// <summary>
        /// NT-Получить строку текущей версии сборки Движка
        /// </summary>
        /// <returns></returns>
        public string getCurrentEngineVersionString()
        {
            return getCurrentEngineVersion().ToString();
        }

        /// <summary>
        /// NT-Проверить допустимость версии движка из проекта
        /// </summary>
        /// <param name="versionString">Проверяемая версия движка</param>
        /// <returns>Возвращает True, если версии совместимы, False в противном случае.</returns>
        /// <remarks>
        /// Должны совпадать Version.Major и Version.Minor значения.
        /// </remarks>
        public bool isCompatibleEngineVersion(String versionString)
        {
            Version prjVersion = new Version(versionString);
            return isCompatibleEngineVersion(prjVersion);
        }

        /// <summary>
        /// NT-Проверить допустимость версии движка из проекта
        /// </summary>
        /// <param name="version">Проверяемая версия движка</param>
        /// <returns>Возвращает True, если версии совместимы, False в противном случае.</returns>
        /// <remarks>
        /// Должны совпадать Version.Major и Version.Minor значения.
        /// Если для Движка принята другая концепция совместимости версий, эту функцию надо переопределить в производном классе.
        /// </remarks>
        public virtual bool isCompatibleEngineVersion(Version version)
        {
            Version curVersion = this.getCurrentEngineVersion();
            return ((curVersion.Major == version.Major) && (curVersion.Minor == version.Minor));
        }



        #endregion
    }
}
