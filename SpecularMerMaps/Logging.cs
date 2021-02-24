using System;

namespace JavaBedrockUtilities
{
    public enum LogLevel { Debug, Info, Success, Warning, Error }
    public static class Logging
    {
        public static void Log(Exception e, LogLevel level = LogLevel.Error) => Log(e.ToString(), level);
        public static void Log(string message, LogLevel level = LogLevel.Info)
        {
            if (level < Program.logLevel) return;
            Console.ForegroundColor = level.LogColor();
            Console.WriteLine(message);
            Console.ForegroundColor = LogLevel.Info.LogColor();
        }
        static ConsoleColor LogColor(this LogLevel level) => level switch
        {
            LogLevel.Debug => ConsoleColor.DarkGray,
            LogLevel.Info => ConsoleColor.Gray,
            LogLevel.Success => ConsoleColor.Green,
            LogLevel.Warning => ConsoleColor.Yellow,
            LogLevel.Error => ConsoleColor.Red,
            _ => ConsoleColor.Gray
        };
    }
}
