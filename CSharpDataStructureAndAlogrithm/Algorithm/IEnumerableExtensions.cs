using System.Numerics;

namespace Algorithm;

public static partial class IEnumerableExtensions
{
    public static IEnumerable<T> AsBubbleSortEnumerable<T>(
        this IEnumerable<T> enumerable, 
        Func<T,T,int>? comparer = null,
        Action<T,T>? onBeforeCompare = null,
        Action<T,T>? onCompare = null,
        Action<T,T>? onAfterCompare = null)
    {
        ArgumentNullException.ThrowIfNull(enumerable);
        IList<T> list = new List<T>(enumerable); //shallow copy
        int n = list.Count;
        comparer ??= Comparer<T>.Default.Compare;

        // Perform Bubble Sort
        for (int i = 0; i < n - 1; i++)
        {
            bool swapped = false;
            for (int j = 0; j < n - i - 1; j++)
            {
                onBeforeCompare?.Invoke(list[j], list[j + 1]);
                if (comparer(list[j], list[j + 1]) > 0)
                {
                    onCompare?.Invoke(list[j], list[j + 1]);
                    // Swap elements
                    (list[j], list[j + 1]) = (list[j + 1], list[j]);
                    swapped = true;
                }
                onAfterCompare?.Invoke(list[j], list[j + 1]);
            }

            // If no two elements were swapped in the inner loop, the list is sorted
            if (!swapped) break;
        }

        return list;
    }

    public static IEnumerable<T> AsBubbleSortEnumerable<T>(
        this IEnumerable<T> enumerable,
        Comparer<T>? comparer = null,
        Action<T, T>? onBeforeCompare = null,
        Action<T, T>? onCompare = null,
        Action<T, T>? onAfterCompare = null)
    {
        ArgumentNullException.ThrowIfNull(enumerable);
        IList<T> list = new List<T>(enumerable); //shallow copy
        int n = list.Count;
        comparer ??= Comparer<T>.Default;

        // Perform Bubble Sorts
        for (int i = 0; i < list.Count - 1; i++)
        {
            bool swapped = false;

            for (int j = 0; j < list.Count - i - 1; j++)
            {
                onBeforeCompare?.Invoke(list[j], list[j + 1]);
                if (comparer.Compare(list[j], list[j + 1]) > 0)
                {
                    onCompare?.Invoke(list[j], list[j + 1]);
                    // Swap elements
                    (list[j], list[j + 1]) = (list[j + 1], list[j]);
                    swapped = true;
                }
                onAfterCompare?.Invoke(list[j], list[j + 1]);
            }

            // If no two elements were swapped in the inner loop, the list is sorted
            if (!swapped) break;
        }
        return list;
    }

    public static IEnumerable<T> AsBubbleSortEnumerable<T>(
        this IEnumerable<T> enumerable,
        Action<T, T>? onBeforeCompare = null,
        Action<T, T>? onCompare = null,
        Action<T, T>? onAfterCompare = null) where T : IComparisonOperators<T, T, bool>
    {
        ArgumentNullException.ThrowIfNull(enumerable);
        IList<T> list = new List<T>(enumerable); //shallow copy
        int n = list.Count;

        for (int i = 0; i < list.Count - 1; i++)
        {
            bool swapped = false;
            for (int j = 0; j < list.Count - i - 1; j++)
            {
                onBeforeCompare?.Invoke(list[j], list[j + 1]);
                if (list[j] > list[j + 1])
                {
                    // Swap elements
                    onCompare?.Invoke(list[j], list[j + 1]);
                    (list[j], list[j + 1]) = (list[j + 1], list[j]);
                    swapped = true;
                }
                onAfterCompare?.Invoke(list[j], list[j + 1]);
            }

            // If no two elements were swapped in the inner loop, the list is sorted
            if (!swapped) break;
        }
        return list;
    }

    public static IEnumerable<IList<T>> AsBubbleSortingEnumerable<T>(
        this IEnumerable<T> enumerable,
        Func<T, T, int>? comparer = null,
        Action<T, T>? onBeforeCompare = null,
        Action<T, T>? onCompare = null,
        Action<T, T>? onAfterCompare = null)
    {
        ArgumentNullException.ThrowIfNull(enumerable);
        IList<T> list = new List<T>(enumerable); //shallow copy
        int n = list.Count;
        comparer ??= Comparer<T>.Default.Compare;

        // Perform Bubble Sort
        for (int i = 0; i < n - 1; i++)
        {
            bool swapped = false;
            for (int j = 0; j < n - i - 1; j++)
            {
                onBeforeCompare?.Invoke(list[j], list[j + 1]);
                if (comparer(list[j], list[j + 1]) > 0)
                {
                    onCompare?.Invoke(list[j], list[j + 1]);
                    // Swap elements
                    (list[j], list[j + 1]) = (list[j + 1], list[j]);
                    swapped = true;
                }
                onAfterCompare?.Invoke(list[j], list[j + 1]);
                yield return list;
            }

            // If no two elements were swapped in the inner loop, the list is sorted
            if (!swapped) break;
        }

        yield return list;
    }

    public static IEnumerable<IList<T>> AsBubbleSortingEnumerable<T>(
        this IEnumerable<T> enumerable,
        Comparer<T>? comparer = null,
        Action<T, T>? onBeforeCompare = null,
        Action<T, T>? onCompare = null,
        Action<T, T>? onAfterCompare = null)
    {
        ArgumentNullException.ThrowIfNull(enumerable);
        IList<T> list = new List<T>(enumerable); //shallow copy
        int n = list.Count;
        comparer ??= Comparer<T>.Default;

        // Perform Bubble Sorts
        for (int i = 0; i < list.Count - 1; i++)
        {
            bool swapped = false;

            for (int j = 0; j < list.Count - i - 1; j++)
            {
                onBeforeCompare?.Invoke(list[j], list[j + 1]);
                if (comparer.Compare(list[j], list[j + 1]) > 0)
                {
                    onCompare?.Invoke(list[j], list[j + 1]);
                    // Swap elements
                    (list[j], list[j + 1]) = (list[j + 1], list[j]);
                    swapped = true;
                }
                onAfterCompare?.Invoke(list[j], list[j + 1]);
                yield return list;
            }

            // If no two elements were swapped in the inner loop, the list is sorted
            if (!swapped) break;
        }
        yield return list;
    }

    public static IEnumerable<IList<T>> AsBubbleSortingEnumerable<T>(
        this IEnumerable<T> enumerable,
        Action<T, T>? onBeforeCompare = null,
        Action<T, T>? onCompare = null,
        Action<T, T>? onAfterCompare = null) where T : IComparisonOperators<T, T, bool>
    {
        ArgumentNullException.ThrowIfNull(enumerable);
        IList<T> list = new List<T>(enumerable); //shallow copy
        int n = list.Count;

        for (int i = 0; i < list.Count - 1; i++)
        {
            bool swapped = false;
            for (int j = 0; j < list.Count - i - 1; j++)
            {
                onBeforeCompare?.Invoke(list[j], list[j + 1]);
                if (list[j] > list[j + 1])
                {
                    // Swap elements
                    onCompare?.Invoke(list[j], list[j + 1]);
                    (list[j], list[j + 1]) = (list[j + 1], list[j]);
                    swapped = true;
                }
                onAfterCompare?.Invoke(list[j], list[j + 1]);
                yield return list;
            }

            // If no two elements were swapped in the inner loop, the list is sorted
            if (!swapped) break;
        }
        yield return list;
    }

    public static IEnumerable<IEnumerable<T>> Permutations<T>(this IEnumerable<T> enumerable, int? count = null)
    {
        if (count == 0) return [];

        if ((count ??= enumerable.Count()) == 1)
            return enumerable.Select(t => new T[] { t });

        return Permutations(enumerable, count - 1)
            .SelectMany(t => enumerable.Where(e => !t.Contains(e)),
                        (t1, t2) => t1.Append(t2));
    }

    public static IEnumerable<IEnumerable<T>> AsPermutationsEnumerable<T>(this IEnumerable<T> enumerable, int? count = null)
    {
        if (count == 0)
        {
            yield break;
        }

        if ((count ??= enumerable.Count()) == 1)
            yield return enumerable.SelectMany(t => new T[] { t });

        yield return AsPermutationsEnumerable(enumerable, count - 1)
            .SelectMany(t => enumerable.Where(e => !t.Contains(e)),
                        (t1, t2) => t1.Append(t2))
            .SelectMany(t => t);
    }

    /// <summary>
    /// Use memorization to store the result of permutations
    /// Need to use Least recently used (LRU) cache to avoid memory overflow
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerable"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static IEnumerable<IEnumerable<T>> MemorizedPermutations<T>(this IEnumerable<T> enumerable, int? count = 0)
    {
        if (count == 0) return [];

        IEnumerable<IEnumerable<T>> GetPermutationsInternal(IEnumerable<T> items, int? count)
        {
            string key = $"{string.Join(",", items)}-{count}";
            if (new Dictionary<string, IEnumerable<IEnumerable<T>>>().TryGetValue(key, out var cachedResult))
            {
                return cachedResult;
            }

            IEnumerable<IEnumerable<T>> result = [];
            if (count == 1)
            {
                result = items.Select(t => new T[] { t });
            }
            else
            {
                result = GetPermutationsInternal(items, count - 1)
                    .SelectMany(t => items.Where(e => !t.Contains(e)),
                                (t1, t2) => t1.Append(t2))
                    .ToList();
            }

            (new Dictionary<string, IEnumerable<IEnumerable<T>>>())[key] = result;
            return result;
        }

        return GetPermutationsInternal(enumerable, count ?? enumerable.Count());
    }
}