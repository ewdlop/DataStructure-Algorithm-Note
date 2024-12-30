namespace DataStructure;

public class BTree
{
    private BTreeNode root;
    private int degree;

    public class BTreeNode
    {
        public int[] Keys;
        public int Degree;
        public BTreeNode[] Children;
        public int KeyCount;
        public bool IsLeaf;

        public BTreeNode(int degree, bool isLeaf)
        {
            Degree = degree;
            IsLeaf = isLeaf;
            Keys = new int[2 * degree - 1];
            Children = new BTreeNode[2 * degree];
            KeyCount = 0;
        }

        public void Traverse()
        {
            int i;
            for (i = 0; i < KeyCount; i++)
            {
                if (!IsLeaf)
                    Children[i].Traverse();
                Console.Write(Keys[i] + " ");
            }
            if (!IsLeaf)
                Children[i].Traverse();
        }

        public BTreeNode Search(int key)
        {
            int i = 0;
            while (i < KeyCount && key > Keys[i])
                i++;
            if (Keys[i] == key)
                return this;
            if (IsLeaf)
                return null;
            return Children[i].Search(key);
        }

        // Insert and other operations would go here...

        // Simplified for brevity
    }


    public BTree(int degree)
    {
        this.degree = degree;
        root = null;
    }

    public void Traverse()
    {
        if (root != null)
            root.Traverse();
    }

    public BTreeNode Search(int key)
    {
        return root == null ? null : root.Search(key);
    }

    // Insert and other operations would go here...

    // Simplified for brevity
}