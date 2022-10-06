using System;
using System.Collections.Generic;
using System.Text;

namespace UAMX_2
{
    /// <summary>
    /// NR-Представляет ссылку на элемент.
    /// Может быть переопределен в дочернем классе
    /// </summary>
    public class ElementLink
    {
        
        
        /// <summary>
        /// Префикс ссылки по умолчанию
        /// </summary>
        /// <remarks>
        /// Обязательно надо установить его значение в конструкторе движка! 
        /// Иначе все ссылки будут без префикса и не будут парситься потом в работе движка.
        /// </remarks>
        public static string Schema;

        /// <summary>
        /// Массив запрещенных для веб-имен символов - здесь для оптимизации функции проверки веб-имен 
        /// </summary>
        protected static Char[] RestrictedWebLinkSymbols = { ' ', '\\', '/', '?', ';', ':', '@', '&', '=', '+', '$', ',', '<', '>', '"', '#', '{', '}', '|', '^', '[', ']', '‘', '%', '\n', '\t', '\r' };


        /// <summary>
        /// Объект на который указывает ссылка.
        /// Инициализируется как null, а ссылка на объект вставляется, если требуется.
        /// </summary>
        protected Object m_ObjectRef;
        /// <summary>
        /// Идентификатор элемента, который обозначен ссылкой
        /// </summary>
        protected ElementId m_ElementId;

        /// <summary>
        /// Веб-имя элемента, который обозначен ссылкой.
        /// </summary>
        protected String m_LinkTitle;
        /// <summary>
        /// Префикс по умолчанию (Scheme), используемый для создания ссылки
        /// </summary>
        protected String m_linkPrefix;

        /// <summary>
        /// NR-Конструктор
        /// </summary>
        /// <remarks>
        /// Создает Пустую ссылку (ЗначениеБезСсылок)
        /// </remarks>
        public ElementLink()
        {
            this.m_linkPrefix = Schema;
            this.m_ElementId = ElementId.EmptyId;
            this.m_LinkTitle = String.Empty;
            this.m_ObjectRef = null;
        }

        /// <summary>
        /// NT-Конструктор со схемой по умолчанию
        /// </summary>
        /// <param name="id">Объект идентификатора элемента</param>
        /// <param name="title">Название элемента или пустая строка</param>
        public ElementLink(ElementId id, String title)
        {
            this.m_linkPrefix = Schema;
            this.m_ElementId = new ElementId(id.Id);
            this.m_LinkTitle = ElementLink.convertToLinkTitle(title);
            this.m_ObjectRef = null;
        }
        
        /// <summary>
        /// NT-Конструктор с указанием схемы
        /// </summary>
        /// <param name="id">Объект идентификатора элемента</param>
        /// <param name="title">Название элемента или пустая строка</param>
        /// <param name="linkPrefix">Префикс ссылки</param>
        /// <param name="idPrefix">Префикс идентификатора</param>
        public ElementLink(ElementId id, String title, String linkPrefix, String idPrefix)
        {
            this.m_linkPrefix = linkPrefix;
            this.m_ElementId = new ElementId(id.Id, idPrefix);
            this.m_LinkTitle = ElementLink.convertToLinkTitle(title);
            this.m_ObjectRef = null;
        }

        /// <summary>
        /// NT-Конструктор
        /// </summary>
        /// <param name="linktext">Строка ссылки</param>
        public ElementLink(String linktext)
        {
            this.DeserializeFromDataString(linktext);
            this.m_ObjectRef = null;
        }

        #region *** Properties ***

        /// <summary>
        /// Получить Идентификатор элемента, который обозначен ссылкой
        /// </summary>
        public ElementId ID
        {
            get { return m_ElementId; }
        }
        /// <summary>
        /// Получить модифицированное Название элемента, который обозначен ссылкой.
        /// </summary>
        public String LinkTitle
        {
            get { return m_LinkTitle; }
        }
        /// <summary>
        /// Объект на который указывает ссылка.
        /// </summary>
        public Object ObjectReference
        {
            get { return m_ObjectRef; }
            set { m_ObjectRef = value; }
        }
        /// <summary>
        /// Получить объект ЗначениеБезСсылок.
        /// Возвращает объект Пустая локальная ссылка.
        /// </summary>
        public static ElementLink EmptyLink
        {
            get 
            { 
                ElementLink result = new ElementLink();
                result.m_LinkTitle = "EmptyLink";//Название ссылки не должно содержать пробелы!
                return result;
            }
        }
        /// <summary>
        /// Убедиться что это Пустая ссылка
        /// </summary>
        public bool IsEmpty
        {
            get { return this.m_ElementId.IsEmpty; }
        }

        /// <summary>
        /// Убедиться что это Локальная ссылка
        /// </summary>
        public bool IsLocal
        {
            get { return String.Equals(this.m_linkPrefix, ElementLink.Schema, StringComparison.OrdinalIgnoreCase); }
        }

        #endregion

        /// <summary>
        /// NT-Инициализация статических данных класса
        /// </summary>
        /// <param name="prefix">Строка префикса ссылки. Например: task</param>
        public static void Init(String prefix)
        {
            Schema = String.Copy(prefix);
        }

        /// <summary>
        /// NT-Получить строковое представление объекта
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.SerializeToDataString255();
        }

        /// <summary>
        /// NT-Проверить что ссылка имеет правильный формат
        /// </summary>
        /// <param name="linkText">Текст ссылки</param>
        /// <returns>Возвращает true или false</returns>
        public virtual bool IsValidLink(String linkText)
        {
            String p1, p2, p3;
            bool result = ElementLink.TryParse(linkText, out p1, out p2, out p3);

            return result;
        }

        /// <summary>
        /// NT-Проверить что ссылка это ЗначениеБезСсылок
        /// </summary>
        /// <param name="linkText">Текст ссылки</param>
        /// <returns>Возвращает true или false</returns>
        /// <exception cref="FormatException">Выбрасывается, если строка ссылки имеет неверный формат.</exception>
        public virtual bool IsEmptyLink(String linkText)
        {
            String prefix;
            String idString;
            String linkTitle;

            bool result = ElementLink.TryParse(linkText, out prefix, out idString, out linkTitle);
            if (result == false)
                throw new FormatException("Неправильная строка ссылки: " + linkText);

            return ElementId.IsEmptyId(idString);
        }

        /// <summary>
        /// NT-Проверить что это Локальная ссылка
        /// </summary>
        /// <param name="linkText">Текст ссылки</param>
        /// <returns>Возвращает true или false</returns>
        /// <exception cref="FormatException">Выбрасывается, если строка ссылки имеет неверный формат.</exception>
        public virtual bool IsLocalLink(String linkText)
        {
            String prefix;
            String idString;
            String linkTitle;

            bool result = ElementLink.TryParse(linkText, out prefix, out idString, out linkTitle);
            if (result == false)
                throw new FormatException("Неправильная строка ссылки: " + linkText);

            return String.Equals(prefix, ElementLink.Schema, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// NT-Получить строковое представление ссылки
        /// </summary>
        /// <returns></returns>
        public virtual string SerializeToDataString()
        {
            //сейчас для простоты возвращаем ограниченный до 255 символов текст ссылки.
            //вряд ли потребуется более длинный текст, содержащий прямо все символы названия элемента. 
            return SerializeToDataString255();
        }
        /// <summary>
        /// NT-Получить строковое представление ссылки, ограниченное 255 символами.
        /// </summary>
        /// <returns>Возвращает строковое представление ссылки, ограниченное 255 символами.</returns>
        /// <remarks>
        /// Эта функция специально для БД выделена. И она используется как основная сейчас.  
        /// </remarks>
        public virtual string SerializeToDataString255()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(this.m_linkPrefix); //"task"
            sb.Append(":///");
            sb.Append(this.m_ElementId.SerializeToDataString());//"t2375534754"
            //есть ли title?
            if(String.IsNullOrEmpty(this.m_LinkTitle) == false)
            {
                sb.Append('_'); //разделитель ID и LinkTitle
                sb.Append(this.m_LinkTitle);//добавим название ссылки
            }
            //limit to 255 chars
            if(sb.Length > 255)
                sb.Length = 255;
            //return
            return sb.ToString();
        }
        /// <summary>
        /// NT-Распарсить строковое представление ссылки.
        /// </summary>
        /// <param name="dataString">Строковое представление ссылки.</param>
        /// <remarks>
        /// Проверять префикс ссылки на локальность не будем здесь
        /// </remarks>
        protected virtual void DeserializeFromDataString(string dataString)
        {
            String prefix;
            String idString;
            String linkTitle;

            bool result = ElementLink.TryParse(dataString, out prefix, out idString, out linkTitle);
            if (result == false)
                throw new FormatException("Неправильная строка ссылки: " + dataString);
            //set object fields
            this.m_ElementId = new ElementId(idString);
            this.m_linkPrefix = prefix;
            this.m_LinkTitle = linkTitle;

            return;
        }

        /// <summary>
        /// NT-Строка текста не годится для использования в качестве описания ссылки
        /// </summary>
        /// <param name="text">Текст, название элемента</param>
        /// <returns>Возвращает true или false</returns>
        public static bool IsInvalidLinkTitle(String text)
        {
            //Описание ссылки не должно содержать пробелы, знаки \ / ? ; : @ & = + $ , < > " # { } | ^ [  ] ‘ %
            int res = text.IndexOfAny(RestrictedWebLinkSymbols);//return -1 if nothing found
            return (res != -1);
        }

        /// <summary>
        /// NT-Создать описание ссылки из названия элемента
        /// </summary>
        /// <param name="text">Название элемента</param>
        /// <returns>Возвращает строку описания ссылки</returns>
        public static string convertToLinkTitle(string text)
        {
            String t = text.Trim();
            //если текст после trim() пустой, вернуть пустую строку.
            if (String.IsNullOrEmpty(t)) 
                return String.Empty;
            //далее в цикле:
            //заменить все пробелы на _
            t = text.Replace(' ', '_');
            //заменить все неправильные символы на -
            t = Utility.replaceRestrictedChars(t, ElementLink.RestrictedWebLinkSymbols, '-');
            //заменить двойные и более подчеркивания на одиночные
            t = Utility.removeSubstring(t, "__");
            //убрать все эти символы с начала и конца названия
            t = t.Trim(new char[] { ' ', '-', '_' });

            return t;
        }

        /// <summary>
        /// NT-Распарсить текст ссылки
        /// </summary>
        /// <param name="text">Текст ссылки</param>
        /// <param name="prefix">Префикс ссылки</param>
        /// <param name="idString">Строка идентификатора элемента</param>
        /// <param name="linkTitle">Название элемента ссылки</param>
        /// <returns>Возвращает true или false</returns>
        public static bool TryParse(String text, out string prefix, out string idString, out string linkTitle)
        {
            //TODO: отладить эту функцию обязательно.
            
            //Сейчас тут только необходимые свойства ссылки парсятся.
            //Потом можно расширить, когда эти отлажу и тесты напишу и прокручу.
            //Чтобы уже все фишки сразу покрыть...

            //еще есть Uri.Host - но он обычно пустой строкой
            //еще ссылка может содержать аргументы
            // вроде: inv:///p123456?arg1=1&arg2=2
            //они в Uri.Query поле содержатся в виде: ?arg1=%D0%9A%D0%B0%D1%80%D1%82%D0%BE%D1%88%D0%BA%D0%B0&arg2=%D0%A1%D0%B0%D0%BB%D0%BE
            //и их надо переводить на русский язык вызовом Uri.UnescapeDataString(..)

            bool result = true;
            try
            {
                Uri uri = new Uri(text);
                prefix = String.Copy(uri.Scheme);
                String local = uri.LocalPath.Substring(1);//удаляем первый символ '/'
                //разделить ид и линктитле
                int _pos = local.IndexOf('_');
                if (_pos == -1)//если нет символа в строке, это короткая ссылка без линктитле
                {
                    idString = local;
                    linkTitle = String.Empty;
                }
                else
                {
                    idString = local.Substring(0, _pos);
                    linkTitle = local.Substring(_pos+1);
                }
            }
            catch (Exception ex)
            {
                result = false;
                //надо присвоить выходные значения аргументам
                prefix = String.Empty;
                idString = String.Empty;
                linkTitle = String.Empty;
            }
            return result;
        }

    }
}
