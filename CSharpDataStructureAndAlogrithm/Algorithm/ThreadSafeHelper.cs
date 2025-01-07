using System.Collections.Concurrent;

namespace Algorithm;

/// <summary>
/// Performs thread safe operations on non-thread safe collections
/// </summary>
/// <typeparam name="T"></typeparam>
public static partial class ThreadSafeHelper<T>
{
    private static readonly Lazy<ConcurrentDictionary<IList<T>, (SemaphoreSlim Semaphore, int Count)>> _lazySemaphoreSlimDictionary = new();

    /// <summary>
    /// Adds an item to the list in a thread safe manner
    /// </summary>
    /// <param name="list"></param>
    /// <param name="item"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public static async Task<bool> TryAddAsync(IList<T> list, T item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(list);
        ConcurrentDictionary<IList<T>, (SemaphoreSlim Semaphore, int Count)>? dictionary = _lazySemaphoreSlimDictionary.Value;

        (SemaphoreSlim entrySemaphore, int count) = dictionary.AddOrUpdate(
            list,
            key => (new SemaphoreSlim(1, 1), 1),
            (key, oldValue) => (oldValue.Semaphore, oldValue.Count + 1)
        );

        try
        {
            await entrySemaphore.WaitAsync(cancellationToken);
            list.Add(item);
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

            dictionary.AddOrUpdate(
                list,
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

            if (dictionary.TryGetValue(list, out (SemaphoreSlim Semaphore, int Count) updatedEntry) && updatedEntry.Count == 0)
            {
                dictionary.TryRemove(list, out _);
            }
            else
            {
                dictionary.AddOrUpdate(
                    list,
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
    public static async Task<bool> TryInsertAsync(IList<T> list, int index, T item, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(list);
        ConcurrentDictionary<IList<T>, (SemaphoreSlim Semaphore, int Count)>? dictionary = _lazySemaphoreSlimDictionary.Value;

        (SemaphoreSlim entrySemaphore, int count) = dictionary.AddOrUpdate(
            list,
            key => (new SemaphoreSlim(1, 1), 1),
            (key, oldValue) => (oldValue.Semaphore, oldValue.Count + 1)
        );

        try
        {
            await entrySemaphore.WaitAsync(cancellationToken);
            list.Insert(index, item);
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

            dictionary.AddOrUpdate(
                list,
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

            if (dictionary.TryGetValue(list, out (SemaphoreSlim Semaphore, int Count) updatedEntry) && updatedEntry.Count == 0)
            {
                dictionary.TryRemove(list, out _);
            }
            else
            {
                dictionary.AddOrUpdate(
                    list,
                    key => (entrySemaphore, count),
                    (key, oldValue) => (oldValue.Semaphore, oldValue.Count - 1)
                );
            }
        }
    }
}