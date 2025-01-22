using System.Numerics;

namespace Algorithm;

public static partial class IListExtensions
{
    public static void Swap<T>(this IList<T> list, int index1, int index2)
    {
        ArgumentNullException.ThrowIfNull(list);
        //check if the indexes are valid
        if (index1 < 0 || index1 >= list.Count || index2 < 0 || index2 >= list.Count)
        {
            throw new IndexOutOfRangeException("The indexes are out of range.");
        }
        (list[index1], list[index2]) = (list[index2], list[index1]);
    }

    public static IEnumerable<T> AsReverse<T>(this IList<T> list)
    {
        ArgumentNullException.ThrowIfNull(list);
        for (int i = list.Count; i >= 0; i--)
        {
            yield return list[i];
        }
    }

    public static IList<T> ToReverse<T>(this IList<T> list)
    {
        ArgumentNullException.ThrowIfNull(list);
        return list.AsReverse().ToList();
    }

    public static (T min, T max) MinMax<T>(this IList<T> list) where T : IComparisonOperators<T,T,bool>
    {
        ArgumentNullException.ThrowIfNull(list);

        if (list.Count == 0)
        {
            throw new InvalidOperationException("The list is empty.");
        }
        T min = list[0];
        T max = list[0];
        for (int i = 1; i < list.Count; i++)
        {
            if (list[i] < min)
            {
                min = list[i];
            }
            if (list[i] > max)
            {
                max = list[i];
            }
        }
        return (min, max);
    }

    public static List<T> QuickSort<T>(this IList<T> list) where T : IComparable<T>
    {
        ArgumentNullException.ThrowIfNull(list);

        // Base case: list of length 0 or 1 is already sorted
        if (list.Count <= 1)
            return [.. list];

        // Choosing pivot (first element in this case)
        T pivot = list[0];

        // Partitioning into two sub-lists
        IEnumerable<T> lessThanPivot = list.Where(x => x.CompareTo(pivot) < 0);
        IEnumerable<T> greaterThanPivot = list.Where(x => x.CompareTo(pivot) > 0);

        // Recursively sort and combine the results
        return [.. lessThanPivot.AsQuickSortEnumerable(), .. new List<T> { pivot }, .. greaterThanPivot.AsQuickSortEnumerable()];
    }
}
