// Define the Graph class

namespace DataStructure;

public class Graph<T>(Dictionary<T, List<T>>? pairs) where T : notnull
{
    // Dictionary to store the adjacency list for each vertex.
    public virtual Dictionary<T, List<T>>? AdjacencyListDictionary { get; protected set; } = pairs;

    // For backward compatibility
    public Dictionary<T, List<T>>? AdjacencyList
    {
        get => AdjacencyListDictionary;
        set
        {
            AdjacencyListDictionary = value;
        }
    }

    // Constructor to initialize the graph
    public Graph() : this([]) { }

    // Method to add a vertex to the graph
    public virtual void AddVertex(T vertex)
    {
        if (!(AdjacencyListDictionary ??= []).ContainsKey(vertex))
        {
            AdjacencyListDictionary[vertex] = [];
        }
    }

    // Method to add an edge to the graph
    public virtual void AddEdge(T vertex1, T vertex2, bool isDirected = false)
    {
        // Add the edge from vertex1 to vertex2
        if ((AdjacencyListDictionary ??= []).TryGetValue(vertex1, out List<T>? value1))
        {
            (value1 ??= []).Add(vertex2);
        }
        else
        {
            AdjacencyListDictionary[vertex1] = [ vertex2 ];
        }
        if(isDirected) return;
        // For an undirected edge, add the reverse edge as well
        if (AdjacencyListDictionary.TryGetValue(vertex2, out List<T>? value2))
        {
            (value2 ??= []).Add(vertex1);
        }
        else
        {
            AdjacencyListDictionary[vertex2] = [ vertex1 ];
        }
    }

    // Method to display the graph
    public virtual void DisplayGraph()
    {
        foreach (T vertex in (AdjacencyListDictionary ??= []).Keys)
        {
            Console.WriteLine($"{vertex} -> {string.Join(" ", AdjacencyListDictionary[vertex])}");
        }
    }

    public virtual void Invert()
    {
        Dictionary<T, List<T>> invertedPairs = [];
        foreach (T vertex in (AdjacencyListDictionary ??= []).Keys)
        {
            foreach (T adjacent in AdjacencyListDictionary[vertex])
            {
                if (invertedPairs.TryGetValue(adjacent, out List<T>? value))
                {
                    (value ??= []).Add(vertex);
                }
                else
                {
                    invertedPairs[adjacent] = [vertex];
                }
            }
        }
        AdjacencyListDictionary = invertedPairs;
    }

    public static readonly Graph<T> Empty = new Graph<T>();
}
