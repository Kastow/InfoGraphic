using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;

namespace InfoGraphic
{
    class Program
    {
        static void Main(string[] args)
        {
            String line, previousline;
            String path = "C:/Users/777/Desktop/Foxhole Management/InfoGraphic/source.txt";
            String newpath = "C:/Users/777/Desktop/Foxhole Management/InfoGraphic/";
            String dateformat = "[dd.MM.yy;HH:mm]";
            int linecounter = 0;
            LinkedList<List<string>> lines = new LinkedList< List <string>> ();
            //int l = File.ReadLines(@path).Count();
            //Console.WriteLine(l);

            //блок названия файла
            Console.WriteLine("Куда пишем?");
            newpath=newpath+Console.ReadLine()+".txt";
            Console.WriteLine(newpath);

            //блок обработки файла
            //Создаем двумерный список, где одна строка - одна временная метка, столбики - сервера
            StreamReader sr = new StreamReader(path);
            List<string> messages = new List<string>();
            while ((line = sr.ReadLine()) != null)
            {
                //Убираем сообщения о взятии городов
                if (!(line.Contains("lost")||line.Contains("taken")))
                {
                    //Форматируем время
                    if (line.Contains("War Correspondent#4734"))
                    {
                        line = line.Substring(line.IndexOf("["));
                        line = line.Substring(0, line.IndexOf("]") + 1);
                        /*line = line.Replace('-', '.');
                        line = line.Replace("янв", "01").Replace("фев", "02").Replace("мар", "03");
                        line = line.Replace("апр", "04").Replace("май", "05").Replace("июн", "06");
                        line = line.Replace("июл", "07").Replace("авг", "08").Replace("сен", "09");
                        line = line.Replace("окт", "10").Replace("ноя", "11").Replace("дек", "12");*/

                        //Если временная метка не пустая то заносим инфу по ней в основной список
                        if(messages.Count>0)
                        {
                            lines.AddLast(messages);
                        }
                        messages.Clear();
                        messages.Add(line); //забиваем время в качестве первого элемента списка
                    }

                    //Общее количество потерь
                    if (line.Contains("casualties")&&(!line.Contains("Lobby")))
                    {
                        messages.Add(line);//забиваем потери как элемент списка
                    }


                    //  lines.Add(line.Split(']')[0]);
                    linecounter++;
                    previousline = line;
                }
                
            }

            //блок записи файла
            TextWriter tw = new StreamWriter(newpath);
            foreach (List<string> s in lines)
                foreach(string n in s)
                 tw.WriteLine(n);
            tw.Close();

            //Вывод количества строк
            Console.WriteLine("Всего строк в файле:"+linecounter);
            Console.ReadLine();
        }
    }
}
