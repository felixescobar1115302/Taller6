namespace PlainFiles.Models
{
    
    public class Person
    {
        public int Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public decimal Balance { get; set; }

        public override string ToString()
        {
           
            return $"{Id}\t{FirstName} {LastName}\n\t" +
                   $"Phone: {Phone}\n\t" +
                   $"City: {City}\n\t" +
                   $"Balance: {Balance,20:C2}\n";
        }
    }
}
