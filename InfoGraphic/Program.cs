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
        public List<message> messagelist = new List<message>();
        public string map, servername;
        public int wloss=0, closs=0;
        public int victor = 0;
        public int length;
        public DateTime start;
        public DateTime end;
        public battle()
        {
          

        }
        //выводит карту
        public string getmap()
        {
            return messagelist.ElementAt(0).mapname;

        }
        //выводит имя сервера
        public string getserver()
        {
            return messagelist.ElementAt(0).servername;
        }
        //выводит победителя
        public int getvictor()
        {
            return messagelist.Last().victory;

        }
        //выводит длину карты
        public int getSpan()
        {
            return (messagelist.Last().date - messagelist.ElementAt(0).date).Hours + 1;
        }
        public DateTime getStart()
        {
            return messagelist.ElementAt(0).date.AddHours(-1);
        }
        public DateTime getEnd()
        {
            return messagelist.Last().date;
        }
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
        //форматирует сырой текст в то что надо
        public List<string> firstFormat(string path)
        {
            List<string> lines = new List<string>();
            lines = toLineList(path);
            lines = removeTrash(lines);
            lines = removeTownCaptures(lines);
            lines = formatCasualties(lines);
            return lines;
        }
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
        //Смотрит кто победил (если сообщение о победе), 1 - колонисты, 2 - вардены, 3 - ничья
        public int getVictory(string message)
        {
            if(message.Contains("surrendered"))
            {
                if (message.Contains("Colonials"))
                    return 2;
                if (message.Contains("Wardens"))
                    return 1;
            }
            if (message.Contains("stalemate"))
                return 3;
            if(message.Contains("defeated"))
            {
                if (message.Contains("**Colonials**"))
                    return 1;
                if (message.Contains("**Wardens**"))
                    return 2;
            }
            return 0;
        }
        //смотрит, новая ли карта или старая
       // public bool newBattle(message input, List<battle> battles)
        //{
        //}
        //Преобразовывает .тхт в список строк с датой в начале
        public List<string> toLineList(string path)
        {
            string line, date = "";
            List<string> lines = new List<string>();
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
                    lines.Add(line);
                }
                
            }
            return lines;
        }

        //Удаляет лишние строки из списка
        public List<string> removeTrash(List<string> inputlines)
        {
            string line = "";
            List<string> lines = new List<string>();
            foreach (string s in inputlines)
            {
                
                if(!(s.Contains("a town") || s.Contains("outpost has") || s.Contains("so far")
                    || s.Contains("Internal") || s.Contains("192.168")
                    || s.Contains("Lobby Islands")||s.Contains("weekly war has started") || s.Contains("Development"))){
                    line = s;
                    //line = Regex.Replace(line, "Day \d+, \d+:\d+ Hours:",);
                    lines.Add(s);
                }
            }
                return lines;
        }


        //Удаляет захват/потерю городов (40к+ строк!)
        public List<string> removeTownCaptures(List<string> inputlines)
        {
            List<string> lines = new List<string>();
            foreach (string s in inputlines)
            {
                if (!(s.Contains("have lost") || s.Contains("have taken")))
                {
                    lines.Add(s);
                }
            }
            return lines;
        }

        //Форматирование сообщений о потерях
        public List<string> formatCasualties(List<string> inputlines)
        {
            List<string> lines = new List<string>();
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
                    lines.Add(line);
                }
                else
                if (line.Contains("total enlistments"))
                {
                    line = line.Replace(" total enlistments,", ";");
                    line = line.Replace(" casualties,", ";").Replace(" casualties.", ";");
                    lines.Add(line);
                }
                else
                    lines.Add(line);
            }
            return lines;
        }

    }
    class Program
    {   
        static void Main(string[] args)
        {
            Parsing p = new Parsing();
            List<string> lines = new List<string>();
            List<message> messagelist = new List<message>();
            String path1 = "C:/Users/777/Desktop/Foxhole Management/InfoGraphic/logs/oldlogs.txt";
            String path2 = "C:/Users/777/Desktop/Foxhole Management/InfoGraphic/logs/newlogs.txt";
            List<battle> battles = new List<battle>();
            DateTime date;
            CultureInfo provider = CultureInfo.InvariantCulture;
            int linecounter = 0;
            /*
            //блок названия файла
            Console.WriteLine("Куда пишем?");
            newpath=newpath+Console.ReadLine()+".txt";
            Console.WriteLine(newpath);
            */     
            //Форматируем строки и убираем мусор
            lines = p.firstFormat(path1);
            //Объединяем потери в старом формате
            for(int i=0; i<lines.Count;i++)
            {
                message buffer = new message();
                string s = lines.ElementAt(i);

                buffer.date = p.GetTime(s);
                buffer.servername = p.getServer(s);
                buffer.mapname = p.getMap(s);
                buffer.ccas = p.getColonials(s);
                buffer.wcas = p.getWardens(s);
                buffer.victory = p.getVictory(s);
                if (buffer.victory == 0)
                {
                    if ((i + 30) < lines.Count)
                    {
                        for (int n = 1; n < 30; n++)
                        {
                            string compare = lines.ElementAt(i + n);
                            if (s.Contains("Colonial") && (compare.Contains("Warden")))
                            {
                                if ((buffer.servername == p.getServer(compare)) && (buffer.mapname == p.getMap(compare)))
                                {
                                    buffer.wcas = p.getWardens(compare);
                                    lines.RemoveAt(i + n);
                                    break;
                                }
                            }
                            else
                            if (compare.Contains("Colonial") && (s.Contains("Warden")))
                            {
                                if ((buffer.servername == p.getServer(compare)) && (buffer.mapname == p.getMap(compare)))
                                {
                                    buffer.ccas = p.getColonials(compare);
                                    lines.RemoveAt(i + n);
                                    break;
                                }
                            }

                        }
                    }
                    else
                    {
                        for (int n = i; n < lines.Count; n++)
                        {
                            string compare = lines.ElementAt(n);
                            if (s.Contains("Colonial") && (compare.Contains("Warden")))
                            {
                                if ((buffer.servername == p.getServer(compare)) && (buffer.mapname == p.getMap(compare)))
                                {
                                    buffer.wcas = p.getWardens(compare);
                                    lines.RemoveAt(n);
                                    break;
                                }
                            }
                            else
                            if (compare.Contains("Colonial") && (s.Contains("Warden")))
                            {
                                if ((buffer.servername == p.getServer(compare)) && (buffer.mapname == p.getMap(compare)))
                                {
                                    buffer.ccas = p.getColonials(compare);
                                    lines.RemoveAt(n);
                                    break;
                                }
                            }

                        }
                    }
                }
                messagelist.Add(buffer);
            }

            lines = p.firstFormat(path2);
            //Вводим сообщения из нового формата
            for (int i = 0; i < lines.Count; i++)
            {
                message buffer = new message();
                string s = lines.ElementAt(i);
                buffer.date = p.GetTime(s);
                buffer.servername = p.getServer(s);
                buffer.mapname = p.getMap(s);
                buffer.ccas = p.getColonials(s);
                buffer.wcas = p.getWardens(s);
                buffer.victory = p.getVictory(s);
                messagelist.Add(buffer);
            }

            //делит все сообщения по играм
            for(int i=0;i<messagelist.Count;i++)
            {
                message buffer = messagelist.ElementAt(i); //загоняем в буфер сообщение из листа
                bool newbattle = true;
                if ((battles.Count < 100)&&(battles.Count>0)) //для первых битв можно и прогнать через все битвы
                {
                for(int k=battles.Count-1;k>=0;k--)
                    {
                        battle compare = battles.ElementAt(k);
                        //если совпадает имя, карта, и потерь меньше или это победное сообщение, добавляем сообщение в конец списка
                        if ((buffer.servername == compare.servername) && (buffer.mapname == compare.map)
                            && (compare.messagelist.Last().victory==0) && (((compare.messagelist.Last().wcas <= buffer.wcas)
                            && (compare.messagelist.Last().ccas <= buffer.ccas))||(buffer.victory!=0)))
                        {
                            battles.ElementAt(k).messagelist.Add(buffer);
                            
                            newbattle = false;
                            if (buffer.victory != 0)
                            {
                                battles.ElementAt(k).victor = buffer.victory;
                                battles.ElementAt(k).length = battles.ElementAt(k).getSpan();
                                battles.ElementAt(k).end = battles.ElementAt(k).getEnd();
                            }
                            else
                            {
                                battles.ElementAt(k).wloss = buffer.wcas;
                                battles.ElementAt(k).closs = buffer.ccas;
                            }
                            break;
                        }
                    }
                }
                if (battles.Count > 100) //если битв больше чем 60 то прогоняем только через последние 60 битв
                {
                    for (int k = battles.Count - 1; k >= battles.Count - 101; k--)
                    {
                        battle compare = battles.ElementAt(k);
                        //если совпадает имя, карта, и потерь меньше или это победное сообщение, добавляем сообщение в конец списка
                        if ((buffer.servername == compare.servername) && (buffer.mapname == compare.map)
                            && (compare.messagelist.Last().victory == 0) && (((compare.messagelist.Last().wcas <= buffer.wcas)
                            && (compare.messagelist.Last().ccas <= buffer.ccas)) || (buffer.victory != 0)))
                        {
                           
                            battles.ElementAt(k).messagelist.Add(buffer);
                            newbattle = false;
                            if(buffer.victory!=0)   //если в сообщении говорится что кто-то победил,                                                     
                            {                       //присваиваем эту победу объекту битвы
                                battles.ElementAt(k).victor = buffer.victory;
                                battles.ElementAt(k).length = battles.ElementAt(k).getSpan();
                                battles.ElementAt(k).end = battles.ElementAt(k).getEnd();
                            }
                            else
                            {
                                battles.ElementAt(k).wloss = buffer.wcas;
                                battles.ElementAt(k).closs = buffer.ccas;
                            }
                            break;
                        }
                    }
                }
                if ((newbattle == true)&&(buffer.victory==0))
                {
                    //если не нашли битву по карте и серверу
                    battle bufferbattle = new battle();
                    bufferbattle.map = buffer.mapname;
                    bufferbattle.servername = buffer.servername;
                    bufferbattle.messagelist.Add(buffer);
                    bufferbattle.start = bufferbattle.getStart();
                    battles.Add(bufferbattle);
                }
            }
            for (int i = 0; i < battles.Count; i++)
            {
                if((battles.ElementAt(i).messagelist.Count==1)&&(battles.ElementAt(i).messagelist.ElementAt(0).victory==0)) //если об игре всего одно сообщение, нахуй оно нам надо?
                { 
                    battles.RemoveAt(i);
                    i--;
                    continue;
                }else
                //на некоторых серверах корреспондент забаговывался и выводил потери уже после победы
                if((battles.ElementAt(i).messagelist.First().ccas== battles.ElementAt(i).messagelist.Last().ccas)
                    && battles.ElementAt(i).victor==0)
                    {
                    battles.RemoveAt(i);
                    i--;
                    continue;
                }
                if (battles.ElementAt(i).victor == 0)
                {
                    
                    linecounter++;
                    Console.WriteLine(i+" "+battles.ElementAt(i).servername+"; "+ battles.ElementAt(i).map);
                }

            }
                /*
                Console.WriteLine("Time:"+p.GetTime(lines.ElementAt(200)));
                Console.WriteLine("Server:"+p.getServer(lines.ElementAt(200)));
                Console.WriteLine("Map:"+p.getMap(lines.ElementAt(200)));
                Console.WriteLine("Colonial casualties:"+p.getColonials(lines.ElementAt(200)));
                Console.WriteLine("Warden casualties:" + p.getWardens(lines.ElementAt(200)));
                */
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
                Console.WriteLine("Проблемные игры:"+linecounter);
            Console.ReadLine();
        }

       
    }
}
