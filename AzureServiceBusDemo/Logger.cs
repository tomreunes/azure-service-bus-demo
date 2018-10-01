using System;

namespace AzureServiceBusDemo
{
    public class Logger
    {
        private static readonly object ConsoleLock = new object();
        public static void Log(string log, ConsoleColor color = ConsoleColor.White)
        {
            lock (ConsoleLock)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(log);
                Console.ResetColor();
            }
        }
    }
}