public class ImmutableBinaryTree<T>
{
    public T Value { get; }
    public ImmutableBinaryTree<T>? Left { get; }
    public ImmutableBinaryTree<T>? Right { get; }

    public ImmutableBinaryTree(T value, ImmutableBinaryTree<T>? left = null, ImmutableBinaryTree<T>? right = null)
    {
        Value = value;
        Left = left;
        Right = right;
    }

    public ImmutableBinaryTree<T> AddLeft(T newValue)
    {
        return new ImmutableBinaryTree<T>(Value, new ImmutableBinaryTree<T>(newValue), Right);
    }

    public ImmutableBinaryTree<T> AddRight(T newValue)
    {
        return new ImmutableBinaryTree<T>(Value, Left, new ImmutableBinaryTree<T>(newValue));
    }

    public ImmutableBinaryTree<T> ReplaceLeft(ImmutableBinaryTree<T>? newLeft)
    {
        return new ImmutableBinaryTree<T>(Value, newLeft, Right);
    }

    public ImmutableBinaryTree<T> ReplaceRight(ImmutableBinaryTree<T>? newRight)
    {
        return new ImmutableBinaryTree<T>(Value, Left, newRight);
    }

     // Replace the root value but keep structure
    public ImmutableBinaryTree<T> ReplaceRootValue(T newValue)
    {
        return new ImmutableBinaryTree<T>(newValue, Left, Right);
    }

    // Example Insert pattern: replace root on match
    public ImmutableBinaryTree<T> InsertAtRoot(T newValue)
    {
        return ReplaceRootValue(newValue);
    }
}
