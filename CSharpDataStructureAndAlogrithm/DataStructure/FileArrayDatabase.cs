namespace DataStructure;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

public class FileArrayDatabase<T>
{
    private string filePath;

    public FileArrayDatabase(string filePath)
    {
        this.filePath = filePath;
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
        }
    }

    // Method to insert data into the file
    public void Insert(T item)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write))
        using (StreamWriter writer = new StreamWriter(fs))
        {
            writer.WriteLine(item?.ToString());
        }
    }

    public Task InsertAsync(T item, CancellationToken cancellationToken = default)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Append, FileAccess.Write))
        using (StreamWriter writer = new StreamWriter(fs))
        {
            ReadOnlyMemory<char> memory = item?.ToString().AsMemory() ?? Memory<char>.Empty;
            return writer.WriteLineAsync(memory, cancellationToken);
        }
    }

    // Method to delete data from the file
    public bool Delete(T item)
    {
        List<string>? lines = new List<string>(File.ReadAllLines(filePath));
        bool removed = lines.Remove(item?.ToString());
        if (removed)
        {
            File.WriteAllLines(filePath, lines);
        }
        return removed;
    }


    // Method to delete data from the file
    public async Task<bool> DeleteAsync(T item, CancellationToken cancellationToken = default)
    {
        var lines = new List<string>(await File.ReadAllLinesAsync(filePath, cancellationToken));
        bool removed = lines.Remove(item?.ToString());
        try
        {
            if (removed)
            {
                await File.WriteAllLinesAsync(filePath, lines, cancellationToken);
            }
            return removed;
        }
        catch(TaskCanceledException)
        {
            return false;
        }
    }

    // Method to get data from the file by index
    public T Get(int index)
    {
        var lines = File.ReadAllLines(filePath);
        if (index >= 0 && index < lines.Length)
        {
            return (T)Convert.ChangeType(lines[index], typeof(T));
        }
        throw new IndexOutOfRangeException("Index out of range.");
    }

    // Method to get data from the file by index
    public async Task<T> GetAsync(int index, CancellationToken cancellationToken = default)
    {
        var lines = await File.ReadAllLinesAsync(filePath, cancellationToken);
        if (index >= 0 && index < lines.Length)
        {
            return (T)Convert.ChangeType(lines[index], typeof(T));
        }
        throw new IndexOutOfRangeException("Index out of range.");
    }

    // Method to display all data in the file
    public void Display()
    {
        var lines = File.ReadAllLines(filePath);
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
    }

    // Method to display all data in the file
    public async void DisplayAsync(CancellationToken cancellationToken = default)
    {
        var lines = await File.ReadAllLinesAsync(filePath, cancellationToken);
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
    }

    // Method to replace data at a specific index
    public void Replace(int index, T newItem)
    {
        var lines = new List<string>(File.ReadAllLines(filePath));
        if (index >= 0 && index < lines.Count)
        {
            lines[index] = newItem?.ToString();
            File.WriteAllLines(filePath, lines);
        }
        else
        {
            throw new IndexOutOfRangeException("Index out of range.");
        }
    }

    // Method to replace data at a specific index
    public async Task ReplaceAsync(int index, T newItem, CancellationToken cancellationToken = default)
    {
        var lines = new List<string>(await File.ReadAllLinesAsync(filePath, cancellationToken));
        if (index >= 0 && index < lines.Count)
        {
            lines[index] = newItem?.ToString();
            await File.WriteAllLinesAsync(filePath, lines, cancellationToken);
        }
        else
        {
            throw new IndexOutOfRangeException("Index out of range.");
        }
    }
}
