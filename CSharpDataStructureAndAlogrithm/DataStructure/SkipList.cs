namespace DataStructure;

public class SkipList<T> where T : IComparable<T>
{
    public class SkipListNode<T>
    {
        public T Value;
        public List<SkipListNode<T>> Forward;

        public SkipListNode(T value, int level)
        {
            Value = value;
            Forward = new List<SkipListNode<T>>(new SkipListNode<T>[level]);
        }
    }

    private SkipListNode<T> head;
    private int maxLevel;
    private Random random;

    public SkipList(int maxLevel)
    {
        this.maxLevel = maxLevel;
        head = new SkipListNode<T>(default, maxLevel);
        random = new Random();
    }

    public void Insert(T value)
    {
        var update = new SkipListNode<T>[maxLevel];
        var x = head;
        for (int i = maxLevel - 1; i >= 0; i--)
        {
            while (x.Forward[i] != null && x.Forward[i].Value.CompareTo(value) < 0)
            {
                x = x.Forward[i];
            }
            update[i] = x;
        }
        int level = RandomLevel();
        var newNode = new SkipListNode<T>(value, level);
        for (int i = 0; i < level; i++)
        {
            newNode.Forward[i] = update[i].Forward[i];
            update[i].Forward[i] = newNode;
        }
    }

    public bool Search(T value)
    {
        var x = head;
        for (int i = maxLevel - 1; i >= 0; i--)
        {
            while (x.Forward[i] != null && x.Forward[i].Value.CompareTo(value) < 0)
            {
                x = x.Forward[i];
            }
        }
        x = x.Forward[0];
        return x != null && x.Value.CompareTo(value) == 0;
    }

    private int RandomLevel()
    {
        int level = 1;
        while (random.NextDouble() < 0.5 && level < maxLevel)
        {
            level++;
        }
        return level;
    }
}