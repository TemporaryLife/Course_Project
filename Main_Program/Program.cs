
using System;
using System.Configuration;
using System.Collections.Specialized;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Configuration;

namespace Main_Program
{
    class Program
    {
        private static string directory;

        static void Main(string[] args)
        {
            string ans, sAttr;
            Console.SetWindowSize(200,50);

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection appsettings = (AppSettingsSection)config.GetSection("appSettings");
            
            directory = appsettings.Settings["Path"].Value;

            while (true)
            {
                Directory.SetCurrentDirectory(directory);
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write($"{directory} >> ");
                ans = Console.ReadLine();
                Console.ForegroundColor = ConsoleColor.Gray;
                string[] ans_arr = ans.Split(' ');


                if (ans == "ls")                                  // look files
                {
                    LookFiles();
                    LookDirectories();
                }

                else if (ans_arr[0] == "cf")                      // copy file
                {
                    CopyFileTo("CmDust-Result.log", "new2.log", @"C:\Users\Konstantin\Videos\new2.log");
                    ans = "";
                }
                else if (ans_arr[0] == "cd")                      // change dir
                {
                    try
                    {
                        if (ans_arr[1] == "~")
                        {
                            directory = Convert.ToString(Directory.GetParent(directory));
                        }
                        else
                        {
                            ChangeDir(ans_arr[1]);
                        }

                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Нет такой директории. Проверьте имя и повторите попытку");

                    }

                }
                else if (ans == "clr")                            // clear console
                {
                    Console.Clear();
                }

                else if (ans_arr[0] == "delfolder") // delete folder
                {
                    Directory.Delete($@"{Directory.GetCurrentDirectory()}\{ans_arr[1]}", true);
                }
                else if (ans_arr[0] == "delfile")
                {
                    File.Delete($@"{Directory.GetCurrentDirectory()}\{ans_arr[1]}");
                }
                appsettings.Settings["Path"].Value = directory;
                config.Save(ConfigurationSaveMode.Modified);
            }

            
        }

        static void LookFiles()
        {

            string[] all_files = Directory.GetFiles(directory);
            DirectoryInfo DirArr = new DirectoryInfo(directory);
            FileInfo[] FileArr = DirArr.GetFiles();

            for (int i = 0; i < all_files.Length; i++)
            {
                
                Console.WriteLine($"{all_files[i]} ");
                Console.SetCursorPosition(120, Console.CursorTop-1);
                Console.Write($"{File.GetLastWriteTime(all_files[i])}\n");
                Console.SetCursorPosition(150, Console.CursorTop-1);
                Console.WriteLine($"{FileArr[i].Length} байт \n");

            }
            
        }



        static void LookDirectories()
        {

            string[] all_directories = Directory.GetDirectories(directory);
            double size = 0;
            for (int i = 0; i < all_directories.Length; i++)
            {
                Console.WriteLine($"{all_directories[i]} ");
                Console.SetCursorPosition(120, Console.CursorTop-1);
                Console.Write($"{Directory.GetLastAccessTime(all_directories[i])}\n");
                Console.SetCursorPosition(150, Console.CursorTop-1);
                Console.WriteLine($"{GetDirSize(all_directories[i], ref size)} байт \n");
            }
        }

        static double GetDirSize(string folder, ref double size)

        {
            DirectoryInfo DirArr = new DirectoryInfo(folder);
            DirectoryInfo[] AllDirs = DirArr.GetDirectories();
            FileInfo[] FileArr = DirArr.GetFiles();
            foreach (FileInfo f in FileArr)
            {
                size = size + f.Length;
            }

            foreach (var d in AllDirs)
            {
                GetDirSize(d.FullName, ref size);
            }

            return size;

        }


        static void CopyFileTo(string old_name, string new_name, string path)
        {
            File.Copy(old_name, new_name);
            File.Move(new_name, path);
        }



        static void CopyDirTo(string old_name, string new_name, string path)    // доработать
        {
            string[] files = Directory.GetFiles(old_name);
            Directory.CreateDirectory(path);
            for (int i = 0; i < files.Length; i++)
            {
                Console.Write(Directory.GetCurrentDirectory());
                File.Copy(files[i], Path.Combine(path, files[i]+"123"), true);
                

            }

        }
        
        static void ChangeDir(string NextDir)                                           // работает
        {
            directory = $@"{Directory.GetCurrentDirectory()}\{NextDir}";
            Directory.SetCurrentDirectory(directory);
        }
    }
}
