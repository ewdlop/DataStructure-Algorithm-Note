using System.Collections;
using System.Text;

namespace DataStructure;

public class Rope(string Data) : IEnumerable<string>
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

        public char this[int i] => Data[i];
        public string this[Range range] => Data[range];
    }


    /// <summary>
    /// (Data)_Data.Length-()_Data.Length-(data)_data.Length
    /// </summary>
    /// <param name="data"></param>
    public void Concat(string data)
    {
        Root = new Node(string.Empty)
        {
            Left = Root,
            Right = new Node(data),
            Weight = Root?.Weight ?? 0
        };
    }

    public void Add(string data) => Concat(data);

    public char this[int i] => Index(i);

    public string this[Range range] => Range(Root, range);
    
    //public string this[Range range] => Enumerable.Range(range.Start.Value, range.End.Value - range.Start.Value)
    //    .Select(i => Index(i))
    //    .Aggregate(new StringBuilder(), (sb, c) => sb.Append(c))
    //    .ToString();

    public char Index(int i) => Index(Root, i);

    protected static char Index(Node? node, int i)
    {
        if (node == null) throw new IndexOutOfRangeException();
        if (node.Data is not null)
        {
            if (i < node.Data.Length) return node[i];
            throw new IndexOutOfRangeException();
        }
        if (i < node.Weight) return Index(node.Left, i);
        return Index(node.Right, i - (node.Weight ?? 0));
    }

    protected static string Range(Node? node, Range range)
    {
        if (node == null) throw new IndexOutOfRangeException();
        if (node.Data is not null)
        {
            if(range.Start.Value < node.Data.Length && range.End.Value < node.Data.Length)
            {
                return node[range];
            }
            throw new IndexOutOfRangeException();
        }
        if (range.End.Value < node.Weight) return Range(node.Left, range);
        int i = range.Start.Value - node.Weight.GetValueOrDefault();
        int j = range.End.Value - node.Weight.GetValueOrDefault();
        return Range(node.Right, i..j);
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

    public IEnumerator<string> GetEnumerator() => AsStringEnumerable(Root).GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}