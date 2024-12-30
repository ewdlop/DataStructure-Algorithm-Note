namespace DataStructure;

public class SuffixTree
{
    public class SuffixTreeNode
{
    public Dictionary<char, SuffixTreeNode> Children { get; } = new Dictionary<char, SuffixTreeNode>();
    public List<int> Indexes { get; } = new List<int>();

    public void InsertSuffix(string suffix, int index)
    {
        Indexes.Add(index);
        if (suffix.Length > 0)
        {
            char c = suffix[0];
            SuffixTreeNode child;
            if (Children.ContainsKey(c))
            {
                child = Children[c];
            }
            else
            {
                child = new SuffixTreeNode();
                Children[c] = child;
            }
            child.InsertSuffix(suffix.Substring(1), index);
        }
    }

    public List<int> Search(string pattern)
    {
        if (pattern.Length == 0)
        {
            return Indexes;
        }
        char c = pattern[0];
        if (Children.ContainsKey(c))
        {
            return Children[c].Search(pattern.Substring(1));
        }
        else
        {
            return new List<int>();
        }
    }
}

    private readonly SuffixTreeNode root = new SuffixTreeNode();

    public SuffixTree(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            root.InsertSuffix(text.Substring(i), i);
        }
    }

    public List<int> Search(string pattern)
    {
        return root.Search(pattern);
    }
}

