using System;
using System.Buffers;
using System.Collections.Generic;
using System.Text;

namespace StringGeneration
{
    ///<summary>
    ///<see href="https://learn.microsoft.com/en-us/aspnet/core/performance/objectpool?view=aspnetcore-9.0">
    ///<see href="https://medium.com/globant/object-pool-creational-design-pattern-in-c-185a1ae5e65e">
    ///<see href="https://learn.unity.com/tutorial/65df850fedbc2a082fb11029#">
    //<see href="https://learn.microsoft.com/en-us/dotnet/api/microsoft.extensions.dependencyinjection.entityframeworkservicecollectionextensions.adddbcontextpool?view=efcore-9.0">
    //<see href="https://learn.microsoft.com/en-us/dotnet/standard/threading/the-managed-thread-pool>
    //<see href="https://afana.me/archive/2023/06/19/array-pool-and-memory-pool/">
    ///<summary>
    public class CharacterPool 
    {
        private static readonly char[] AlphanumericChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
        private static readonly char[] AlphaChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        private static readonly char[] NumericChars = "0123456789".ToCharArray();
        private static readonly char[] SpecialChars = "!@#$%^&*()_+-=[]{}|;:,.<>?".ToCharArray();

        private readonly Random _random;
        private readonly ArrayPool<char> _arrayPool;

        public CharacterPool(int? seed = null)
        {
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
            _arrayPool = ArrayPool<char>.Shared;
        }

        // Generate string using Span<char>
        public string GenerateSpan(int length, bool includeSpecial = false, bool numbersOnly = false)
        {
            char[] rentedArray = _arrayPool.Rent(length);
            try
            {
                Span<char> span = rentedArray.AsSpan(0, length);
                FillSpan(span, includeSpecial, numbersOnly);
                return new string(span);
            }
            finally
            {
                _arrayPool.Return(rentedArray);
            }
        }

        // Generate string using Memory<char>
        public string GenerateMemory(int length, bool includeSpecial = false, bool numbersOnly = false)
        {
            char[] rentedArray = _arrayPool.Rent(length);
            try
            {
                Memory<char> memory = rentedArray.AsMemory(0, length);
                FillMemory(memory, includeSpecial, numbersOnly);
                return new string(memory.Span);
            }
            finally
            {
                _arrayPool.Return(rentedArray);
            }
        }

        // Generate multiple strings using Span<char>
        public IEnumerable<string> GenerateMultipleSpan(int count, int length, bool includeSpecial = false, bool numbersOnly = false)
        {
            var results = new List<string>(count);
            char[] rentedArray = _arrayPool.Rent(length);
            try
            {
                Span<char> span = rentedArray.AsSpan(0, length);
                for (int i = 0; i < count; i++)
                {
                    FillSpan(span, includeSpecial, numbersOnly);
                    results.Add(new string(span));
                }
                return results;
            }
            finally
            {
                _arrayPool.Return(rentedArray);
            }
        }

        // Generate a string builder with pooled characters
        public StringBuilder GenerateStringBuilder(int length, bool includeSpecial = false, bool numbersOnly = false)
        {
            var builder = new StringBuilder(length);
            char[] sourceChars = GetSourceChars(includeSpecial, numbersOnly);

            for (int i = 0; i < length; i++)
            {
                builder.Append(sourceChars[_random.Next(sourceChars.Length)]);
            }

            return builder;
        }

        // Helper method to fill Span<char>
        private void FillSpan(Span<char> span, bool includeSpecial, bool numbersOnly)
        {
            char[] sourceChars = GetSourceChars(includeSpecial, numbersOnly);

            for (int i = 0; i < span.Length; i++)
            {
                span[i] = sourceChars[_random.Next(sourceChars.Length)];
            }
        }

        // Helper method to fill Memory<char>
        private void FillMemory(Memory<char> memory, bool includeSpecial, bool numbersOnly)
        {
            FillSpan(memory.Span, includeSpecial, numbersOnly);
        }

        // Helper method to get source characters based on options
        private char[] GetSourceChars(bool includeSpecial, bool numbersOnly)
        {
            if (numbersOnly)
                return NumericChars;

            if (includeSpecial)
            {
                var combined = new char[AlphanumericChars.Length + SpecialChars.Length];
                Array.Copy(AlphanumericChars, 0, combined, 0, AlphanumericChars.Length);
                Array.Copy(SpecialChars, 0, combined, AlphanumericChars.Length, SpecialChars.Length);
                return combined;
            }

            return AlphanumericChars;
        }
    }

    // Example usage
    public class Program
    {
        public static void Main()
        {
            var pool = new CharacterPool();

            // Generate using Span
            string spanString = pool.GenerateSpan(10);
            Console.WriteLine($"Span Generated: {spanString}");

            // Generate using Memory
            string memoryString = pool.GenerateMemory(10);
            Console.WriteLine($"Memory Generated: {memoryString}");

            // Generate multiple strings
            var multipleStrings = pool.GenerateMultipleSpan(5, 8);
            Console.WriteLine("Multiple Strings Generated:");
            foreach (var str in multipleStrings)
            {
                Console.WriteLine(str);
            }

            // Generate with special characters
            string specialString = pool.GenerateSpan(12, includeSpecial: true);
            Console.WriteLine($"With Special Characters: {specialString}");

            // Generate numbers only
            string numbersString = pool.GenerateSpan(8, numbersOnly: true);
            Console.WriteLine($"Numbers Only: {numbersString}");

            // Generate using StringBuilder
            var stringBuilder = pool.GenerateStringBuilder(15);
            Console.WriteLine($"StringBuilder Generated: {stringBuilder}");
        }
    }
}
