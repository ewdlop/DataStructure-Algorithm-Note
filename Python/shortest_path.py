from typing import Dict, List, Set, Tuple, Optional
import heapq
from collections import defaultdict
from dataclasses import dataclass
import math

@dataclass
class Edge:
    """Graph edge with weight"""
    src: int
    dst: int
    weight: float

class Graph:
    """Graph representation supporting both algorithms"""
    
    def __init__(self, vertices: int):
        self.V = vertices
        self.adj_list: Dict[int, List[Tuple[int, float]]] = defaultdict(list)
        self.edges: List[Edge] = []
    
    def add_edge(self, src: int, dst: int, weight: float) -> None:
        """Add edge to graph"""
        self.adj_list[src].append((dst, weight))
        self.edges.append(Edge(src, dst, weight))
    
    def get_edges(self) -> List[Edge]:
        """Get all edges"""
        return self.edges
    
    def get_neighbors(self, vertex: int) -> List[Tuple[int, float]]:
        """Get neighbors and weights for vertex"""
        return self.adj_list[vertex]

class DijkstraAlgorithm:
    """Dijkstra's Shortest Path Algorithm Implementation"""
    
    def __init__(self, graph: Graph):
        self.graph = graph
        self.distances: Dict[int, float] = {}
        self.predecessors: Dict[int, Optional[int]] = {}
        
    def find_shortest_paths(self, source: int) -> Tuple[Dict[int, float], Dict[int, Optional[int]]]:
        """
        Find shortest paths from source to all vertices
        Returns (distances, predecessors)
        """
        # Initialize distances and predecessors
        self.distances = {v: float('inf') for v in range(self.graph.V)}
        self.predecessors = {v: None for v in range(self.graph.V)}
        self.distances[source] = 0
        
        # Priority queue of vertices with distances
        # Format: (distance, vertex)
        pq = [(0, source)]
        visited: Set[int] = set()
        
        while pq:
            current_dist, current = heapq.heappop(pq)
            
            # Skip if vertex already processed
            if current in visited:
                continue
                
            visited.add(current)
            
            # Process neighbors
            for neighbor, weight in self.graph.get_neighbors(current):
                distance = current_dist + weight
                
                # Update if shorter path found
                if distance < self.distances[neighbor]:
                    self.distances[neighbor] = distance
                    self.predecessors[neighbor] = current
                    heapq.heappush(pq, (distance, neighbor))
        
        return self.distances, self.predecessors
    
    def get_path(self, target: int) -> List[int]:
        """Reconstruct path to target vertex"""
        if target not in self.predecessors:
            return []
            
        path = []
        current = target
        while current is not None:
            path.append(current)
            current = self.predecessors[current]
            
        return path[::-1]
    
    def has_negative_weights(self) -> bool:
        """Check if graph has negative weights"""
        return any(edge.weight < 0 for edge in self.graph.get_edges())

class BellmanFordAlgorithm:
    """Bellman-Ford Shortest Path Algorithm Implementation"""
    
    def __init__(self, graph: Graph):
        self.graph = graph
        self.distances: Dict[int, float] = {}
        self.predecessors: Dict[int, Optional[int]] = {}
        
    def find_shortest_paths(self, source: int) -> Tuple[Dict[int, float], Dict[int, Optional[int]], bool]:
        """
        Find shortest paths from source to all vertices
        Returns (distances, predecessors, has_negative_cycle)
        """
        # Initialize distances and predecessors
        self.distances = {v: float('inf') for v in range(self.graph.V)}
        self.predecessors = {v: None for v in range(self.graph.V)}
        self.distances[source] = 0
        
        # Relax edges |V| - 1 times
        for _ in range(self.graph.V - 1):
            for edge in self.graph.get_edges():
                if (self.distances[edge.src] != float('inf') and
                    self.distances[edge.src] + edge.weight < self.distances[edge.dst]):
                    self.distances[edge.dst] = self.distances[edge.src] + edge.weight
                    self.predecessors[edge.dst] = edge.src
        
        # Check for negative cycles
        has_negative_cycle = False
        for edge in self.graph.get_edges():
            if (self.distances[edge.src] != float('inf') and
                self.distances[edge.src] + edge.weight < self.distances[edge.dst]):
                has_negative_cycle = True
                break
        
        return self.distances, self.predecessors, has_negative_cycle
    
    def get_path(self, target: int) -> List[int]:
        """Reconstruct path to target vertex"""
        if target not in self.predecessors:
            return []
            
        path = []
        current = target
        visited = set()
        
        while current is not None:
            if current in visited:  # Detect cycles
                return []  # Cycle detected
            visited.add(current)
            path.append(current)
            current = self.predecessors[current]
            
        return path[::-1]

class ShortestPathVisualizer:
    """Visualize shortest paths and graph structure"""
    
    @staticmethod
    def print_path(path: List[int], distances: Dict[int, float], target: int) -> None:
        """Print path and distance information"""
        if not path:
            print(f"No path exists to vertex {target}")
            return
            
        path_str = " -> ".join(str(v) for v in path)
        distance = distances[target]
        print(f"Path to {target}: {path_str}")
        print(f"Total distance: {distance}")
    
    @staticmethod
    def print_distances(distances: Dict[int, float]) -> None:
        """Print distances to all vertices"""
        print("\nDistances from source:")
        for vertex, distance in sorted(distances.items()):
            if distance == float('inf'):
                print(f"Vertex {vertex}: INFINITY")
            else:
                print(f"Vertex {vertex}: {distance}")

def example_usage():
    """Demonstrate usage of shortest path algorithms"""
    
    # Create example graph
    graph = Graph(5)
    graph.add_edge(0, 1, 4)
    graph.add_edge(0, 2, 2)
    graph.add_edge(1, 2, 3)
    graph.add_edge(1, 3, 2)
    graph.add_edge(2, 3, 1)
    graph.add_edge(3, 4, 5)
    
    # Test Dijkstra's algorithm
    print("\nTesting Dijkstra's Algorithm:")
    dijkstra = DijkstraAlgorithm(graph)
    distances, predecessors = dijkstra.find_shortest_paths(0)
    
    # Print results
    ShortestPathVisualizer.print_distances(distances)
    path = dijkstra.get_path(4)
    ShortestPathVisualizer.print_path(path, distances, 4)
    
    # Create graph with negative weights
    neg_graph = Graph(5)
    neg_graph.add_edge(0, 1, 4)
    neg_graph.add_edge(0, 2, 2)
    neg_graph.add_edge(1, 2, -3)
    neg_graph.add_edge(2, 3, 1)
    neg_graph.add_edge(3, 1, -2)
    
    # Test Bellman-Ford algorithm
    print("\nTesting Bellman-Ford Algorithm:")
    bellman_ford = BellmanFordAlgorithm(neg_graph)
    distances, predecessors, has_negative_cycle = bellman_ford.find_shortest_paths(0)
    
    if has_negative_cycle:
        print("Graph contains negative cycle!")
    else:
        ShortestPathVisualizer.print_distances(distances)
        path = bellman_ford.get_path(3)
        ShortestPathVisualizer.print_path(path, distances, 3)

if __name__ == "__main__":
    example_usage()
