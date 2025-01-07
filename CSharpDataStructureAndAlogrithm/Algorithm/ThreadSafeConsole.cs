using System.Numerics;
using System.Text;
using System.Threading;

namespace Algorithm;


/// <summary>
/// Thread safe operations for Console
/// </summary>
public static class ThreadSafeConsole
{
    private static readonly object _lock = new object();

    public static IEnumerable<string> Write(IEnumerable<string> messages)
    {
        lock (_lock)
        {
            foreach (string message in messages)
            {
                Console.Write(message);
                yield return message;
            }
        }
    }

    /// <summary>
    /// Console.WriteLine() and Console.Write() are thread safe, but they can still interleave output
    /// </summary>
    /// <param name="message"></param>
    public static string? Write<T>(T? message) where T : notnull
    {
        lock (_lock)
        {
            string? output = message?.ToString();
            Console.Write(output);
            return output;
        }

    }

    /// <summary>
    /// Console.WriteLine() and Console.Write() are thread safe, but they can still interleave output
    /// </summary>
    /// <param name="message"></param>
    public static string Write(string message)
    {
        lock (_lock)
        {
            Console.Write(message);
            return message;
        }
    }

    public static string WriteLine<T>(IEnumerable<T> messages, string delimiter = "")
    {
        lock (_lock)
        {
            return string.Format(string.Join(delimiter, messages));
        }
    }

    public static string? WriteLine<T>(T? message) where T : notnull
    {
        lock (_lock)
        {
            string? output = message?.ToString();
            Console.WriteLine(output);
            return output;
        }
    }

    /// <summary>
    /// Console.WriteLine() and Console.Write() are thread safe, but they can still interleave output
    /// </summary>
    /// <param name="message"></param>
    public static string WriteLine(string message)
    {
        lock (_lock)
        {
            Console.WriteLine(message);
            return message;
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


    public static Task WriteLineAsync(string message, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            lock (_lock)
            {
                Console.WriteLine(message);
            }
        }, cancellationToken);
    }

    public static Task WriteAsync(string message, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            lock (_lock)
            {
                Console.Write(message);
            }
        }, cancellationToken);
    }

    public static Task<string?> ReadLineAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            lock (_lock)
            {
                return Console.ReadLine();
            }
        }, cancellationToken);
    }

    public static Task<ConsoleKeyInfo> ReadKeyAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            lock (_lock)
            {
                return Console.ReadKey();
            }
        }, cancellationToken);
    }

    public static Task ClearAsync(CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            lock (_lock)
            {
                Console.Clear();
            }
        }, cancellationToken);
    }

    public static Task SetCursorPositionAsync(int left, int top, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            lock (_lock)
            {
                Console.SetCursorPosition(left, top);
            }
        }, cancellationToken);
    }

    public static Task WriteUnicodeAsync(string message, bool keep = false, CancellationToken cancellationToken = default)
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
        }, cancellationToken);
    }

    public static Task SetCursorPositionAsync(int left, int top, string message, CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            lock (_lock)
            {
                Console.SetCursorPosition(left, top);
                Console.Write(message);
            }
        }, cancellationToken);
    }

    public static Task SetCursorPositionAsync(int left, int top, string message, CancellationToken cancellationToken = default, params object[] args)
    {
        return Task.Run(() =>
        {
            lock (_lock)
            {
                Console.SetCursorPosition(left, top);
                Console.Write(message, args);
            }
        }, cancellationToken);
    }
}