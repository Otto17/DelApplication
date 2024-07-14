/*
   Простая консольная программа для автоматического удаления *.msi, *.exe программ, а так же конкретных файлов и папок.

   Данная программа является свободным программным обеспечением, распространяющимся по лицензии MIT.
   Копия лицензии: https://opensource.org/licenses/MIT

   Copyright (c) 2024 Otto
   Автор: Otto
   Версия: 14.07.24
   GitHub страница:  https://github.com/Otto17/DelApplication
   GitFlic страница: https://gitflic.ru/project/otto/delapplication

   г. Омск 2024
*/


using System;               // Библиотека предоставляет доступ к базовым классам и функциональности .NET Framework
using System.Diagnostics;   // Библиотека позволяет получать доступ к информации о процессах, потоках, событиях и выполнении кода приложения
using System.IO;            // Библиотека отвечает за ввод и вывод данных, включая чтение и запись файлов
using Microsoft.Win32;      // Библиотека позволяет работать с реестром Windows


namespace DelApplication
{
    internal class DelApplication
    {
        static void Main(string[] args)
        {
            //Если получили менее двух аргументов, тогда выводим справку
            if (args.Length < 2)
            {
                //Вывод справки
                Console.ForegroundColor = ConsoleColor.White; // Устанавливаем белый цвет для строк ниже
                Console.WriteLine("Простая консольная программа для автоматического удаления *.msi, *.exe программ, а так же конкретных файлов и папок.");
                Console.WriteLine("Использование: DelApplication.exe <Ключ> <Значение> [/KEY <Ключи для удаления (не обязательно)>]\n");

                Console.ResetColor(); // Сбрасываем цвет на стандартный
                Console.WriteLine("Поддерживается 3 способа удаления для программ с расширениями *.exe и *.msi:");

                Console.ForegroundColor = ConsoleColor.DarkGreen; // Устанавливаем тёмно-зелёный цвет для строк ниже
                Console.WriteLine("1) По имени файла (поиск осуществляется из реестра), используется ключ \"/N\" или \"/NAME\".");
                Console.WriteLine("2) По ID (из реестра) установленной программы, используется ключ \"/R\" или \"/REGISTRY\".");
                Console.WriteLine("3) По полному пути к файлу программы удаления \"unins000.exe\", используется ключ \"/P\" или \"/PATH\".\n");

                Console.WriteLine("Дополнительно:");
                Console.WriteLine("Поддерживаются опциональные ключи от удаляемых программ, если таковы есть (например для тихого удаления без перезагрузки), указываются в самом конце после вызова \"/K\" или \"/KEY\".");
                Console.WriteLine("Так же поддерживается удаление любых папок или файлов, то есть можно удалять \"Portable\" программы, для этого используется ключ \"/D\" или \"/DELETE\".\n");

                Console.ForegroundColor = ConsoleColor.Red; // Устанавливаем синий цвет для строки ниже
                Console.WriteLine("Разные программы могут не поддерживать тот или иной способ удаления, поэтому способов больше, чем один.\n");

                Console.ResetColor(); // Сбрасываем цвет на стандартный
                Console.WriteLine("Примеры использования на реальных программах.");

                Console.ForegroundColor = ConsoleColor.Blue; // Устанавливаем синий цвет для строки ниже
                Console.WriteLine("Программа \"WinSCP\" для подключения через SCP, FTP, SFTP, S3 и WebDAV:");

                Console.ForegroundColor = ConsoleColor.White; // Устанавливаем синий цвет для строк ниже
                Console.WriteLine("(*.exe) - DelApplication /N \"WinSCP\" /KEY \"/VERYSILENT\"");
                Console.WriteLine("(*.exe) - DelApplication /PATH \"C:\\Users\\user\\AppData\\Local\\Programs\\WinSCP\\unins000.exe\" /KEY \"/VERYSILENT\"");
                Console.WriteLine("(*.msi) - DelApplication /NAME \"WinSCP\" /K \"/quiet\"");
                Console.WriteLine("(*.msi) - DelApplication /R \"MsiExec.exe /X{7F02DF31-4309-4D68-B740-C3ED6F48FF9C}\" /K \"/quiet\"\n");

                Console.ForegroundColor = ConsoleColor.Blue; // Устанавливаем синий цвет для строки ниже
                Console.WriteLine("Программа \"Samsung Magician\" для работы с SSD:");

                Console.ForegroundColor = ConsoleColor.White; // Устанавливаем синий цвет для строки ниже
                Console.WriteLine("DelApplication.exe /PATH \"C:\\Program Files (x86)\\Samsung\\Samsung Magician\\unins000.exe\" /KEY \"/VERYSILENT /NORESTART\"\n");

                Console.ForegroundColor = ConsoleColor.Blue; // Устанавливаем синий цвет для строки ниже
                Console.WriteLine("Программа удалённого доступа \"Ассистент\":");

                Console.ForegroundColor = ConsoleColor.White; // Устанавливаем синий цвет для строки ниже
                Console.WriteLine("DelApplication.exe /NAME \"Ассистент\" /K \"/quiet\"\n");

                Console.ForegroundColor = ConsoleColor.Blue; // Устанавливаем синий цвет для строки ниже
                Console.WriteLine("Удаление конкретного файла или папки со всем её содержимым:");

                Console.ForegroundColor = ConsoleColor.White; // Устанавливаем синий цвет для строки ниже
                Console.WriteLine("DelApplication.exe /DELETE \"D:\\Шлак\\Тестовая папка\\tst OK\\DelApplication.pdb\"");
                Console.WriteLine("DelApplication /D \"C:\\Users\\Public\\Desktop\\Тестовая папка\"\n");

                Console.ForegroundColor = ConsoleColor.Yellow; // Устанавливаем жёлтый цвет для строк ниже
                Console.WriteLine("Автор Otto, г.Омск 2024");
                Console.WriteLine("GitHub страница:  https://github.com/Otto17/DelApplication");
                Console.WriteLine("GitFlic страница: https://gitflic.ru/project/otto/delapplication");
                Console.ResetColor(); // Сбрасываем цвет на стандартный
                return;
            }

            string key = args[0].ToUpper(); // Первый аргумент преобразуем в верхний регистр
            string value = args[1];         // Второй аргумент
            string uninstallKeys = "";      // Не обязательный аргумент (дополнительные ключи от удаляемой программы)

            //Поиск необязательных ключей для удаления, начиная с 3-го аргумента
            for (int i = 2; i < args.Length; i++)
            {
                //Если аргумент начинается с "/K" или "/KEY", то следующий аргумент сохраняется в переменную "uninstallKeys" и цикл прерывается
                if (args[i].StartsWith("/K") || args[i].StartsWith("/KEY"))
                {
                    uninstallKeys = args[i + 1];
                    break;
                }
            }

            //Выбирается один из методов в зависимости от полученного аргумента "key"
            switch (key)
            {
                case "/N":
                case "/NAME":
                    UninstallByName(value, uninstallKeys);  // Удаление по имени файла
                    break;
                case "/R":
                case "/REGISTRY":
                    UninstallByRegistry(value, uninstallKeys);  // Удаление по ID (из реестра) установленной программы
                    break;
                case "/P":
                case "/PATH":
                    UninstallByPath(value, uninstallKeys);  // Удаление по полному пути к файлу программы удаления "unins000.exe"
                    break;
                case "/D":
                case "/DELETE":
                    DeleteFileOrFolder(value);  // Удаление папки (со всем их содержимым) или файла
                    break;
                default:
                    Console.WriteLine("Неизвестный ключ.");
                    break;
            }
        }


        //Функция удаления по имени файла
        static void UninstallByName(string programName, string uninstallKeys)
        {
            string uninstallPath = GetUninstallPath(programName);   // Получаем путь к программе для удаления

            if (string.IsNullOrEmpty(uninstallPath))    // Если путь пустой или равен null
            {
                Console.WriteLine($"Программа \"{programName}\" не найдена.");
                return; // Завершаем работу программы
            }

            Console.WriteLine($"Найдена программа \"{programName}\". Путь для удаления: {uninstallPath}");
            UninstallProgram(uninstallPath, uninstallKeys, programName);    // Вызываем метод удаления найденной программы
        }


        //Функция удаления по ID (из реестра) установленной программы
        static void UninstallByRegistry(string uninstallCommand, string uninstallKeys)
        {
            if (string.IsNullOrEmpty(uninstallCommand)) // Если путь пустой или равен null
            {
                Console.WriteLine("Команда для удаления не указана.");
                return; // Завершаем работу программы
            }

            Console.WriteLine($"Использование команды для удаления: {uninstallCommand}");
            UninstallProgram(uninstallCommand, uninstallKeys, uninstallCommand);    // Вызываем метод удаления программы
        }


        //Функция удаления по полному пути к файлу программы удаления "unins000.exe"
        static void UninstallByPath(string executablePath, string uninstallKeys)
        {
            if (string.IsNullOrEmpty(executablePath))   // Если путь пустой или равен null
            {
                Console.WriteLine("Путь к исполняемому файлу не указан.");
                return; // Завершаем работу программы
            }

            if (!File.Exists(executablePath))   // Проверяем, существует ли файл
            {
                Console.WriteLine($"Исполняемый файл \"{executablePath}\" не найден.");
                return; // Завершаем работу программы
            }

            Console.WriteLine($"Использование пути для удаления: {executablePath}");
            ExecuteCommand(executablePath, uninstallKeys);  // Вызываем метод удаления программы
        }


        //Метод поиска пути установленной программы в разных ветках и пользователях
        static string GetUninstallPath(string programName)
        {
            //Создаём массив строк для поиска в реестре установленных программах, как x86, так и x64
            string[] registryKeys = {
                @"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
                @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
            };

            //Перебираем элементы массива
            foreach (var registryKey in registryKeys)
            {
                string path = GetUninstallPathFromRegistry(Registry.LocalMachine, registryKey, programName);    // Ищем по пути в локальной машине
                if (!string.IsNullOrEmpty(path)) return path;                                                   // Если нашли, возвращаем строку

                path = GetUninstallPathFromRegistry(Registry.CurrentUser, registryKey, programName);     // Ищем по пути в конкретном пользователе (от которого запущена программа)
                if (!string.IsNullOrEmpty(path)) return path;                                            // Если нашли, возвращаем строку
            }

            return null;    // Если ничего не нашли, возвращаем "null"
        }


        //Метод поиска пути программы в реестре
        static string GetUninstallPathFromRegistry(RegistryKey rootKey, string registryKey, string programName)
        {
            //Открываем ключ реестра с заданным путём
            using (RegistryKey key = rootKey.OpenSubKey(registryKey))
            {
                //Если ключ не равен "null"
                if (key != null)
                {
                    //Проходим по всем подключам (subkey) ключа
                    foreach (string subkeyName in key.GetSubKeyNames())
                    {
                        //Для каждого подключа открывается отдельный ключ "subkey"
                        using (RegistryKey subkey = key.OpenSubKey(subkeyName))
                        {
                            //Если подключ существует, то извлекаем как строки значения "DisplayName", "DisplayVersion", "Publisher" из подключа
                            if (subkey != null)
                            {
                                string displayName = subkey.GetValue("DisplayName") as string;
                                string displayVersion = subkey.GetValue("DisplayVersion") as string;
                                string publisher = subkey.GetValue("Publisher") as string;

                                //Проверяем наличие совпадения искомого "programName" с одним из значений: "displayName", "displayVersion", "publisher" (игнорируя регистр символов)
                                if ((displayName != null && displayName.IndexOf(programName, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                    (displayVersion != null && displayVersion.IndexOf(programName, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                    (publisher != null && publisher.IndexOf(programName, StringComparison.OrdinalIgnoreCase) >= 0))
                                {
                                    Console.WriteLine($"Найдено совпадение: {displayName}, {displayVersion}, {publisher}");
                                    return subkey.GetValue("UninstallString") as string;    // Возвращаем значение строки из ключа "subkey" по ключу "UninstallString"
                                }
                            }
                        }
                    }
                }
            }
            return null;    // Если не нашли совпадений, возвращаем "null"
        }


        //Метод удаления найденной программы
        static void UninstallProgram(string uninstallPath, string uninstallKeys, string programName)
        {
            //Инициализируем переменные
            string executablePath = uninstallPath;
            string arguments = "";

            //Если путь начинается с кавычки
            if (uninstallPath.StartsWith("\""))
            {
                // Тогда находим индекс закрывающей кавычки после первой
                int endQuoteIndex = uninstallPath.IndexOf("\"", 1);
                if (endQuoteIndex > 0)  // И если индекс больше нуля
                {
                    //Тогда переопределяем "executablePath" и "arguments" соответствующим образом
                    executablePath = uninstallPath.Substring(1, endQuoteIndex - 1);
                    arguments = uninstallPath.Substring(endQuoteIndex + 1).Trim();
                }
            }
            else
            {
                //Если путь не начинается с кавычек, тогда разбиваем его по частям на пробелы
                string[] parts = uninstallPath.Split(new char[] { ' ' }, 2);
                if (parts.Length > 1)   // И если частей больше, чем 1
                {
                    //Тогда переопределяем "executablePath" и "arguments" соответствующим образом
                    executablePath = parts[0];
                    arguments = parts[1];
                }
            }

            arguments = arguments.Replace("/I", "/x").Trim(); // Любые вхождения строки "/I" в "arguments" заменяются на "/x" для утилиты "msiexec", если это потребуется

            //Если ключ удаления "uninstallKeys" не пустой, то добавляется к "arguments"
            if (!string.IsNullOrEmpty(uninstallKeys))
            {
                arguments += $" {uninstallKeys}";
            }

            ExecuteCommand(executablePath, arguments);  // Вызываем метод для выполнения команды
        }


        //Метод удаления программы
        static void ExecuteCommand(string executablePath, string arguments)
        {
            Process process = new Process();                // Создаём объект "Process"
            process.StartInfo.FileName = executablePath;    // Устанавливаем путь к исполняемому файлу
            process.StartInfo.Arguments = arguments;        // Устанавливаем аргументы для этого исполняемого файла
            process.StartInfo.UseShellExecute = true;       // Разрешаем использование оболочки
            process.StartInfo.Verb = "runas";               // Запускаем с наивысшими правами

            try
            {
                process.Start();        // Запускаем процесс
                process.WaitForExit();  // Ожидаем завершение процесса

                if (process.ExitCode == 0)
                {
                    Console.WriteLine($"Команда \"{executablePath}\" успешно выполнена.");
                }
                else
                {
                    Console.WriteLine($"Ошибка при выполнении команды \"{executablePath}\". Код ошибки: {process.ExitCode}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }


        //Функция удаления папки (со всем их содержимым) или файла
        static void DeleteFileOrFolder(string path)
        {
            try
            {
                //Проверяем, существует ли файл по указанному пути
                if (File.Exists(path))
                {
                    File.Delete(path);  // Если файл существует, удаляем его
                    Console.WriteLine($"Файл \"{path}\" успешно удалён.");
                }
                else if (Directory.Exists(path))    // Если файла не существует, проверяем, существует ли папка с указанным путём
                {
                    Directory.Delete(path, true);   // Если папка существует, удаляем её вместе со всем содержимым
                    Console.WriteLine($"Папка \"{path}\" успешно удалена со всем её содержимым.");
                }
                else
                {
                    Console.WriteLine($"Файл или папка \"{path}\" не найдены.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при удалении \"{path}\": {ex.Message}");
            }
        }
    }
}
