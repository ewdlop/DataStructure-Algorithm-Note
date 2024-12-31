namespace Algorithm;

public static class GenericExtensions
{
    public static void Print<T>(this T obj)
    {
        Console.WriteLine(obj?.ToString()??string.Empty);
    }

    public static void ThreadSafePrint<T>(this T obj)
    {
        ThreadSafeConsole.WriteLine(obj?.ToString() ?? string.Empty);
    }
}

