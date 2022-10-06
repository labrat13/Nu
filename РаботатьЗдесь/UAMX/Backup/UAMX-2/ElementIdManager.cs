using System;
using System.Collections.Generic;
using System.Text;

namespace UAMX_2
{
    /// <summary>
    /// NT-Менеджер идентификаторов элементов. Подобен такому же из Тапп.
    /// </summary>
    /// <remarks>
    /// Этот кеш идентификаторов элемента работает так:
    /// 1. Когда приложение стартует, до начала создания элементов надо провести инициализацию менеджера.
    ///    Для этого функция InitCacheValues() вызывает intGetMaxId(), которая запрашивает хранилище 
    ///    (БД адаптер или что там в качестве хранилища), получить максимальный ИД элементов, существующих в БД.
    /// 2. Когда создается новый элемент, для него нужно получить новый уникальный идентификатор.
    ///    Для этого вызывается функция getNewElementId(), которая вызывает intGetMaxId();. 
    ///    Полученный из кеша или из БД максимальный идентификатор существующего элемента инкрементируется,
    ///    и новое значение используется для создания объекта ИдентификаторЭлемента. Но в кеш не записывается!
    ///    После того, как элемент успешно создан и записан в БД, вызовом ChangeIdCashOnCreateElement() кеш обновляется до актуального значения.
    ///     Такой механизм предполагает, что элементы создаются и записываются в БД строго последовательно.
    ///     В противном случае нескольким элементам будет выдан одинаковый идентификатор.
    /// 3. При удалении элемента нужно вызывать ChangeIdCashOnRemoveElement().
    ///    Она нужна только если удаляется последний созданный элемент.
    ///    Если это не так, то ничего делать с кешем не надо - ведь последний элемент имеет максимальный ид, значит все в порядке.
    ///    Если же удаляется последний элемент, с максимальным ид, то кеш обнуляется,
    ///    чтобы при следующем обращении к кешу получать значение максимального идентификатора элемента из БД.
    ///    Поскольку неизвестно, сколько еще элементов было удалено, и каким должен быть теперь новый максимальный идентификатор элемента.
    ///    
    /// Использование класса:
    /// - Если класс используется внутри предлагаемой схемы с движком Engine, то ничего делать не надо, можно просто его использовать.
    /// Класс получает значение наибольшего существующего идентификатора элемента из this.m_manager.DbAdapter.getMaxElementId();
    /// - Если класс используется в другом раскладе, например, с другим типом хранилища данных, то
    /// следует переопределить функцию protected virtual Int32 intGetMaxId() в производном классе. 
    /// Она должна извлечь из БД значение наибольшего существующего идентификатора элемента.
    /// Остальные аспекты поведения класса не изменяются.
    /// </remarks> 
    public class ElementIdManager
    {
        /// <summary>
        /// Кеш-значение максимального идентификатора элемента из всех что есть в системе
        /// </summary>
        private Int32 m_max_id;
        /// <summary>
        /// Обратная ссылка на объект менеджера, содержащий адаптер БД.
        /// </summary>
        private Engine m_manager;

        /// <summary>
        /// NT-Конструктор
        /// </summary>
        /// <param name="man">Ссылка на объект Движка</param>
        public ElementIdManager(Engine man)
        {
            m_max_id = 0;
            m_manager = man;
        }

        /// <summary>
        /// NT-Clear id cash variables
        /// </summary>
        /// <remarks></remarks>
        public virtual void ClearIdCache()
        {
            m_max_id = 0;
        }

        /// <summary>
        /// NT-Загрузить существующие значения для кеша из БД при старте движка
        /// </summary>
        public void InitCacheValues()
        {
            m_max_id = 0;
            intGetMaxId(); //получить максимальный ид из таблиц БД
        }

        /// <summary>
        /// NT-Получить максимальный ИД элементов из таблиц БД
        /// </summary>
        /// <returns></returns>
        protected virtual Int32 intGetMaxId()
        {
            if (m_max_id == 0)
            {
                //получить максимальный идентификатор из таблиц БД
                m_max_id = this.m_manager.DbAdapter.getMaxElementId();
            }
            return m_max_id;
        }

        /// <summary>
        /// NT-Returns id for new element without update cash values
        /// Cash values must be updated after succesful element creation
        /// </summary>
        /// <returns></returns>
        public ElementId getNewElementId()
        {
            Int32 id = intGetMaxId();
            id += 1;//make new id
            //return new identifier
            return new ElementId(id);
        }

        /// <summary>
        /// NT-Подтвердить создание нового элемента
        /// Изменяет значение кеша идентификатора элемента
        /// </summary>
        /// <param name="elementid">Identifier of new element</param>
        /// <remarks>
        /// Должен вызываться после успешного создания элемента.
        /// </remarks>
        public void ChangeIdCashOnCreateElement(Int32 elementid)
        {
            m_max_id = elementid;
        }

        /// <summary>
        /// NT-Подтвердить создание нового элемента
        /// Изменяет значение кеша идентификатора элемента
        /// </summary>
        /// <param name="elementid">Идентификатор нового элемента</param>
        /// <remarks>
        /// Должен вызываться после успешного создания элемента.
        /// </remarks>
        public void ChangeIdCashOnCreateElement(ElementId i)
        {
            m_max_id = i.Id;
        }

        /// <summary>
        /// NT-Подтвердить удаление существующего элемента
        /// Изменяет значение кеша для  элемента.
        /// </summary>
        /// <remarks>
        /// Предполагается, что ChangeIdCashOnCreateElement() уже была вызвана для текущего элемента.
        /// А если еще не вызвана, то и не надо вызывать эту, так как кеш не изменился еще.
        /// </remarks>
        /// <param name="elementid">Identifier of new element</param>
        public void ChangeIdCashOnRemoveElement(Int32 elementid)
        {
            //Если удаляемый элемент имеет наибольший ИД, сбрасываем кеш для последующего пересчета ИД.
            if (m_max_id == elementid) m_max_id = 0;
            //а если нет, то значит уже есть элемент с большим ид и это пустое место останется незанятым.
        }

        /// <summary>
        /// NT-Подтвердить удаление существующего элемента
        /// Изменяет значение кеша для  элемента.
        /// </summary>
        /// <remarks>
        /// Предполагается, что ChangeIdCashOnCreateElement() уже была вызвана для текущего элемента.
        /// А если еще не вызвана, то и не надо вызывать эту, так как кеш не изменился еще.
        /// </remarks>
        /// <param name="elementid">Identifier of new element</param>
        public void ChangeIdCashOnRemoveElement(ElementId i)
        {
            //Если удаляемый элемент имеет наибольший ИД, сбрасываем кеш для последующего пересчета ИД.
            if (m_max_id == i.Id) m_max_id = 0;
            //а если нет, то значит уже есть элемент с большим ид и это пустое место останется незанятым.
        }

        /// <summary>
        /// NT-Получить строковое представление объекта
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "TopID=" + m_max_id.ToString();
        } 


    }
}
