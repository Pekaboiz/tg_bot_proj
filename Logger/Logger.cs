using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelegramServiceHandler.Logger
{
    public static class tgLog
    {
        public static void Log(string message)
        {
            string logFilePath = @"D:\Git\TelegramServiceHandler\log.json";
            using (StreamWriter writer = new StreamWriter(logFilePath, true))
            {
                writer.WriteLine($"{DateTime.Now}: {message}");
            }
        }
    }
}
