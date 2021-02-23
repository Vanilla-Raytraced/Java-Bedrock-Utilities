using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace JavaBedrockUtilities
{
    class Program
    {
        static void Main(string[] args)
        {
            string input = "";
            if (args.Contains("-input")) input = args[Array.IndexOf(args, "-input") + 1];
            else
            {
                Console.Write("Input dir: ");
                input = Console.ReadLine();
            }

            string outputDir = "";
            if (args.Contains("-output")) outputDir = args[Array.IndexOf(args, "-output") + 1];
            else
            {
                Console.Write("Output dir (default: out): ");
                outputDir = Console.ReadLine();
            }
            outputDir = Path.GetFullPath(string.IsNullOrWhiteSpace(outputDir) ? "out" : outputDir).TrimEnd('\\', '/') + "/";
            if (!Directory.Exists(outputDir)) Directory.CreateDirectory(outputDir);

            object outputTypeObj;
            if (args.Contains("-type")) Enum.TryParse(typeof(Type), args[Array.IndexOf(args, "-type") + 1], out outputTypeObj);
            else
            {
                Console.Write("Output format (available values: mer, s): ");
                Enum.TryParse(typeof(Type), Console.ReadLine(), out outputTypeObj);
            }
            Type outputType = outputTypeObj == null ? Type.mer : (Type)outputTypeObj;

            foreach (var path in Directory.EnumerateFiles(input, "*", SearchOption.AllDirectories))
            {
                var texName = Path.GetFileNameWithoutExtension(path);
                var texTypeTxt = texName.Substring(texName.LastIndexOf("_") + 1);
                var texType = (Type)Enum.Parse(typeof(Type), texTypeTxt);
                texName = texName.Substring(0, texName.Length - texTypeTxt.Length - 1);
                var outputName = $"{outputDir}{texName}_{Enum.GetName(typeof(Type), outputType)}.png";
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
        }
    }
}
