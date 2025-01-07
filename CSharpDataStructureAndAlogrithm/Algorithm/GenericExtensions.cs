namespace Algorithm;

public static partial class GenericExtensions
{
    public enum PrintType
    {
        Console,
        ThreadSafeConsole
    }

    public enum WriteType
    {
        Write,
        WriteLine
    }

    public static IEnumerable<string?> Print<T>(this T obj, string delimiter = "", PrintType printType = PrintType.Console, WriteType writeType = WriteType.WriteLine)
    {
        if (printType == PrintType.Console)
        {
            string output = obj?.ToString() ?? string.Empty;
            switch (writeType)
            {
                case WriteType.Write:
                    Console.Write(output);
                    break;
                case WriteType.WriteLine:
                    Console.WriteLine(output);
                    break;
            }
            yield return output;
        }
        else
        {
            yield return (obj, printType, writeType) switch
            {
                (IEnumerable<object> enumerable, PrintType.ThreadSafeConsole, WriteType.Write) => ThreadSafeConsole.WriteLine(enumerable, delimiter),
                (IEnumerable<object> enumerable, PrintType.ThreadSafeConsole, WriteType.WriteLine) => ThreadSafeConsole.WriteLine(enumerable, delimiter),
                (object x, PrintType.ThreadSafeConsole, WriteType.Write) => ThreadSafeConsole.Write(x),
                (object x, PrintType.ThreadSafeConsole, WriteType.WriteLine) => ThreadSafeConsole.WriteLine(x),
                _ => string.Empty
            };
        }
    }

    public static void ThreadSafePrint<T>(this T obj)
    {
        ThreadSafeConsole.WriteLine(obj?.ToString() ?? string.Empty);
    }

    //public static void Print<T1,T2>(this T1 generic)
    //{
    //    switch (generic)
    //    {
    //        case KeyValuePair<T1, T2> pair:
    //            Console.WriteLine(pair.Key);
    //            Console.WriteLine(pair.Value);
    //            break;
    //        case IEnumerable<T2> enumerable:
    //            foreach (var item in enumerable)
    //            {
    //                Console.WriteLine(item);
    //            }
    //            break;
    //        default:
    //            Console.WriteLine(generic);
    //            break;
    //    }
    //}
}

