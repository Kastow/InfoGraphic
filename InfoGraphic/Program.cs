using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace InfoGraphic
{
    class battle
    {
        public LinkedList<message> messagelist;
        public string map, servername;
        public int wloss, closs;
        public int victor;
        public TimeSpan length;
        public DateTime start;
        public DateTime end;
    }
    class message
    {
        public DateTime date;    //дата
        public string servername; //название сервера
        public string mapname; //название карты
        public int ccas, wcas; //потери
        public int victory; //победа, 1 - колонисты, 2 - вардены
    }
    class Parsing
    {
        

        //преобразовывает первые 16 символов в переменную времени
        public DateTime GetTime(string dateString)
        {
            dateString = dateString.Substring(0, 16);
            CultureInfo provider = CultureInfo.InvariantCulture;
            String dateformat = "[dd.MM.yy;HH:mm]";
            DateTime time;
            time = DateTime.ParseExact(dateString, dateformat, provider);
            return time;
        }
        //выделяет из сообщения название сервера
        public string getServer(string message)
        {
            string servername = "";
            servername = message.Substring(17, message.Length-19);
            string[] splitter = servername.Split(';',']');
            servername = splitter[0];
            return servername;
        }
        //выделяет из сообщения название карты
        public string getMap(string message)
        {
            string mapname = "";
            mapname = message.Substring(17, message.Length-19);
            string[] splitter = mapname.Split(']',';');
            mapname = splitter[1];
            return mapname;
        }
        //выводит потери колонистов
        public int getColonials(string message)
        {
            int mapname = 0;
            if (message.Contains("**Colonial**"))
            {
                string[] splitter = message.Split(']', ';');
                string cas = Regex.Replace(splitter.Last().Trim('.'), ",", "");
                return Convert.ToInt32(cas);
            }
            if (message.Contains("Colonial;"))
            {
                string[] splitter = message.Split(']', ';');
                string[] splitter2 = splitter[splitter.Length - 3].Split(' ');
                string cas = Regex.Replace(splitter2[1], ",", "");
                return Convert.ToInt32(cas);
            }
            return 0;
        }
        //выводит потери варденов
        public int getWardens(string message)
        {
 
            if (message.Contains("**Warden**"))
            {
                string[] splitter = message.Split(']', ';');
                string cas = Regex.Replace(splitter.Last().Trim('.'),",","");
                return Convert.ToInt32(cas);
            }
            if (message.Contains("Warden;"))
            {
                string[] splitter = message.Split(']', ';');
                string[] splitter2 = splitter[splitter.Length - 2].Split(' ');
                string cas = Regex.Replace(splitter2[1], ",", "");
                return Convert.ToInt32(cas);
            }
            return 0;
        }
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
            string line = "";
            LinkedList<string> lines = new LinkedList<string>();
            foreach (string s in inputlines)
            {
                
                if(!(s.Contains("a town") || s.Contains("outpost has") || s.Contains("so far")
                    || s.Contains("Internal") || s.Contains("192.168")
                    || s.Contains("Lobby Islands")||s.Contains("weekly war has started") || s.Contains("Development"))){
                    line = s;
                    //line = Regex.Replace(line, "Day \d+, \d+:\d+ Hours:",);
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
                if (line.Contains("(W)"))
                {
                    line = line.Replace("(W)", "");

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
            LinkedList<string> lines = new LinkedList<string>();
            String path1 = "C:/Users/777/Desktop/Foxhole Management/InfoGraphic/logs/oldlogs.txt";
            String path2 = "C:/Users/777/Desktop/Foxhole Management/InfoGraphic/newlogs";
            
            DateTime date;
            CultureInfo provider = CultureInfo.InvariantCulture;


            int linecounter = 0;

      
            //Console.WriteLine(date.ToString());


            //блок названия файла
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
             29.11.16 - Deadlands
             06.12.16 - Upper Heartlands
             10.12.16 - Upper Heartlands
             17.12.16 - Deadlands 
             20.12.16 - Callahan's Passage
             24.12.16 - Callahan's Passage
             28.12.16 - Deadlands
             31.12.16 - Upper Heartlands
             07.01.17 - Callahan's Passage
             14.01.17 - Deadlands
             21.01.17 - F1 Upper Heartlands, F2 Callahan's Passage
             28.01.17 - F1 Callahan's Passage, F2 Deadlands
             04.02.17 - Upper Heartlands, Endless Shore



             Формат образца начало 2017: [20.02.17;12:30][Fox3 - Deadlands (Weekly War)] **Warden** casualties have reached 12,651.
             Потери сторон отдельно, карта указана

             Современный формат (с марта '17): [17.03.17;17:12][Fox1 - Deadlands - Day 2] 133 total enlistments, 9 Colonial casualties, 4 Warden casualties.
             Потери сторон вместе, карта указана
             */
             

            
            //lines.CopyTo(Firstformat(path));

            //Форматируем строки и убираем мусор
            lines = p.toLineList(path);
            lines = p.removeTrash(lines);
            lines = p.removeTownCaptures(lines);
            lines = p.formatCasualties(lines);



            Console.WriteLine("Time:"+p.GetTime(lines.ElementAt(200)));
            Console.WriteLine("Server:"+p.getServer(lines.ElementAt(200)));
            Console.WriteLine("Map:"+p.getMap(lines.ElementAt(200)));
            Console.WriteLine("Colonial casualties:"+p.getColonials(lines.ElementAt(200)));
            Console.WriteLine("Warden casualties:" + p.getWardens(lines.ElementAt(200)));
            //блок записи файла
            /* TextWriter tw = new StreamWriter(newpath);
             foreach (string s in lines)
             {
                 p.GetTime(s);
                 tw.WriteLine(s);
                 linecounter++;

             }
             tw.WriteLine("Всего строк в файле:" + linecounter);
             tw.Close();
             */
            //Вывод количества строк
            Console.WriteLine("Всего строк в файле:"+linecounter);
            Console.ReadLine();
        }

       
    }
}
