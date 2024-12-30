using System.Text;

namespace Algorithm;

public static class StringExtensions
{
    public const int MaxAnsiCode = 255;

    public static string ToSegmentString(this string s, int lineLength)
    {
        int remainder = s.Length % lineLength;
        int times = (s.Length - remainder) / lineLength;
        return string.Join(Environment.NewLine, Enumerable.Range(0, times + 1).Select(i =>
        {
            string ce = s.Substring(i * lineLength, Math.Min(s.Length - i * lineLength, lineLength));
            if (ce.Length < lineLength) ce = $"{ce}{new string(Enumerable.Repeat(' ', lineLength - ce.Length).ToArray())}";
            return new string(ce.Reverse().ToArray());
        }));
    }

    public static char[][] ToCharArray(this string s, int lineLength)
    {
        int remainder = s.Length % lineLength;
        int times = (s.Length - remainder) / lineLength;
        char[][] result = new char[times + 1][];
        for (int i = 0; i < times; i++)
        {
            result[i] = [.. s.Substring(i * lineLength, lineLength)];
        }
        result[times] = [.. s[^remainder..]];
        return result;
    }

    /// <summary>
    /// ANSI
    /// Google Search/ Stands for American National Standards Institute, and is a generic term for 8-bit character sets that include the ASCII character set plus additional characters 128–255. The additional characters vary by ANSI character set, and can include special characters, umlauts, Arabic, Greek, or Cyrillic characters.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ContainsUnicodeCharacter(this string input)
    {
        return input.Any(c => c > MaxAnsiCode);
    }
}
