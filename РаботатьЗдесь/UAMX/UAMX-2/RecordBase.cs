using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IO;
using System.Xml;

namespace UAMX_2
{
    /// <summary>
    /// RT-Базовый класс переноса данных Хранилища
    /// </summary>
    /// <remarks>
    /// Основан на словаре для хранения данных в виде пар ключ-значение.
    /// Ключом является название поля в XML-файле.
    /// Данные хранятся в виде строк, и конвертируются в нужный формат перед использованием.
    /// Но надо добавить эти ключи в словарь после создания объекта. Сами они не создаются.
    /// </remarks>
    public class RecordBase
    {
        //Если надо скрыть в производном классе здешние функции, то надо вставить между классами еще один класс, который и будет их публиковать, а здешние объявить protected.
        
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
        public Dictionary<String, String> getBaseDictionary()
        {
            return m_dictionary;
        }

        /// <summary>
        /// NT-получить строковое представление объекта для отладчика
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0} items", this.m_dictionary.Count);
        }

        #region Функции Get-Set
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

        protected void setValue(string name, string val)
        {
            this.m_dictionary[name] = val;
        }

        protected void setValue(string name, bool val)
        {
            this.m_dictionary[name] = val.ToString();
        }

        protected void setValue(string name, DateTime val)
        {
            this.m_dictionary[name] = Utility.StringFromDateTime(val);
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
        protected DateTime getValueAsDateTime(string name)
        {
            return Utility.DateTimeFromString(this.m_dictionary[name]);
        }

#endregion
        #region Выгрузка-загрузка XML
        //TODO: Я пока не тестировал эти функции работы с XML на больших файлах

        /// <summary>
        /// NT-Прочитать файл
        /// </summary>
        /// <param name="filepath">Файл данных XML. Если файл не существует, будет выброшено исключение.</param>
        /// <remarks>
        /// 
        /// содержимое полей файла загружается в внутренний словарь.
        /// если такое поле уже есть в словаре, выдается исключение.
        /// Поэтому одинаковые имена полей в файле не допускаются.
        /// Словарь перед заполнением не очищается!
        /// 
        /// Далее вызывающий код должен вытаскивать данные из словаря и разносить по соответствующим местам.
        /// </remarks>
        /// <example>
        /// //2. read xml file
        /// TaskEngine.TestXmlFile t2 = new TaskEngine.TestXmlFile();
        /// t2.Load(filepath);
        /// //вытаскивать данные и пихать их в объект.
        /// TestItem ti = new TestItem();
        /// //тут нужны функции-конвертеры класса TestXmlFile, но они объявлены protected
        /// ti.item0 = Int32.Parse(t2.getBaseDictionary()[items[0]]);
        /// ti.item1 = t2.getBaseDictionary()[items[1]];
        /// ti.item2 = Int32.Parse(t2.getBaseDictionary()[items[2]]);
        /// 
        /// return ti;
        /// </example>
        protected void Load(String filepath)
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
            // XmlReader атрибуты тегов не читает!

            //skip message for user
            rd.ReadStartElement("DoNotEdit");
            rd.ReadString();
            rd.ReadEndElement();
            //read list of properties
            //удивительно, но этот код работает правильно! 
            //Он читает ряд последовательных полей и завершается, когда ряд закрывается закрывающим тегом </Settings>
            String val;
            while (rd.IsStartElement() && rd.Name != "Settings")
            {
                //сейчас rd.name = item0, rd.IsEmpty = true
                String item = rd.Name;
                if (rd.IsEmptyElement == true)
                {
                    //обрабатываем пустой тег вроде <item0 />
                    val = "";
                    rd.ReadStartElement();
                    //сохранить значения
                    this.m_dictionary.Add(item, val);
                    continue;
                }
                val = rd.ReadString();
                rd.ReadEndElement();

                //тут записать значения
                this.m_dictionary.Add(item, val);
            }
            //end of properties list
            rd.ReadEndElement();
            rd.Close();
            //сбросить строку хмл-контента, она может быть большой, и так ее быстрее очистит GC, наверно...
            xml = null;
            return;
        }
        /// <summary>
        /// NT-Записать файл
        /// </summary>
        /// <param name="filepath">Файл данных XML. Если файл существует, будет перезаписан.</param>
        /// <param name="items">Массив строк уникальных названий полей как XML-тегов, расположенных в правильном порядке.</param>
        /// <remarks>
        /// сначала вызывающий код должен заполнить словарь данными
        /// поля выводятся в файл по порядку их следования в items. 
        /// Для этого и нужен этот массив.
        /// </remarks>
        /// <example>
        /// String filepath = "C:\\Temp\\file1.test";
        /// // массив задает порядок следования полей в файле
        /// String[] items = new string[] {"item0", "item1", "item2" };
        /// 
        /// //1. create xml file 
        /// TaskEngine.TestXmlFile t1 = new TaskEngine.TestXmlFile();
        /// //внести данные в объект
        /// t1.getBaseDictionary().Add(items[0], "777");
        /// t1.getBaseDictionary().Add(items[1], "");
        /// t1.getBaseDictionary().Add(items[2], "");
        /// //вывести данные в файл
        /// t1.Store(filepath, items);
        /// t1 = null;
        /// 
        /// </example>
        protected void Store(String filepath, String[] items)
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
            //первым должен идти класс движка, потом версия движка, потом остальные поля.
            foreach (String item in items)
            {
                String val = this.m_dictionary[item];
                wr.WriteElementString(item, val);
            }
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
        #endregion


    }
}
