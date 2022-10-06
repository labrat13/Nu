using System;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Text;

namespace PMEngine
{
    /// <summary>
    /// Содержит различные вспомогательные операции 
    /// </summary>
    public class Utility
    {
        /// <summary>
        /// Разделитель строк CSV для процедур парсинга
        /// </summary>
        internal static Char[] CsvDelimiter = { ';' };

        /// <summary>
        /// NT-Удалить из строки текста CSV-разделители
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static string StringRemoveCsvChars(string p)
        {
            String result = p.Replace(";", ".,");

            return result;
        }
        /// <summary>
        /// NT-Return date string as 23.01.2018 23:14:59
        /// </summary>
        /// <param name="time"></param>
        /// <returns></returns>
        public static string StringFromDateTime(DateTime time)
        {
            return time.ToString(CultureInfo.GetCultureInfo("ru-RU"));//23.01.2018 23:14:59
        }

        internal static DateTime DateTimeFromString(string p)
        {
            return DateTime.Parse(p, CultureInfo.GetCultureInfo("ru-RU"));
        }

        /// <summary>
        /// NR-Конвертировать локальный путь в сетевой путь
        /// </summary>
        /// <param name="localPath">Локальный файловый путь к документу</param>
        /// <returns>Возвращает сетевой путь к документу</returns>
        public static string LocalPathToUNCpath(string localPath)
        {
            //TODO: использовать готовую функцию из моей библиотеки классов
            //а тут просто обертка к ней будет
            //А ее там и нет - она в Инвентарь была реализована, а в библиотеку классов я ее не внес.
            //Наверно, думал, что нигде более не потребуется.
            //Надо и тут ее реализовать и в библиотеку добавить после тестирования.
            
            throw new NotImplementedException();
        }
        /// <summary>
        /// NR-Конвертировать сетевой путь в локальный путь
        /// </summary>
        /// <param name="UNCpath">сетевой путь к документу</param>
        /// <returns>Возвращает локальный файловый путь к документу</returns>
        public static string UNCpathToLocalPath(string UNCpath)
        {
            //TODO: использовать готовую функцию из моей библиотеки классов
            //а тут просто обертка к ней будет
            throw new NotImplementedException();
        }

        /// <summary>
        /// NT-Получить текущую версию движка (сборки).
        /// </summary>
        /// <returns></returns>
        public static Version getCurrentEngineVersion()
        {
            return Assembly.GetExecutingAssembly().GetName().Version;
        }

        /// <summary>
        /// NT-Проверить допустимость версии движка из проекта
        /// </summary>
        /// <param name="versionString">Проверяемая версия движка</param>
        /// <returns>Возвращает True, если версии совместимы, False в противном случае.</returns>
        /// <remarks>
        /// Должны совпадать Version.Major и Version.Minor значения.
        /// </remarks>
        public static bool isCompatibleEngineVersion(String versionString)
        {
            Version prjVersion = new Version(versionString);
            return isCompatibleEngineVersion(prjVersion);
        }

        /// <summary>
        /// NT-Проверить допустимость версии движка из проекта
        /// </summary>
        /// <param name="version">Проверяемая версия движка</param>
        /// <returns>Возвращает True, если версии совместимы, False в противном случае.</returns>
        /// <remarks>
        /// Должны совпадать Version.Major и Version.Minor значения.
        /// </remarks>
        public static bool isCompatibleEngineVersion(Version version)
        {
            Version curVersion = Utility.getCurrentEngineVersion();
            return ((curVersion.Major == version.Major) && (curVersion.Minor == version.Minor));
        }

        #region Forms functions
        /// <summary>
        /// Цвет для выделения неправильного веб-имени  фоном текстового поля
        /// </summary>
        internal static Color InvalidWebNameBackColor = Color.MistyRose;

        /// <summary>
        /// NT-Установить цвет ошибки для текстбокса и вывести сообщение в строке состояния, если она есть
        /// </summary>
        /// <param name="wrong">Флаг ошибки</param>
        /// <param name="control">Текстбокс</param>
        /// <param name="statusBarLabel">Объект текста на статусбаре или null</param>
        /// <param name="statusMsg">Сообщение об ошибке, для статусбара</param>
        public static void colorizeWrongTextBox(bool wrong, TextBox control, ToolStripStatusLabel statusBarLabel, String statusMsg)
        {
            Color backColor;
            //set error color for textbox
            if (wrong)
            {
                backColor = InvalidWebNameBackColor;

            }
            else
            {
                backColor = Color.White;
            }
            control.BackColor = backColor;

            //set new status bar message text
            String msg;
            if (statusBarLabel != null)
            {
                if (wrong)
                    msg = statusMsg;
                else
                    msg = String.Empty;
                //set new text
                statusBarLabel.Text = msg;
            }

            return;
        }
        #endregion
        #region File and naming functions

        /// <summary>
        /// NT-Возвращает флаг что указанное Хранилище может обновляться.
        /// </summary>
        /// <returns>Returns True if storage is ReadOnly, False otherwise</returns>
        public static bool isReadOnly(String folderPath)
        {
            bool ro = false;
            //generate test file name
            String test = Path.Combine(folderPath, "writetest.txt");

            try
            {
                //if test file already exists, try remove it
                if (File.Exists(test))
                    File.Delete(test);//тут тоже будет исключение, если каталог read-only
                //test creation 
                FileStream fs = File.Create(test);
                fs.Close();
            }
            catch (Exception)
            {
                ro = true;
            }
            finally
            {
                File.Delete(test);
            }
            return ro;
        }

        /// <summary>
        /// NR-Изменить название каталога так, чтобы оно не содержало недопустимых символов.
        /// И пробелов. И не короче 3 символов, и не длиннее 16 символов.
        /// </summary>
        /// <param name="folderName">Название каталога</param>
        /// <returns></returns>
        public static string makeSafeFolderName(string folderName)
        {
            String result = RemoveWrongSymbols(folderName, 16);
            return result;
        }

        /// <summary>
        /// Заменить неправильные символы пути файла в строке на указанный символ
        /// </summary>
        /// <param name="s">Строка пути файла</param>
        /// <param name="rChar">Символ на замену</param>
        /// <returns>Возвращает строку не содержащую неправильных символов имени файла</returns>
        public static string ReplaceInvalidFilenameChars(string s, char rChar)
        {
            //Получаем массив запрещенных символов
            char[] inv = Path.GetInvalidFileNameChars();
            //Создаем билдер для сборки символов
            StringBuilder sb = new StringBuilder();
            foreach (Char c in s)
            {
                //если символ есть в массиве, то вместо него пишем замену, иначе пишем сам символ
                if (Array.IndexOf(inv, c) == -1)
                    sb.Append(c);
                else sb.Append(rChar);
            }
            return sb.ToString();
        }

        /// <summary>
        /// NFT-Нормализовать имя файла или каталога
        /// </summary>
        /// <param name="title">имя файла без расширения</param>
        /// <param name="maxLength">Максимальная длина имени, в символах</param>
        /// <returns>Возвращает нормализованное название файла или каталога, без расширения.</returns>
        /// <remarks>
        /// Функция заменяет на подчеркивания _ все символы, кроме букв и цифр.
        /// Если в названии есть пробелы, они удаляются, а следующий символ переводится в верхний регистр.
        /// Если в названии есть символ 'µ', он заменяется на символ 'u'.
        /// Если получившееся название длиннее maxLength, то оно обрезается до maxLength.
        /// Если получившееся название является зарезервированным системным названием (вроде CON), или
        /// если получившееся название короче 3 символов, к нему добавляется случайное число.
        /// </remarks>
        public static string RemoveWrongSymbols(string title, int maxLength)
        {
            //TODO: Optimize - переработать для ускорения работы насколько это возможно
            //надо удалить все запрещенные символы
            //если пробелы, то символ после них перевести в верхний регистр
            //если прочие символы, заменить на подчеркивание
            //если имя длиннее maxLength, то обрезать до maxLength.
            Char[] chars = title.ToCharArray();
            //create string builder
            StringBuilder sb = new StringBuilder(chars.Length);
            //если символ в строке является недопустимым, заменить его на подчеркивание.
            Char c;
            bool toUp = false;//для перевода в верхний регистр
            foreach (char ch in chars)
            {
                c = ch;
                if (ch == ' ')
                {
                    toUp = true;
                    //ничего не записываем в выходной накопитель - так пропускаем все пробелы.
                }
                else
                {
                    //foreach (char ct in RestrictedWebLinkSymbols)
                    //{
                    //    if (ch.Equals(ct))
                    //        c = '_';//замена недопустимого символа на подчеркивание
                    //}
                    //Unicode chars throw exceptions

                    //тут надо пропускать только -_A-Za-zА-Яа-я и все равно будут проблемы с именами файлов архива

                    if (!Char.IsLetterOrDigit(ch))
                        c = '_';//замена недопустимого символа на подчеркивание
                    //перевод в верхний регистр после пробела
                    if (toUp == true)
                    {
                        c = Char.ToUpper(c);
                        toUp = false;
                    }
                    //if c == мю then c = u
                    if (c == 'µ') c = 'u';
                    //добавить в выходной накопитель
                    sb.Append(c);
                }
            }
            //если имя длиннее максимума, обрезать по максимуму
            if (sb.Length > maxLength) sb.Length = maxLength;
            //если имя короче минимума, добавить псевдослучайную последовательность.
            //и проверить, что имя не запрещенное
            if ((sb.Length < 3) || isRestrictedFileName(sb.ToString()))
            {
                sb.Append('_');
                sb.Append(new Random().Next(10, 100).ToString(CultureInfo.InvariantCulture));
            }

            return sb.ToString();
        }

        /// <summary>
        /// NT-Создать каталог, в котором запрещено индексирование средствами операционной системы
        /// </summary>
        /// <param name="folderPath">Путь к создаваемому каталогу</param>
        public static void CreateNotIndexedFolder(String folderPath)
        {
            DirectoryInfo di = new DirectoryInfo(folderPath);
            //create directory
            if (!di.Exists)
                di.Create();
            //set attribute Not indexed
            di.Attributes = FileAttributes.NotContentIndexed | FileAttributes.Directory;
            di = null;
            return;
        }



        /// <summary>
        /// Массив запрещенных имен файлов - для коррекции имен файлов
        /// </summary>
        private static String[] RestrictedFileNames = { "CON", "PRN", "AUX", "CLOCK$", "NUL", "COM1", "LPT1", "LPT2", "LPT3", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "COM10", "COM11", "COM12", "COM13", "COM14", "COM15", "COM16", "COM17", "COM18", "COM19" };

        /// <summary>
        /// NT-Проверить, что имя файла является неправильным
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool isRestrictedFileName(string p)
        {
            //fast check = check length
            if ((p.Length > 6) || (p.Length < 3))
                return false;
            //slow check - check content
            foreach (String s in RestrictedFileNames)
                if (String.Equals(s, p, StringComparison.OrdinalIgnoreCase))
                    return true;
            //no restrictions
            return false;
        }
        #endregion
    }
}
