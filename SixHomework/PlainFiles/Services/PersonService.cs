using PlainFiles.Helpers;
using PlainFiles.Models;
using PlainFiles.Helpers;
using PlainFiles.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace PlainFiles.Services
{
    
    public class PersonService
    {
        private readonly string _csvPath;
        private readonly CsvHelperExample _csv;
        private readonly LogWriter _logger;
        public List<Person> People { get; private set; } = new List<Person>();

        public PersonService(string csvPath, LogWriter logger)
        {
            _csvPath = csvPath;
            _logger = logger;
            _csv = new CsvHelperExample();
        }

        
        public void Load()
        {
            People = _csv.Read(_csvPath);
        }

        
        public void ShowAll()
        {
            if (!People.Any())
            {
                Console.WriteLine("No persons found.");
                return;
            }
            Console.WriteLine("============= List of Persons =============");
            foreach (var p in People)
            {
                Console.WriteLine(p);
            }
        }

        
        public void Add()
        {
            Console.WriteLine("------ Add new person ------");

            
            int id;
            while (true)
            {
                Console.Write("Enter ID (numeric & unique): ");
                var idStr = Console.ReadLine() ?? string.Empty;
                if (!int.TryParse(idStr, out id))
                {
                    Console.WriteLine("ID must be a valid integer.");
                    continue;
                }
                if (People.Any(x => x.Id == id))
                {
                    Console.WriteLine("This ID already exists. Try another.");
                    continue;
                }
                break;
            }

            
            string firstName;
            do
            {
                Console.Write("Enter First Name: ");
                firstName = Console.ReadLine() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(firstName))
                    Console.WriteLine("First Name cannot be empty.");
            } while (string.IsNullOrWhiteSpace(firstName));

            
            string lastName;
            do
            {
                Console.Write("Enter Last Name: ");
                lastName = Console.ReadLine() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(lastName))
                    Console.WriteLine("Last Name cannot be empty.");
            } while (string.IsNullOrWhiteSpace(lastName));

            
            string phone;
            do
            {
                Console.Write("Enter Phone (digits and optional '-'): ");
                phone = Console.ReadLine() ?? string.Empty;
                if (!IsValidPhone(phone))
                    Console.WriteLine("Invalid phone format. Example: 311-123-4567 or 3121234567");
            } while (!IsValidPhone(phone));

            
            string city;
            do
            {
                Console.Write("Enter City: ");
                city = Console.ReadLine() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(city))
                    Console.WriteLine("City cannot be empty.");
            } while (string.IsNullOrWhiteSpace(city));

            
            decimal balance;
            while (true)
            {
                Console.Write("Enter Balance (positive decimal): ");
                var balStr = Console.ReadLine() ?? string.Empty;
                if (!decimal.TryParse(balStr, NumberStyles.Number, CultureInfo.InvariantCulture, out balance))
                {
                    Console.WriteLine("Balance must be a valid decimal number (e.g., 1000.00).");
                    continue;
                }
                if (balance < 0)
                {
                    Console.WriteLine("Balance must be positive.");
                    continue;
                }
                break;
            }

            var newPerson = new Person
            {
                Id = id,
                FirstName = firstName.Trim(),
                LastName = lastName.Trim(),
                Phone = phone.Trim(),
                City = city.Trim(),
                Balance = balance
            };

            People.Add(newPerson);
            _logger.WriteLog("INFO", $"Added new Person: ID={newPerson.Id}, {newPerson.FirstName} {newPerson.LastName}");
            Console.WriteLine("New person added successfully.");
        }

        
        public void Edit()
        {
            Console.WriteLine("------ Edit person ------");
            Console.Write("Enter the ID of the person to edit: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var person = People.FirstOrDefault(x => x.Id == id);
            if (person == null)
            {
                Console.WriteLine($"No person found with ID = {id}.");
                return;
            }

            Console.WriteLine($"Editing Person: {person.FirstName} {person.LastName} (ID={person.Id})");
            Console.WriteLine("Press ENTER to keep the current value.");

           
            Console.Write($"First Name ({person.FirstName}): ");
            var inp = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(inp)) person.FirstName = inp.Trim();

            
            Console.Write($"Last Name ({person.LastName}): ");
            inp = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(inp)) person.LastName = inp.Trim();

            
            Console.Write($"Phone ({person.Phone}): ");
            inp = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(inp) && IsValidPhone(inp)) person.Phone = inp.Trim();

            
            Console.Write($"City ({person.City}): ");
            inp = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(inp)) person.City = inp.Trim();

            
            Console.Write($"Balance ({person.Balance.ToString("F2", CultureInfo.InvariantCulture)}): ");
            inp = Console.ReadLine();
            if (!string.IsNullOrWhiteSpace(inp)
                && decimal.TryParse(inp, NumberStyles.Number, CultureInfo.InvariantCulture, out var b)
                && b >= 0)
            {
                person.Balance = b;
            }

            _logger.WriteLog("INFO", $"Edited Person: ID={person.Id}, {person.FirstName} {person.LastName}");
            Console.WriteLine("Person updated successfully.");
        }

        
        public void Delete()
        {
            Console.WriteLine("------ Delete person ------");
            Console.Write("Enter the ID of the person to delete: ");
            if (!int.TryParse(Console.ReadLine(), out int id))
            {
                Console.WriteLine("Invalid ID format.");
                return;
            }

            var person = People.FirstOrDefault(x => x.Id == id);
            if (person == null)
            {
                Console.WriteLine($"No person found with ID = {id}.");
                return;
            }

            Console.WriteLine($"Found: {person.FirstName} {person.LastName}, City: {person.City}, Balance: {person.Balance:C2}");
            Console.Write("Are you sure you want to delete this person? (Y/N): ");
            var confirm = Console.ReadLine() ?? "N";
            if (confirm.Equals("Y", StringComparison.OrdinalIgnoreCase))
            {
                People.Remove(person);
                _logger.WriteLog("INFO", $"Deleted Person: ID={person.Id}, {person.FirstName} {person.LastName}");
                Console.WriteLine("Person deleted successfully.");
            }
            else
            {
                Console.WriteLine("Delete operation canceled.");
            }
        }

        
        public void Save()
        {
            _csv.Write(_csvPath, People);
            _logger.WriteLog("INFO", $"Saved people to {_csvPath}. ({People.Count} records)");
            Console.WriteLine("Changes saved to CSV.");
        }

        
        private bool IsValidPhone(string phone)
        {
            return !string.IsNullOrWhiteSpace(phone)
                && phone.Length >= 7
                && phone.All(ch => char.IsDigit(ch) || ch == '-' || ch == ' ');
        }
    }
}
