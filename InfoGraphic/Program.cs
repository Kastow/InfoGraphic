using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace InfoGraphic
{
    class Parsing
    {
        //Преобразовывает .тхт в список строк с датой в начале
        public LinkedList<string> toLineList(string path)
        {
            string line, date = "";
            LinkedList<string> lines = new LinkedList<string>();
            StreamReader sr = new StreamReader(path);
            
            while ((line = sr.ReadLine()) != null)
            {
                //Форматируем время
                if (line.Contains("War Correspondent#4734"))
                {
                    line = line.Substring(line.IndexOf("["));
                    line = line.Substring(0, line.IndexOf("]") + 1);
                    date = line;
                }
                else
                {
                    line = date + line;

                    if(line.Length!=16)
                    lines.AddLast(line);
                }
                
            }
            return lines;
        }

        //Удаляет лишние строки из списка
        public LinkedList<string> removeTrash(LinkedList<string> inputlines)
        {
            LinkedList<string> lines = new LinkedList<string>();
            foreach (string s in inputlines)
            {
                if(!(s.Contains("a town") || s.Contains("outpost has") || s.Contains("so far")
                    || s.Contains("Internal") || s.Contains("192.168")
                    || s.Contains("Lobby Islands")||s.Contains("weekly war has started") || s.Contains("Development"))){
                    lines.AddLast(s);
                }
            }
                return lines;
        }


        //Удаляет захват/потерю городов (40к+ строк!)
        public LinkedList<string> removeTownCaptures(LinkedList<string> inputlines)
        {
            LinkedList<string> lines = new LinkedList<string>();
            foreach (string s in inputlines)
            {
                if (!(s.Contains("have lost") || s.Contains("have taken")))
                {
                    lines.AddLast(s);
                }
            }
            return lines;
        }

        //Форматирование сообщений о потерях
        public LinkedList<string> formatCasualties(LinkedList<string> inputlines)
        {
            LinkedList<string> lines = new LinkedList<string>();
            foreach (string s in inputlines)
            {
                string line = s;
                if (line.Contains(" - "))
                {
                    line = line.Replace(" - ", ";");
                  
                }
                if (line.Contains("(Weekly War)"))
                {
                    line = line.Replace("(Weekly War)", "");

                }
                if (line.Contains("Day"))
                {
                    line = line.Replace("Day /d+", "");

                }
                if (line.Contains("casualties have reached"))
                {
                    line = line.Replace(" casualties have reached ", ";");
                    lines.AddLast(line);
                }
                else
                if (line.Contains("total enlistments"))
                {
                    line = line.Replace(" total enlistments,", ";");
                    line = line.Replace(" casualties,", ";").Replace(" casualties.", ";");
                    lines.AddLast(line);
                }
                else
                    lines.AddLast(line);
            }
            return lines;
        }

    }
    class Program
    {   
        static void Main(string[] args)
        {
            Parsing p = new Parsing();
            String path = "C:/Users/777/Desktop/Foxhole Management/InfoGraphic/source.txt";
            String newpath = "C:/Users/777/Desktop/Foxhole Management/InfoGraphic/";
            String dateformat = "[dd.MM.yy;HH:mm]";
            DateTime date;
            CultureInfo provider = CultureInfo.InvariantCulture;


            int linecounter = 0;

            //блок названия файла
            date = DateTime.ParseExact("[04.03.17;20:30]", dateformat, provider);
            Console.WriteLine(date.ToString());
            Console.WriteLine("Куда пишем?");
            newpath=newpath+Console.ReadLine()+".txt";
            Console.WriteLine(newpath);
            //блок обработки файла
            /*Создаем список массивов
             1 - дата
             2 - сервер
             3 - карта
             4 - потери колонистов
             5 - потери варденов
             Формат образца конец 2016: [20.10.16;02:53][Development] Day 1, 1:54 Hours: **Colonial** casualties have reached 1.
             Потери сторон отдельно, карта не указана
             
             Формат образца начало 2017: [20.02.17;12:30][Fox3 - Deadlands (Weekly War)] **Warden** casualties have reached 12,651.
             Потери сторон отдельно, карта указана

             Современный формат (с марта '17): [17.03.17;17:12][Fox1 - Deadlands - Day 2] 133 total enlistments, 9 Colonial casualties, 4 Warden casualties.
             Потери сторон вместе, карта указана
             */


            LinkedList<string> lines = new LinkedList<string>();
            //lines.CopyTo(Firstformat(path));
            //Форматируем строки и убираем мусор
            lines = p.toLineList(path);
            lines = p.removeTrash(lines);
            lines = p.removeTownCaptures(lines);
            lines = p.formatCasualties(lines);


            //блок записи файла
            TextWriter tw = new StreamWriter(newpath);
            foreach (string s in lines)
            {
                tw.WriteLine(s);
                linecounter++;
               
            }
            tw.WriteLine("Всего строк в файле:" + linecounter);
            tw.Close();
            
            //Вывод количества строк
            Console.WriteLine("Всего строк в файле:"+linecounter);
            Console.ReadLine();
        }

       
    }
}
