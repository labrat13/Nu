using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using UAMX_2;

namespace Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            //LinkTest();//тестирование ссылок ElementLink

            //Test_ElementLink();
            Test_UtilityLink();
        }

        private static void Test_UtilityLink()
        {
            String[] uris = { 
            @"C:\Temp\clipboard.exe",
            @"C:\Temp\clipboard manager.exe",
            @"C:\Сталкер\клипборд менеджер.exe",
            @"C:\!Сталкер\клипборд -менеджер.exe",
            @"C:\Documents and Settings\1\Рабочий стол\12Ё`~!@#$%^&()_-+=,.txt"
        };
            //test functions
            List<String> li = new List<string>();

            foreach (String s in uris)
            {
                //При ошибке возвращают пустую строку
                //string p = Utility.makeFileUrlFromAbsoluteNTPath(s);// - работает так как надо
                //string p = Utility.makeUriFromAbsoluteFilePath(s);//экранирует все русские символы, текст ссылки не читаемый
                string p = Utility.makeUriFromRelativeFilePath("C:\\", s);//работает только для существующих файлов
                li.Add(p);
            }

            return;
        }

        private static void Test_ElementLink()
        {
            String[] uris = { 
            //правильные версии ссылок
            @"inv:///p123456",//Это правильная короткая версия ссылки            
            @"inv:///p123456?arg1=1&arg2=2",//Это правильная короткая версия ссылки с аргументами
            @"inv:///p123456_Название_задачи",
            @"inv:///p123456_Название_задачи?arg1=Картошка&arg2=Сало",
        };
            List<ElementLink> links = new List<ElementLink>();


            foreach (String s in uris)
            {
                String p1;
                String p2;
                String p3;            
                //links.Add(new ElementLink(s));
                ElementLink.TryParse(s, out p1, out p2, out p3);
            }
                return;
        }


        #region Link testing
        private static void LinkTest()
        {
            //вот написал код, провел тест, получил результаты - а все равно не понимаю, что это и что с этим делать.
            //тупняк какой-то в голове.
            //file:///C:\Documents%20and%20Settings\1\Мои%20документы\Visual%20Studio%202008\Projects\UAMX\Tester\docs\urilog.txt
            //а имя хоста вообще тут нужно? Это же локальные ссылки. Вряд ли они будут на другом компьютере находиться.
            //короче, пока я не могу разобраться, как устроить ссылки и организовать их для работы в сети.
            //поэтому проще всего сделать как получится, а потом переработать их отдельным изучением проблемы - тогда, когда она проявится и будет доступна для изучения.
            //Вот поэтому-то у меня код все время такой кривой... И его всегда можно переделывать, чтобы улучшить.
            //А, понятно: приложение не получит аргументы ссылки - оно должно получить саму ссылку как аргумент. 
            // И вот в ней-то и можно указывать параметры.
            //Например: ссылка file:///C:/Temp/clv.exe?task://p123456%20Задача%20Название%20задачи сработает:
            // будет вызвано само приложение, но ему не будет передана ссылка никак.
            //А: inv://Computer/p123456?t=Задача%20Название%20задачи сработает так: 
            //- будет вызвано приложение, ассоциированное с схемой inv, и ему будет передана вся ссылка как 1 аргумент.
            //- приложение должно распарсить ссылку и извлечь из нее все, что нужно.
            //При этом, место для Хост - ни пригодится никак, так как все установлено на одном локальном компе.
            //Но оставим /// - оно и по стандарту, и места много не занимает, и в будущем может пригодиться, наверное.
            //Итак, правильная ссылка: inv:///p123456 (короткая ссылка) и inv:///p123456_Название_задачи  (длинная ссылка)
            //а все слеши в путях файлов заменить на /
            //Пути для ярлыков: такие же, как и в ссылках.
            //Параметры к ссылке прицеплять, если надо: inv:///p123456?arg1=1&arg2=2

            #region results log
            /*
             * Только результаты для правильных ссылок тут
--------------------------------------
inv:///p123456
--------------------------------------
AbsolutePath=/p123456
AbsoluteUri=inv:///p123456
Authority=
DnsSafeHost=
Fragment=
Host=
HostNameType=Basic
IsAbsoluteUri=True
IsDefaultPort=True
IsFile=False
IsLoopback=True
IsUnc=False
IsWellFormedOriginalString()=True
LocalPath=/p123456
OriginalString=inv:///p123456
PathAndQuery=/p123456
Port=-1
Query=
Scheme=inv
Segments=System.String[]
	/
	p123456
UserEscaped=False
UserInfo=

--------------------------------------
inv:///p123456?arg1=1&arg2=2
--------------------------------------
AbsolutePath=/p123456
AbsoluteUri=inv:///p123456?arg1=1&arg2=2
Authority=
DnsSafeHost=
Fragment=
Host=
HostNameType=Basic
IsAbsoluteUri=True
IsDefaultPort=True
IsFile=False
IsLoopback=True
IsUnc=False
IsWellFormedOriginalString()=True
LocalPath=/p123456
OriginalString=inv:///p123456?arg1=1&arg2=2
PathAndQuery=/p123456?arg1=1&arg2=2
Port=-1
Query=?arg1=1&arg2=2
Scheme=inv
Segments=System.String[]
	/
	p123456
UserEscaped=False
UserInfo=

--------------------------------------
inv:///p123456_Название_задачи
--------------------------------------
AbsolutePath=/p123456_%D0%9D%D0%B0%D0%B7%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5_%D0%B7%D0%B0%D0%B4%D0%B0%D1%87%D0%B8
AbsoluteUri=inv:///p123456_%D0%9D%D0%B0%D0%B7%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5_%D0%B7%D0%B0%D0%B4%D0%B0%D1%87%D0%B8
Authority=
DnsSafeHost=
Fragment=
Host=
HostNameType=Basic
IsAbsoluteUri=True
IsDefaultPort=True
IsFile=False
IsLoopback=True
IsUnc=False
IsWellFormedOriginalString()=False
LocalPath=/p123456_Название_задачи
OriginalString=inv:///p123456_Название_задачи
PathAndQuery=/p123456_%D0%9D%D0%B0%D0%B7%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5_%D0%B7%D0%B0%D0%B4%D0%B0%D1%87%D0%B8
Port=-1
Query=
Scheme=inv
Segments=System.String[]
	/
	p123456_%D0%9D%D0%B0%D0%B7%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5_%D0%B7%D0%B0%D0%B4%D0%B0%D1%87%D0%B8
UserEscaped=False
UserInfo=

--------------------------------------
inv:///p123456_Название_задачи?arg1=Картошка&arg2=Сало
--------------------------------------
AbsolutePath=/p123456_%D0%9D%D0%B0%D0%B7%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5_%D0%B7%D0%B0%D0%B4%D0%B0%D1%87%D0%B8
AbsoluteUri=inv:///p123456_%D0%9D%D0%B0%D0%B7%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5_%D0%B7%D0%B0%D0%B4%D0%B0%D1%87%D0%B8?arg1=%D0%9A%D0%B0%D1%80%D1%82%D0%BE%D1%88%D0%BA%D0%B0&arg2=%D0%A1%D0%B0%D0%BB%D0%BE
Authority=
DnsSafeHost=
Fragment=
Host=
HostNameType=Basic
IsAbsoluteUri=True
IsDefaultPort=True
IsFile=False
IsLoopback=True
IsUnc=False
IsWellFormedOriginalString()=False
LocalPath=/p123456_Название_задачи
OriginalString=inv:///p123456_Название_задачи?arg1=Картошка&arg2=Сало
PathAndQuery=/p123456_%D0%9D%D0%B0%D0%B7%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5_%D0%B7%D0%B0%D0%B4%D0%B0%D1%87%D0%B8?arg1=%D0%9A%D0%B0%D1%80%D1%82%D0%BE%D1%88%D0%BA%D0%B0&arg2=%D0%A1%D0%B0%D0%BB%D0%BE
Port=-1
Query=?arg1=%D0%9A%D0%B0%D1%80%D1%82%D0%BE%D1%88%D0%BA%D0%B0&arg2=%D0%A1%D0%B0%D0%BB%D0%BE
Scheme=inv
Segments=System.String[]
	/
	p123456_%D0%9D%D0%B0%D0%B7%D0%B2%D0%B0%D0%BD%D0%B8%D0%B5_%D0%B7%D0%B0%D0%B4%D0%B0%D1%87%D0%B8
UserEscaped=False
UserInfo=
            */

            #endregion

            //test URI parser
            String[] uris = {
            @"inv://Computer/p123456",//Это линк с именем компьютера (этого или другого компьютера в сети)
            @"file:///C:/Temp/clv.exe?item=катодный%20дефлектор&value=1",// Это линк для ярлыка виндовс. Но как приложение получит эти аргументы?
                //exe запускается, но аргументы ему не передаются. Облом.
            @"inv://p123456",//обычная ссылка, как сейчас, короткая. хост=p123456 путь=""
            @"task://p123456",//то же, другой префикс
            @"task://p123456?Задача%20Название%20задачи",//кривая версия ссылки: домен ? название задачи
            @"file:///C:/Temp/clv.exe?task://p123456%20Задача%20Название%20задачи",//Это версия, которая будет передана БраузеруЗадач через ярлык рабочего стола
                //exe запускается, но аргументы ему не передаются. Облом.
            @"inv://Computer/p123456?Задача%20Название%20задачи",//Это нестандартная ссылка? 
            @"inv://Computer/p123456?t=Задача%20Название%20задачи",//Это вот стандартная ссылка хтмл с аргументами
            @"inv://Computer/p123456_Задача_Название_задачи", //тут надо парсить домен p123456_Задача_Название_задачи. Это кастомная ссылка, не стандартная.
            
            //правильные версии ссылок
            @"inv:///p123456",//Это правильная короткая версия ссылки            
            @"inv:///p123456?arg1=1&arg2=2",//Это правильная короткая версия ссылки с аргументами
            @"inv:///p123456_Название_задачи",
            @"inv:///p123456_Название_задачи?arg1=Картошка&arg2=Сало",
        };
            //куча ссылок, я наклепал без понимания, зачем и как. Почему-то мысль не работает.

            StreamWriter sw = new StreamWriter("C:\\Temp\\urilog.txt");
            sw.WriteLine("Содержимое объекта Uri и тест парсинга ссылок");
            sw.WriteLine();
            foreach (String u in uris)
            {
                sw.WriteLine("--------------------------------------");
                sw.WriteLine(u);
                sw.WriteLine("--------------------------------------");
                printUriContent(sw, u);
                sw.WriteLine();
            }
            sw.Close();
            return;
        }


        private static void printUriContent(StreamWriter sw, string u)
        {
            try
            {
                Uri uri = new Uri(u);
                printWithNull(sw, "AbsolutePath", uri.AbsolutePath);
                printWithNull(sw, "AbsoluteUri", uri.AbsoluteUri);
                printWithNull(sw, "Authority", uri.Authority);
                printWithNull(sw, "DnsSafeHost", uri.DnsSafeHost);
                printWithNull(sw, "Fragment", uri.Fragment);
                printWithNull(sw, "Host", uri.Host);
                printWithNull(sw, "HostNameType", uri.HostNameType);
                printWithNull(sw, "IsAbsoluteUri", uri.IsAbsoluteUri);
                printWithNull(sw, "IsDefaultPort", uri.IsDefaultPort);
                printWithNull(sw, "IsFile", uri.IsFile);
                printWithNull(sw, "IsLoopback", uri.IsLoopback);
                printWithNull(sw, "IsUnc", uri.IsUnc);
                printWithNull(sw, "IsWellFormedOriginalString()", uri.IsWellFormedOriginalString());
                printWithNull(sw, "LocalPath", uri.LocalPath);
                printWithNull(sw, "OriginalString", uri.OriginalString);
                printWithNull(sw, "PathAndQuery", uri.PathAndQuery);
                printWithNull(sw, "Port", uri.Port);
                printWithNull(sw, "Query", uri.Query);
                printWithNull(sw, "Scheme", uri.Scheme);
                printWithNull(sw, "Segments", uri.Segments);
                printWithNull(sw, "UserEscaped", uri.UserEscaped);
                printWithNull(sw, "UserInfo", uri.UserInfo);

                return;
            }
            catch (Exception e)
            {
                sw.WriteLine(e.ToString());
            }
            return;
        }

        private static void printWithNull(StreamWriter sw, string title, string[] field)
        {
            if (field == null)
                sw.WriteLine(String.Format("{0}=Null", title));
            else
            {
                sw.WriteLine(String.Format("{0}={1}", title, field.ToString()));
                foreach (String s in field)
                    sw.WriteLine(String.Format("\t{0}", s));
            }
        }

        private static void printWithNull(StreamWriter sw, string title, int field)
        {
                sw.WriteLine(String.Format("{0}={1}", title, field.ToString()));
        }

        private static void printWithNull(StreamWriter sw, string title, bool field)
        {
                sw.WriteLine(String.Format("{0}={1}", title, field.ToString()));
        }

        private static void printWithNull(StreamWriter sw, string title, UriHostNameType field)
        {
                sw.WriteLine(String.Format("{0}={1}", title, field.ToString()));
        }

        private static void printWithNull(StreamWriter sw, string title, string field)
        {
            if (field == null)
                sw.WriteLine(String.Format("{0}=Null", title));
            else
                sw.WriteLine(String.Format("{0}={1}", title, field));
            
            return;
        }
#endregion




    }
}
