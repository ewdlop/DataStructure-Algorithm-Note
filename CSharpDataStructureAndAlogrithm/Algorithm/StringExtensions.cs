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

    public static int DamerauLevenshteinDistance(this string s1, string s2)
    {
        int len1 = s1.Length;
        int len2 = s2.Length;
        int[,] d = new int[len1 + 1, len2 + 1];

        for (int i = 0; i <= len1; i++) d[i, 0] = i;
        for (int j = 0; j <= len2; j++) d[0, j] = j;

        for (int i = 1; i <= len1; i++)
        {
            for (int j = 1; j <= len2; j++)
            {
                int cost = (s1[i - 1] == s2[j - 1]) ? 0 : 1;

                d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);

                if (i > 1 && j > 1 && s1[i - 1] == s2[j - 2] && s1[i - 2] == s2[j - 1])
                {
                    d[i, j] = Math.Min(d[i, j], d[i - 2, j - 2] + cost);
                }
            }
        }

        return d[len1, len2];
    }

    public static double JaccardSimilarity(this string s1, string s2)
    {
        HashSet<char> set1 = new HashSet<char>(s1);
        HashSet<char> set2 = new HashSet<char>(s2);

        HashSet<char>? intersection = new HashSet<char>(set1);
        intersection.IntersectWith(set2);

        HashSet<char> union = new HashSet<char>(set1);
        union.UnionWith(set2);

        return (double)intersection.Count / union.Count;
    }


    public static double JaroDistance(this string s1, string s2)
    {
        int len1 = s1.Length;
        int len2 = s2.Length;

        if (len1 == 0) return len2 == 0 ? 1.0 : 0.0;

        int matchDistance = Math.Max(len1, len2) / 2 - 1;

        bool[] s1Matches = new bool[len1];
        bool[] s2Matches = new bool[len2];

        int matches = 0;
        int transpositions = 0;

        for (int i = 0; i < len1; i++)
        {
            int start = Math.Max(0, i - matchDistance);
            int end = Math.Min(i + matchDistance + 1, len2);

            for (int j = start; j < end; j++)
            {
                if (s2Matches[j]) continue;
                if (s1[i] != s2[j]) continue;
                s1Matches[i] = true;
                s2Matches[j] = true;
                matches++;
                break;
            }
        }

        if (matches == 0) return 0.0;

        int k = 0;
        for (int i = 0; i < len1; i++)
        {
            if (!s1Matches[i]) continue;
            while (!s2Matches[k]) k++;
            if (s1[i] != s2[k]) transpositions++;
            k++;
        }

        return ((matches / (double)len1) + (matches / (double)len2) + ((matches - transpositions / 2.0) / matches)) / 3.0;
    }

    public static double JaroWinklerDistance(this string s1, string s2)
    {
        double jaroDist = JaroDistance(s1, s2);

        int prefixLength = 0;
        for (int i = 0; i < Math.Min(s1.Length, s2.Length); i++)
        {
            if (s1[i] == s2[i])
                prefixLength++;
            else
                break;
        }

        prefixLength = Math.Min(4, prefixLength);

        return jaroDist + (prefixLength * 0.1 * (1 - jaroDist));
    }
}