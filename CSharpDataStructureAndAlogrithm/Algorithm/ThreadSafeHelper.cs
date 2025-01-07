using System.Collections.Concurrent;

namespace Algorithm;

/// <summary>
/// Performs thread safe operations on non-thread safe collections
/// </summary>
/// <typeparam name="T"></typeparam>
public static partial class ThreadSafeHelper<T>
{
    private static readonly Lazy<ConcurrentDictionary<IEnumerable<T>, (SemaphoreSlim Semaphore, int Count)>> _lazySemaphoreSlimDictionary = new();
    private static ConcurrentDictionary<IEnumerable<T>, (SemaphoreSlim Semaphore, int Count)> SemaphoreSlimDictionary => _lazySemaphoreSlimDictionary.Value;
    /// <summary>
    /// Adds an item to the list in a thread safe manner
    /// </summary>
    /// <param name="list"></param>
    /// <param name="item"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<bool> TryAddAsync(IEnumerable<T> enumerable, T item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(enumerable);

        (SemaphoreSlim entrySemaphore, int count) = SemaphoreSlimDictionary.AddOrUpdate(
            enumerable,
            key => (new SemaphoreSlim(1, 1), 1),
            (key, oldValue) => (oldValue.Semaphore, oldValue.Count + 1)
        );

        try
        {
            await entrySemaphore.WaitAsync(cancellationToken);
            if(enumerable is IList<T> list)
            {
                list.Add(item);
                return true;
            }
            return true;
        }

        catch (Exception ex)
        {
            if (ex is NotSupportedException || ex is ArgumentNullException || ex is TaskCanceledException)
            {
                return false;
            }
            else
            {
                throw;
            }
        }
        finally
        {
            entrySemaphore.Release();

            SemaphoreSlimDictionary.AddOrUpdate(
                enumerable,
                key => (entrySemaphore, count),
                (key,oldValue) =>
                {
                    if (oldValue.Count == 1)
                    {
                        oldValue.Semaphore.Dispose();
                        return (oldValue.Semaphore, 0);
                    }
                    return (oldValue.Semaphore, oldValue.Count - 1);
                }
            );

            if (SemaphoreSlimDictionary.TryGetValue(enumerable, out (SemaphoreSlim Semaphore, int Count) updatedEntry) && updatedEntry.Count == 0)
            {
                SemaphoreSlimDictionary.TryRemove(enumerable, out _);
            }
            else
            {
                SemaphoreSlimDictionary.AddOrUpdate(
                    enumerable,
                    key => (entrySemaphore, count),
                    (key, oldValue) => (oldValue.Semaphore, oldValue.Count - 1)
                );
            }
        }
    }

    /// <summary>
    /// Inserts an item to the list in a thread safe manner
    /// </summary>
    /// <param name="list"></param>
    /// <param name="index"></param>
    /// <param name="item"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<bool> TryInsertAsync(IEnumerable<T> enumerable, int index, T item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(enumerable);

        (SemaphoreSlim entrySemaphore, int count) = SemaphoreSlimDictionary.AddOrUpdate(
            enumerable,
            key => (new SemaphoreSlim(1, 1), 1),
            (key, oldValue) => (oldValue.Semaphore, oldValue.Count + 1)
        );

        try
        {
            await entrySemaphore.WaitAsync(cancellationToken);
            if(enumerable is IList<T> list)
            {
                list.Insert(index, item);
                return true;
            }
            return true;
        }
        catch (Exception ex)
        {
            if (ex is NotSupportedException || ex is ArgumentNullException || ex is TaskCanceledException)
            {
                return false;
            }
            else
            {
                throw;
            }
        }
        finally
        {
            entrySemaphore.Release();

            SemaphoreSlimDictionary.AddOrUpdate(
                enumerable,
                key => (entrySemaphore, count),
                (key, oldValue) =>
                {
                    if (oldValue.Count == 1)
                    {
                        oldValue.Semaphore.Dispose();
                        return (oldValue.Semaphore, 0);
                    }
                    return (oldValue.Semaphore, oldValue.Count - 1);
                }
            );

            if (SemaphoreSlimDictionary.TryGetValue(enumerable, out (SemaphoreSlim Semaphore, int Count) updatedEntry) && updatedEntry.Count == 0)
            {
                SemaphoreSlimDictionary.TryRemove(enumerable, out _);
            }
            else
            {
                SemaphoreSlimDictionary.AddOrUpdate(
                    enumerable,
                    key => (entrySemaphore, count),
                    (key, oldValue) => (oldValue.Semaphore, oldValue.Count - 1)
                );
            }
        }
    }
}