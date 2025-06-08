using CsvHelper;
using PlainFiles.Models;
using System.Globalization;
using System.Text;

namespace PlainFiles.Helpers
{
    public class CsvHelperExample
    {
       
        public void Write(string path, IEnumerable<Person> people)
        {
            using var writer = new StreamWriter(path, false, Encoding.UTF8);
            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);
            csv.WriteHeader<Person>();   
            csv.NextRecord();
            csv.WriteRecords(people);    
        }

        
        public List<Person> Read(string path)
        {
            if (!File.Exists(path))
            {
                return new List<Person>();
            }

            using var reader = new StreamReader(path, Encoding.UTF8);
            using var csv = new CsvReader(reader, CultureInfo.InvariantCulture);
            var records = csv.GetRecords<Person>().ToList();
            return records;
        }
    }
}
