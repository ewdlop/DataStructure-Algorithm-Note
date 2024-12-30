namespace DataStructure;

public class BPlusTree
{
    public class BPlusTreeNode
    {
        public int[] Keys;
        public int Degree;
        public BPlusTreeNode[] Children;
        public int KeyCount;
        public bool IsLeaf;
        public BPlusTreeNode Next; // Pointer to the next leaf

        public BPlusTreeNode(int degree, bool isLeaf)
        {
            Degree = degree;
            IsLeaf = isLeaf;
            Keys = new int[2 * degree - 1];
            Children = new BPlusTreeNode[2 * degree];
            KeyCount = 0;
            Next = null;
        }

        public void Traverse()
        {
            if (IsLeaf)
            {
                for (int i = 0; i < KeyCount; i++)
                    Console.Write(Keys[i] + " ");
                if (Next != null)
                {
                    Console.Write(" -> ");
                    Next.Traverse();
                }
            }
            else
            {
                for (int i = 0; i < KeyCount; i++)
                {
                    Children[i].Traverse();
                    Console.Write("|" + Keys[i] + "| ");
                }
                Children[KeyCount].Traverse();
            }
        }

        public BPlusTreeNode Search(int key)
        {
            int i = 0;
            while (i < KeyCount && key > Keys[i])
                i++;
            if (IsLeaf)
                return this;
            return Children[i].Search(key);
        }

        // Insert and other operations would go here...

        // Simplified for brevity
    }

    private BPlusTreeNode root;
    private int degree;

    public BPlusTree(int degree)
    {
        this.degree = degree;
        root = new BPlusTreeNode(degree, true);
    }

    public void Traverse()
    {
        if (root != null)
            root.Traverse();
    }

    public BPlusTreeNode Search(int key)
    {
        return root == null ? null : root.Search(key);
    }

    // Insert and other operations would go here...

    // Simplified for brevity
}
