namespace DataStructure;

public class RTree
{
    public class RTreeNode
    {
        public List<Rectangle> Entries { get; set; }
        public List<RTreeNode> Children { get; set; }
        public bool IsLeaf { get; set; }

        public RTreeNode(bool isLeaf)
        {
            Entries = new List<Rectangle>();
            Children = new List<RTreeNode>();
            IsLeaf = isLeaf;
        }
    }


    private RTreeNode root;
    private int maxEntries;

    public RTree(int maxEntries = 4)
    {
        root = new RTreeNode(true);
        this.maxEntries = maxEntries;
    }

    public void Insert(Rectangle rect)
    {
        RTreeNode node = ChooseLeaf(root, rect);
        node.Entries.Add(rect);
        if (node.Entries.Count > maxEntries)
        {
            SplitNode(node);
        }
    }

    private RTreeNode ChooseLeaf(RTreeNode node, Rectangle rect)
    {
        if (node.IsLeaf)
        {
            return node;
        }

        RTreeNode? bestChild = null;
        System.Double minAreaIncrease = System.Double.MaxValue;

        foreach (RTreeNode child in node.Children)
        {
            Rectangle union = Rectangle.Union(child.Entries[0], rect);
            System.Double areaIncrease = union.Area() - child.Entries[0].Area();
            if (areaIncrease < minAreaIncrease)
            {
                minAreaIncrease = areaIncrease;
                bestChild = child;
            }
        }

        return ChooseLeaf(bestChild, rect);
    }

    private void SplitNode(RTreeNode node)
    {
        // Simplified linear split algorithm
        List<Rectangle> entries = node.Entries;
        entries.Sort((a, b) => a.MinX.CompareTo(b.MinX));

        RTreeNode newNode = new RTreeNode(node.IsLeaf);
        int splitIndex = entries.Count / 2;

        newNode.Entries.AddRange(entries.Skip(splitIndex).ToList());
        node.Entries = entries.Take(splitIndex).ToList();

        if (node == root)
        {
            RTreeNode newRoot = new RTreeNode(false);
            newRoot.Children.Add(node);
            newRoot.Children.Add(newNode);
            newRoot.Entries.Add(Rectangle.Union(node.Entries[0], newNode.Entries[0]));
            root = newRoot;
        }
        else
        {
            RTreeNode? parent = FindParent(root, node) ?? throw new InvalidOperationException("Parent not found");
            parent.Children.Add(newNode);
            if (parent.Children.Count > maxEntries)
            {
                SplitNode(parent);
            }
        }
    }

    private RTreeNode? FindParent(RTreeNode root, RTreeNode node)
    {
        foreach (RTreeNode child in root.Children)
        {
            if (child.Children.Contains(node))
            {
                return root;
            }
            else
            {
                RTreeNode? parent = FindParent(child, node);
                if (parent != null)
                {
                    return parent;
                }
            }
        }
        return null;
    }

    public List<Rectangle> Search(Rectangle query)
    {
        List<Rectangle> result = new List<Rectangle>();
        Search(root, query, result);
        return result;
    }

    private static void Search(RTreeNode node, Rectangle query, List<Rectangle> result)
    {
        foreach (Rectangle entry in node.Entries)
        {
            if (query.Intersects(entry))
            {
                if (node.IsLeaf)
                {
                    result.Add(entry);
                }
                else
                {
                    foreach (RTreeNode child in node.Children)
                    {
                        Search(child, query, result);
                    }
                }
            }
        }
    }
}
