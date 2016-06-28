using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace lab1
{
    static class Rozszerzenie {
       static  DateTime najstarszy = DateTime.MaxValue;
      
       public static DateTime znajdzNajstarszego(this DirectoryInfo sciezka)
       {
            foreach (DirectoryInfo folder in sciezka.GetDirectories())
            {
                if (folder.CreationTime < najstarszy)
                {
                    najstarszy = folder.CreationTime;
                }
                najstarszy = znajdzNajstarszego(folder);
            }
            foreach (FileInfo plik in sciezka.GetFiles())
            {
                if (plik.CreationTime < najstarszy)
                {
                    najstarszy = plik.CreationTime;
                }
            }


            return najstarszy;
    
    
        }

       public static string atrybuty(this FileSystemInfo plik) {
           FileAttributes a = plik.Attributes;
           string wynik;
           if ((a & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
               wynik = " r";
           else
               wynik = " -";
           if( (a & FileAttributes.Archive) == FileAttributes.Archive)
               wynik += "a";
           else
               wynik += "-";
           if ((a & FileAttributes.Hidden) == FileAttributes.Hidden)
               wynik += "h";
           else
               wynik += "-";
           if ((a & FileAttributes.System) == FileAttributes.System)
               wynik += "s";
           else
               wynik += "-";
           return wynik;
       
       }
    
    }
        [Serializable]
      public class Komparator : IComparer<string> 
      {

        public int Compare(string a, string b) {
            if (a.Length > b.Length)
                return 2;
            if (a.Length < b.Length)
                return -2;
            return String.Compare(a, b);
        
        
        }
    
    }

    class Program
    {
        static SortedList<string, long> lista = new SortedList<string, long>(new Komparator());

        private static void buduj(DirectoryInfo sciezka, string margines)
        {
            foreach (DirectoryInfo folder in sciezka.GetDirectories()) {
                Console.WriteLine("{0} {1} ({2}) {3}", margines, folder, folder.GetFiles().Length + folder.GetDirectories().Length, folder.atrybuty());
                buduj(folder, "\t" + margines);
            }
            foreach (FileInfo plik in sciezka.GetFiles())
            {
                Console.WriteLine(margines + plik+" "+ plik.Length+ plik.atrybuty());
                lista.Add(plik.Name, plik.Length);

            }
        }



        static void Main(string[] args)
        {
            DirectoryInfo sciezka = new DirectoryInfo(args[0]);
            buduj(sciezka, "");
            DateTime stary = sciezka.znajdzNajstarszego();
            Console.WriteLine("\nNajstarszy plik: {0}\n",stary);
           
            lista.Clear();
            foreach (DirectoryInfo folder in sciezka.GetDirectories())
            {
                lista.Add(folder.Name, folder.GetFiles().Length + folder.GetDirectories().Length);
            }
            foreach (FileInfo plik in sciezka.GetFiles())
            {
                lista.Add(plik.Name, plik.Length);
            }

          
            FileStream fs = new FileStream("C:\\Users\\Ada\\Music\\temp.dat", FileMode.Create);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, lista);
            fs.Close();

         
            lista.Clear();
            fs = new FileStream("C:\\Users\\Ada\\Music\\temp.dat", FileMode.Open);
            lista = (SortedList<string, long>)bf.Deserialize(fs);
            fs.Close(); 

            foreach(KeyValuePair<string, long> wartosc in lista)
                    Console.WriteLine("{0} -> {1}", wartosc.Key, wartosc.Value);
            System.Console.WriteLine("Hello, World!");
        }
        
    }
}
