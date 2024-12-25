// Define the Graph class
namespace DataStructure;

public class Graph<T>(Dictionary<T, List<T>> pairs) where T : notnull
{
    // Dictionary to store the adjacency list
    protected Dictionary<T, List<T>> AdjacencyList { get; init; } = pairs;

    // Constructor to initialize the graph
    public Graph() : this([]) { }

    // Method to add a vertex to the graph
    public void AddVertex(T vertex)
    {
        if (!AdjacencyList.ContainsKey(vertex))
        {
            AdjacencyList[vertex] = [];
        }
    }

    // Method to add an edge to the graph
    public void AddEdge(T vertex1, T vertex2, bool isDirected = false)
    {
        // Add the edge from vertex1 to vertex2
        if (AdjacencyList.TryGetValue(vertex1, out List<T>? value1))
        {
            value1.Add(vertex2);
        }
        else
        {
            AdjacencyList[vertex1] = [ vertex2 ];
        }
        if(isDirected) return;
        // For an undirected edge, add the reverse edge as well
        if (AdjacencyList.TryGetValue(vertex2, out List<T>? value2))
        {
            value2.Add(vertex1);
        }
        else
        {
            AdjacencyList[vertex2] = [ vertex1 ];
        }
    }

    // Method to display the graph
    public void DisplayGraph()
    {
        foreach (T vertex in AdjacencyList.Keys)
        {
            Console.WriteLine($"{vertex} -> {string.Join(" ", AdjacencyList[vertex])}");
        }
    }

    public static readonly Graph<T> Empty = new Graph<T>();
}