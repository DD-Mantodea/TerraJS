using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace TerraJS.Contents.Utils
{
    public class ColorUtils
    {
        public static Color FromHex(int hex)
        {
            if (hex < 0 || hex > 0xFFFFFF)
                return Color.Transparent;

            byte r = (byte)((hex >> 16) & 0xFF);

            byte g = (byte)((hex >> 8) & 0xFF);

            byte b = (byte)(hex & 0xFF);

            return new(r, g, b);
        }

        public static Color FromHexString(string hex)
        {
            if (string.IsNullOrEmpty(hex))
                return Color.Transparent;

            if (hex.Length != 6)
                return Color.Transparent;

            if (!int.TryParse(hex[0..2], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int r))
                return Color.Transparent;

            if (!int.TryParse(hex[2..4], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int g))
                return Color.Transparent;

            if (!int.TryParse(hex[4..6], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out int b))
                return Color.Transparent;

            return new(r, g, b);
        }
    }
}
