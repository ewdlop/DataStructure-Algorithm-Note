using Algorithm;
using DataStructure;
using DataStructure.Models;
using System.Collections;
using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Runtime.InteropServices;
using static DataStructure.BPlusTree;

{
    Console.WriteLine("Queue");

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
    Console.WriteLine("Stack");

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
    Console.WriteLine("LinkedList");

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

    Console.WriteLine("Dictionary");

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
    Console.WriteLine("list");

    List<int> list = [1, 2, 3, 4, 5, 6];
    list.Insert(0, 0);
    list.Remove(3);
    list.RemoveAt(0);

    Console.WriteLine(list.Count); // 5
}
{
    Console.WriteLine("HashSet");

    HashSet<int> hashSet = [1, 2, 3, 4, 4, 6];
    hashSet.Remove(3);
    hashSet.Add(4);
    hashSet.Add(5);

    Console.WriteLine(hashSet.Count); // 5
}
{
    Console.WriteLine("ArrayList");

    ArrayList arrayList = [decimal.MaxValue, decimal.MinValue, decimal.MinusOne, 5];

    Console.WriteLine(arrayList.Count); // 4
    Console.WriteLine(arrayList[0]); // 79228162514264337593543950335m
    Console.WriteLine(arrayList[1]); // -79228162514264337593543950335m
    Console.WriteLine(arrayList[2]); // -1
    Console.WriteLine(arrayList[3]); // 5

}
{
    Console.WriteLine("PriorityQueue");

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
    Console.WriteLine("Graph");

    Graph<int> graph = new();
    // Adding vertices
    graph.AddVertex(1);
    graph.AddVertex(2);
    graph.AddVertex(3);
    graph.AddVertex(4);

    // Adding edges
    graph.AddEdge(1, 2, isDirected: true);
    graph.AddEdge(1, 3);
    graph.AddEdge(2, 4);
    graph.AddEdge(3, 4);
    // Display the graph
    graph.DisplayGraph();

    Console.WriteLine("Inverted Graph");
    graph.Invert();
    graph.DisplayGraph();
}
{
    Rope rope = new("Hello, World!");
    Console.WriteLine(rope[0..5]); // Hello
}
{
    SplayTree<int> tree = new SplayTree<int>();

    tree.Insert(10);
    tree.Insert(20);
    tree.Insert(30);
    tree.Insert(40);
    tree.Insert(50);
    tree.Insert(25);

    Console.WriteLine("In-order traversal of the tree:");
    tree.PrintTree();

    Console.WriteLine("Search for 20 in the tree:");
    bool found = tree.Search(20);
    Console.WriteLine(found ? "Found" : "Not Found");

    Console.WriteLine("In-order traversal after searching for 20:");
    tree.PrintTree();

    Console.WriteLine("Delete 20 from the tree:");
    tree.Delete(20);

    Console.WriteLine("In-order traversal after deleting 20:");
    tree.PrintTree();
}
{
    string inputFilePath = "input.txt";
    string outputFilePath = "output.txt";
    int chunkSize = 1000; // Number of integers that fit into memory

    try
    {
        List<string> tempFileNames = Sorter.SortAndSaveChunks(inputFilePath, chunkSize);
        Sorter.NWayMerge(tempFileNames, outputFilePath);
        Console.WriteLine("Sorting completed. Output written to " + outputFilePath);
    }
    catch (Exception ex)
    {
        Console.WriteLine("An error occurred: " + ex.Message);
    }
}
{
    BTree tree = new BTree(3);
    // Insertions would go here...

    tree.Traverse();
    Console.WriteLine();
    int key = 10;
    BTree.BTreeNode result = tree.Search(key);
    Console.WriteLine(result != null ? "Key found" : "Key not found");
}
{
    BPlusTree tree = new BPlusTree(3);
    // Insertions would go here...

    tree.Traverse();
    Console.WriteLine();

    int key = 10;
    BPlusTreeNode result = tree.Search(key);
    Console.WriteLine(result != null ? "Key found" : "Key not found");
}
{
    RTree rTree = new RTree();

    rTree.Insert(new Rectangle(1, 1, 2, 2));
    rTree.Insert(new Rectangle(2, 2, 3, 3));
    rTree.Insert(new Rectangle(3, 3, 4, 4));
    rTree.Insert(new Rectangle(4, 4, 5, 5));
    rTree.Insert(new Rectangle(5, 5, 6, 6));

    Rectangle query = new Rectangle(2.5, 2.5, 4.5, 4.5);
    List<Rectangle> result = rTree.Search(query);

    Console.WriteLine("Rectangles intersecting with query:");
    foreach (var rect in result)
    {
        Console.WriteLine($"({rect.MinX}, {rect.MinY}) - ({rect.MaxX}, {rect.MaxY})");
    }
}
{
    MinHeap minHeap = new MinHeap();

    minHeap.Insert(3);
    minHeap.Insert(2);
    minHeap.Insert(15);
    minHeap.Insert(5);
    minHeap.Insert(4);
    minHeap.Insert(45);

    Console.WriteLine("Min-Heap:");
    minHeap.PrintHeap();

    Console.WriteLine("Extracted Min: " + minHeap.ExtractMin());

    Console.WriteLine("Min-Heap after extraction:");
    minHeap.PrintHeap();
}
{
    System.Collections.Frozen.FrozenDictionary<int, StudentName> frozenDictionary = Enumerable.Range(1, 10)
        .Select(static i => new StudentName($"First-{i}", LastName: $"Last-{i}", ID: i))
        .ToFrozenDictionary(x => x.ID, x => x);

    //immutable dictionary
    System.Collections.Frozen.FrozenDictionary<int, StudentName> frozenDictionary2 = Enumerable.Range(1, 10)
        .Select(static i => new StudentName($"First-{i}", LastName: $"Last-{i}", ID: i))
        .ToFrozenDictionary(x => x.ID, x => x);

    frozenDictionary2.TryAdd(11, new StudentName("First-11", "Last-11", 11));
    frozenDictionary2.TryAdd(12, new StudentName("First-12", "Last-12", 12));
    frozenDictionary2.TryAdd(13, new StudentName("First-13", "Last-13", 13));

    //immutable dictionary

    //https://github.com/dotnet/csharplang/discussions/96


    frozenDictionary.Equals(frozenDictionary2).Print();
    frozenDictionary.SequenceEqual(frozenDictionary2).Print();
}
ImmutableArray<int> immutableArray2 = ImmutableArray.Create(1, 2, 3, 4, 5);
{
    ImmutableArray<int> immutableArray = ImmutableArray.Create(1, 2, 3, 4, 5);
    immutableArray.Equals(immutableArray2).Print();
    immutableArray.SequenceEqual(immutableArray2).Print();
    immutableArray2 = immutableArray.Add(6);
}
ImmutableDictionary<int, StudentName> immutableDictionary = Enumerable.Range(1, 10)
        .Select(static i => new StudentName($"First-{i}", LastName: $"Last-{i}", ID: i))
        .ToImmutableDictionary(x => x.ID, x => x);
{
    ImmutableDictionary<int, StudentName> immutableDictionary2 = Enumerable.Range(1, 10)
        .Select(static i => new StudentName($"First-{i}", LastName: $"Last-{i}", ID: i))
        .ToImmutableDictionary(x => x.ID, x => x);
    immutableDictionary.Equals(immutableDictionary2).Print();
    immutableDictionary.SequenceEqual(immutableDictionary2).Print();
    immutableDictionary2 = immutableDictionary.Add(11, new StudentName("First-11", "Last-11", 11));
}
{
    ImmutableHashSet<int> immutableHashSet = ImmutableHashSet.Create(1, 2, 3, 4, 5);
    ImmutableHashSet<int> immutableHashSet2 = ImmutableHashSet.Create(1, 2, 3, 4, 5);
    immutableHashSet.Equals(immutableHashSet2).Print();
    immutableHashSet.SequenceEqual(immutableHashSet2).Print();
    immutableHashSet.Add(6).Print();
}
{
    Console.WriteLine("ImmutableQueue");

    ImmutableQueue<int> immutableQueue = ImmutableQueue.Create(1, 2, 3, 4, 5);
    ImmutableQueue<int> immutableQueue2 = ImmutableQueue.Create(1, 2, 3, 4, 5);
    immutableQueue.Equals(immutableQueue2).Print();
    immutableQueue.SequenceEqual(immutableQueue2).Print();
    immutableQueue.Dequeue();
    immutableQueue.GetHashCode().Print();
    immutableQueue.Dequeue();
    immutableQueue.GetHashCode().Print();
    foreach (int item in immutableQueue)
    {
        Console.Write(item);
    }
    immutableQueue.Dequeue();
    immutableQueue.GetHashCode().Print();
    immutableQueue.Dequeue();
    immutableQueue.GetHashCode().Print();
    immutableQueue = immutableQueue.Dequeue();

    immutableQueue.GetHashCode().Print();
    if (immutableQueue.IsEmpty)
    {
        Console.WriteLine("Queue is empty");
    }
    immutableQueue.Enqueue(1);
}
{
    ImmutableStack<int> immutableStack = ImmutableStack.Create(1, 2, 3, 4, 5);
    ImmutableStack<int> immutableStack2 = ImmutableStack.Create(1, 2, 3, 4, 5);
    immutableStack.Equals(immutableStack2).Print();
    immutableStack.SequenceEqual(immutableStack2).Print();
}
ImmutableSortedDictionary<int, StudentName> immutableSortedDictionary = Enumerable.Range(1, 10)
    .Select(static i => new StudentName($"First-{i}", LastName: $"Last-{i}", ID: i))
    .ToImmutableSortedDictionary(x => x.ID, x => x);
{
    ImmutableSortedDictionary<int, StudentName> immutableSortedDictionary2 = Enumerable.Range(1, 10)
        .Select(static i => new StudentName($"First-{i}", LastName: $"Last-{i}", ID: i))
        .ToImmutableSortedDictionary(x => x.ID, x => x);

    immutableSortedDictionary.Equals(immutableSortedDictionary2).Print();

    _ = Task.Run(() =>
    {
        immutableSortedDictionary.Equals(immutableSortedDictionary2).ThreadSafePrint();
        foreach (KeyValuePair<int, StudentName> item in immutableSortedDictionary)
        {
            Console.WriteLine(item);
        }
    });
    _ = Task.Run(() =>
    {
        immutableSortedDictionary.SequenceEqual(immutableSortedDictionary2).ThreadSafePrint();
        foreach (KeyValuePair<int, StudentName> item in immutableSortedDictionary2)
        {
            Console.WriteLine(item);
        }
    });
}
{
    ImmutableList<int> immutableList = ImmutableList.Create(1, 2, 3, 4, 5);
    _ = Task.Run(() =>
    {
        ImmutableInterlocked.Update(ref immutableList, t=>t.Add(7)).ThreadSafePrint();
        ImmutableInterlocked.TryAdd(ref immutableDictionary, 11, new StudentName("First-11", "Last-11", 11)).ThreadSafePrint();
    });

    _ = Task.Run(() =>
    {
        immutableList.Equals(ImmutableList.Create(1, 2, 3, 4, 5)).ThreadSafePrint();
        foreach (int item in immutableList)
        {
            Console.WriteLine(item);
        }
        ImmutableInterlocked.Update(ref immutableList, t => t.Remove(7)).ThreadSafePrint();

    });
}
{
    ImmutableArray<int> immutableArray = ImmutableCollectionsMarshal.AsImmutableArray([1, 2, 3, 4, 5]);
    ImmutableCollectionsMarshal.AsImmutableArray(new Rope("").ToArray()).Print();
    //reference type
    int[] array = ImmutableCollectionsMarshal.AsArray(immutableArray2) ?? [];
    //struct\
    //slide window = buffer
    Memory<int> memory = array.AsMemory();
}
{
    Console.WriteLine("Enter a word:");
    string word = Console.ReadLine()?.ToUpper() ?? string.Empty;

    Console.WriteLine($"\n1. Alphabet Position Factorial: {word.CalculateAlphabetPositionFactorial()}");

    Console.WriteLine($"\n2. Permutational Factorial: {StringExtensions.CalculatePermutationalFactorial(word)}");

    Console.WriteLine($"\n3. Word Length Factorial: {StringExtensions.CalculateWordLengthFactorial(word)}");

    Console.WriteLine($"\n4. ASCII Factorial: {StringExtensions.CalculateASCIIFactorial(word)}");
}