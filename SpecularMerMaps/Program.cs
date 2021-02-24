using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace JavaBedrockUtilities
{
    class Program
    {
        public static LogLevel logLevel { get; private set; }
        public static string inputPath { get; private set; }
        public static Type inputType { get; private set; }
        public static string outputPath { get; private set; }
        public static Type outputType { get; private set; }

        static void Main(string[] args)
        {
            try { Prog(args); }
            catch (Exception e) { Log(e, LogLevel.Error); }
            finally
            {
                Console.WriteLine("Press enter to close...");
                Console.ReadLine();
            }
        }
        static void Prog(string[] args)
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

            var tasks = new List<Task>();
            foreach (var path in Directory.EnumerateFiles(inputPath, "*.png", SearchOption.AllDirectories))
                tasks.Add(Task.Factory.StartNew(() => ProcessFile(path)));
            Task.WaitAll(tasks.ToArray());

            void ProcessFile(string path)
            {
                try
                {
                    Log($"Processing {path}", LogLevel.Debug);
                    var texPath = Path.GetRelativePath(inputPath, Path.GetDirectoryName(path));
                    var texName = Path.GetFileNameWithoutExtension(path);
                    var texType = inputType;
                    if (inputType == (Type)(-1))
                    {
                        var texTypeTxt = texName.Substring(texName.LastIndexOf("_") + 1);
                        if (Enum.TryParse(typeof(Type), texTypeTxt, out var texTypeObj)) texType = (Type)texTypeObj;
                        else throw new NotSupportedException($"Format \"{texTypeTxt}\" isn't supported: {path}");
                        texName = texName.Substring(0, texName.Length - texTypeTxt.Length - 1);
                    }
                    var outputName = $"{outputPath}{texPath}/{texName}_{Enum.GetName(typeof(Type), outputType)}.png";
                    if (!Directory.Exists(outputPath + texPath)) Directory.CreateDirectory(outputPath + texPath);

                    Log($"Converting {Path.GetRelativePath(inputPath, path)} from {texType} to {outputType}", LogLevel.Debug);
                    using (Bitmap source = new Bitmap(path))
                    {
                        var dest = new Bitmap(source.Width, source.Height);

                        for (int x = 0; x < source.Width; x++)
                            for (int y = 0; y < source.Height; y++)
                                dest.SetPixel(x, y, Data.ToColor(Data.Parse(source.GetPixel(x, y), texType), outputType));
                        dest.Save(outputName, ImageFormat.Png);
                    }

                    Log($"Saved at {Path.GetFullPath(outputName)}", LogLevel.Success);
                }
                catch (NotSupportedException e) { Log(e.Message, LogLevel.Warning); }
                catch (Exception e) { Log(e, LogLevel.Error); }
            }
        }

        public enum LogLevel { Debug, Info, Success, Warning, Error }
        static void Log(Exception e, LogLevel level = LogLevel.Error) => Log(e.ToString(), level);
        static void Log(string message, LogLevel level = LogLevel.Info)
        {
            if (level < logLevel) return;
            Console.ForegroundColor = LogColor(level);
            Console.WriteLine(message);
            Console.ForegroundColor = LogColor(LogLevel.Info);
        }
        static ConsoleColor LogColor(LogLevel level) => level switch
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
