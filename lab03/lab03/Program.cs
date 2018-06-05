using System;
using System.Collections.Generic;
using System.Threading;
using System.IO;

namespace lab03
{
    static class Const
    {
        public const double MaxDist = 3.0;
        public static int n = 10;
        public const int timeConcert = 2000;
        public const int timeIntervalMessage = 100;
    }

    class Program
    {

        private static StreamReader  OpenFile()
        {
            StreamReader file = null;
            try
            {
                file = new StreamReader("positions.txt");
            }
            catch (Exception e)
            {
                if (e is DirectoryNotFoundException)
                    Console.WriteLine("File not found !");

                return file;
            }
            return file;
        }
        private static List<Location> GetListLocationFromFile()
        {

            List<Location> result = new List<Location>();
            StreamReader file = OpenFile();

            string buffor = file.ReadLine();
            int n = int.Parse(buffor);

            Const.n = n;

            for(int i=0;i<n;i++)
            {
                buffor = file.ReadLine();
                string[] coordinates = buffor.Split(null as string[], StringSplitOptions.RemoveEmptyEntries);
                Location location = new Location(int.Parse(coordinates[0]), int.Parse(coordinates[1]));

                result.Add(location);
            }

            return result;
        }

        static void Main(string[] args)
        {

            List<Location> listLocation = GetListLocationFromFile();

            List<Jankiel> listJankiel = new List<Jankiel>();

            for(int i=0;i<Const.n;i++)
            {
                listJankiel.Add(new Jankiel(i,listLocation));
            }

            for(int i=0;i<listJankiel.Count;i++)
            {
                listJankiel[i].Init();
            }

            Thread.Sleep(100 * 1000);

        }
    }
}
