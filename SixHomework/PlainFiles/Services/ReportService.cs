using PlainFiles.Models;
using PlainFiles.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace PlainFiles.Services
{
    
    public class ReportService
    {
        private readonly LogWriter _logger;

        public ReportService(LogWriter logger)
        {
            _logger = logger;
        }

        
        public void ByCity(IEnumerable<Person> people)
        {
            if (people == null)
                throw new ArgumentNullException(nameof(people));

            var grouped = people
                .GroupBy(p => p.City)
                .OrderBy(g => g.Key, StringComparer.OrdinalIgnoreCase);

            decimal totalGeneral = 0m;

           
            const int idWidth = 5;
            const int firstNameWidth = 15;
            const int lastNameWidth = 15;
            const int balanceWidth = 15;

            foreach (var group in grouped)
            {
                Console.WriteLine($"City: {group.Key}");
                
                Console.WriteLine(
                    $"{"ID".PadRight(idWidth)}"
                    + $"{"FirstName".PadRight(firstNameWidth)}"
                    + $"{"LastName".PadRight(lastNameWidth)}"
                    + $"{"Balance".PadLeft(balanceWidth)}"
                );
                Console.WriteLine(
                    new string('-', idWidth + firstNameWidth + lastNameWidth + balanceWidth)
                );

                decimal subtotal = 0m;
                foreach (var p in group.OrderBy(p => p.Id))
                {                    
                    string line =
                        p.Id.ToString().PadRight(idWidth)
                        + p.FirstName.PadRight(firstNameWidth)
                        + p.LastName.PadRight(lastNameWidth)
                        + p.Balance.ToString("C2", CultureInfo.CurrentCulture).PadLeft(balanceWidth);

                    Console.WriteLine(line);
                    subtotal += p.Balance;
                }

                totalGeneral += subtotal;

                Console.WriteLine(new string('=', idWidth + firstNameWidth + lastNameWidth + balanceWidth));
                Console.WriteLine(
                    $"Subtotal {group.Key}:".PadRight(idWidth + firstNameWidth + lastNameWidth)
                    + subtotal.ToString("C2", CultureInfo.CurrentCulture).PadLeft(balanceWidth)
                );
                Console.WriteLine();
            }

            Console.WriteLine(new string('=', idWidth + firstNameWidth + lastNameWidth + balanceWidth));
            Console.WriteLine(
                $"GRAND TOTAL:".PadRight(idWidth + firstNameWidth + lastNameWidth)
                + totalGeneral.ToString("C2", CultureInfo.CurrentCulture).PadLeft(balanceWidth)
            );

            _logger.WriteLog("INFO", "Generated report by city.");
        }
    }
}
