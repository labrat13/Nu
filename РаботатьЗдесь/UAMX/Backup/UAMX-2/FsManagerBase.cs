using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace UAMX_2
{
    /// <summary>
    /// NR-Базовый класс Менеджера файловой системы.
    /// </summary>
    /// <remarks>
    /// Управляет корневым каталогом и его содержимым
    /// </remarks>
    public class FsManagerBase
    {
        #region Константы



        #endregion

        #region Поля

        /// <summary>
        /// Путь к каталогу, которым управляет менеджер
        /// </summary>
        protected string m_mainFolderPath;
        /// <summary>
        /// Обратная ссылка на движок
        /// </summary>
        protected Engine m_engine;//проперти не нужно здесь

        #endregion

        /// <summary>
        /// NT-Конструктор
        /// </summary>
        /// <param name="engine">Обратная ссылка на движок</param>
        /// <param name="storagePath">Путь к управляемому каталогу</param>
        public FsManagerBase(Engine engine, String storagePath)
        {
            this.m_engine = engine;
            this.m_mainFolderPath = storagePath;
        }

        #region Проперти

        /// <summary>
        /// Storage main folder path
        /// </summary>
        public string MainFolderPath
        {
            get
            {
                return this.m_mainFolderPath;
            }
            set
            {
                m_mainFolderPath = value;
            }
        }

        #endregion

        /// <summary>
        /// NT-Получить текстовое представление объекта
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("Dir={0}", this.m_mainFolderPath);
        }

        /// <summary>
        /// NR-Проверить, что указанный каталог является каталогом Хранилища
        /// </summary>
        /// <param name="path">Путь к каталогу</param>
        /// <returns>Возвращает true, если каталог является каталогом Хранилища. В противном случае возвращает false</returns>
        public static bool isFolderForThis(string path)
        {
            //TODO: эту функцию переопределить в производном классе
            //Не получится сделать эту функцию не-статической - она должна вызываться из других сборок, она - часть стандарта на движки.  
            throw new NotImplementedException();
        }

        /// <summary>
        /// NT-Проверить что каталог Хранилища только для чтения
        /// </summary>
        /// <returns></returns>
        public virtual bool isReadOnly()
        {
            return Utility.isFolderReadOnly(this.m_mainFolderPath);
        }

        /// <summary>
        /// NT-Собрать путь к файлу или каталогу
        /// </summary>
        /// <param name="item">Относительный путь к файлу или каталогу внутри управляемого каталога. Пример: \pics\image.png</param>
        /// <returns>Возвращает полный путь к файлу или каталогу внутри ГлавныйКаталог</returns>
        protected virtual string makePath(string item)
        {
            return Path.Combine(this.m_mainFolderPath, item);
        }


    }
}
