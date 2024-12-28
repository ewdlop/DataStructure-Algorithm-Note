using System;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DataStructure;

public class Rope(string Data)
{
    public Node? Root { get; protected set; } = new Node(Data);

    public class Node(string Data)
    {
        public string Data { get; init; } = Data ?? string.Empty;
        public Node? Left { get; init; }
        public Node? Right { get; init; }
        public int Weight { get; init; } = Data?.Length ?? 0; // The length of the string in the node
    }

    public void Concat(string data)
    {
        Root = new Node(string.Empty)
        {
            Left = Root,
            Right = new Node(data),
            Weight = Root?.Weight ?? 0
        };
    }

    public char this[int i] => Index(i);

    public char Index(int i)
    {
        return Index(Root, i);
    }

    protected static char Index(Node? node, int i)
    {
        if (node == null) throw new IndexOutOfRangeException();
        if (node.Data is not null) return node.Data[i];
        if (i < node.Weight) return Index(node.Left, i);
        return Index(node.Right, i - node?.Weight ?? 0);
    }

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();
        foreach (string data in ToString(Root))
        {
            sb.Append(data);
        }
        return sb.ToString();
    }

    protected static IEnumerable<string> ToString(Node? node)
    {
        yield return node?.Data ?? string.Empty;
        foreach (string data in ToString(node?.Left))
        {
            yield return data;
        }
        foreach (string data in ToString(node?.Right))
        {
            yield return data;
        }
    }
}