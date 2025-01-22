using System;
using System.Buffers;
using System.Numerics;
using System.Threading;

namespace Algorithm;

public static partial class Sorter
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
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentNullException.ThrowIfNull(buffer);

        int count = 0;
        string? line;
        
        while (count < buffer.Length && (line = reader.ReadLine()) != null)
        {
            buffer[count++] = int.Parse(line);
        }
        return count;
    }

    // Function to read a chunk of data
    public static int ReadChunk(StreamReader reader, in int[] buffer)
    {
        ArgumentNullException.ThrowIfNull(reader);
        ArgumentNullException.ThrowIfNull(buffer);

        int count = 0;
        string? line;

        while (count < buffer.Length && (line = reader.ReadLine()) != null)
        {
            buffer[count++] = int.Parse(line);
        }
        return count;
    }

    // Function to read a chunk of data
    public static IEnumerable<IMemoryOwner<int?>> GetReadChunkEnumerable(StreamReader reader, int size)
    {
        ArgumentNullException.ThrowIfNull(reader);

        string? line = string.Empty;
        int count = 0;
        IMemoryOwner<int?> memoryOwner = MemoryPool<int?>.Shared.Rent(size);
        while ((line = reader.ReadLine()) is not null)
        {
            memoryOwner.Memory.Span[count++] = int.TryParse(line, out int result) ? result : null;
            if (count == size)
            {
                yield return memoryOwner;
                memoryOwner = MemoryPool<int?>.Shared.Rent(size);
            }
        }
    }

    // Function to read a chunk of data
    public static async IAsyncEnumerable<IMemoryOwner<int?>> GetReadChunkAsyncEnumerable(StreamReader reader, int size, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(reader);

        string? line = string.Empty;
        int count = 0;
        IMemoryOwner<int?> memoryOwner = MemoryPool<int?>.Shared.Rent(size);
        while ((line = await reader.ReadLineAsync(cancellationToken)) is not null)
        {
            memoryOwner.Memory.Span[count++] = int.TryParse(line, out int result) ? result : null;
            if (count == size)
            {
                yield return memoryOwner;
                memoryOwner = MemoryPool<int?>.Shared.Rent(size);
            }
        }
    }

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

    public static void WriteChunk(string filePath, in int[] buffer, int count)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int i = 0; i < count; i++)
            {
                writer.WriteLine(buffer[i]);
            }
        }
    }

    public static void WriteChunk(string filePath, ReadOnlyMemory<int> buffer, int count)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int i = 0; i < count; i++)
            {
                writer.WriteLine(buffer.Span[i]);
            }
        }
    }

    public static void WriteChunk(string filePath, in ReadOnlyMemory<int> buffer, int count)
    {
        using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int i = 0; i < count; i++)
            {
                writer.WriteLine(buffer.Span[i]);
            }
        }
    }

    // Function to write a chunk of data to a temporary file
    public static async Task WriteChunkAsync(string filePath, int[] buffer, int count, CancellationToken cancellationToken = default)
    {
        await using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int i = 0; i < count; i++)
            {
                await writer.WriteLineAsync(buffer[i].ToString().AsMemory(), cancellationToken);
            }
        }
    }

    // Function to write a chunk of data to a temporary file
    public static async IAsyncEnumerable<Task> GetWriteChunkAsyncEnumerable(string filePath, int[] buffer, int count, CancellationToken cancellationToken = default)
    {
        await using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int i = 0; i < count; i++)
            {
                yield return writer.WriteLineAsync(buffer[i].ToString().AsMemory(), cancellationToken);
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

    // Function to perform n-way merge
    public static async Task NWayMergeAsync(List<string> tempFileNames, string outputFilePath, CancellationToken cancellationToken = default)
    {
        using (StreamWriter writer = new StreamWriter(outputFilePath))
        {
            List<StreamReader> readers = tempFileNames.Select(fileName => new StreamReader(fileName)).ToList();


            //what if the heap get blown up?

            PriorityQueue<(int value, int index), int> minHeap = new PriorityQueue<(int value, int index), int>();

            for (int i = 0; i < readers.Count; i++)
            {
                if (int.TryParse(await readers[i].ReadLineAsync(cancellationToken), out int value))
                {
                    minHeap.Enqueue((value, i), value);
                }
            }

            while (minHeap.Count > 0)
            {
                var (value, index) = minHeap.Dequeue();
                await writer.WriteLineAsync(value.ToString().AsMemory(), cancellationToken);

                if (int.TryParse(await readers[index].ReadLineAsync(cancellationToken), out int nextValue))
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

    public const string NULL_FILE_NAME = "null";
    public const int MAX_HEAP_SIZE = 1024;
    public const int MAX_BACKUP_FILE_SIZE = 1024 * 8;

    public static async Task NWayMergeWithNestedBackupAsync(
        List<string> tempFileNames,
        string outputFilePath,
        //int chunkSize,
        int maxHeapSize,
        int maxBackupFileSize,
        string nullFileName = NULL_FILE_NAME,
        CancellationToken cancellationToken = default)
    {
        await using StreamWriter writer = new StreamWriter(outputFilePath);
        var readers = tempFileNames.Select(fileName => new StreamReader(fileName)).ToList();

        readers.Add(new StreamReader(NULL_FILE_NAME)); // Add a null file to the end of the list

        PriorityQueue<(int? value, int? index), int?> minHeap = new PriorityQueue<(int? value, int? index), int?>(maxHeapSize);
        Queue<string> backupFiles = new Queue<string>();

        // Initialize the heap with the first value from each file
        for (int i = 0; i < readers.Count; i++)
        {
            if (await TryReadNextAsync(readers[i], i, minHeap, MAX_HEAP_SIZE, MAX_BACKUP_FILE_SIZE, cancellationToken))
            {
                continue;
            }
        }

        while (minHeap.Count > 0 || backupFiles.Count > 0)
        {
            // Spill excess heap data to temporary files if heap size exceeds maxHeapSize
            if (minHeap.Count > maxHeapSize)
            {
                await SpillToBackupFilesAsync(minHeap, backupFiles, maxBackupFileSize, cancellationToken);
            }

            // Process the smallest item in the heap
            if (minHeap.Count > 0)
            {
                var (value, index) = minHeap.Dequeue();
                await writer.WriteLineAsync(value.ToString().AsMemory(), cancellationToken);

                // Refill the heap from the same file
                if (index is not null)
                {
                    await TryReadNextAsync(
                        readers[index.Value],
                        index.Value,
                        minHeap,
                        MAX_HEAP_SIZE,
                        MAX_BACKUP_FILE_SIZE,
                        cancellationToken);
                }
                else
                {
                    await TryReadNextAsync(
                        readers[^1],
                        readers.Count - 1,
                        minHeap,
                        MAX_HEAP_SIZE,
                        MAX_BACKUP_FILE_SIZE,
                        cancellationToken);
                }
            }
            else if (backupFiles.Count > 0)
            {
                // Reload data from the oldest backup file into the heap
                string oldestBackup = backupFiles.Dequeue();
                await ReloadBackupFileAsync(oldestBackup, minHeap, backupFiles, maxBackupFileSize, cancellationToken);
                File.Delete(oldestBackup); // Clean up processed backup file
            }
        }

        // Clean up temporary files
        foreach (var tempFileName in tempFileNames)
        {
            File.Delete(tempFileName);
        }
    }

    public static async Task<PriorityQueue<(int? value, int? index), int?>> SpillToBackupFilesAsync(
        PriorityQueue<(int? value, int? index), int?> minHeap,
        Queue<string> backupFiles,
        int maxBackupFileSize,
        CancellationToken cancellationToken = default)
    {
        int spillCount = minHeap.Count - maxBackupFileSize / 2;
        List<(int? value, int? index)> spillData = new List<(int? value, int? index)>();

        for (int i = 0; i < spillCount; i++)
        {
            spillData.Add(minHeap.Dequeue());
        }

        while (spillData.Count > 0)
        {
            //unique file name
            string backupFile = Path.GetTempFileName();
            using StreamWriter writer = new StreamWriter(backupFile);

            int writeCount = Math.Min(spillData.Count, maxBackupFileSize);
            for (int i = 0; i < writeCount; i++)
            {
                var (value, index) = spillData[i];
                await writer.WriteLineAsync($"{value},{index}".AsMemory(), cancellationToken);
            }

            spillData.RemoveRange(0, writeCount);
            backupFiles.Enqueue(backupFile);
        }

        return minHeap;
    }

    public static async Task ReloadBackupFileAsync(
        string backupFile,
        PriorityQueue<(int? value, int? index), int?> heap,
        Queue<string> backupFiles,
        int maxBackupFileSize,
        CancellationToken cancellationToken = default)
    {
        using StreamReader reader = new StreamReader(backupFile);
        List<(int? value, int? index)> reloadData = [];

        string? line;
        while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
        {
            var parts = line.Split(',');
            int? value = int.TryParse(parts[0], out int result1) ? result1 : null;
            int? index = int.TryParse(parts[1], out int result2) ? result2 : null;
            reloadData.Add((value, index));
        }

        while (reloadData.Count > 0)
        {
            if (reloadData.Count <= maxBackupFileSize)
            {
                //Load directly into the heap
                foreach (var item in reloadData)
                {
                    heap.Enqueue(item, item.value);
                }
                reloadData.Clear();
            }
            else
            {
                //Spill excess to new backup files
                //Unique file name
                string splitBackup = Path.GetTempFileName();
                using StreamWriter writer = new StreamWriter(splitBackup);

                int writeCount = Math.Min(reloadData.Count, maxBackupFileSize);
                for (int i = 0; i < writeCount; i++)
                {
                    var (value, index) = reloadData[i];
                    await writer.WriteLineAsync($"{value},{index}".AsMemory(), cancellationToken);
                }

                reloadData.RemoveRange(0, writeCount);
                backupFiles.Enqueue(splitBackup);
            }
        }
    }

    public static async Task<bool> TryReadNextAsync(
        StreamReader reader,
        int fileIndex,
        PriorityQueue<(int? value, int? index), int?> heap,
        int maxHeapSize,
        int maxBackupFileSize,
        CancellationToken cancellationToken = default)
    {
        string? line = await reader.ReadLineAsync(cancellationToken);
        if (line != null)
        {
            int? value = int.TryParse(line, out int result) ? result : null;
            heap.Enqueue((value, fileIndex), value);

            // Spill excess heap data if it exceeds the maximum size
            if (heap.Count > maxHeapSize)
            {
                await SpillToBackupFilesAsync(heap, new Queue<string>(), maxBackupFileSize, cancellationToken);
            }

            return true;
        }

        return false;
    }

}
