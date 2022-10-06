using System;
using System.Collections.Generic;
using System.Text;

namespace UAMX_2
{
    /// <summary>
    /// NR-Представляет и хранит дерево классов Сущности
    /// </summary>
    public class ElementClassTree
    {
        //Может содержать ноль и более, несколько классов одной и той же Сущности.
        //Должен сериализовать дерево классов в строку для хранения в БД
        //Должен разобрать строку классов из БД в дерево

        //TODO: надо переработать эти классы в два класса, выполняющих все требуемые функции.
        
        //описание стандарта описания классов: file:///V:/МоиПроекты/РечевойИнтерфейс/SpeakWiki/SpeakWiki.wiki?page=ФорматЗаписиТипаМеста 
        
        /// <summary>
        /// NT-Получить строковое представление дерева классов элемента.
        /// </summary>
        /// <returns></returns>
        public virtual string SerializeToDataString()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// NT-Распарсить строковое представление дерева классов элемента.
        /// </summary>
        /// <param name="dataString"></param>
        public virtual void DeserializeFromDataString(string dataString)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Представляет класс сущности
    /// </summary>
    public class EntityType
    {

        #region *** Fields ***
        /// <summary>
        /// абстрактные надклассы
        /// </summary>
        private Dictionary<String, EntityType> m_AbstractionSuperClasses;


        /// <summary>
        /// агрегатные подклассы
        /// </summary>
        private Dictionary<String, EntityType> m_AggregationSubClasses;


        /// <summary>
        /// Название класса сущности
        /// </summary>
        private String m_Title;

        #endregion

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="title">Название создаваемого типа</param>
        public EntityType(string title)
        {
            this.m_Title = title;
            this.m_AbstractionSuperClasses = new Dictionary<string, EntityType>();
            this.m_AggregationSubClasses = new Dictionary<string, EntityType>();
        }

        /// <summary>
        /// Конструктор
        /// </summary>
        /// <param name="title">Название создаваемого типа</param>
        public EntityType()
        {
            this.m_Title = String.Empty;
            this.m_AbstractionSuperClasses = new Dictionary<string, EntityType>();
            this.m_AggregationSubClasses = new Dictionary<string, EntityType>();
        }


        #region *** Properties ***

        /// <summary>
        /// Название класса сущности
        /// </summary>
        public String Title
        {
            get { return m_Title; }
            set { m_Title = value; }
        }

        /// <summary>
        /// абстрактные надклассы
        /// </summary
        internal Dictionary<String, EntityType> AbstractionSuperClasses
        {
            get { return m_AbstractionSuperClasses; }
            set { m_AbstractionSuperClasses = value; }
        }

        /// <summary>
        /// агрегатные подклассы
        /// </summary>
        internal Dictionary<String, EntityType> AggregationSubClasses
        {
            get { return m_AggregationSubClasses; }
            set { m_AggregationSubClasses = value; }
        }

        #endregion

        /// <summary>
        /// NT- return text representation of object
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0};{1};{2}", this.m_Title, this.m_AbstractionSuperClasses.Count, this.AggregationSubClasses.Count);
        }



        /// <summary>
        /// NT-Ищет запись типа по его названию. Если тип не упомянут, возвращается null
        /// </summary>
        /// <param name="nameOfType">Название типа</param>
        /// <returns>Возвращает первый найденный объект записи типа или null если объект не найден</returns>
        public EntityType ContainsType(string nameOfType)
        {
            //проверяем элементы первого уровня Абстракции
            if (this.m_AbstractionSuperClasses.ContainsKey(nameOfType))
                return this.m_AbstractionSuperClasses[nameOfType];
            else
            {
                //проверяем элементы нижнего уровня
                foreach (KeyValuePair<String, EntityType> kvp in this.m_AbstractionSuperClasses)
                {
                    EntityType res = kvp.Value.ContainsType(nameOfType);
                    if (res != null) return res;
                }
            }
            //ничего не нашли

            //проверяем элементы первого уровня Агрегации
            if (this.m_AggregationSubClasses.ContainsKey(nameOfType))
                return this.m_AggregationSubClasses[nameOfType];
            else
            {
                //проверяем элементы нижнего уровня
                foreach (KeyValuePair<String, EntityType> kvp in this.m_AggregationSubClasses)
                {
                    EntityType res = kvp.Value.ContainsType(nameOfType);
                    if (res != null) return res;
                }
            }
            //ничего не нашли
            return null;
        }

        public void ParseExpression(string expression)
        {
            //варианты выражений:
            //[0] Мои места:: Коллекция музыки<Файл::ФайлМузыки>
            //[1] Файловая система ::Папка < Файловая система::Папка,Файл>
            //[2] ФайлМузыки

            //парсим
            //1) удаляем пробелы с начала и конца выражения
            String exp = expression.Trim();
            //2) разделяем на элементы по < и >
            String[] sar = exp.Split(new char[] { '<', '>' }, StringSplitOptions.RemoveEmptyEntries);
            //3) обрабатываем разделы
            //[0] суперкласс и класс - Мои места:: Коллекция музыки / Файловая система ::Папка / ФайлМузыки
            //[1] агрегированные подклассы -  Файл::ФайлМузыки / Файловая система::Папка,Файл / нет 

            //3.1) обрабатываем название класса и суперкласса. Элемент 0 всегда должен существовать.
            ParseClassTitle(sar[0]);

            //3.2) парсим агрегатные субклассы, если они есть
            if (sar.Length == 1) return; //все распарсили, выходим
            //если более 2 элементов массива, это неправильный формат, выбрасываем исключение.
            if (sar.Length > 2) throw new Exception(String.Format("Неправильное выражение: {0}", expression));
            if (sar.Length == 2)
            {
                String s = sar[1];
                //делим по запятым
                String[] sar2 = s.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                //теперь получаем 1 и более элементов вида Файл::ФайлМузыки  Файловая система::Папка  Файл 
                if (sar.Length == 0)
                    throw new Exception(String.Format("Неправильная запись агрегированных субклассов: {0}", expression));
                foreach (string ss in sar2)
                {
                    //trim делается в функции
                    EntityType tt = new EntityType(); //создаем новый объект для класса
                    tt.ParseClassTitle(ss);//отправляем на парсинг для выделения суперкласса и название класса
                    this.m_AggregationSubClasses.Add(tt.Title, tt);//добавляем в коллекцию агрегатных субклассов. Обратной ссылки не делаем - некуда ее писать.
                }
            }

            return;
        }

        /// <summary>
        /// Парсим первую часть выражения класса, содержащую имя суперкласса и имя класса
        /// </summary>
        /// <param name="expression">Строка выражения. Например, "Файл::Файл музыки"</param>
        public void ParseClassTitle(String expression)
        {
            //варианты выражений:
            //[0] Мои места:: Коллекция музыки
            //[1] Файловая система ::Папка 
            //[2] ФайлМузыки
            String s = expression.Trim();

            //проверяем наличие суперкласса в выражении
            if (s.Contains("::"))
            {
                //Входные случаи [1] и [2]
                String[] sar2 = s.Split(new string[] { "::" }, StringSplitOptions.RemoveEmptyEntries); //делим на название суперкласса и название текущего класса
                this.m_Title = sar2[1].Trim(); //пишем название текущего класса
                EntityType et = new EntityType(sar2[0].Trim()); //создаем объект для суперкласса и пишем в него имя
                this.m_AbstractionSuperClasses.Add(et.Title, et); //добавляем в коллекцию суперклассов. Обратной ссылки не делаем - некуда ее писать.
            }
            else
                this.m_Title = s; //входной случай [2], в выражении нет названия суперкласса
        }
    }


    /// <summary>
    /// Представляет коллекцию из ноль, одного и более класса Сущности.
    /// </summary>
    public class EntityTypesCollection
    {
        #region *** Fields ***
        /// <summary>
        /// Словарь типов сущностей первого уровня
        /// </summary>
        private Dictionary<String, EntityType> m_EntityTypes;
        #endregion


        /// <summary>
        /// Default constructor
        /// </summary>
        public EntityTypesCollection()
        {
            this.m_EntityTypes = new Dictionary<string, EntityType>();
        }
        #region *** Properties ***
        /// <summary>
        /// Словарь типов сущностей первого уровня
        /// </summary>
        public Dictionary<String, EntityType> EntityTypes
        {
            get { return m_EntityTypes; }
            set { m_EntityTypes = value; }
        }
        #endregion

        /// <summary>
        /// NT-Ищет запись типа по его названию. Если тип не упомянут, возвращается null
        /// </summary>
        /// <param name="nameOfType">Название типа</param>
        /// <returns>Возвращает первый найденный объект записи типа или null если объект не найден</returns>
        public EntityType ContainsType(String nameOfType)
        {
            if (m_EntityTypes.ContainsKey(nameOfType))
                return m_EntityTypes[nameOfType];
            else
            {
                foreach (KeyValuePair<String, EntityType> kvp in m_EntityTypes)
                {
                    EntityType res = kvp.Value.ContainsType(nameOfType);
                    if (res != null) return res;
                }
                return null;
            }
        }

        /// <summary>
        /// NT-Распарсить строку описания классов сущности в дерево классов
        /// </summary>
        /// <param name="expression">Выражение описания классов сущности </param>
        public void ParseExpression(String expression)
        {
            //Это образец формата для разработки парсинга.
            //выражение представляет собой список выражений типов через ;
            //Мои места:: Коллекция музыки<Файл::ФайлМузыки>;Файловая система ::Папка < Файловая система::Папка,Файл>; 

            //тут надо бы исключение перехватывать и выводить boolean результат парсинга.

            //парсим
            //1) удаляем пробелы с начала и конца выражения
            String exp = expression.Trim();
            //2) разделяем на элементы по ;
            String[] sar = exp.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            //тут мы получим несколько элементов:
            //[0] Мои места:: Коллекция музыки<Файл::ФайлМузыки>
            //[1] Файловая система ::Папка < Файловая система::Папка,Файл>
            //[2] элемент после последней ; должен уже быть удален как пустой элемент
            //3) передаем каждое выражение для парсинга в объект типа 
            foreach (String s in sar)
            {
                EntityType t = new EntityType(); //создаем новый объект
                t.ParseExpression(s); //отправляем строку на парсинг
                this.m_EntityTypes.Add(t.Title, t); //добавляем объект в коллекцию.
            }

            return;
        }
    }//end class
}
