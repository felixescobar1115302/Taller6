using System;
using System.IO;
using System.Text;

namespace PlainFiles.Helpers
{    
    public class LogWriter : IDisposable
    {
        private readonly StreamWriter _writer;
        private readonly string _username;

        
        public LogWriter(string path, string username)
        {
            _username = username;
            _writer = new StreamWriter(path, append: true, encoding: Encoding.UTF8)
            {
                AutoFlush = true
            };
        }

       
        public void WriteLog(string level, string message)
        {
            string timestamp = DateTime.Now.ToString("s");
            _writer.WriteLine($"{timestamp} [{level.ToUpper()}] [{_username}] {message}");
        }

        public void Dispose()
        {
            _writer?.Dispose();
        }
    }
}
