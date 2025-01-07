namespace Algorithm;

public static partial class KMPAlgorithm
{
    // Compute the Longest Proper Prefix which is also Suffix (LPS) array
    public static int[] ComputeLPSArray(this string pattern)
    {
        int[] lps = new int[pattern.Length];

        // Length of the previous longest prefix & suffix
        int len = 0;
        int i = 1;
        lps[0] = 0; // First element is always 0

        // Calculate lps[i] for i = 1 to pattern.Length-1
        while (i < pattern.Length)
        {
            if (pattern[i] == pattern[len])
            {
                len++;
                lps[i] = len;
                i++;
            }
            else
            {
                if (len != 0)
                {
                    // Try to find a shorter prefix which is also a suffix
                    len = lps[len - 1];
                }
                else
                {
                    // No matching prefix found
                    lps[i] = 0;
                    i++;
                }
            }
        }
        return lps;
    }
    public static int[] ComputeLPSArray(this ReadOnlyMemory<char> pattern)
    {
        int[] lps = new int[pattern.Length];

        // Length of the previous longest prefix & suffix
        int len = 0;
        int i = 1;
        lps[0] = 0; // First element is always 0

        // Calculate lps[i] for i = 1 to pattern.Length-1
        while (i < pattern.Length)
        {
            if (pattern.Span[i] == pattern.Span[len])
            {
                len++;
                lps[i] = len;
                i++;
            }
            else
            {
                if (len != 0)
                {
                    // Try to find a shorter prefix which is also a suffix
                    len = lps[len - 1];
                }
                else
                {
                    // No matching prefix found
                    lps[i] = 0;
                    i++;
                }
            }
        }
        return lps;
    }

    // Find all occurrences of pattern in text
    public static List<int> Search(this string text, string pattern)
    {
        if(pattern is not string)
            throw new ArgumentException("T must be a string");
        
        List<int> matches = new List<int>();

        if (string.IsNullOrEmpty(text) || string.IsNullOrEmpty(pattern))
            return matches;

        if (pattern.Length > text.Length)
            return matches;

        // Create the LPS array that will hold the longest prefix suffix values for pattern
        int[] lps = ComputeLPSArray(pattern);

        int i = 0; // Index for text
        int j = 0; // Index for pattern

        while (i < text.Length)
        {
            if (pattern[j] == text[i])
            {
                i++;
                j++;
            }

            if (j == pattern.Length)
            {
                // Pattern found at index i-j
                matches.Add(i - j);
                // Use lps array to find the next matching position
                j = lps[j - 1];
            }
            else if (i < text.Length && pattern[j] != text[i])
            {
                if (j != 0)
                {
                    j = lps[j - 1];
                }
                else
                {
                    i++;
                }
            }
        }

        return matches;
    }

    public static List<int> Search(this ReadOnlyMemory<char> text, string pattern)
    {
        if (pattern is not string)
            throw new ArgumentException("T must be a string");

        List<int> matches = [];

        if (text.Span.IsWhiteSpace() || text.Span.IsEmpty || string.IsNullOrEmpty(pattern))
            return matches;

        if (pattern.Length > text.Length)
            return matches;

        // Create the LPS array that will hold the longest prefix suffix values for pattern
        int[] lps = ComputeLPSArray(pattern);

        int i = 0; // Index for text
        int j = 0; // Index for pattern

        while (i < text.Length)
        {
            if (pattern[j] == text.Span[i])
            {
                i++;
                j++;
            }

            if (j == pattern.Length)
            {
                // Pattern found at index i-j
                matches.Add(i - j);
                // Use lps array to find the next matching position
                j = lps[j - 1];
            }
            else if (i < text.Length && pattern[j] != text.Span[i])
            {
                if (j != 0)
                {
                    j = lps[j - 1];
                }
                else
                {
                    i++;
                }
            }
        }

        return matches;
    }
}
