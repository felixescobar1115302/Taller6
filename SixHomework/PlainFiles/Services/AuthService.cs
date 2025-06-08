using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PlainFiles.Models;

namespace PlainFiles.Services
{
    
    public class AuthService
    {
        private readonly string _usersFilePath;
        private readonly List<User> _users = new();

        public User? CurrentUser { get; private set; }

        public AuthService(string usersFilePath)
        {
            _usersFilePath = usersFilePath;
        }

       
        public void LoadUsers()
        {
            _users.Clear();
            if (!File.Exists(_usersFilePath))
                throw new FileNotFoundException("Users file not found", _usersFilePath);

            foreach (var line in File.ReadAllLines(_usersFilePath))
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                try { _users.Add(User.Parse(line.Trim())); }
                catch {  }
            }
        }

       
        public bool Login()
        {
            Console.WriteLine("========== Welcome – Please log in ==========");
            int attempts = 0;
            User? attemptedUser = null;

            while (attempts < 3)
            {
                Console.Write("Username: ");
                var username = Console.ReadLine() ?? string.Empty;

                attemptedUser = _users
                    .FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));

                Console.Write("Password: ");
                var password = ReadPassword();

                if (attemptedUser == null)
                {
                    Console.WriteLine("User not found.");
                    attempts++;
                    continue;
                }

                if (!attemptedUser.IsActive)
                {
                    Console.WriteLine("User is blocked. Contact administrator.");
                    return false;
                }

                if (attemptedUser.Password == password)
                {
                    CurrentUser = attemptedUser;
                    return true;
                }

                Console.WriteLine("Incorrect password.");
                attempts++;
            }

            
            if (attemptedUser != null)
            {
                Console.WriteLine("Too many failed attempts. User will be blocked.");
                BlockUser(attemptedUser);
                SyncUsersBack();
            }
            else
            {
                Console.WriteLine("Too many failed attempts.");
            }

            return false;
        }

        
        private void BlockUser(User user)
        {
            user.IsActive = false;
            File.WriteAllLines(
                _usersFilePath,
                _users.Select(u => u.ToString())
            );
        }

       
        private void SyncUsersBack()
        {
            var binFolder = AppContext.BaseDirectory;
            var binData = Path.Combine(binFolder, "Data");
            
            var projectRoot = Directory.GetParent(binFolder)!.Parent!.Parent!.Parent!.FullName;
            var projectData = Path.Combine(projectRoot, "Data");
            var fileName = Path.GetFileName(_usersFilePath);

            var srcPath = Path.Combine(binData, fileName);
            var destPath = Path.Combine(projectData, fileName);

            if (File.Exists(srcPath))
            {
                Directory.CreateDirectory(projectData);
                File.Copy(srcPath, destPath, overwrite: true);
            }
        }

        
        private string ReadPassword()
        {
            var pwd = string.Empty;
            ConsoleKeyInfo key;
            do
            {
                key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Backspace && pwd.Length > 0)
                {
                    pwd = pwd[0..^1];
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    pwd += key.KeyChar;
                    Console.Write("*");
                }
            } while (key.Key != ConsoleKey.Enter);
            Console.WriteLine();
            return pwd;
        }
    }
}
