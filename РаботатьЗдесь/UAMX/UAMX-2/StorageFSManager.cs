using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UAMX_2
{
    /// <summary>
    /// NR-Менеджер файловой системы Хранилища
    /// </summary>
    /// <remarks>
    /// Управляет корневым каталогом Хранилища и его содержимым
    /// </remarks>
    public class StorageFsManagerBase: FsManagerBase
    {
        #region Константы



        #endregion

        #region Поля
        //поля определены в производном классе
        #endregion

        /// <summary>
        /// NT-Конструктор
        /// </summary>
        /// <param name="engine">Обратная ссылка на движок</param>
        /// <param name="storagePath">Путь к управляемому каталогу</param>
        public StorageFsManagerBase(Engine engine, String storagePath)
            : base(engine, storagePath)
        {
        }

        #region Проперти

        #endregion

        /// <summary>
        /// NT-Получить путь к файлу БД в текущей конфигурации Движка.
        /// </summary>
        /// <returns></returns>
        public virtual string getDbFilePath()
        {
            return Path.Combine(m_mainFolderPath, DbAdapterBase.DatabaseFileName);
        }

        //use base class func
        ///// <summary>
        ///// NR-Получить текстовое представление объекта
        ///// </summary>
        ///// <returns></returns>
        //public override string ToString()
        //{
        //    return String.Format("Dir={0}", this.m_mainFolderPath);
        //}

        /// <summary>
        /// NR-Создать каталог Хранилища
        /// </summary>
        /// <param name="prjFolder">Путь к еще не существующему каталогу Хранилища</param>
        public static void CreateStorageFolder(string prjFolder, EngineSettingsBase sett)
        {
            throw new NotImplementedException();
            
            //TODO: эту функцию переопределить в производном классе
            
            ////check folder is not exists
            //if (Directory.Exists(prjFolder))
            //    throw new ArgumentException(String.Format("Directory already exists: {0}", prjFolder), "prjFolder");
            ////create project folder with all files and folders
            //CreateNotIndexedFolder(prjFolder);
            ////1 create db file
            //String path = Path.Combine(prjFolder, DbAdapter.DatabaseFileName);
            //DbAdapter.extractDbFile(path);
            ////2 create setting file
            //path = Path.Combine(prjFolder, EngineSettings.DescriptionFileName);
            //sett.Store(path);
            ////3 create other elements...
            ////TODO: Project folder create: create other files and folders

            //return;
        }

        /// <summary>
        /// NR-Проверить, что указанный каталог является каталогом Хранилища
        /// </summary>
        /// <param name="path">Путь к каталогу</param>
        /// <returns>Возвращает true, если каталог является каталогом Хранилища. В противном случае возвращает false</returns>
        public static new bool isFolderForThis(string path)
        {
            throw new NotImplementedException();
            
            ////TODO: эту функцию переопределить в производном классе
            ////Не получится сделать эту функцию не-статической - она должна вызываться из других сборок, она - часть стандарта на движки.  
            
            ////Этот код только для примера, его нужно переписать
            ////TODO: сделать качественно эту функцию
            ////проверить:
            ////1 что это не пустая строка
            //if (String.IsNullOrEmpty(path)) return false;
            ////2 что это каталог и он существует
            //if (!Directory.Exists(path)) return false;
            ////3 что это каталог проекта движка

            ////критерии:
            ////папка должна содержать файл "settings.xml"
            ////папка должна содержать файл db.mdb
            ////папка должна содержать ... TODO: добавить признаки каталога данных проекта здесь
            ////файл "settings.xml" должен читаться без проблем

            //String p;

            ////проверяем наличие обязательных файлов и каталогов
            ////p = Path.Combine(path, ArchiveController.DocumentsDir); - пример
            ////if (!Directory.Exists(p)) return false;

            ////проверяем наличие файла БД
            //p = Path.Combine(path, DbAdapterBase.DatabaseFileName);
            //if (!File.Exists(p)) return false;
            ////напоследок пытаемся загрузить и прочитать файл настроек движка
            //p = Path.Combine(path, EngineSettingsBase.DescriptionFileName);
            //if (!File.Exists(p)) return false;
            ////try load description file
            //if (EngineSettingsBase.TryLoad(p) == null) return false;

            //return true;
        }






    }
}
