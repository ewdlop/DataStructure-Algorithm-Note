using System.Text;

namespace Algorithm;

public static class ThreadSafeConsole
{
    private static readonly object _lock = new object();

    public static void WriteLine(string message)
    {
        lock (_lock)
        {
            Console.WriteLine(message);
        }
    }
    public static void Write(string message)
    {
        lock (_lock)
        {
            Console.Write(message);
        }
    }
    public static string? ReadLine()
    {
        lock (_lock)
        {
            return Console.ReadLine();
        }
    }
    public static ConsoleKeyInfo ReadKey()
    {
        lock (_lock)
        {
            return Console.ReadKey();
        }
    }

    public static void Clear()
    {
        lock (_lock)
        {
            Console.Clear();
        }
    }

    public static void SetCursorPosition(int left, int top)
    {
        lock (_lock)
        {
            Console.SetCursorPosition(left, top);
        }
    }
    
    public static void WriteUnicode(string message, bool keep = false)
    {
        lock (_lock)
        {
            Encoding previousEncoding = Console.OutputEncoding;
            Console.OutputEncoding = Encoding.Unicode;
            Console.Write(message);
            if (!keep) Console.OutputEncoding = previousEncoding;
        }
    }

    public static void SetCursorPosition(int left, int top, string message)
    {
        lock (_lock)
        {
            Console.SetCursorPosition(left, top);
            Console.Write(message);
        }
    }

    public static void SetCursorPosition(int left, int top, string message, params object[] args)
    {
        lock (_lock)
        {
            Console.SetCursorPosition(left, top);
            Console.Write(message, args);
        }
    }


    public static Task WriteLineAsync(string message)
    {
        return Task.Run(() =>
        {
            lock (_lock)
            {
                Console.WriteLine(message);
            }
        });
    }

    public static Task WriteAsync(string message)
    {
        return Task.Run(() =>
        {
            lock (_lock)
            {
                Console.Write(message);
            }
        });
    }

    public static Task<string?> ReadLineAsync()
    {
        return Task.Run(() =>
        {
            lock (_lock)
            {
                return Console.ReadLine();
            }
        });
    }

    public static Task<ConsoleKeyInfo> ReadKeyAsync()
    {
        return Task.Run(() =>
        {
            lock (_lock)
            {
                return Console.ReadKey();
            }
        });
    }

    public static Task ClearAsync()
    {
        return Task.Run(() =>
        {
            lock (_lock)
            {
                Console.Clear();
            }
        });
    }

    public static Task SetCursorPositionAsync(int left, int top)
    {
        return Task.Run(() =>
        {
            lock (_lock)
            {
                Console.SetCursorPosition(left, top);
            }
        });
    }

    public static Task WriteUnicodeAsync(string message, bool keep = false)
    {
        return Task.Run(() =>
        {
            lock (_lock)
            {
                Encoding previousEncoding = Console.OutputEncoding;
                Console.OutputEncoding = Encoding.Unicode;
                Console.Write(message);
                if (!keep) Console.OutputEncoding = previousEncoding;
            }
        });
    }

    public static Task SetCursorPositionAsync(int left, int top, string message)
    {
        return Task.Run(() =>
        {
            lock (_lock)
            {
                Console.SetCursorPosition(left, top);
                Console.Write(message);
            }
        });
    }

    public static Task SetCursorPositionAsync(int left, int top, string message, params object[] args)
    {
        return Task.Run(() =>
        {
            lock (_lock)
            {
                Console.SetCursorPosition(left, top);
                Console.Write(message, args);
            }
        });
    }
}

