namespace PlainFiles.Models
{
   
    public class User
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public bool IsActive { get; set; }

        public User() { }

        public User(string username, string password, bool isActive)
        {
            Username = username;
            Password = password;
            IsActive = isActive;
        }

        public override string ToString()
        {
            return $"{Username},{Password},{IsActive}";
        }

        
        public static User Parse(string line)
        {
            var parts = line.Split(',');
            if (parts.Length != 3)
                throw new FormatException("Invalid format in Users.txt. Must be: user, password, isActive");

            string user = parts[0];
            string pass = parts[1];
            if (!bool.TryParse(parts[2], out bool active))
                throw new FormatException("The third field must be 'true' or 'false'.");

            return new User(user, pass, active);
        }
    }
}
