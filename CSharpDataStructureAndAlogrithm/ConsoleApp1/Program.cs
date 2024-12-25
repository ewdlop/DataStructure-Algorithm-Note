using DataStructure;
using System.Collections;
using System.Collections.Generic;

{
    Queue<int> queue = [];
    queue.Enqueue(1);
    queue.Enqueue(2);
    queue.Enqueue(3);
    queue.Enqueue(4);
    queue.Enqueue(5);


    Console.WriteLine(queue.Dequeue()); // 1
    Console.WriteLine(queue.Dequeue()); // 2
    Console.WriteLine(queue.Dequeue()); // 3
    Console.WriteLine(queue.Dequeue()); // 4
    Console.WriteLine(queue.Dequeue()); // 5
}
{
    Stack<int> stack = [];
    stack.Push(1);
    stack.Push(2);
    stack.Push(3);
    stack.Push(4);
    stack.Push(5);

    Console.WriteLine(stack.Pop()); // 5
    Console.WriteLine(stack.Pop()); // 4
    Console.WriteLine(stack.Pop()); // 3
    Console.WriteLine(stack.Pop()); // 2
    Console.WriteLine(stack.Pop()); // 1
}
{
    LinkedList<int> linkedList = [];
    linkedList.AddFirst(1);
    linkedList.AddFirst(2);

    linkedList.AddFirst(3);
    if (linkedList.First is not null)
    {

        linkedList.AddAfter(linkedList.First, 4);
        linkedList.AddBefore(linkedList.First, 5);
        Console.WriteLine(linkedList.First.Value); // 5

        if (linkedList.Last is not null)
        {
            Console.WriteLine(linkedList.Last.Value); // 1
        }
    }
}
{
    Dictionary<int, string> dictionary = new()
    {
        { 1, "One" },
        { 2, "Two" }
    };
    dictionary.Add(3, value: "Three");
    dictionary.Add(4, "Four");

    dictionary.TryGetValue(1, out string? value);
    Console.WriteLine(value); // One
}
{
    List<int> list = [1, 2, 3, 4, 5, 6];
    list.Insert(0, 0);
    list.Remove(3);
    list.RemoveAt(0);
}
{
    HashSet<int> hashSet = [1, 2, 3, 4, 4, 6];
    hashSet.Remove(3);
    hashSet.Add(4);
    hashSet.Add(5);
    Console.WriteLine(hashSet.Count); // 5
}
{
    ArrayList arrayList = [decimal.MaxValue, decimal.MinValue, decimal.MinusOne, 5];
}
{
    PriorityQueue<int, int> priorityQueue = new();
    priorityQueue.Enqueue(1, 1);
    priorityQueue.Enqueue(2, 2);
    priorityQueue.Enqueue(3, 3);

    // Dequeue the elements
    Console.WriteLine(priorityQueue.Dequeue()); // 1

    priorityQueue.Enqueue(4, 1);

    Console.WriteLine(priorityQueue.Dequeue()); // 4
    Console.WriteLine(priorityQueue.Dequeue()); // 2
}
{
    Graph<int> graph = new();
    // Adding vertices
    graph.AddVertex(1);
    graph.AddVertex(2);
    graph.AddVertex(3);
    graph.AddVertex(4);
    // Adding edgess
    graph.AddEdge(1, 2, isDirected: true);
    graph.AddEdge(1, 3);
    graph.AddEdge(2, 4);
    graph.AddEdge(3, 4);
    // Display the graph
    graph.DisplayGraph();

    graph.Invert();
    graph.DisplayGraph();
}
{
}