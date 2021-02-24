using System;
using System.IO;
using System.Linq;

namespace JavaBedrockUtilities
{
    partial class Program
    {
        public static LogLevel logLevel { get; private set; }
        public static string inputPath { get; private set; }
        public static Type inputType { get; private set; }
        public static string outputPath { get; private set; }
        public static Type outputType { get; private set; }

        static void Main(string[] args)
        {
            try
            {
                AskForArgs(args);
                Processing.ProcessFiles();
            }
            catch (Exception e) { Logging.Log(e, LogLevel.Error); }
            finally
            {
                Console.WriteLine("Press enter to close...");
                Console.ReadLine();
            }
        }

        static void AskForArgs(string[] args)
        {
            object logLevelObj;
            if (args.Contains("-log")) Enum.TryParse(typeof(LogLevel), args[Array.IndexOf(args, "-log") + 1], out logLevelObj);
            else
            {
                Console.Write($"Level of logging (default: Info; available: {string.Join(", ", Enum.GetNames(typeof(LogLevel)))}): ");
                Enum.TryParse(typeof(LogLevel), Console.ReadLine(), out logLevelObj);
            }
            logLevel = logLevelObj == null ? LogLevel.Info : (LogLevel)logLevelObj;

            if (args.Contains("-inputPath")) inputPath = args[Array.IndexOf(args, "-inputPath") + 1];
            else
            {
                Console.Write("Input path (default: in): ");
                inputPath = Console.ReadLine();
            }
            inputPath = Path.GetFullPath(string.IsNullOrWhiteSpace(inputPath) ? "in" : inputPath).TrimEnd('\\', '/') + "/";

            object inputTypeObj;
            if (args.Contains("-inputType")) Enum.TryParse(typeof(Type), args[Array.IndexOf(args, "-inputType") + 1], out inputTypeObj);
            else
            {
                Console.Write($"Input format (default: auto; available: auto, {string.Join(", ", Enum.GetNames(typeof(Type)))}): ");
                Enum.TryParse(typeof(Type), Console.ReadLine(), out inputTypeObj);
            }
            inputType = inputTypeObj == null ? (Type)(-1) : (Type)inputTypeObj;

            if (args.Contains("-outputPath")) outputPath = args[Array.IndexOf(args, "-outputPath") + 1];
            else
            {
                Console.Write("Output path (default: out): ");
                outputPath = Console.ReadLine();
            }
            outputPath = Path.GetFullPath(string.IsNullOrWhiteSpace(outputPath) ? "out" : outputPath).TrimEnd('\\', '/') + "/";

            object outputTypeObj;
            if (args.Contains("-outputType")) Enum.TryParse(typeof(Type), args[Array.IndexOf(args, "-outputType") + 1], out outputTypeObj);
            else
            {
                Console.Write($"Output format (default: mer; available: {string.Join(", ", Enum.GetNames(typeof(Type)))}): ");
                Enum.TryParse(typeof(Type), Console.ReadLine(), out outputTypeObj);
            }
            outputType = outputTypeObj == null ? Type.mer : (Type)outputTypeObj;
        }
    }
}
