using System;
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
                if(!(s.Contains("a town") ||s.Contains("so far")||s.Contains("Lobby Islands")||s.Contains("weekly war has started"))){
                    lines.AddLast(s);
                }
            }
                return lines;
        }

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


        public int hueta()
        {
            return 1;
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
            int linecounter = 0;
            
            //int l = File.ReadLines(@path).Count();
            //Console.WriteLine(l);

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
             */
                
            
            LinkedList<string> lines = new LinkedList<string>();
            //lines.CopyTo(Firstformat(path));

            lines = p.toLineList(path);
            lines = p.removeTrash(lines);
            lines = p.removeTownCaptures(lines);
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
