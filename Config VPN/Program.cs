using System;
using System.IO;
using System.Collections.Generic;

namespace Config_VPN
{
    class ParsConfVPN
    {
        //private string[][] conf = new string[0][];
        private string[] PathFileConfig()
        {
            string[] names = Directory.GetFiles(@"D:\MaILW\Documents\Visual Studio 2019\Projects\Config VPN\Config");
            return names;
        }
        private void PrintHelp(string[][] conf)
        {
            Console.WriteLine("\n");
            for (int i = 0; i < conf.Length; i++)
            {
               
                   Console.WriteLine(conf[i][0]);
                
              
            }
            Console.WriteLine("\n");
        }
        private void PrintFullConf(string[][] conf)
        {
            Console.WriteLine("\n");
            for (int i = 0; i < conf.Length; i++)
            {
                for (int j = 0; j < conf[i].Length; j++)
                {


                    Console.Write(conf[i][j]+" ");
                }
                Console.WriteLine();

            }
            Console.WriteLine("\n");
        }

        private string[][] ReadFile()
        {
            string[][] conf = new string[0][];
            foreach (string a in PathFileConfig())
            {
                
                StreamReader reader = new StreamReader(a);
                string newLine;
                int k = 0;
                while (reader.EndOfStream == false)
                {
                    if ((newLine = reader.ReadLine()) != "")
                    {
                        if (newLine.StartsWith("#") || newLine.StartsWith(";")) continue;
                        Array.Resize(ref conf, conf.Length + 1);
                        if (newLine.IndexOf("#") != -1)
                        {
                            newLine = newLine.Remove(newLine.IndexOf("#"));
                        }
                        if (newLine.IndexOf(";") != -1)
                        {
                            newLine = newLine.Remove(newLine.IndexOf(";"));
                        }
                        conf[k] = newLine.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        k++;
                    }
                }
                reader.Dispose();
                reader.Close();
                
            }

            return conf;

        }
        
        //public void FindParamValue()
        //{
        //    ReadFile();
        //    do
        //    {
        //        string target = EnterParam();
        //        foreach (string[] param in conf)
        //        {
        //            if (target == param[0])
        //            {
        //                Console.Write("Параметр: " + param[0] + "\tзначение: ");
        //                if (param.Length == 1) Console.WriteLine(" отсутствует.");
        //                for (int i = 1; i < param.Length; i++) Console.Write( param[i] + " ");
        //            }
        //        }
        //        Console.Write("\nДля выхода нажмите q: ");
        //    } while(Console.ReadLine() != "q");
            
        //}
        public void FindParamValue()
        {
            string[][] conf;
            string parametr;
            bool notFoundParam;
            do
            {
                conf = ReadFile();
                do
                {
                    notFoundParam = true;
                    Console.Write("\nВведите интересущей параметр: ");
                    parametr = Console.ReadLine();

                    foreach (string[] param in conf)
                    {
                        if (parametr == param[0])
                        {
                            Console.Write("Параметр: " + param[0] + "\tзначение: ");
                            if (param.Length == 1) Console.WriteLine(" отсутствует.");
                            for (int i = 1; i < param.Length; i++)
                                Console.Write(param[i] + " ");
                            notFoundParam = false;
                            break;
                        }
                    }
                    if (notFoundParam == true)
                    {
                        Console.WriteLine("Параметр отсутсвует. Проверьте правильность ввода.\nЧтобы просмотреть список доступных устройств введите help или fullhelp.\t");

                       
                        switch (Console.ReadLine())
                        {
                            case "help":
                                PrintHelp(conf);
                                break;
                            case "fullhelp":
                                PrintFullConf(conf);
                                break;
                            default:
                                break;
                        }

                    }
                } while (notFoundParam == true);
                Console.WriteLine();
                Console.WriteLine("Если хотите изменить значение параметра, нажмите s.");
                if(Console.ReadLine() == "s")
                {
                    SetParamValue(parametr);
                }
                Console.WriteLine("\nДля выхода нажмите ESC.");
            } while (Console.ReadKey().Key != ConsoleKey.Escape);
            
        }

        private void SetParamValue(string parametr)
        {
            //var reader = new StreamReader(PathFileConfig()[0]);
            string[] allConfig = File.ReadAllLines(PathFileConfig()[0]);
            int p = 0;
            string[] workLines = new string[0];
            string newValue;
            string localComment = null; ;
            for (int i = 0; i < allConfig.Length; i++)
            {
                if (allConfig[i].StartsWith("#") || allConfig[i].StartsWith(";")) continue;
                if (allConfig[i].IndexOf(parametr) == -1) continue;

                if (allConfig[i].IndexOf("#") != -1)
                {
                    localComment = allConfig[i].Substring(allConfig[i].IndexOf("#"));
                    allConfig[i] = allConfig[i].Remove(allConfig[i].IndexOf("#"));
                }
                if (allConfig[i].IndexOf(";") != -1)
                {
                    localComment = allConfig[i].Substring(allConfig[i].IndexOf(";"));
                    allConfig[i] = allConfig[i].Remove(allConfig[i].IndexOf(";"));
                }
                workLines = allConfig[i].Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                p = i;
                break;
            }
            Console.WriteLine("Изменяемый параметр: " + parametr);
            Console.Write("Старое значение: ");
            for (int i = 1; i < workLines.Length; i++)
                Console.Write(workLines[i] + " ");
                Console.Write("\nВведите новое значение: ");


            if (workLines.Length == 1)
            {
                do
                {
                    newValue = Console.ReadLine();
                    if (newValue == "")
                    {
                        Console.WriteLine("Поле не может быть пустым. Повторите ввод.");
                        continue;

                        //allConfig[p] = workLines[0];
                        //File.WriteAllLines(PathFileConfig()[0], allConfig);
                    }
                    if (newValue.IndexOf(" ") == 0)
                    {
                        bool emptyNewValue = true;
                        for (int i = 0; i < newValue.Length; i++)
                        {
                            if (newValue[i] != ' ')
                            {
                                emptyNewValue = false;
                                break;
                            }
                        }
                        if (emptyNewValue == true)
                        {
                            Console.WriteLine("Поле не может быть пустым. Повторите ввод.");
                            continue;
                        }
                    }
                    newValue = newValue.Trim();
                    allConfig[p] = newValue + " " + localComment;
                    allConfig[p] = allConfig[p].Trim();
                    File.WriteAllLines(PathFileConfig()[0], allConfig);
                    break;
                } while (true);
               

               
            }
            else
            {
                do
                {
                    newValue = Console.ReadLine();
                    if (newValue == "")
                    {
                        Console.WriteLine("Поле не может быть пустым. Повторите ввод.");
                        continue;

                        //allConfig[p] = workLines[0];
                        //File.WriteAllLines(PathFileConfig()[0], allConfig);
                    }
                    if (newValue.IndexOf(" ") == 0)
                    {
                        bool emptyNewValue = true;
                        for (int i = 0; i < newValue.Length; i++)
                        {
                            if (newValue[i] != ' ')
                            {
                                emptyNewValue = false;
                                break;
                            }
                        }
                        if (emptyNewValue == true)
                        {
                            Console.WriteLine("Поле не может быть пустым. Повторите ввод.");
                            continue;
                        }
                    }
                    newValue = newValue.Trim();
                    allConfig[p] = workLines[0] + " " + newValue + " " + localComment;
                    allConfig[p] = allConfig[p].Trim();
                    File.WriteAllLines(PathFileConfig()[0], allConfig);
                    break;
                } while (true);
            }

        }
    }
    class Program
    {

        static void Main(string[] args)
        {
           
           
            ParsConfVPN a = new ParsConfVPN();
            a.FindParamValue();
           
        }
    }
}
