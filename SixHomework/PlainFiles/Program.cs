using PlainFiles.Helpers;
using PlainFiles.Services;
using System;
using System.IO;

namespace PlainFiles
{
    class Program
    {
        static void Main()
        {            
            var baseFolder = AppContext.BaseDirectory;                  
            var dataFolder = Path.Combine(baseFolder, "Data");         
            var usersFile = Path.Combine(dataFolder, "Users.txt");
            var peopleFile = Path.Combine(dataFolder, "people.csv");
            var logFile = Path.Combine(baseFolder, "log.txt");

            
            var authService = new AuthService(usersFile);
            try
            {
                authService.LoadUsers();
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            if (!authService.Login())
                return;

            var username = authService.CurrentUser!.Username;

           
            using var logger = new LogWriter(logFile, username);
            logger.WriteLog("INFO", "User logged in.");

            
            var personService = new PersonService(peopleFile, logger);
            personService.Load();
            var reportService = new ReportService(logger);

            
            string option;
            do
            {
                Console.Clear();
                Console.WriteLine($"Welcome, {username}!");
                Console.WriteLine("1. Show content");
                Console.WriteLine("2. Add person");
                Console.WriteLine("3. Edit person");
                Console.WriteLine("4. Delete person");
                Console.WriteLine("5. Save changes");
                Console.WriteLine("6. Report by city");
                Console.WriteLine("0. Exit");
                Console.Write("Choose an option: ");
                option = Console.ReadLine() ?? "0";
                Console.WriteLine();

                switch (option)
                {
                    case "1":
                        personService.ShowAll();
                        break;
                    case "2":
                        personService.Add();
                        break;
                    case "3":
                        personService.Edit();
                        break;
                    case "4":
                        personService.Delete();
                        break;
                    case "5":
                        personService.Save();
                        break;
                    case "6":
                        reportService.ByCity(personService.People);
                        break;
                    case "0":
                        personService.Save();
                        logger.WriteLog("INFO", "Application closed.");
                        Console.WriteLine("Goodbye!");
                        break;
                    default:
                        Console.WriteLine("Invalid option. Try again.");
                        break;
                }

                if (option != "0")
                {
                    Console.WriteLine("\nPress ENTER to continue...");
                    Console.ReadLine();
                }

            } while (option != "0");

            
            SyncDataBack();
        }

        
        private static void SyncDataBack()
        {
            var binFolder = AppContext.BaseDirectory;
            var binData = Path.Combine(binFolder, "Data");
           
            var projectRoot = Directory.GetParent(binFolder)!.Parent!.Parent!.Parent!.FullName;
            var projectData = Path.Combine(projectRoot, "Data");

            if (!Directory.Exists(binData))
                return;

            if (!Directory.Exists(projectData))
                Directory.CreateDirectory(projectData);

            foreach (var srcFile in Directory.GetFiles(binData))
            {
                var fileName = Path.GetFileName(srcFile);
                var destFile = Path.Combine(projectData, fileName);
                File.Copy(srcFile, destFile, overwrite: true);
            }

            Console.WriteLine("Data folder synced back to project.");
        }
    }
}
