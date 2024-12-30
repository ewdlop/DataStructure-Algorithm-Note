public class MaxHeap
{
    private List<int> heap;

    public MaxHeap()
    {
        heap = new List<int>();
    }

    // Get the index of the parent of the node at index i
    private int Parent(int i) => (i - 1) / 2;

    // Get the index of the left child of the node at index i
    private int LeftChild(int i) => 2 * i + 1;

    // Get the index of the right child of the node at index i
    private int RightChild(int i) => 2 * i + 2;

    // Insert a new element into the heap
    public void Insert(int key)
    {
        heap.Add(key);
        int i = heap.Count - 1;

        // Fix the max heap property if it's violated
        while (i != 0 && heap[Parent(i)] < heap[i])
        {
            Swap(i, Parent(i));
            i = Parent(i);
        }
    }

    // Extract the root element (maximum element) from the heap
    public int ExtractMax()
    {
        if (heap.Count <= 0)
            throw new InvalidOperationException("Heap is empty");

        if (heap.Count == 1)
        {
            int temp_root = heap[0];
            heap.RemoveAt(0);
            return temp_root;
        }

        int root = heap[0];
        heap[0] = heap[heap.Count - 1];
        heap.RemoveAt(heap.Count - 1);

        MaxHeapify(0);

        return root;
    }

    // A utility function to maintain the max heap property
    private void MaxHeapify(int i)
    {
        int left = LeftChild(i);
        int right = RightChild(i);
        int largest = i;

        if (left < heap.Count && heap[left] > heap[largest])
            largest = left;

        if (right < heap.Count && heap[right] > heap[largest])
            largest = right;

        if (largest != i)
        {
            Swap(i, largest);
            MaxHeapify(largest);
        }
    }

    // Utility function to swap two elements in the heap
    private void Swap(int i, int j)
    {
        int temp = heap[i];
        heap[i] = heap[j];
        heap[j] = temp;
    }

    // Print the heap
    public void PrintHeap()
    {
        foreach (var item in heap)
        {
            Console.Write(item + " ");
        }
        Console.WriteLine();
    }
}