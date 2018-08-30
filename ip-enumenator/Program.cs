﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace ip_enumenator
{
    class Program
    {
        static string pattern_ip = @"\d*\u002E\d*\u002E\d*\u002E\d*";
        static string pattern_error = @"\s\d{3}\s";

        static List<string> addresses = new List<string>();
        static List<string> workedoutIps = new List<string>();
        static List<string> workedoutErrors = new List<string>();
        static List<KeyValuePair<string, int>> sortList = new List<KeyValuePair<string, int>>();
        static List<string> bans = new List<string>();
        static List<string> good = new List<string>();
        static List<string> questionable = new List<string>();

        // Settings
        static string namefile;
        static string bansfile = "bans.txt";
        static string goodfile = "good.txt";
        static string questionablefile = "questionable.txt";
        static bool isToFile = false;
        static bool isInverse = false;
        static int border;

        static void Main(string[] args)
        {
            File.Create("output.txt");

            Console.WriteLine("File name: ");
            namefile = Console.ReadLine() + ".log";
            Console.WriteLine("-----------------------------------------");

            Console.WriteLine("Turn ON record in the file (y/n)?");
            if (Console.ReadLine() == "y")
                isToFile = true;
            Console.WriteLine("-----------------------------------------");

            Console.WriteLine("Inverse sorting (do big to small)?");
            if (Console.ReadLine() == "y")
                isInverse = true;
            Console.WriteLine("-----------------------------------------");

            Console.WriteLine("Enter the critical boundary of occurrences:");
            border = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine("-----------------------------------------");

            FillArrayAddresses(namefile);
            FillArrayBans();
            FillArrayGood();
            FillArrayQuestionable();

            Formating();

            Output();

            Console.ReadKey();
        }

        static private void FillArrayBans()
        {
            if (File.Exists(bansfile))
            {
                using (StreamReader sr = new StreamReader(bansfile))
                {
                    string sLine = "";

                    while (sLine != null)
                    {
                        sLine = sr.ReadLine();

                        if (sLine != null)
                            bans.Add(sLine);
                    }
                }
            }
            else
            {
                Console.WriteLine("File not found");
            }
        }

        static private void FillArrayGood()
        {
            if (File.Exists(goodfile))
            {
                using (StreamReader sr = new StreamReader(goodfile))
                {
                    string sLine = "";

                    while (sLine != null)
                    {
                        sLine = sr.ReadLine();

                        if (sLine != null)
                            good.Add(sLine);
                    }
                }
            }
            else
            {
                Console.WriteLine("File not found");
            }
        }

        static private void FillArrayQuestionable()
        {
            if (File.Exists(questionablefile))
            {
                using (StreamReader sr = new StreamReader(questionablefile))
                {
                    string sLine = "";

                    while (sLine != null)
                    {
                        sLine = sr.ReadLine();

                        if (sLine != null)
                            questionable.Add(sLine);
                    }
                }
            }
            else
            {
                Console.WriteLine("File not found");
            }
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

                if(border <= countIp)
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
            sortList.Sort(delegate (KeyValuePair<string, int> x, KeyValuePair<string, int> y) 
                {
                    if (isInverse)
                        return y.Value.CompareTo(x.Value);
                    else
                        return x.Value.CompareTo(y.Value);
                });

            using (StreamWriter sw = new StreamWriter("output.txt", true))
            {
                foreach (KeyValuePair<string, int> kvp in sortList)
                {
                    bool isInListBan = false;
                    bool isInListGood = false;
                    bool isInListQuestion = false;

                    List<string> workedoutErrors = new List<string>();
                    string error_str = " ";
                    foreach (string error in GetErrorsList(kvp.Key))
                    {
                        error_str += error;
                    }

                    Console.WriteLine();
                    foreach (string banIp in bans)
                    {
                        if(kvp.Key == banIp)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("BAN\t" + kvp.Key + "\t\t Occurrences: " + kvp.Value + "\t\t Errors type: " + error_str);
                            Console.ForegroundColor = ConsoleColor.White;
                            isInListBan = true;
                            break;
                        }
                        else
                        {
                            isInListBan = false;
                        }
                    }
                    foreach (string goodIp in good)
                    {
                        if (kvp.Key == goodIp)
                        {
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("GOOD\t" + kvp.Key + "\t\t Occurrences: " + kvp.Value + "\t\t Errors type: " + error_str);
                            Console.ForegroundColor = ConsoleColor.White;
                            isInListGood = true;
                            break;
                        }
                        else
                        {
                            isInListGood = false;
                        }
                    }

                    foreach (string questionIp in questionable)
                    {
                        if (kvp.Key == questionIp)
                        {
                            Console.ForegroundColor = ConsoleColor.DarkYellow;
                            Console.WriteLine("???\t" + kvp.Key + "\t\t Occurrences: " + kvp.Value + "\t\t Errors type: " + error_str);
                            Console.ForegroundColor = ConsoleColor.White;
                            isInListQuestion = true;
                            break;
                        }
                        else
                        {
                            isInListQuestion = false;
                        }
                    }

                    if (!isInListGood && !isInListBan && !isInListQuestion)
                        Console.WriteLine(kvp.Key + "\t\t Occurrences: " + kvp.Value + "\t\t Errors type: " + error_str);

                    Console.WriteLine("_____________________________________________________________________________________________________");

                    if (isToFile)
                        sw.WriteLine(kvp.Key + "\t\t Occurrences: " + kvp.Value + "\t\t Errors type: " + error_str);
                }

                Console.WriteLine();
                Console.WriteLine("|/////////////////////////////////////////////|");
                Console.WriteLine("Unique occurrences:\t" + workedoutIps.Count);
                Console.WriteLine("|/////////////////////////////////////////////|");

                if (isToFile)
                {
                    sw.WriteLine();
                    sw.WriteLine("|/////////////////////////////////////////////|");
                    sw.WriteLine("Unique occurrences:\t" + workedoutIps.Count);
                    sw.WriteLine("|/////////////////////////////////////////////|");
                }
            }
        }
    }
}
