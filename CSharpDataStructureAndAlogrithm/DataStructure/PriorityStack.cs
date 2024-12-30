namespace DataStructure;

public class PriorityStack<T>
{
    private SortedDictionary<int, Stack<T>> stacks;
    private int count;

    public PriorityStack()
    {
        stacks = new SortedDictionary<int, Stack<T>>();
        count = 0;
    }

    // Push an element onto the stack with a given priority
    public void Push(T item, int priority)
    {
        if (!stacks.ContainsKey(priority))
        {
            stacks[priority] = new Stack<T>();
        }
        stacks[priority].Push(item);
        count++;
    }

    // Pop the highest priority element from the stack
    public T Pop()
    {
        if (count == 0)
        {
            throw new InvalidOperationException("The priority stack is empty.");
        }

        var highestPriority = stacks.Keys.Max();
        var stack = stacks[highestPriority];
        var item = stack.Pop();
        count--;

        if (stack.Count == 0)
        {
            stacks.Remove(highestPriority);
        }

        return item;
    }

    // Peek the highest priority element without removing it
    public T Peek()
    {
        if (count == 0)
        {
            throw new InvalidOperationException("The priority stack is empty.");
        }

        var highestPriority = stacks.Keys.Max();
        return stacks[highestPriority].Peek();
    }

    // Check if the priority stack is empty
    public bool IsEmpty()
    {
        return count == 0;
    }

    // Get the number of elements in the priority stack
    public int Count()
    {
        return count;
    }
}