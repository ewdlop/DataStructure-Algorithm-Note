using System.Collections;
using System.Text;

namespace DataStructure;

public class Rope(string Data)
{
    public Node? Root { get; protected set; } = new Node(Data);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="Data"></param>
    /// <param name="Left"></param>
    /// <param name="Right"></param>
    /// <param name="Weight">The length of the string in the node</param>
    public record Node(string Data, Node? Left, Node? Right, int? Weight) : IEnumerable<Node?>
    {
        public Node(string? Data) : this(Data ?? string.Empty, null, null, Data?.Length) { }
        public Node(string? Data, Node? Left, Node? Right) : this(Data ?? string.Empty, Left, Right, Data?.Length) { }

        public IEnumerable<string> AsStringEnumerable()
        {
            yield return Data ?? string.Empty;
            foreach (Node? node in Left ?? Enumerable.Empty<Node?>())
            {
                yield return node?.Data ?? string.Empty;
            }
            foreach (Node? node in Right ?? Enumerable.Empty<Node?>())
            {
                yield return node?.Data ?? string.Empty;
            }
        }

        public IEnumerator<Node?> GetEnumerator()
        {
            yield return this;
            foreach (Node? node in Left ?? Enumerable.Empty<Node?>())
            {
                yield return node;
            }
            foreach (Node? node in Right ?? Enumerable.Empty<Node?>())
            {
                yield return node;
            }
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
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

    public char Index(int i) => Index(Root, i);

    protected static char Index(Node? node, int i)
    {
        if (node == null) throw new IndexOutOfRangeException();
        if (node.Data is not null) return node.Data[i];
        if (i < node.Weight) return Index(node.Left, i);
        return Index(node.Right, i - node?.Weight ?? 0);
    }

    public virtual string AsString()
    {
        StringBuilder? sb = null;
        foreach (string data in Root?.AsStringEnumerable() ?? [])
        {
            (sb ??= new StringBuilder()).Append(data);
        }
        return sb?.ToString() ?? string.Empty;
    }

    public static IEnumerable<string> AsString(Node? node)
    {
        yield return node?.Data ?? string.Empty;
        foreach (string data in AsString(node?.Left))
        {
            yield return data;
        }
        foreach (string data in AsString(node?.Right))
        {
            yield return data;
        }
    }

    public static IEnumerable<string> AsStringEnumerable(Node? node) => node?.AsStringEnumerable() ?? [];
}