using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace UAMX_2
{
    /// <summary>
    /// NT-Вспомогательные функции для разных классов 
    /// </summary>
    public class Utility
    {

        #region Функции таймштампа
        /// <summary>
        /// NT-Получить строку таймштампа как 23.01.2018 23:14:59
        /// </summary>
        /// <param name="time">Объект DateTime</param>
        /// <returns>Возвращает строку таймштампа</returns>
        public static string StringFromDateTime(DateTime time)
        {
            return time.ToString(CultureInfo.GetCultureInfo("ru-RU"));//23.01.2018 23:14:59
        }
        /// <summary>
        /// NT-Получить DateTime объект из строки таймштампа
        /// </summary>
        /// <param name="p">Строка таймштампа</param>
        /// <returns>Возвращает объект DateTime</returns>
        public static DateTime DateTimeFromString(string p)
        {
            return DateTime.Parse(p, CultureInfo.GetCultureInfo("ru-RU"));
        }
        #endregion


 

        #region File and naming functions



        /// <summary>
        /// NT-Изменить название каталога так, чтобы оно не содержало недопустимых символов.
        /// И пробелов. И не короче 3 символов, и не длиннее 16 символов.
        /// </summary>
        /// <param name="folderName">Название каталога</param>
        /// <returns>Возвращает переработанное название каталога</returns>
        public static string makeSafeFolderName(string folderName)
        {
            String result = RemoveWrongSymbols(folderName, 16);
            return result;
        }

        /// <summary>
        /// NT-Удалить указанную подстроку итеративно.
        /// </summary>
        /// <param name="text">Исходный текст</param>
        /// <param name="subString">Удаляемая последовательность символов</param>
        /// <returns>Возвращает измененный текст, не содержащий указанную последовательность символов.</returns>
        /// <remarks>
        /// Подстрока удаляется в несколько проходов, пока она существует.
        /// </remarks>
        public static string removeSubstring(String text, String subString)
        {
            //я хотел переделать на StringBuilder, но в нем нет функции text.Contains(subString)
            while (text.Contains(subString))
                text = text.Replace(subString, String.Empty);

            return text;
        }

        /// <summary>
        /// NT-Заменить неправильные символы пути файла в строке на указанный символ
        /// </summary>
        /// <param name="s">Строка пути файла</param>
        /// <param name="rChar">Символ на замену</param>
        /// <returns>Возвращает строку не содержащую неправильных символов имени файла</returns>
        public static string ReplaceInvalidFilenameChars(string s, char rChar)
        {
            //Получаем массив запрещенных символов
            char[] inv = Path.GetInvalidFileNameChars();

            return replaceRestrictedChars(s, inv, rChar);
        }

        /// <summary>
        /// NT-Заменить все неправильные символы в тексте на указанный символ
        /// </summary>
        /// <param name="text">Исходная строка текста</param>
        /// <param name="restrictedChars">Массив неправильных символов</param>
        /// <param name="replace">Символ на замену</param>
        /// <returns>Возвращает строку не содержащую неправильных символов</returns>
        public static string replaceRestrictedChars(String text, Char[] restrictedChars, Char replace)
        {
            //Создаем билдер для сборки символов
            StringBuilder sb = new StringBuilder();
            foreach (Char c in text)
            {
                //если символ есть в массиве, то вместо него пишем замену, иначе пишем сам символ
                if (Array.IndexOf(restrictedChars, c) == -1)
                    sb.Append(c);
                else sb.Append(replace);
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
            if ((sb.Length < 3) || isRestrictedFSName(sb.ToString()))
            {
                sb.Append('_');
                sb.Append(new Random().Next(10, 100).ToString(CultureInfo.InvariantCulture));
            }

            return sb.ToString();
        }





        /// <summary>
        /// Массив запрещенных имен файлов - для коррекции имен файлов
        /// </summary>
        private static String[] RestrictedFileNames = { "CON", "PRN", "AUX", "CLOCK$", "NUL", "COM1", "LPT1", "LPT2", "LPT3", "COM2", "COM3", "COM4", "COM5", "COM6", "COM7", "COM8", "COM9", "COM10", "COM11", "COM12", "COM13", "COM14", "COM15", "COM16", "COM17", "COM18", "COM19" };

        /// <summary>
        /// NT-Проверить, что имя файла или папки является неправильным
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public static bool isRestrictedFSName(string p)
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

        #region Функции разбора названий

        /// <summary>
        /// NT-Разбить название на группы символов
        /// </summary>
        /// <param name="partName">Название детали</param>
        /// <returns>
        /// Возвращает массив частей названий.
        /// Например, ADM202EARN-z6 будет возвращен как {ADM, 202, EARN, z6 }
        /// Так эти названия проще распознавать и разделять на классы по некоторой стандартной классификации деталей.
        /// Хотя еще нужны способы определить, что название принадлежит к данной классификации деталей.
        /// - Для этого надо знать какой-то начальный абстрактный класс детали: например, Радиодеталь.
        ///   И уже в нем искать подходящий классификатор.
        /// </returns>
        public static string[] GetTitleParts(String title)
        {
            //разобрать строку на части: цифры в одну секцию, буквы в другую секцию, пробел,дефис, запятая итд - начало новой секции
            int state = 1; //0 - make new section, 1 - letter section, 2 - digit section
            StringBuilder sb = new StringBuilder();//buffer for section letters
            List<String> sections = new List<string>();//list of output sections
            foreach (Char c in title)
            {
                if (Char.IsLetter(c))
                {
                    if (state != 1)
                    {
                        if (sb.Length > 0) sections.Add(sb.ToString());//добавить секцию в список секций
                        sb.Length = 0;//очистить буфер символов
                        state = 1;//установить режим сборки букв
                    }
                    sb.Append(c);//добавить текущий символ в буфер
                }
                else if (Char.IsDigit(c))
                {
                    if (state != 2)
                    {
                        if (sb.Length > 0) sections.Add(sb.ToString());//добавить секцию в список секций
                        sb.Length = 0;//очистить буфер символов
                        state = 2;//установить режим сборки цифр
                    }
                    sb.Append(c);//добавить текущий символ в буфер
                }
                else //if (c == '-')
                {
                    //просто сбросить буфер в список секций, не добавлять текущий символ в буфер
                    if (sb.Length > 0) sections.Add(sb.ToString());//добавить секцию в список секций
                    sb.Length = 0;//очистить буфер символов
                    state = 1;//установить режим сборки начальный
                }
            }
            //символы кончились, надо остаток добавить в список секций
            if (sb.Length > 0) sections.Add(sb.ToString());
            return sections.ToArray();
        }

        #endregion

        /// <summary>
        /// NR- Получить название Элемента как безопасное для использования в качестве названия файла или папки
        /// </summary>
        /// <returns></returns>
        public static string makeSafeTitle(string elementTitle)
        {
            throw new NotImplementedException();//TODO: Add code here
        }

        /// <summary>
        /// NT-Возвращает флаг что указанное Хранилище может обновляться.
        /// </summary>
        /// <returns>Returns True if storage is ReadOnly, False otherwise</returns>
        public static bool isFolderReadOnly(String folderPath)
        {
            bool ro = false;
            //флаг, что исключение вызвано попыткой удаления файла из read-only папки
            bool failOnExists = false;
            //generate test file name
            String test = Path.Combine(folderPath, "writetest.txt");

            try
            {
                //if test file already exists, try remove it
                if (File.Exists(test))
                {
                    failOnExists = true;
                    File.Delete(test);//тут тоже будет исключение, если каталог read-only
                    failOnExists = false;
                }
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
                //удалять временный файл только если не он был причиной исключения
                if (failOnExists == false)
                    File.Delete(test);
            }
            return ro;
        }

        /// <summary>
        /// NT-Создать каталог, в котором запрещено индексирование средствами операционной системы
        /// </summary>
        /// <param name="folderPath">Путь к создаваемому каталогу</param>
        public static void createNotIndexedFolder(String folderPath)
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

    }
}
