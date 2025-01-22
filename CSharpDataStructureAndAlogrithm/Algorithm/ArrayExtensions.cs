using System.Numerics;

namespace Algorithm;

public static class ArrayExtensions
{
    public static void Swap<T>(T[] array, int index1, int index2)
    {
        ArgumentNullException.ThrowIfNull(array);
        //check if the indexes are valid
        if (index1 < 0 || index1 >= array.Length || index2 < 0 || index2 >= array.Length)
        {
            throw new IndexOutOfRangeException("The indexes are out of range.");
        }
        (array[index1], array[index2]) = (array[index2], array[index1]);
    }

    public static IEnumerable<T> AsReverse<T>(this T[] array)
    {
        ArgumentNullException.ThrowIfNull(array);
        for (int i = array.Length; i >= 0; i--)
        {
            yield return array[i];
        }
    }
    public static T[] ToReverse<T>(this T[] array)
    {
        ArgumentNullException.ThrowIfNull(array);
        return array.AsReverse().ToArray();
    }

    public static (T min, T max) MinMax<T>(T[] array) where T : IComparisonOperators<T, T, bool>
    {
        ArgumentNullException.ThrowIfNull(array);
        if (array.Length == 0)
        {
            throw new InvalidOperationException("The array is empty.");
        }
        T min = array[0];
        T max = array[0];
        for (int i = 1; i < array.Length; i++)
        {
            if (array[i] < min)
            {
                min = array[i];
            }
            if (array[i] > max)
            {
                max = array[i];
            }
        }
        return (min, max);
    }

    public static T?[] QuickSort<T>(this T?[] array) where T : IComparable<T>
    {
        ArgumentNullException.ThrowIfNull(array);   

        // Base case: array of length 0 or 1 is already sorted
        if (array.Length <= 1)
            return [..array];

        // Choosing pivot (first element in this case)
        T? pivot = array[0];

        // Partitioning into two sub-arrays
        IEnumerable<T?> lessThanPivot = array.Where(x => x?.CompareTo(pivot) < 0);
        IEnumerable<T?> greaterThanPivot = array.Where(x => x?.CompareTo(pivot) > 0);

        // Recursively sort and combine the results
        return [.. lessThanPivot.AsQuickSortEnumerable(), .. new[] { pivot }, .. greaterThanPivot.AsQuickSortEnumerable()];
    }
}
