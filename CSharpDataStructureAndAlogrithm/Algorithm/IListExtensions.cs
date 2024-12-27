using System.Numerics;

namespace Algorithm;

public static class IListExtensions
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
}
