# DataStructureNote

## Overview
This project is a collection of data structures and algorithms implemented in C#. It includes various data structures such as graphs, inverted indexes, ropes, and more. The project also provides implementations of common algorithms like bubble sort.

## Data Structures and Algorithms
### Data Structures
- **Graph**: A class representing a graph data structure with methods to add vertices, add edges, display the graph, and invert the graph.
- **Inverted Index**: A class representing an inverted index, which is used to map terms to the documents that contain them.
- **Rope**: A class representing a rope data structure, which is a binary tree used to store and manipulate strings efficiently.

### Algorithms
- **Bubble Sort**: An implementation of the bubble sort algorithm with various extension methods for sorting collections.

## Building and Running the Project
To build and run the project, follow these steps:
1. Ensure you have .NET 9.0 SDK installed on your machine.
2. Clone the repository: `git clone https://github.com/ewdlop/DataStructure-Algorithm-Note.git`
3. Navigate to the project directory: `cd DataStructure-Algorithm-Note/CSharpDataStructureAndAlogrithm`
4. Build the project: `dotnet build`
5. Run the project: `dotnet run --project ConsoleApp1`

## Examples
### Bubble Sort
The following example demonstrates how to use the bubble sort implementation provided in the project:
```csharp
using Algorithm;

List<int> numbers = new List<int> { 5, 3, 8, 4, 2 };
IEnumerable<int> sortedNumbers = numbers.AsBubbleSortEnumerable();
foreach (int number in sortedNumbers)
{
    Console.WriteLine(number);
}
```

## Dependencies
This project requires the following tools and libraries:
- .NET 9.0 SDK

## Contributing
Contributions are welcome! If you would like to contribute to the project, please follow these guidelines:
1. Fork the repository.
2. Create a new branch for your feature or bugfix.
3. Make your changes and commit them with descriptive commit messages.
4. Push your changes to your forked repository.
5. Create a pull request to the main repository.

If you encounter any issues or have any questions, please open an issue on the GitHub repository.
