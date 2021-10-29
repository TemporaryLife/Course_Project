
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Hosting;

namespace Main_Program
{
    class Program
    {
        private static string directory;   //адрес текущей директории
        private static int positions;      //Количество файлов на одной странице
        private static int pages=1;        //текущая страница
        private static string path_new = "", Initial_Dir="";


        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            // Get the subdirectories for the specified directory.
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);

            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            DirectoryInfo[] dirs = dir.GetDirectories();
        
            // If the destination directory doesn't exist, create it.       
            Directory.CreateDirectory(destDirName);        

            // Get the files in the directory and copy them to the new location.
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string tempPath = Path.Combine(destDirName, file.Name);
                file.CopyTo(tempPath, false);
            }

            // If copying subdirectories, copy them and their contents to new location.
            if (copySubDirs)
            {
                foreach (DirectoryInfo subdir in dirs)
                {
                    string tempPath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, tempPath, copySubDirs);
                }
            }
        }



        static void Main(string[] args)
        {
            
            string ans;
            Console.SetWindowSize(200, 50);

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            AppSettingsSection appsettings = (AppSettingsSection)config.GetSection("appSettings");              //Изменение секции "AppSettings" конфигурационного файла

            directory = appsettings.Settings["Path"].Value;                         //Загрузка последней  директории
            positions = int.Parse(appsettings.Settings["Positions"].Value);         // Загрузка количества элементов на одной стр


            while (true)
            {
                Directory.SetCurrentDirectory(directory);                           //Переход терминала к текущей папке
                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write($"{directory} >> ");
                ans = Console.ReadLine();                                           //Ввод команды
                Console.ForegroundColor = ConsoleColor.Gray;
                string[] ans_arr = ans.Split(' ');                                  //Разбиение на аргументы


                if (ans == "ls")                                  // look files
                {
                    while (true)
                    {

                        Console.Clear();
                        LookFiles();                                                //Отображение файлов в консоли
                        ConsoleKeyInfo ans2 = Console.ReadKey();                    //Ключ для переключения страниц ls

                        if (ans2.Key == ConsoleKey.RightArrow)
                        {
                            pages++;
                        }

                        else if (ans2.Key == ConsoleKey.LeftArrow)
                        {
                            pages--;
                        }

                        else                                                        // выход из ls
                        {
                            Console.Clear();
                            pages = 1;
                            break;

                        }

                    }
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
                        if (ans_arr[1] == "~")                                                      //возврат на уровень выше
                        {
                            directory = Convert.ToString(Directory.GetParent(directory));
                        }
                        else
                        {
                            ChangeDir(ans_arr[1]);                                                  //смена директории
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

                else if (ans_arr[0] == "copydir")
                {
                    Console.WriteLine(Directory.GetCurrentDirectory());
                    Console.WriteLine(Path.Combine(Directory.GetCurrentDirectory(), ans_arr[1]));
                    Console.WriteLine(Path.Combine(Directory.GetCurrentDirectory(), ans_arr[2]));
                    DirectoryCopy(Path.Combine(Directory.GetCurrentDirectory(), ans_arr[1]), Path.Combine(Directory.GetCurrentDirectory(), ans_arr[2]), true);

                }


                else if (ans_arr[0] == "delfolder") // delete folder <имя папки>
                {
                    Directory.Delete($@"{Directory.GetCurrentDirectory()}\{ans_arr[1]}", true);
                }


                else if (ans_arr[0] == "delfile")   // delfile <имя файла>
                {
                    File.Delete($@"{Directory.GetCurrentDirectory()}\{ans_arr[1]}");
                }


                appsettings.Settings["Path"].Value = directory;             //обновление и сохранение текущей директории
                config.Save(ConfigurationSaveMode.Modified);
            }


        }

         static void LookFiles()
         {

             List<string> all_files = Directory.GetFiles(directory).ToList();                    /*сбор и объединение в один список*/
             List<string> all_directories = Directory.GetDirectories(directory).ToList();         /*файлов и директорий для прохождения циклом*/                                
            List<string> all_objects = all_files.Union(all_directories).ToList();
            DirectoryInfo DirArr = new DirectoryInfo(directory);
            FileInfo[] FileArr = DirArr.GetFiles();
            
            double size = 0;                                                                //переменная для хранения размера папок
            

            if (pages < 1)                                                                  //Ограничение по выходу за пределы страниц
            {
                pages = 1;
            }
            else if (pages > all_objects.Count / positions)
            {
                pages = all_objects.Count / positions+1;
            }


            for (int i = (pages - 1) * positions; i >= 0 && i < pages * positions && i < all_files.Count; i++)  //Постраничный вывод файлов
            {

                Console.WriteLine($"{all_files[i]} ");
                Console.SetCursorPosition(120, Console.CursorTop - 1);
                Console.Write($"{File.GetLastWriteTime(all_files[i])}\n");
                Console.SetCursorPosition(150, Console.CursorTop - 1);
                Console.WriteLine($"{FileArr[i].Length} байт \n");
                

            }

            for (int i = (pages - 1) * positions; i < pages * positions && i >= all_files.Count && i < all_objects.Count; i++)//Постраничный вывод директорий

            {
                try
                {
                    Console.WriteLine($"{all_objects[i]} ");
                    Console.SetCursorPosition(120, Console.CursorTop - 1);
                    Console.Write($"{Directory.GetLastAccessTime(all_objects[i])}\n");
                    Console.SetCursorPosition(150, Console.CursorTop - 1);
                    Console.WriteLine($"{GetDirSize(all_objects[i], ref size)} байт \n");
                }
                catch (UnauthorizedAccessException)         //Для таких папок, как "Application Data"
                {
                    Console.WriteLine("Нет доступа к этой папке\n");
                
                }

            }

            Console.WriteLine($"Страница {pages} из {all_objects.Count / positions + 1}");




        }



        static double GetDirSize(string folder, ref double size) //Функция для получения размера папки. Используя рекурсию, проходимся по папкам и суммируем размеры

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


        static void CopyFileTo(string old_name, string new_name, string path)       //копирование файла и перемещение в другую директорию
        {
            File.Copy(old_name, new_name);
            File.Move(new_name, path);
        }

        static void ChangeDir(string NextDir)                                           //Изменение директории.
        {
            directory = $@"{Directory.GetCurrentDirectory()}\{NextDir}";
            Directory.SetCurrentDirectory(directory);
        }
    }
}
