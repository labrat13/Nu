using System;
using System.Collections.Generic;
using System.Text;

namespace UAMX_2
{
    /// <summary>
    /// NR-Представляет коллекцию ссылок на элементы
    /// </summary>
    public class ElementLinkCollection
    {
        //TODO: сейчас реализована как список. 
        //Но было бы удобнее, если бы реализовать коллекцию через словарь.
        //с ключом по elementId
        //Но памяти она занимать будет больше, а работать быстрее.
        //- сначала напишем весь проект, выявим применение класса,  потом видно будет, словарь или список. 

        /// <summary>
        /// Список ссылок на элементы
        /// </summary>
        private List<ElementLink> m_list;
        /// <summary>
        /// Разделитель ссылок в строке коллекции
        /// </summary>
        private const string LinkDelimiter = ";";

        /// <summary>
        /// Конструктор
        /// </summary>
        public ElementLinkCollection()
        {
            m_list = new List<ElementLink>();
        }
        /// <summary>
        /// NT-Конструктор для десериализации
        /// </summary>
        /// <param name="listContent">Строка набора ссылок</param>
        public ElementLinkCollection(String listContent)
        {
            m_list = new List<ElementLink>();
            this.DeserializeFromDataString(listContent);
        }

        /// <summary>
        /// Список ссылок на элементы
        /// </summary>
        public List<ElementLink> Elements
        {
            get { return m_list; }
        }
        /// <summary>
        /// NT-Возвращает True, если Список содержит ссылку на указанный элемент
        /// </summary>
        /// <param name="id">Идентификатор элемента</param>
        /// <returns></returns>
        public bool Contains(ElementId id)
        {
            //поиск перебором списка
            foreach (ElementLink link in m_list)
                if (link.ID.Id == id.Id)
                    return true;
            return false;
        }
        /// <summary>
        /// NT- Добавить ссылку на элемент, если ее нет в списке
        /// </summary>
        /// <param name="link"></param>
        public void Add(ElementLink link)
        {
            if (this.Contains(link.ID))
                return;
            //add
            this.m_list.Add(link);

            return;
        }
        /// <summary>
        /// NT-Очистить коллекцию
        /// </summary>
        public void Clear()
        {
            this.m_list.Clear();
        }
        /// <summary>
        /// NT-Получить строковое представление объекта
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0} ссылок", this.m_list.Count);
        }

        /// <summary>
        /// NT-собрать строку со ссылками элементов
        /// </summary>
        /// <returns>Строка данных элементов</returns>
        public virtual string SerializeToDataString()
        {
            //ссылки не должны содержать значение ; (ElementLinkCollection.LinkDelimiter)
            //но так как это веб-ссылки, то они и не могут содержать это значение
            StringBuilder sb = new StringBuilder();
            foreach (ElementLink link in m_list)
            {
                sb.Append(link.SerializeToDataString());
                sb.Append(ElementLinkCollection.LinkDelimiter);
            }

            return sb.ToString().Trim();
        }
        /// <summary>
        /// NT-Распарсить строку на ссылки элементов
        /// </summary>
        /// <param name="dataString">Строка данных элементов</param>
        protected virtual void DeserializeFromDataString(string dataString)
        {
            //если строка пустая, ничего не парсим
            String src = dataString.Trim();
            if (src.Length == 0) 
                return;
            
            //делим строку по ;
            String[] sar = src.Split(ElementLinkCollection.LinkDelimiter.ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
            foreach (String slink in sar)
            {
                ElementLink link = new ElementLink(slink.Trim());
                this.m_list.Add(link);
            }

            return;
        }


    }
}
