using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.IO;

namespace PMEngine
{
    /// <summary>
    /// NR-Представляет собственно движок.
    /// Должен запускаться только в одном экземпляре?
    /// </summary>
    public class Engine
    {
        #region *** Поля и константы движка ***
        /// <summary>
        /// Адаптер БД для хранения элементов
        /// </summary>
        private DbAdapter m_dbAdapter;
        /// <summary>
        /// Флаг, что движок работает в режиме Только чтение.
        /// </summary>
        private bool m_ReadOnly;
        /// <summary>
        /// Объект настроек проекта движка
        /// </summary>
        private EngineSettings m_settings;
        /// <summary>
        /// Менеджер файловой системы проекта данных движка
        /// </summary>
        private ProjectFolderManager m_dataFolderManager; //проперти для него пока не делаем, потом видно будет.
        #endregion

        /// <summary>
        /// Default constructor
        /// </summary>
        public Engine()
        {
            //TODO: add code here
            //создать пакет настроек по умолчанию, пока для отладки.
            this.m_settings = new EngineSettings(); 
        }
        #region *** Проперти движка ***
        /// <summary>
        /// Флаг, что движок работает в режиме Только чтение.
        /// </summary>
        public bool ReadOnly
        {
            get { return m_ReadOnly; }
        }
        
        /// <summary>
        /// Адаптер БД для хранения элементов
        /// </summary>
        public DbAdapter DbAdapter
        {
            get { return m_dbAdapter; }
        }

        /// <summary>
        /// NT-Получить текущую версию движка
        /// </summary>
        public Version EngineVersion
        {
            get { return Utility.getCurrentEngineVersion(); }
        }

        /// <summary>
        /// Объект настроек проекта движка
        /// </summary>
        public EngineSettings Settings
        {
            get { return m_settings; }
        }
        #endregion


        #region *** Главные функции движка ***
        /// <summary>
        /// NT-Создать новый проект
        /// </summary>
        public static void Create(string rootFolder, EngineSettings si)
        {
            //0 проверить аргументы
            if (!Directory.Exists(rootFolder)) throw new ArgumentException("Root folder must be exists", "rootFolder");
            if (si == null) throw new ArgumentNullException("si", "Engine settings object cannot be null");
            if (String.IsNullOrEmpty(si.Title)) throw new ArgumentException("Project title cannot be empty");
            //1 создать каталог данных проекта
            String prjSafeName = Utility.makeSafeFolderName(si.Title);
            String prjFolder = Path.Combine(rootFolder, prjSafeName);
            //  записать правильный путь к проекту в переданный объект EngineSettings
            si.StoragePath = prjFolder;
            ProjectFolderManager.CreateProjectFolder(prjFolder, si);
            //не буду тут открывать движок, его некуда возвращать. Потом откроем соответствующей фукцией.
            ////2 открыть движок, чтобы проверить и записать данные если нужно.
            ////3 закрыть движок

            //TODO: добавить дополнительный код создания движка менеджера проектов

            return;
        }

        /// <summary>
        /// NT-Открыть проект
        /// </summary>
        /// <param name="storagePath">Путь к каталогу данных проекта</param>
        /// <param name="readOnly">Открыть только для чтения</param>
        public void Open(String storagePath, bool readOnly)
        {
            //1 инициализировать менеджер каталога проекта движка
            this.m_dataFolderManager = new ProjectFolderManager(this, storagePath);
            //2 загрузить настройки движка
            this.m_settings = EngineSettings.Load(storagePath);
            //3 проверить что каталог доступен для записи
            //если каталог проекта реально рид-онли или пользователь хочет рид-онли, или настройки проекта - рид-онли, то выставляем рид-онли флаг.
            this.m_ReadOnly = (this.m_dataFolderManager.isReadOnly() || readOnly || m_settings.ReadOnly);
            //4 инициализировать адаптер БД и подключиться к БД, даже в рид-онли режиме
            //String dbpath = Path.Combine(storagePath, PMEngine.DbAdapter.DatabaseFileName); 
            this.m_dbAdapter = PMEngine.DbAdapter.SetupDbAdapter(this.m_dataFolderManager.DbFilePath, this.m_ReadOnly);
            //5 инициализировать остальное
            //TODO: добавить дополнительный код открытия движка менеджера проектов
            //создать тут объекты, которые требуют доступной БД для своей инициализации
            
            return;
        }

        ///// <summary>
        ///// NR-Открыть проект один раз при его создании, 
        ///// когда БД еще не содержит всех нужных данных, а ее уже надо открыть.  
        ///// </summary>
        ///// <param name="si"></param>
        //private void openIfNewOnly(EngineSettings si)
        //{
        //    throw new System.NotImplementedException();//TODO: add code here
        //}

        /// <summary>
        /// NT-Завершить сеанс работы движка
        /// </summary>
        public void Close()
        {
            //1 Закрыть всякие прочие объекты
            //например, this.m_IdManager = null;
            //2 close database adapter
            this.m_dbAdapter.Disconnect();
            this.m_dbAdapter = null;
            //3 настройки сейчас сохранять надо сразу после изменения, а не в конце работы движка.
            //поэтому тут их не сохраняем.
            //4 менеджер каталога сейчас ничего не содержит такого, чтобы его специально закрывать.
            this.m_ReadOnly = false;
            this.m_dataFolderManager = null;
            //TODO: добавить дополнительный код закрытия движка менеджера проектов

            return;
        }

        /// <summary>
        /// NR-Оптимизация проекта - незакончено, неясно как сделать и как использовать потом
        /// </summary>
        public void Optimize()
        {
            CheckReadOnly();
            //TODO: добавить код оптимизаци Хранилища здесь
            throw new System.NotImplementedException();
        }

        
        //Функция очистки. Ни разу не пользовался, проще создать новый проект. Но для комплекта тут реализована.
        ///// <summary>
        ///// NFT-Очистить проект
        ///// </summary>
        ///// <returns>Return True if success, False otherwise</returns>
        //public bool ClearStorage()
        //{
        //    CheckReadOnly();
        //    //Тут очищаем все таблицы БД кроме таблицы свойств, удаляем все архивы, пересчитываем статистику и вносим ее в БД.
        //    //в результате должно получиься пустое Хранилище, сохранившее свойства - имя, квалифицированное имя, путь итп.
        //    if (m_db.ClearDb() == true)
        //    {
        //        m_docController.Clear();
        //        m_picController.Clear();
        //        this.updateStorageInfo();//это будет выполнено также при закрытии менеджера.
        //        return true;
        //    }
        //    else return false;
        //}

        /// <summary>
        /// NT-Проверить, что указанный каталог является каталогом Хранилища
        /// </summary>
        /// <param name="path">Путь к каталогу</param>
        /// <returns>Возвращает true, если каталог является каталогом Хранилища. В противном случае возвращает false</returns>
        public static bool IsSolutionFolder(string path)
        {
            return SolutionManager.IsSolutionFolder(path);
        }

        /// <summary>
        /// NR-Удалить проект
        /// </summary>
        /// <param name="storagePath">Путь к каталогу Хранилища</param>
        /// <returns>Возвращает true, если Хранилище успешно удалено или его каталог не существует.
        /// Возвращает false, если удалить Хранилище не удалось по какой-либо причине.</returns>
        public static bool DeleteProject(String storagePath)
        {
            //Этот код только для примера, его нужно переписать
            throw new System.NotImplementedException();//TODO: add code here
            
            ////если каталог не существует, возвращаем  true
            //if (!Directory.Exists(storagePath)) return true;
            ////1) если Хранилище на диске только для чтения, то вернуть false.
            //if (isReadOnly(storagePath)) return false;
            ////2) пробуем переименовать каталог Хранилища
            ////если получится, то каталог никем не используется. удалим каталог и вернем true.
            ////иначе будет выброшено исключение - перехватим его и вернем false
            ////сначала еще надо сгенерировать такое новое имя каталога, чтобы незанятое было.
            //String newName = String.Empty;
            //String preRoot = Path.GetDirectoryName(storagePath);
            //for (int i = 0; i < 16384; i++)
            //{
            //    newName = Path.Combine(preRoot, "tmp" + i.ToString());
            //    if (!Directory.Exists(newName)) break;
            //}
            ////тут мы должны оказаться с уникальным именем
            //if (Directory.Exists(newName))
            //    throw new Exception("Error! Cannot create temp directory name!");
            ////пробуем переименовать каталог
            //try
            //{
            //    Directory.Move(storagePath, newName);
            //}
            //catch (Exception)
            //{
            //    return false;
            //}
            ////каталог не используется, удалим его
            ////TODO: вот лучше бы его через шелл и в корзину удалить. Хотя... Удалять же будет не приложение. Некому показывать шелл.
            ////Надо это решить как удобнее будет. Может, через аргументы передавать способ - с гуем в корзину или нет.
            //Directory.Delete(newName, true);
            ////если тут возникнут проблемы, то хранилище все равно уже будет повреждено.
            ////так что выброшенное исключение достанется вызывающему коду.
            //return true;
        }

        //Еще есть функция получения информации о проекте, но я не знаю, пригодится ли тут она. 
        //Она просто возвращает содержимое файла настроек, который содержит также и статистику проекта.
        //Поэтому я не стал ее тут приводить.

        /// <summary>
        /// NR-зарегистрировать единственный экземпляр. Если это не так, вернуть False.
        /// </summary>
        /// <param name="uid">Уникальный строковый идентификатор приложения</param>
        public bool RegisterSingleInstance(string uid)
        {
            throw new System.NotImplementedException();//TODO: add code here
        }

        /// <summary>
        /// NR-Получить текстовое представление движка для отладки
        /// </summary>
        /// <returns></returns>
        public override String ToString()
        {
            return base.ToString();//TODO: add code here
        }

#endregion


        #region *** BotAPI functions ***
        //TODO: добавить тут рабочие функции движка 
        #endregion

        #region *** Вспомогательные функции движка ***
        //Тут размещать внутренние функции движка



        /// <summary>
        /// NT-Получить строку текущей версии сборки движка
        /// </summary>
        /// <returns></returns>
        public static string getEngineVersionString()
        {
            //DONE: Убедиться что это возвращает версию текущей сборки а не приложения.
            return Assembly.GetExecutingAssembly().GetName().Version.ToString();
        }

        /// <summary>
        /// NT-Получить класс Сущности Движка.
        /// Он нужен, чтобы раличать движки и их Каталог Проекта между собой.
        /// </summary>
        /// <returns>Возвращает строковый идентификатор класса Сущности Движка</returns>
        public static string getEngineClass()
        {
            return Assembly.GetExecutingAssembly().GetName().Name;
        }

        /// <summary>
        /// NT-Проверить режим read-only и выбросить исключение
        /// </summary>
        private void CheckReadOnly()
        {
            if (this.m_ReadOnly == true)
                throw new Exception("Error: Writing to read-only storage!");
        }

        #endregion

    }
}
