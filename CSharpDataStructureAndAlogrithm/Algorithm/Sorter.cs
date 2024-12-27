using System.Numerics;

namespace Algorithm;

public static class Sorter
{
    public static void BubbleSort<T>(this IList<T>? list,
        Func<T, T, int>? comparer = null,
        Action<T, T>? onBeforeCompare = null,
        Action<T, T>? onCompare = null,
        Action<T, T>? onAfterCompare = null)
    {
        ArgumentNullException.ThrowIfNull(list);
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
    }

    public static void BubbleSort<T>(this IList<T>? list,
        Comparer<T>? comparer = null,
        Action<T, T>? onBeforeCompare = null,
        Action<T, T>? onCompare = null,
        Action<T, T>? onAfterCompare = null)
    {
        ArgumentNullException.ThrowIfNull(list);
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
    }

    public static void BubbleSort<T>(this IList<T>? list,
        Action<T, T>? onBeforeCompare = null,
        Action<T, T>? onCompare = null,
        Action<T, T>? onAfterCompare = null) where T : IComparisonOperators<T, T, bool>
    {
        ArgumentNullException.ThrowIfNull(list);
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
    }
}
