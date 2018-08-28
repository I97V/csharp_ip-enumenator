using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ip_enumenator
{
    class Program
    {
        static List<string> addresses = new List<string>();
        static List<string> workedoutIps = new List<string>();
        static List<KeyValuePair<string, int>> myList = new List<KeyValuePair<string, int>>();

        static string pattern = @"\d*\u002E\d*\u002E\d*\u002E\d*";

        static void Main(string[] args)
        {
            Console.WriteLine("Enter name of file (name.log): ");
            string namefile = Console.ReadLine() + ".log";
            FillArrayAddresses(namefile);
            Console.WriteLine("-----------------------------------------");
            Formating();
            Output();

            Console.ReadKey();
        }

        static private void FillArrayAddresses(string _namefile)
        {
            if (File.Exists(_namefile))
            {
                using (StreamReader sr = new StreamReader(_namefile))
                {
                    string sLine = "";

                    while (sLine != null)
                    {
                        sLine = sr.ReadLine();

                        if (sLine != null)
                        {
                            Match mtch = Regex.Match(sLine, pattern);
                            if (mtch.Success)
                            {
                                addresses.Add(mtch.Value);
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("File not found");
            }
        }

        static bool IsWas(string _str)
        {
            foreach (string s in workedoutIps)
            {
                if (_str == s)
                    return true;
            }

            return false;
        }

        static private void Formating()
        {
            for (int i = 0; i < addresses.Count; i++)
            {
                int countIp = 0;

                if (IsWas(addresses[i]))
                    continue;

                for (int j = i; j < addresses.Count; j++)
                {
                    if (addresses[j] == addresses[i])
                        countIp++;
                }

                workedoutIps.Add(addresses[i]);

                myList.Add(new KeyValuePair<string, int>(addresses[i], countIp));
            }
        }

        static private void Output()
        {
            myList.Sort(delegate (KeyValuePair<string, int> x, KeyValuePair<string, int> y) { return x.Value.CompareTo(y.Value); });

            foreach (KeyValuePair<string, int> kvp in myList)
            {
                Console.WriteLine();
                Console.WriteLine(kvp.Key + "\t Occurrences: " + kvp.Value);
                Console.WriteLine("_________________________________________");
            }
        }
    }
}
