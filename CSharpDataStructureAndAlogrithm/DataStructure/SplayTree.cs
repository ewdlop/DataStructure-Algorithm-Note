using System;

namespace DataStructure;

public class SplayTree<T> where T : IComparable<T>
{
    public class SplayTreeNode<T> where T : IComparable<T>
    {
        public T Value;
        public SplayTreeNode<T>? Left, Right;

        public SplayTreeNode(T value)
        {
            Value = value;
            Left = Right = null;
        }
    }

    private SplayTreeNode<T>? root;

    public SplayTree()
    {
        root = null;
    }

    private static SplayTreeNode<T>? RightRotate(SplayTreeNode<T> x)
    {
        SplayTreeNode<T>? y = x.Left;
        x.Left = y?.Right;
        if(y != null)
        {
            y.Right = x;
        }
        return y;
    }

    private static SplayTreeNode<T>? LeftRotate(SplayTreeNode<T> x)
    {
        SplayTreeNode<T>? y = x.Right;
        x.Right = y?.Left;
        if (y != null)
        {
            y.Left = x;
        }
        return y;
    }

    private static SplayTreeNode<T>? Splay(SplayTreeNode<T>? root, T key)
    {
        if (root == null || root.Value.CompareTo(key) == 0)
            return root;

        if (root.Value.CompareTo(key) > 0)
        {
            if (root.Left == null) return root;

            if (root.Left.Value.CompareTo(key) > 0)
            {
                root.Left.Left = Splay(root.Left.Left, key);
                root = RightRotate(root);
            }
            else if (root.Left.Value.CompareTo(key) < 0)
            {
                root.Left.Right = Splay(root.Left.Right, key);
                if (root.Left.Right != null)
                    root.Left = LeftRotate(root.Left);
            }

            return root?.Left == null ? root : RightRotate(root);
        }
        else
        {
            if (root.Right == null) return root;

            if (root.Right.Value.CompareTo(key) > 0)
            {
                root.Right.Left = Splay(root.Right.Left, key);
                if (root.Right.Left != null)
                    root.Right = RightRotate(root.Right);
            }
            else if (root.Right.Value.CompareTo(key) < 0)
            {
                root.Right.Right = Splay(root.Right.Right, key);
                root = LeftRotate(root);
            }

            return root?.Right == null ? root : LeftRotate(root);
        }
    }

    public void Insert(T key)
    {
        if (root == null)
        {
            root = new SplayTreeNode<T>(key);
            return;
        }

        root = Splay(root, key);

        if (root.Value.CompareTo(key) == 0) return;

        SplayTreeNode<T> newNode = new SplayTreeNode<T>(key);

        if (root.Value.CompareTo(key) > 0)
        {
            newNode.Right = root;
            newNode.Left = root.Left;
            root.Left = null;
        }
        else
        {
            newNode.Left = root;
            newNode.Right = root.Right;
            root.Right = null;
        }

        root = newNode;
    }

    public void Delete(T key)
    {
        if (root == null) return;

        root = Splay(root, key);

        if (root?.Value.CompareTo(key) != 0) return;

        if (root.Left == null)
        {
            root = root.Right;
        }
        else
        {
            SplayTreeNode<T>? temp = root.Right;
            root = root.Left;
            Splay(root, key);
            root.Right = temp;
        }
    }

    public bool Search(T key)
    {
        root = Splay(root, key);
        return root != null && root.Value.CompareTo(key) == 0;
    }

    public void InOrderTraversal(SplayTreeNode<T>? node)
    {
        if (node != null)
        {
            InOrderTraversal(node?.Left);
            Console.Write(node?.Value.ToString() + " ");
            InOrderTraversal(node?.Right);
        }
    }

    public void PrintTree()
    {
        InOrderTraversal(root);
        Console.WriteLine();
    }
}