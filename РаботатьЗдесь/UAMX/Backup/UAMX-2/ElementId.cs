using System;
using System.Collections.Generic;
using System.Text;

namespace UAMX_2
{
    /// <summary>
    /// NT-Представляет идентификатор элемента.
    /// </summary>
    /// <remarks>
    /// Класс достаточно универсален, чтобы использоваться самостоятельно.
    /// Может быть переопределен в дочернем классе. 
    /// 
    /// ИдентификаторЭлемента состоит из строки префикса и числового значения: "t123456".
    /// Для удобочитаемости длина числовой части идентификатора не должна быть менее 3 цифр: "t000".
    /// Префикс (помимо прочих функций) отражает принадлежность идентификатора: Локальный идентификатор или Внешний идентификатор.
    ///  Хотя это пока не используется.
    /// </remarks>
    public class ElementId
    {
        /// <summary>
        /// Префикс - первые символы в текстовой форме идентификатора
        /// </summary>
        public static String Schema;
        /// <summary>
        /// Значение пустого идентификатора
        /// </summary>
        internal const Int32 EmptyIdValue = 0;

        /// <summary>
        /// Значение идентификатора
        /// </summary>
        /// <remarks>
        /// Тип поля Int32, так как в БД хранятся только Int32 числа
        /// </remarks>
        private Int32 m_id;
        /// <summary>
        /// Значение префикса идентификатора
        /// </summary>
        private String m_prefix;


        /// <summary>
        /// NT-Конструктор пустого локального идентификатора
        /// </summary>
        public ElementId()
        {
            m_id = ElementId.EmptyIdValue;
            m_prefix = ElementId.Schema;
        }

        /// <summary>
        /// NT-Конструктор локального идентификатора
        /// </summary>
        /// <param name="id">Значение идентификатора</param>
        public ElementId(Int32 id)
        {
            m_id = id;
            m_prefix = ElementId.Schema;
        }

        /// <summary>
        /// NT-Конструктор идентификатора
        /// </summary>
        /// <param name="id">Значение идентификатора</param>
        /// <param name="prefix">Префикс идентификатора</param>
        public ElementId(Int32 id, string prefix)
        {
            m_id = id;
            m_prefix = prefix;
        }

        /// <summary>
        /// NT-Конструктор десериализации
        /// </summary>
        /// <param name="id"></param>
        public ElementId(String val)
        {
            this.DeserializeFromDataString(val);
        }

        #region Properties

        /// <summary>
        /// Значение идентификатора
        /// </summary>
        public Int32 Id 
        {
            get { return m_id; }
            set { m_id = value; }
        }
        /// <summary>
        /// Префикс идентификатора
        /// </summary>
        public String Prefix
        {
            get { return m_prefix; }
            set { m_prefix = value; }
        }
        /// <summary>
        /// NT-Получить пустой идентификатор элемента
        /// </summary>
        public static ElementId EmptyId
        {
            get { return new ElementId(); }
        }
        /// <summary>
        /// Убедиться, что это локальный идентификатор элемента
        /// </summary>
        public bool IsLocal
        {
            get { return String.Equals(this.m_prefix, ElementId.Schema, StringComparison.InvariantCultureIgnoreCase); }
        }
        /// <summary>
        /// Убедиться, что это Пустой идентификатор элемента
        /// </summary>
        public bool IsEmpty
        {
            get { return (this.m_id == ElementId.EmptyIdValue); }
        }
        #endregion

        /// <summary>
        /// NT-Инициализация статических данных класса
        /// </summary>
        /// <param name="prefix">Строка префикса идентификатора. Например: t</param>
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
            return SerializeToDataString();
        }

        /// <summary>
        /// NT-Проверить, что идентификатор элемента пустой. 
        /// </summary>
        /// <returns></returns>
        public static bool IsEmptyId(ElementId id)
        {
            if (id == null) return false;
            return (id.m_id == ElementId.EmptyIdValue);
        }
        /// <summary>
        /// NT-Проверить, что идентификатор элемента пустой. 
        /// </summary>
        /// <param name="idText">Строковое выражение идентификатора</param>
        /// <returns>Возвращает true или false</returns>
        /// <exception cref="FormatException">Выбрасывается, если строка идентификатора элемента имеет неправильный формат</exception>
        public static bool IsEmptyId(String idText)
        {
            //распарсить строку
            String prefix;
            Int32 id;
            bool result = TryParse(idText, out prefix, out id);
            if (result == false)
                throw new FormatException("Неправильный идентификатор элемента: " + idText);

            return (id == ElementId.EmptyIdValue);
        }
        /// <summary>
        /// NT-Проверить, что строка идентификатора элемента имеет правильный формат
        /// </summary>
        /// <param name="idText">Строковое выражение идентификатора</param>
        /// <returns>Возвращает true или false</returns>
        public static bool IsValidId(String idText)
        {
            //распарсить строку
            String prefix;
            Int32 id;
            bool result = TryParse(idText, out prefix, out id);

            return result;
        }

        /// <summary>
        /// NT-Проверить, что строка идентификатора элемента описывает локальный идентификатор, принятый для текущего Хранилища.
        /// И заодно проверить правильность формата строки.
        /// </summary>
        /// <param name="idText">Строковое выражение идентификатора</param>
        /// <returns>Возвращает true или false</returns>
        public static bool IsLocalId(String idText)
        {
            //распарсить строку
            String prefix;
            Int32 id;
            bool result = TryParse(idText, out prefix, out id);
            if (result == false) return false;//неправильный формат
            //сравнить префиксы
            result = String.Equals(prefix, ElementId.Schema, StringComparison.InvariantCultureIgnoreCase);
            
            return result;
        }

        /// <summary>
        /// NT-Проверить, что объект идентификатора элемента описывает локальный идентификатор, принятый для текущего Хранилища.
        /// </summary>
        /// <param name="id">Объект идентификатора</param>
        /// <returns>Возвращает true или false</returns>
        public static bool IsLocalId(ElementId id)
        {
            //сравнить префиксы
            bool result = String.Equals(id.m_prefix, ElementId.Schema, StringComparison.InvariantCultureIgnoreCase);

            return result;
        }

        /// <summary>
        /// NT-Получить строковое представление идентификатора элемента.
        /// </summary>
        /// <returns></returns>
        public virtual string SerializeToDataString()
        {
            //"t2375534754"
            //идентификатор должен быть не менее 3 десятичных знаков длиной
            return ElementId.Schema + this.m_id.ToString("D3");
        }

        /// <summary>
        /// NT-Распарсить строковое представление идентификатора элемента.
        /// </summary>
        /// <param name="dataString"></param>
        public virtual void DeserializeFromDataString(string dataString)
        {
            //распарсить строку типа "t2375534754"
            String prefix;
            Int32 id;
            bool result = ElementId.TryParse(dataString, out prefix, out id);
            if (result == false)
                throw new FormatException("Неправильный идентификатор элемента: " + dataString);

            this.m_id = id;
            this.m_prefix = prefix;

            return;
        }

        /// <summary>
        /// NT-Разделить строку идентификатора на префикс и число
        /// </summary>
        /// <param name="text">Строка идентификатора</param>
        /// <param name="prefix">Переменная-место под строку префикса</param>
        /// <param name="id">Переменная-место под число</param>
        /// <returns>Возвращает True если строка правильного формата и распарсена успешно, False в противном случае.</returns>
        public static bool TryParse(String text, out String prefix, out Int32 id)
        {
            
            if (String.IsNullOrEmpty(text))
                goto label_Fail;
            
            String[] sar = Utility.GetTitleParts(text);
            //проверки
            if (sar.Length != 2) goto label_Fail;
            
            Int32 tmp;
            bool result = Int32.TryParse(sar[1], out tmp);
            if (result == false) goto label_Fail;

            prefix = sar[0];
            id = tmp;
            return true;

        label_Fail:
            prefix = String.Empty;
            id = 0;
            return false;
        }

    }
}
