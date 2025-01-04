﻿using System.Buffers;
using System.Numerics;

namespace Algorithm;

public static partial class ReadOnlyMemoryExtensions
{
    public const int MaxAnsiCode = 255;

    public static string ToSegments<T>(this ReadOnlyMemory<T> s, int lineLength)
    {
        int remainder = s.Length % lineLength;
        int times = (s.Length - remainder) / lineLength;
        return string.Join(Environment.NewLine, Enumerable.Range(0, times + 1).Select(i =>
        {
            string ce = s.Slice(i * lineLength, Math.Min(s.Length - i * lineLength, lineLength)).ToString();
            if (ce.Length < lineLength) ce = $"{ce}{new string(Enumerable.Repeat(' ', lineLength - ce.Length).ToArray())}";
            return new string(ce.Reverse().ToArray());
        }));
    }

    public static T[][] ToArrays<T>(this ReadOnlyMemory<T> s, int lineLength)
    {
        int remainder = s.Length % lineLength;
        int times = (s.Length - remainder) / lineLength;
        T[][] result = new T[times + 1][];
        for (int i = 0; i < times; i++)
        {
            result[i] = s.Slice(i * lineLength, lineLength).ToArray();
        }
        result[times] = s.Span.Slice(times * lineLength, remainder).ToArray();
        return result;
    }

    /// <summary>
    /// ANSI
    /// Google Search/ Stands for American National Standards Institute, and is a generic term for 8-bit character sets that include the ASCII character set plus additional characters 128–255. The additional characters vary by ANSI character set, and can include special characters, umlauts, Arabic, Greek, or Cyrillic characters.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public static bool ContainsUnicodeCharacter(this ReadOnlyMemory<char> s)
    {
        for (int i = 0; i < s.Length; i++)
        {
            if (s.Span[i] > MaxAnsiCode) return true;
        }
        return false;
    }


    //ascii?
    public static int DamerauLevenshteinDistance(this ReadOnlyMemory<char> s1, ReadOnlyMemory<char> s2)
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
                int cost = (s1.Span[i - 1] == s2.Span[j - 1]) ? 0 : 1;

                d[i, j] = Math.Min(Math.Min(d[i - 1, j] + 1, d[i, j - 1] + 1), d[i - 1, j - 1] + cost);

                if (i > 1 && j > 1 && s1.Span[i - 1] == s2.Span[j - 2] && s1.Span[i - 2] == s2.Span[j - 1])
                {
                    d[i, j] = Math.Min(d[i, j], d[i - 2, j - 2] + cost);
                }
            }
        }

        return d[len1, len2];
    }

    public static double JaccardSimilarity(this ReadOnlyMemory<char> s1, ReadOnlyMemory<char> s2)
    {
        HashSet<char> set1 = new HashSet<char>();
        foreach (char c in s1.Span)
        {
            set1.Add(c);
        }
        HashSet<char> set2 = new HashSet<char>();
        foreach (char c in s2.Span)
        {
            set2.Add(c);
        }

        HashSet<char>? intersection = new HashSet<char>(set1);
        intersection.IntersectWith(set2);

        HashSet<char> union = new HashSet<char>(set1);
        union.UnionWith(set2);

        return (double)intersection.Count / union.Count;
    }

    /// <summary>
    /// Matthew A. Jaro. 1989. "Advances in record linkage methodology as applied to the 1985 census of Tampa Florida". Journal of the American Statistical Association. 84 (406): 414–420.
    /// </summary>
    /// <param name="s1"></param>
    /// <param name="s2"></param>
    /// <returns></returns>
    public static double JaroDistance(this ReadOnlyMemory<char> s1, ReadOnlyMemory<char> s2)
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
                if (s1.Span[i] != s2.Span[j]) continue;
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
            if (s1.Span[i] != s2.Span[k]) transpositions++;
            k++;
        }

        return ((matches / (double)len1) + (matches / (double)len2) + ((matches - transpositions / 2.0) / matches)) / 3.0;
    }

    /// <summary>
    /// Matthew A. Jaro and Paul Winkler. 1999. "A String Comparator". In Proceedings of the Third International Conference on Knowledge Discovery and Data Mining (KDD'99). AAAI Press, 39-48.
    /// </summary>
    /// <param name="s1"></param>
    /// <param name="s2"></param>
    /// <returns></returns>
    public static double JaroWinklerDistance(this ReadOnlyMemory<char> s1, ReadOnlyMemory<char> s2)
    {
        double jaroDistance = JaroDistance(s1, s2);

        int prefixLength = 0;
        for (int i = 0; i < Math.Min(s1.Length, s2.Length); i++)
        {
            if (s1.Span[i] == s2.Span[i])
                prefixLength++;
            else
                break;
        }

        prefixLength = Math.Min(4, prefixLength);

        return jaroDistance + (prefixLength * 0.1 * (1 - jaroDistance));
    }

    public static Span<T> Factorial<T>(this ReadOnlyMemory<T> s1, Keep decrement = Keep.First)
    {
        if (s1.IsEmpty) return [];
        int totalLength = s1.Length * (1 + s1.Length) / 2;
        MemoryPool<T> memoryPool = MemoryPool<T>.Shared;
        IMemoryOwner<T> memoryOwner = memoryPool.Rent(totalLength);
        int subStringLength = s1.Length;
        try
        {
            if (decrement == Keep.First)
            {
                int j = 0;
                for (int i = 0; i < totalLength; i++)
                {
                    memoryOwner.Memory.Span[i] = s1.Span[j];
                    if (j == subStringLength - 1)
                    {
                        subStringLength--;
                        j = 0;
                    }
                    else
                    {
                        j++;
                    }
                }
            }
            else if (decrement == Keep.Last)
            {
                // abc -> abcbcc
                int j = 0;
                int k = 0;
                for (int i = 0; i < totalLength; i++)
                {
                    memoryOwner.Memory.Span[i] = s1.Span[j];
                    if (j == subStringLength - 1)
                    {
                        // abc -> abcbcc
                        k++;
                        j = k;
                    }
                    else
                    {
                        j++;
                    }
                }
            }
            return memoryOwner.Memory.Span[..totalLength];
        }
        finally
        {
            memoryOwner.Dispose();
        }
    }

    public static BigInteger CalculateAlphabetPositionFactorial(this ReadOnlyMemory<char> word)
    {
        BigInteger result = 1;
        foreach (char c in word.Span)
        {
            int position = c - 'A' + 1;
            BigInteger factorial = position.Factorial();
            string value = $"{c} = {position} → {position}! = {factorial}";
            Console.WriteLine(value);
            result *= factorial;
        }
        Console.WriteLine($"Final result: {result}");
        return result;
    }

    /// <summary>
    /// Number of unique arrangements of a word with repeated letters (e.g. MISSISSIPPI)
    /// </summary>
    /// <param name="word"></param>
    /// <returns></returns>
    public static BigInteger CalculatePermutationalFactorial(this string word)
    {
        int length = word.Length;
        Dictionary<char, int> letterCounts =
            word.GroupBy(c => c)
                .ToDictionary(g => g.Key, g => g.Count());

        BigInteger denominator = 1;
        foreach (var count in letterCounts.Values)
        {
            if (count > 1)
            {
                denominator *= count.Factorial();
            }
        }

        BigInteger result = length.Factorial() / denominator;
        Console.WriteLine($"Number of unique arrangements: {result}");

        if (denominator > 1)
        {
            Console.WriteLine("(Adjusted for repeated letters)");
            string value = $"Calculation: {length}! / ({string.Join(" × ", letterCounts.Where(kv => kv.Value > 1).Select(kv => $"{kv.Value}!"))})";
            Console.WriteLine(value);
        }
        return result;
    }

    public static BigInteger CalculateWordLengthFactorial(this string word)
    {
        BigInteger result = word.Length.Factorial();
        string value = $"Length = {word.Length} → {word.Length}! = {result}";
        Console.WriteLine(value);
        return result;
    }

    public static BigInteger CalculateASCIIFactorial(this string word)
    {
        BigInteger result = 1;
        foreach (char c in word)
        {
            int asciiValue = (int)c;
            Console.WriteLine($"{c} = {asciiValue} → {asciiValue}!");
            result *= asciiValue.Factorial();
        }
        Console.WriteLine("(Result would be the product of these extremely large factorials)");
        Console.WriteLine("Note: Actual computation of such large factorials may exceed available memory");
        return result;
    }
}