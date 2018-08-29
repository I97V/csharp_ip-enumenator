using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace ip_enumenator
{
    class Program
    {
        static string namefile;

        static string pattern_ip = @"\d*\u002E\d*\u002E\d*\u002E\d*";
        static string pattern_error = @"\s\d{3}\s";

        static List<string> addresses = new List<string>();
        static List<string> workedoutIps = new List<string>();
        static List<string> workedoutErrors = new List<string>();
        static List<KeyValuePair<string, int>> sortList = new List<KeyValuePair<string, int>>();

        static void Main(string[] args)
        {
            Console.WriteLine("Enter name of file (name.log): ");
            namefile = Console.ReadLine() + ".log";

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
                            Match mtch = Regex.Match(sLine, pattern_ip);
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

        static bool IsWasIp(string _str)
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

                if (IsWasIp(addresses[i]))
                    continue;

                for (int j = i; j < addresses.Count; j++)
                    if (addresses[j] == addresses[i])
                        countIp++;

                workedoutIps.Add(addresses[i]);

                sortList.Add(new KeyValuePair<string, int>(addresses[i], countIp));
            }
        }

        static bool IsWasError(List<string> _list, string _error)
        {
            foreach (string s in _list)
            {
                if (_error == s)
                    return true;
            }

            return false;
        }

        static List<string> GetErrorsList(string ip)
        {
            List<string> errors = new List<string>();

            if (File.Exists(namefile))
            {
                using (StreamReader sr = new StreamReader(namefile))
                {
                    string sLine = "";

                    while (sLine != null)
                    {
                        sLine = sr.ReadLine();

                        if (sLine != null)
                        {
                            Match mtch1 = Regex.Match(sLine, ip);
                            if (mtch1.Success)
                            {
                                Match mtch2 = Regex.Match(sLine, pattern_error);
                                if (mtch2.Success)
                                {
                                    if (IsWasError(errors, mtch2.Value))
                                        continue;

                                    errors.Add(mtch2.Value);
                                    workedoutErrors.Add(mtch2.Value);
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine("File not found");
            }

            return errors;
        }

        static private void Output()
        {
            sortList.Sort(delegate (KeyValuePair<string, int> x, KeyValuePair<string, int> y) { return x.Value.CompareTo(y.Value); });

            foreach (KeyValuePair<string, int> kvp in sortList)
            {
                List<string> workedoutErrors = new List<string>();
                string error_str = " ";
                foreach(string error in GetErrorsList(kvp.Key))/*//*/
                {
                    error_str += error;
                }

                Console.WriteLine();
                Console.WriteLine(kvp.Key + "\t\t Occurrences: " + kvp.Value + "\t\t Errors type: "+ error_str);
                Console.WriteLine("_____________________________________________________________________________________________________");
            }
        }
    }
}
