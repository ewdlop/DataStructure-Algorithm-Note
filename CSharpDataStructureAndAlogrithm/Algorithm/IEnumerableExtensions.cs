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
        List<T> list = new List<T>(enumerable); //shallow copy
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
        List<T> list = new List<T>(enumerable); //shallow copy
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
        List<T> list = new List<T>(enumerable); //shallow copy
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
        List<T> list = new List<T>(enumerable); //shallow copy
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
        List<T> list = new List<T>(enumerable); //shallow copy
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
}