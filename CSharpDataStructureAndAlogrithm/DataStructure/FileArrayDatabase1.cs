namespace DataStructure;

using System;
using System.Collections.Generic;
using System.IO;


public class FileArrayDatabase1<T>
{
    private string filePath;

    public FileArrayDatabase1(string filePath)
    {
        this.filePath = filePath;
        if (!File.Exists(filePath))
        {
            File.Create(filePath).Close();
        }
    }

    public void Insert(T item)
    {
        using (StreamWriter sw = File.AppendText(filePath))
        {
            sw.WriteLine(item?.ToString());
        }
    }

    public bool Delete(T item)
    {
        var lines = new List<string>(File.ReadAllLines(filePath));
        bool removed = lines.Remove(item?.ToString());
        File.WriteAllLines(filePath, lines);
        return removed;
    }

    public void DeleteAt(int index)
    {
        var lines = new List<string>(File.ReadLines(filePath).Skip(index));
        if (index >= 0 && index < lines.Count)
        {
            lines.RemoveAt(index);
            File.WriteAllLines(filePath, lines);
        }
        else
        {
            throw new IndexOutOfRangeException("Index out of range.");
        }
    }

    public void Update(T oldItem, T newItem)
    {
        var lines = new List<string>(File.ReadAllLines(filePath));
        int index = lines.IndexOf(oldItem?.ToString());
        if (index != -1)
        {
            lines[index] = newItem?.ToString();
            File.WriteAllLines(filePath, lines);
        }
    }

    public bool Search(T item)
    {
        var lines = File.ReadAllLines(filePath);
        return lines.Contains(item?.ToString());
    }


    public T Get(int index)
    {
        var lines = File.ReadAllLines(filePath);
        if (index >= 0 && index < lines.Length)
        {
            return (T)Convert.ChangeType(lines[index], typeof(T));
        }
        throw new IndexOutOfRangeException("Index out of range.");
    }

    public void Display()
    {
        var lines = File.ReadAllLines(filePath);
        foreach (var line in lines)
        {
            Console.WriteLine(line);
        }
    }
}