using System;
using System.IO;

namespace PMEngine
{
    /// <summary>
    /// NR-Это класс, управляющий Каталогом данных проекта. 
    /// </summary>
    public class ProjectFolderManager
    {
        
        #region Константы



        #endregion

        #region Поля

        /// <summary>
        /// Путь к каталогу данных проекта.
        /// </summary>
        private string m_MainFolderPath;
        /// <summary>
        /// Обратная ссылка на движок
        /// </summary>
        private Engine m_Engine;//проперти не нужно здесь

        #endregion

        /// <summary>
        /// NT-Конструктор
        /// </summary>
        /// <param name="engine">Обратная ссылка на движок</param>
        public ProjectFolderManager(Engine engine, String storagePath)
        {
            this.m_Engine = engine;
            this.m_MainFolderPath = storagePath;
            //TODO: Add code here...
        }

        #region Проперти

        /// <summary>
        /// Root project folder path
        /// </summary>
        public string MainFolderPath
        {
            get
            {
                return this.m_MainFolderPath;
            }
            set
            {
                m_MainFolderPath = value;
            }
        }

        /// <summary>
        /// Database file path
        /// </summary>
        public string DbFilePath
        {
            get
            {
                return Path.Combine(m_MainFolderPath, DbAdapter.DatabaseFileName);
            }
        }

        #endregion


        /// <summary>
        /// NR-Получить текстовое представление объекта
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Dir={0}", this.m_MainFolderPath);
        }

        /// <summary>
        /// NR-Создать каталог Коллекции проектов
        /// </summary>
        /// <param name="prjFolder">Путь к еще не существующему каталогу проекта</param>
        internal static void CreateProjectFolder(string prjFolder, EngineSettings sett)
        {
            //check folder is not exists
            if (Directory.Exists(prjFolder))
                throw new ArgumentException(String.Format("Directory already exists: {0}", prjFolder), "prjFolder");
            //create project folder with all files and folders
            Utility.CreateNotIndexedFolder(prjFolder);
            //1 create db file
            String path = Path.Combine(prjFolder, DbAdapter.DatabaseFileName);
            DbAdapter.extractDbFile(path);
            //2 create setting file
            path = Path.Combine(prjFolder, EngineSettings.DescriptionFileName);
            sett.Store(path);
            //3 create other elements...
            //TODO: Project folder create: create other files and folders

            return;
        }

        /// <summary>
        /// NR-Проверить, что указанный каталог является каталогом Хранилища
        /// </summary>
        /// <param name="path">Путь к каталогу</param>
        /// <returns>Возвращает true, если каталог является каталогом Хранилища. В противном случае возвращает false</returns>
        public static bool IsProjectFolder(string path)
        {
            //Этот код только для примера, его нужно переписать
            //TODO: сделать качественно функцию IsProjectFolder()
            //проверить:
            //1 что это не пустая строка
            if (String.IsNullOrEmpty(path)) return false;
            //2 что это каталог и он существует
            if (!Directory.Exists(path)) return false;
            //3 что это каталог проекта движка

            //критерии:
            //папка должна содержать файл "settings.xml"
            //папка должна содержать файл db.mdb
            //папка должна содержать ... TODO: добавить признаки каталога данных проекта здесь
            //файл "settings.xml" должен читаться без проблем

            String p;

            //проверяем наличие обязательных файлов и каталогов
            //p = Path.Combine(path, ArchiveController.DocumentsDir); - пример
            //if (!Directory.Exists(p)) return false;

            //проверяем наличие файла БД
            p = Path.Combine(path, DbAdapter.DatabaseFileName);
            if (!File.Exists(p)) return false;
            //напоследок пытаемся загрузить и прочитать файл настроек движка
            p = Path.Combine(path, EngineSettings.DescriptionFileName);
            if (!File.Exists(p)) return false;
            //try load description file
            if (EngineSettings.TryLoad(p) == null) return false;

            return true;
        }

        /// <summary>
        /// NT-Проверить что каталог проекта только для чтения
        /// </summary>
        /// <returns></returns>
        public bool isReadOnly()
        {
            return Utility.isReadOnly(this.m_MainFolderPath);
        }
    }
}
