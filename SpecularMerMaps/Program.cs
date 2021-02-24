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
        static void Main(string[] args)
        {
            try { Prog(args); }
            catch (Exception e) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(e); Console.ForegroundColor = ConsoleColor.White; }
            finally
            {
                Console.WriteLine("Press enter to close...");
                Console.ReadLine();
            }
        }
        static void Prog(string[] args)
        {
            string input;
            if (args.Contains("-input")) input = args[Array.IndexOf(args, "-input") + 1];
            else
            {
                Console.Write("Input dir (default: in): ");
                input = Console.ReadLine();
            }
            input = Path.GetFullPath(string.IsNullOrWhiteSpace(input) ? "in" : input).TrimEnd('\\', '/') + "/";

            object inputTypeObj;
            if (args.Contains("-type")) Enum.TryParse(typeof(Type), args[Array.IndexOf(args, "-type") + 1], out inputTypeObj);
            else
            {
                Console.Write("Input format (default: auto; available: auto, mer, s): ");
                Enum.TryParse(typeof(Type), Console.ReadLine(), out inputTypeObj);
            }
            var inputType = inputTypeObj == null ? (Type)(-1) : (Type)inputTypeObj;

            string outputDir;
            if (args.Contains("-output")) outputDir = args[Array.IndexOf(args, "-output") + 1];
            else
            {
                Console.Write("Output dir (default: out): ");
                outputDir = Console.ReadLine();
            }
            outputDir = Path.GetFullPath(string.IsNullOrWhiteSpace(outputDir) ? "out" : outputDir).TrimEnd('\\', '/') + "/";

            object outputTypeObj;
            if (args.Contains("-type")) Enum.TryParse(typeof(Type), args[Array.IndexOf(args, "-type") + 1], out outputTypeObj);
            else
            {
                Console.Write("Output format (default: mer; available: mer, s): ");
                Enum.TryParse(typeof(Type), Console.ReadLine(), out outputTypeObj);
            }
            var outputType = outputTypeObj == null ? Type.mer : (Type)outputTypeObj;

            var tasks = new List<Task>();
            foreach (var path in Directory.EnumerateFiles(input, "*.png", SearchOption.AllDirectories))
                tasks.Add(Task.Factory.StartNew(() => ProcessFile(path)));
            Task.WaitAll(tasks.ToArray());

            void ProcessFile(string path)
            {
                try
                {
                    var texPath = Path.GetRelativePath(input, Path.GetDirectoryName(path));
                    var texName = Path.GetFileNameWithoutExtension(path);
                    var texType = inputType;
                    if (inputType == (Type)(-1))
                    {
                        var texTypeTxt = texName.Substring(texName.LastIndexOf("_") + 1);
                        if (Enum.TryParse(typeof(Type), texTypeTxt, out var texTypeObj)) texType = (Type)texTypeObj;
                        else throw new NotSupportedException("Format " + texTypeTxt + " isn't supported");
                        texName = texName.Substring(0, texName.Length - texTypeTxt.Length - 1);
                    }
                    var outputName = $"{outputDir}{texPath}/{texName}_{Enum.GetName(typeof(Type), outputType)}.png";
                    if (!Directory.Exists(outputDir + texPath)) Directory.CreateDirectory(outputDir + texPath);
                    using (Bitmap source = new Bitmap(path))
                    {
                        var dest = new Bitmap(source.Width, source.Height);

                        for (int x = 0; x < source.Width; x++)
                            for (int y = 0; y < source.Height; y++)
                                dest.SetPixel(x, y, Data.ToColor(Data.Parse(source.GetPixel(x, y), texType), outputType));
                        dest.Save(outputName, ImageFormat.Png);
                    }
                    Console.WriteLine($"Saved at {outputName}");
                }
                catch (Exception e) { Console.ForegroundColor = ConsoleColor.Red; Console.WriteLine(e); Console.ForegroundColor = ConsoleColor.White; }
            }
        }
    }
}
