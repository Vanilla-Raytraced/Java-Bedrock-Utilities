using System.Drawing;

namespace JavaBedrockUtilities
{
    public enum Type
    {
        mer, // MER
        s // Specular
    }
    public class Data
    {
        public byte Metalness;
        public byte Emissive;
        public byte Roughness;

        public static Data Parse(Color c, Type type)
        {
            if (type == Type.mer) return ParseMER(c);
            else if (type == Type.s) return ParseSpecular(c);
            else return new Data();
        }
        public static Color ToColor(Data d, Type type)
        {
            if (type == Type.mer) return ToMER(d);
            else if (type == Type.s) return ToSpecular(d);
            else return Color.Transparent;
        }

        public static Data ParseMER(Color c) => new Data { Metalness = c.R, Emissive = c.G, Roughness = c.B };
        public static Color ToMER(Data d) => Color.FromArgb(d.Metalness, d.Emissive, d.Roughness);

        public static Data ParseSpecular(Color c) => new Data { Roughness = (byte)(255 - c.R), Metalness = c.G, Emissive = c.B };
        public static Color ToSpecular(Data d) => Color.FromArgb((byte)(255 - d.Roughness), d.Metalness, d.Emissive);
    }
}
