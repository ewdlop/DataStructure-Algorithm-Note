namespace DataStructure;

public class RedBlackTree
{
    public enum Color
    {
        Red,
        Black
    }

    public class Node
    {
        public int Data { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
        public Node Parent { get; set; }
        public Color NodeColor { get; set; }

        public Node(int data)
        {
            Data = data;
            Left = null;
            Right = null;
            Parent = null;
            NodeColor = Color.Red;
        }
    }

    private Node root;
    private Node TNULL;

    public RedBlackTree()
    {
        TNULL = new Node(0);
        TNULL.NodeColor = Color.Black;
        TNULL.Left = null;
        TNULL.Right = null;
        root = TNULL;
    }

    // Preorder
    public void PreOrderTraversal(Node node)
    {
        if (node != TNULL)
        {
            Console.Write($"{node.Data} ");
            PreOrderTraversal(node.Left);
            PreOrderTraversal(node.Right);
        }
    }

    public Node GetRoot()
    {
        return root;
    }

    private void RotateLeft(Node x)
    {
        Node y = x.Right;
        x.Right = y.Left;
        if (y.Left != TNULL)
        {
            y.Left.Parent = x;
        }
        y.Parent = x.Parent;
        if (x.Parent == null)
        {
            this.root = y;
        }
        else if (x == x.Parent.Left)
        {
            x.Parent.Left = y;
        }
        else
        {
            x.Parent.Right = y;
        }
        y.Left = x;
        x.Parent = y;
    }

    private void RotateRight(Node x)
    {
        Node y = x.Left;
        x.Left = y.Right;
        if (y.Right != TNULL)
        {
            y.Right.Parent = x;
        }
        y.Parent = x.Parent;
        if (x.Parent == null)
        {
            this.root = y;
        }
        else if (x == x.Parent.Right)
        {
            x.Parent.Right = y;
        }
        else
        {
            x.Parent.Left = y;
        }
        y.Right = x;
        x.Parent = y;
    }

    // Balance the tree after deletion of a node
    private void BalanceInsert(Node k)
    {
        Node u;
        while (k.Parent.NodeColor == Color.Red)
        {
            if (k.Parent == k.Parent.Parent.Right)
            {
                u = k.Parent.Parent.Left; // uncle
                if (u.NodeColor == Color.Red)
                {
                    u.NodeColor = Color.Black;
                    k.Parent.NodeColor = Color.Black;
                    k.Parent.Parent.NodeColor = Color.Red;
                    k = k.Parent.Parent;
                }
                else
                {
                    if (k == k.Parent.Left)
                    {
                        k = k.Parent;
                        RotateRight(k);
                    }
                    k.Parent.NodeColor = Color.Black;
                    k.Parent.Parent.NodeColor = Color.Red;
                    RotateLeft(k.Parent.Parent);
                }
            }
            else
            {
                u = k.Parent.Parent.Right; // uncle

                if (u.NodeColor == Color.Red)
                {
                    u.NodeColor = Color.Black;
                    k.Parent.NodeColor = Color.Black;
                    k.Parent.Parent.NodeColor = Color.Red;
                    k = k.Parent.Parent;
                }
                else
                {
                    if (k == k.Parent.Right)
                    {
                        k = k.Parent;
                        RotateLeft(k);
                    }
                    k.Parent.NodeColor = Color.Black;
                    k.Parent.Parent.NodeColor = Color.Red;
                    RotateRight(k.Parent.Parent);
                }
            }
            if (k == root)
            {
                break;
            }
        }
        root.NodeColor = Color.Black;
    }

    public void Insert(int key)
    {
        Node node = new Node(key);
        node.Parent = null;
        node.Data = key;
        node.Left = TNULL;
        node.Right = TNULL;
        node.NodeColor = Color.Red;

        Node y = null;
        Node x = this.root;

        while (x != TNULL)
        {
            y = x;
            if (node.Data < x.Data)
            {
                x = x.Left;
            }
            else
            {
                x = x.Right;
            }
        }

        node.Parent = y;
        if (y == null)
        {
            root = node;
        }
        else if (node.Data < y.Data)
        {
            y.Left = node;
        }
        else
        {
            y.Right = node;
        }

        if (node.Parent == null)
        {
            node.NodeColor = Color.Black;
            return;
        }

        if (node.Parent.Parent == null)
        {
            return;
        }

        BalanceInsert(node);
    }
}