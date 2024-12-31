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

    public static List<string> SortAndSaveChunks(string inputFilePath, int chunkSize)
    {
        List<string> tempFileNames = new List<string>();
        if(File.Exists(inputFilePath))
        {
            using (StreamReader reader = new StreamReader(inputFilePath))
            {
                int[] buffer = new int[chunkSize];
                int count;
                while ((count = ReadChunk(reader, buffer)) > 0)
                {
                    Array.Sort(buffer, 0, count);
                    string tempFileName = Path.GetTempFileName();
                    WriteChunk(tempFileName, buffer, count);
                    tempFileNames.Add(tempFileName);
                }
            }
        }
        else
        {
            throw new FileNotFoundException("The file does not exist.", inputFilePath);
        }
        return tempFileNames;
    }

    // Function to read a chunk of data
    public static int ReadChunk(StreamReader reader, int[] buffer)
    {
        int count = 0;
        string line;
        while (count < buffer.Length && (line = reader.ReadLine()) != null)
        {
            buffer[count++] = int.Parse(line);
        }
        return count;
    }

    // Function to write a chunk of data to a temporary file
    public static void WriteChunk(string filePath, int[] buffer, int count)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int i = 0; i < count; i++)
            {
                writer.WriteLine(buffer[i]);
            }
        }
    }

    // Function to perform n-way merge
    public static void NWayMerge(List<string> tempFileNames, string outputFilePath)
    {
        using (StreamWriter writer = new StreamWriter(outputFilePath))
        {
            List<StreamReader> readers = tempFileNames.Select(fileName => new StreamReader(fileName)).ToList();

            PriorityQueue<(int value, int index), int> minHeap = new PriorityQueue<(int value, int index), int>();

            for (int i = 0; i < readers.Count; i++)
            {
                if (int.TryParse(readers[i].ReadLine(), out int value))
                {
                    minHeap.Enqueue((value, i), value);
                }
            }

            while (minHeap.Count > 0)
            {
                var (value, index) = minHeap.Dequeue();
                writer.WriteLine(value);

                if (int.TryParse(readers[index].ReadLine(), out int nextValue))
                {
                    minHeap.Enqueue((nextValue, index), nextValue);
                }
            }

            foreach (var reader in readers)
            {
                reader.Dispose();
            }
        }

        foreach (var tempFileName in tempFileNames)
        {
            File.Delete(tempFileName);
        }
    }
}
