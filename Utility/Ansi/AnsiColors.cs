using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GamedayTracker.Utility.Ansi
{
    public class AnsiColors
    {
        private static readonly Dictionary<string, string> _colors =
        new(StringComparer.OrdinalIgnoreCase)
        {
            // Basic colors
            ["black"] = "\x1b[30m",
            ["red"] = "\x1b[31m",
            ["green"] = "\x1b[32m",
            ["yellow"] = "\x1b[33m",
            ["blue"] = "\x1b[34m",
            ["magenta"] = "\x1b[35m",
            ["cyan"] = "\x1b[36m",
            ["white"] = "\x1b[37m",

            // Bright colors
            ["brightblack"] = "\x1b[90m",
            ["brightred"] = "\x1b[91m",
            ["brightgreen"] = "\x1b[92m",
            ["brightyellow"] = "\x1b[93m",
            ["brightblue"] = "\x1b[94m",
            ["brightmagenta"] = "\x1b[95m",
            ["brightcyan"] = "\x1b[96m",
            ["brightwhite"] = "\x1b[97m",

            // Orange variants (256-color)
            ["orange"] = "\x1b[38;5;208m",
            ["darkorange"] = "\x1b[38;5;202m",
            ["lightorange"] = "\x1b[38;5;214m",

            // TrueColor (RGB) orange
            ["rgborange"] = "\x1b[38;2;255;165;0m",

            // Reset
            ["reset"] = "\x1b[0m"
        };

        public static string GetAnsiCode(string colorName)
        {
            if (string.IsNullOrWhiteSpace(colorName))
                return string.Empty;

            return _colors.TryGetValue(colorName, out var code)
                ? code
                : string.Empty; // or return null
        }
    }
}
