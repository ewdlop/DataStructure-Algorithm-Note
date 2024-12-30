namespace DataStructure;

using System;
using System.Collections.Generic;
using System.IO;

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

    // Method to delete data from the file
    public bool Delete(T item)
    {
        var lines = new List<string>(File.ReadAllLines(filePath));
        bool removed = lines.Remove(item?.ToString());
        if (removed)
        {
            File.WriteAllLines(filePath, lines);
        }
        return removed;
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

    // Method to display all data in the file
    public void Display()
    {
        var lines = File.ReadAllLines(filePath);
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
}
