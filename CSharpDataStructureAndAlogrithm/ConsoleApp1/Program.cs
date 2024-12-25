using DataStructure;

Graph<int> graph = new();

// Adding vertices
graph.AddVertex(1);
graph.AddVertex(2);
graph.AddVertex(3);
graph.AddVertex(4);

// Adding edges
graph.AddEdge(1, 2);
graph.AddEdge(1, 3);
graph.AddEdge(2, 4);
graph.AddEdge(3, 4);

// Display the graph
graph.DisplayGraph();