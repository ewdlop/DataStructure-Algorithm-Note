namespace DataStructure;

using System;
using System.IO;

public class FileTree
{
    private string rootFilePath;
    private string directoryPath;

    public FileTree(string rootData, string directoryPath)
    {
        this.directoryPath = directoryPath;
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        rootFilePath = Path.Combine(directoryPath, "root.bin");
        if (!File.Exists(rootFilePath))
        {
            SaveNode(new TreeNode(rootData), rootFilePath);
        }
    }

    private void SaveNode(TreeNode node, string filePath)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        using (BinaryWriter writer = new BinaryWriter(fs))
        {
            // Write node data
            writer.Write(node.Data);
            // Write number of children
            writer.Write(node.Children.Count);
            // Write each child's file path
            foreach (var child in node.Children)
            {
                writer.Write(child);
            }
        }
    }

    private TreeNode LoadNode(string filePath)
    {
        using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        using (BinaryReader reader = new BinaryReader(fs))
        {
            // Read node data
            string data = reader.ReadString();
            TreeNode node = new TreeNode(data);

            // Read number of children
            int childrenCount = reader.ReadInt32();
            for (int i = 0; i < childrenCount; i++)
            {
                node.Children.Add(reader.ReadString());
            }

            return node;
        }
    }

    public void AddChild(string parentFilePath, string childData)
    {
        var parentNode = LoadNode(parentFilePath);
        var childFilePath = Path.Combine(directoryPath, $"{Guid.NewGuid()}.bin");
        parentNode.Children.Add(childFilePath);
        SaveNode(parentNode, parentFilePath);
        SaveNode(new TreeNode(childData), childFilePath);
    }

    public void Traverse(string filePath, Action<TreeNode, string> action)
    {
        var node = LoadNode(filePath);
        action(node, filePath);
        foreach (var childFilePath in node.Children)
        {
            Traverse(childFilePath, action);
        }
    }

    public void Display()
    {
        Traverse(rootFilePath, (node, filePath) => Console.WriteLine(node.Data));
    }
}