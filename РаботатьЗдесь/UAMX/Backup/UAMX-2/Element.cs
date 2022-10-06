using System;
using System.Collections.Generic;
using System.Text;

namespace UAMX_2
{
    /// <summary>
    /// Абстракция по УАМХ-2 для Проекта, Задачи, Категории, Класса.
    /// </summary>
    /// <remarks>
    /// Должен быть доопределен в производном классе внутри сборки Движка:
    /// 1. В конструкторе надо присвоить значение полю m_StorageVersion, вписав в него значение версии сборки Движка.
    /// 2. Для различения типов элементов надо добавить поле (и проперти) енума ТипЭлемента. Здесь это сделать нельзя, так как состав енума может быть определен только в производном Движке.
    /// </remarks>
    public class Element
    {
         
        #region *** Fields ***
        //10 полей УАМХ-2

        /// <summary>
        /// Это уникальный числовой идентификатор элемента для удобства внутри кода.
        /// </summary>
        protected ElementId m_elementId;

        /// <summary>
        /// Название элемента, может быть неуникальным
        /// </summary>
        protected string m_elementTitle;

        /// <summary>
        /// Текстовое описание, формулировка. 
        /// Подобно Инвентарь, это короткое описание элемента для автоматического показа пользователю приложением как расшифровка названия элемента.
        /// </summary>
        protected String m_elementDescription;

        /// <summary>
        /// Флаг состояния элемента 
        /// Показывает, что сущность активна - или что она помечена для мягкого удаления в корзину.
        /// </summary>
        protected EnumElementState m_elementStatus;

        /// <summary>
        /// ДатаСоздания элемента
        /// </summary>
        protected DateTime m_elementCreationDate;

        /// <summary>
        /// Поле для замечаний, ссылок и пояснительных записей к ним.
        /// Может содержать ссылки на файлы и другие сущности.
        /// </summary>
        protected string m_elementRemarks;

        /// <summary>
        /// Ссылка на Категорию элемента
        /// </summary>
        protected ElementLink m_CategoryLink;

        /// <summary>
        /// Ссылка на Контейнер элемента
        /// </summary>
        protected ElementLink m_ContainerLink;

        /// <summary>
        /// Версия Хранилища проектов. Требуется для корректной работы движка ХранилищеПроектов более новых версий.
        /// </summary>
        protected Version m_StorageVersion;

        /// <summary>
        /// Ссылка на Класс элемента
        /// </summary>
        protected ElementLink m_ClassLink;

        #endregion

        /// <summary>
        /// NT-Конструктор по умолчанию
        /// </summary>
        public Element()
        {
            this.m_elementId = ElementId.EmptyId;//пустой идентификатор элемента
            this.m_CategoryLink = ElementLink.EmptyLink;//пустая ссылка
            this.m_ContainerLink = ElementLink.EmptyLink;//пустая ссылка
            this.m_ClassLink = ElementLink.EmptyLink;//пустая ссылка 
            this.m_elementCreationDate = DateTime.Now;
            this.m_elementDescription = String.Empty;
            this.m_elementRemarks = String.Empty;
            this.m_elementStatus = EnumElementState.Default;
            this.m_elementTitle = String.Empty;
            //this.m_StorageVersion надо наполнять внутри производного Движка

            return;
        }

        //TODO: тут нужны конструкторы с параметрами?
        //или только в производных классах?

        #region *** Properties ***
        //12 properties in UAMX-2

        /// <summary>
        /// Это уникальный числовой идентификатор элемента.
        /// </summary>
        public ElementId Id
        {
            get { return m_elementId; }
            set { m_elementId = value; }
        }

        /// <summary>
        /// Название элемента, может быть неуникальным
        /// </summary>
        public string Title
        {
            get { return m_elementTitle; }
            set { m_elementTitle = value; }
        }

        /// <summary>
        /// Текстовое описание, формулировка. 
        /// Подобно Инвентарь, это короткое описание элемента для автоматического показа пользователю приложением как расшифровка названия.
        /// </summary>
        public String Description
        {
            get { return m_elementDescription; }
            set { m_elementDescription = value; }
        }

        /// <summary>
        /// Статус Элемента.
        /// Флаг показывает, что сущность активна - или что она помечена для мягкого удаления в корзину.
        /// </summary>
        public EnumElementState ElementStatus
        {
            get { return m_elementStatus; }
            set { m_elementStatus = value; }
        }

        /// <summary>
        /// ДатаСоздания элемента
        /// </summary>
        public DateTime CreationDate
        {
            get { return m_elementCreationDate; }
            set { m_elementCreationDate = value; }
        }

        /// <summary>
        /// Поле для замечаний, ссылок и пояснительных записей к ним. 
        /// Может содержать ссылки на файлы и другие сущности.
        /// </summary>
        public string Remarks
        {
            get { return m_elementRemarks; }
            set { m_elementRemarks = value; }
        }
        
        /// <summary>
        /// Ссылка на Класс элемента
        /// </summary>
        public ElementLink ClassLink
        {
            get { return this.m_ClassLink; }
            set { this.m_ClassLink = value; }
        }

        /// <summary>
        /// Ссылка на категорию.
        /// </summary>
        public ElementLink CategoryLink
        {
            get { return m_CategoryLink; }
            set { m_CategoryLink = value; }
        }

        /// <summary>
        /// Ссылка на контейнер.
        /// </summary>
        public ElementLink ContainerLink
        {
            get { return m_ContainerLink; }
            set { m_ContainerLink = value; }
        }

        /// <summary>
        /// NT-Предоставляет готовую корректную ссылку на задачу для использования вне и внутри системы.
        /// </summary>
        public ElementLink CurrentElementLink
        {
            get
            {
                return createElementLink();
            }
        }

        /// <summary>
        /// Это обычно вычисляемое значение для проверки того, что все поля в карточке заполнены и карточка может считаться пригодной для анализа. Если это не так, то автоматический механизм должен предлагать пользователю дополнить карточку сведениями в удобное время либо немедленно.
        /// </summary>
        public EnumCardState CardState
        {
            get
            {
                return определитьСостояниеКарточки();
            }
        }

        /// <summary>
        /// Версия Хранилища проектов. Требуется для корректной работы движка ХранилищеПроектов более новых версий.
        /// </summary>
        public Version StorageVersion
        {
            get { return m_StorageVersion; }
            set { m_StorageVersion = value; }
        }

        #endregion


        /// <summary>
        /// NT-создать ссылку на текущий элемент
        /// </summary>
        /// <returns></returns>
        protected virtual ElementLink createElementLink()
        {
            //создать новый экземпляр ссылки на текущий объект
            //сразу с ссылкой на объект в памяти
            ElementLink link = new ElementLink(this.m_elementId, this.m_elementTitle);
            link.ObjectReference = this;

            return link;
        }
        
        /// <summary>
        /// NT-Проверка, что все поля в карточке заполнены. 
        /// </summary>
        /// <returns>Возвращает флаги состояния карточки элемента.</returns>
        /// <remarks>
        /// Следует переопределить эту функцию в производном классе, для своего набора полей.
        /// Тут нельзя определить, какое именно поле нуждается в дозаполнении. И сколько их.
        /// Только можно определить, важное это поле или нет.
        /// Автоматический механизм должен предлагать пользователю дополнить карточку сведениями в удобное время либо немедленно.
        /// </remarks>
        protected virtual EnumCardState определитьСостояниеКарточки()
        {
            //список полей:
            //первостепенные:
            //this.m_elementTitle - вводится пользователем, важен, но не обязателен.
            //this.m_ClassLink - вводится пользователем, важен, обязательность зависит от назначения Хранилища.
            //this.m_CategoryLink - вводится пользователем, важен, обязательность зависит от типа элемента. Невозможно это определить.
            //this.m_ContainerLink - вводится пользователем, важен, обязательность зависит от типа элемента. Невозможно это определить.

            //второстепенные:
            //this.m_elementDescription - вводится пользователем, не важен, не обязателен.
            //this.m_elementRemarks - вводится пользователем, не важен, не обязателен.

            //автоматически заполняемые: не проверяются
            //this.m_elementId - автоматически присваивается при создании элемента
            //this.m_StorageVersion - автоматически присваивается при создании элемента
            //this.m_elementState - автоматически присваивается при создании элемента
            //this.m_elementCreationDate - автоматически присваивается при создании элемента

            EnumCardState state = EnumCardState.НеЗаполнена;
            //проверяем обязательные поля
            bool ok = true;
            if(String.IsNullOrEmpty(this.m_elementTitle.Trim())) ok = false;
            if (this.m_CategoryLink.IsEmpty) ok = false;
            //нельзя тут сказать, каким должно быть содержимое полей ссылок.
            //если одна из ссылок - пустая, то это может быть нормально для Категории или Контейнера.
            //если обе ссылки пустые, то это точно неправильно.
            if (this.m_CategoryLink.IsEmpty || this.m_ContainerLink.IsEmpty) ok = false;
            //установим флаг обязательного поля
            if (ok == true)
                state = state | EnumCardState.ЗаполненыОбязательныеПоля;
            //проверяем необязательные поля:
            ok = true;
            if (String.IsNullOrEmpty(this.m_elementDescription.Trim())) ok = false;
            if (String.IsNullOrEmpty(this.m_elementRemarks.Trim())) ok = false;
            //установим флаг необязательного поля
            if (ok == true)
                state = state | EnumCardState.ЗаполненыВторостепенныеПоля;
            //проверки завершены

            return state;
        }

        /// <summary>
        /// NR- Получить название Элемента как безопасное для использования в качестве названия файла или папки
        /// </summary>
        /// <returns></returns>
        public virtual String getSafeTitle()
        {
            return Utility.makeSafeTitle(this.m_elementTitle);
        }

        /// <summary>
        /// NT-Получить описание объекта для отладки
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("{0} {1}", this.m_elementId.SerializeToDataString(), this.m_elementTitle);
        }
    }
}
