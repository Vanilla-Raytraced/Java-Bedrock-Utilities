using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Threading.Tasks;

namespace JavaBedrockUtilities
{
    public static class Processing
    {
        public static void ProcessFiles()
        {
            var tasks = new List<Task>();
            foreach (var path in Directory.EnumerateFiles(Program.inputPath, "*.png", SearchOption.AllDirectories))
                tasks.Add(Task.Factory.StartNew(() =>
                {
                    try { ProcessFile(path); }
                    catch (NotSupportedException e) { Logging.Log(e.Message, LogLevel.Warning); }
                    catch (Exception e) { Logging.Log(e, LogLevel.Error); }
                }));
            Task.WaitAll(tasks.ToArray());
        }

        public static void ProcessFile(string path)
        {
            Logging.Log($"Processing {path}", LogLevel.Debug);
            var texPath = Path.GetRelativePath(Program.inputPath, Path.GetDirectoryName(path));
            var texName = Path.GetFileNameWithoutExtension(path);
            var texType = Program.inputType;
            if (Program.inputType == (Type)(-1))
            {
                var texTypeTxt = texName.Substring(texName.LastIndexOf("_") + 1);
                if (Enum.TryParse(typeof(Type), texTypeTxt, out var texTypeObj)) texType = (Type)texTypeObj;
                else throw new NotSupportedException($"Format \"{texTypeTxt}\" isn't supported: {path}");
                texName = texName.Substring(0, texName.Length - texTypeTxt.Length - 1);
            }
            var outputName = $"{Program.outputPath}{texPath}/{texName}_{Enum.GetName(typeof(Type), Program.outputType)}.png";
            if (!Directory.Exists(Program.outputPath + texPath)) Directory.CreateDirectory(Program.outputPath + texPath);

            Logging.Log($"Converting {Path.GetRelativePath(Program.inputPath, path)} from {texType} to {Program.outputType}", LogLevel.Debug);
            using (Bitmap source = new Bitmap(path))
            {
                var dest = new Bitmap(source.Width, source.Height);

                for (int x = 0; x < source.Width; x++)
                    for (int y = 0; y < source.Height; y++)
                        dest.SetPixel(x, y, Data.ToColor(Data.Parse(source.GetPixel(x, y), texType), Program.outputType));
                dest.Save(outputName, ImageFormat.Png);
            }

            Logging.Log($"Saved at {Path.GetFullPath(outputName)}", LogLevel.Success);
        }
    }
}
