import heapq
from typing import List, Tuple, Iterator, TypeVar, Generic
from dataclasses import dataclass
import numpy as np
from collections import defaultdict

T = TypeVar('T')

class ThreeWayPartition:
    """Three-way partitioning implementation"""
    
    @staticmethod
    def partition(arr: List[T], pivot1: T, pivot2: T) -> Tuple[int, int]:
        """
        Partition array into three parts: <pivot1, pivot1<=x<=pivot2, >pivot2
        Returns indices of partition points
        """
        if pivot1 > pivot2:
            pivot1, pivot2 = pivot2, pivot1
            
        low = 0
        high = len(arr) - 1
        mid = 0
        
        while mid <= high:
            if arr[mid] < pivot1:
                arr[low], arr[mid] = arr[mid], arr[low]
                low += 1
                mid += 1
            elif arr[mid] > pivot2:
                arr[mid], arr[high] = arr[high], arr[mid]
                high -= 1
            else:
                mid += 1
                
        return low, high

    @staticmethod
    def three_way_quicksort(arr: List[T]) -> None:
        """Sort array using three-way partitioning"""
        def _quicksort(arr: List[T], low: int, high: int) -> None:
            if high <= low:
                return
                
            # Choose two pivots
            if arr[low] > arr[high]:
                arr[low], arr[high] = arr[high], arr[low]
            pivot1, pivot2 = arr[low], arr[high]
            
            lt, gt = ThreeWayPartition.partition(arr[low:high+1], pivot1, pivot2)
            lt += low
            gt += low
            
            _quicksort(arr, low, lt-1)
            _quicksort(arr, lt, gt)
            _quicksort(arr, gt+1, high)
            
        _quicksort(arr, 0, len(arr)-1)

@dataclass
class ListNode(Generic[T]):
    """Node for linked list implementation"""
    value: T
    next: 'ListNode[T]' = None

class KWayMerge:
    """K-way merge implementation with three-way breakup"""
    
    @staticmethod
    def merge_k_sorted_arrays(arrays: List[List[T]]) -> List[T]:
        """Merge k sorted arrays using min heap"""
        heap: List[Tuple[T, int, int]] = []  # (value, array_index, element_index)
        result = []
        
        # Initialize heap with first element from each array
        for i, arr in enumerate(arrays):
            if arr:
                heapq.heappush(heap, (arr[0], i, 0))
                
        while heap:
            val, arr_idx, elem_idx = heapq.heappop(heap)
            result.append(val)
            
            if elem_idx + 1 < len(arrays[arr_idx]):
                next_val = arrays[arr_idx][elem_idx + 1]
                heapq.heappush(heap, (next_val, arr_idx, elem_idx + 1))
                
        return result

    @staticmethod
    def merge_k_sorted_lists(lists: List[ListNode[T]]) -> ListNode[T]:
        """Merge k sorted linked lists"""
        heap: List[Tuple[T, int]] = []  # (value, list_index)
        dummy = ListNode(None)  # Dummy head
        tail = dummy
        
        # Initialize heap with first node from each list
        for i, head in enumerate(lists):
            if head:
                heapq.heappush(heap, (head.value, i))
                lists[i] = head.next
                
        while heap:
            val, list_idx = heapq.heappop(heap)
            tail.next = ListNode(val)
            tail = tail.next
            
            if lists[list_idx]:
                heapq.heappush(heap, (lists[list_idx].value, list_idx))
                lists[list_idx] = lists[list_idx].next
                
        return dummy.next

class ThreeWayMergeSort:
    """Three-way merge sort implementation"""
    
    @staticmethod
    def sort(arr: List[T]) -> List[T]:
        """Sort array using three-way merge sort"""
        if len(arr) <= 1:
            return arr
            
        # Divide array into three parts
        third = len(arr) // 3
        left = ThreeWayMergeSort.sort(arr[:third])
        mid = ThreeWayMergeSort.sort(arr[third:2*third])
        right = ThreeWayMergeSort.sort(arr[2*third:])
        
        # Merge three sorted arrays
        return ThreeWayMergeSort._merge(left, mid, right)
    
    @staticmethod
    def _merge(left: List[T], mid: List[T], right: List[T]) -> List[T]:
        """Merge three sorted arrays"""
        result = []
        i = j = k = 0
        
        while i < len(left) and j < len(mid) and k < len(right):
            min_val = min(left[i], mid[j], right[k])
            if min_val == left[i]:
                result.append(left[i])
                i += 1
            elif min_val == mid[j]:
                result.append(mid[j])
                j += 1
            else:
                result.append(right[k])
                k += 1
                
        # Merge remaining elements
        while i < len(left) and j < len(mid):
            if left[i] <= mid[j]:
                result.append(left[i])
                i += 1
            else:
                result.append(mid[j])
                j += 1
                
        while j < len(mid) and k < len(right):
            if mid[j] <= right[k]:
                result.append(mid[j])
                j += 1
            else:
                result.append(right[k])
                k += 1
                
        while i < len(left) and k < len(right):
            if left[i] <= right[k]:
                result.append(left[i])
                i += 1
            else:
                result.append(right[k])
                k += 1
                
        # Append remaining elements
        result.extend(left[i:])
        result.extend(mid[j:])
        result.extend(right[k:])
        
        return result

class ExternalThreeWayMerge:
    """External three-way merge for large datasets"""
    
    @staticmethod
    def external_sort(iterator: Iterator[T], buffer_size: int = 1000) -> Iterator[T]:
        """Sort large dataset using external three-way merge"""
        # Phase 1: Create sorted runs
        runs: List[List[T]] = []
        buffer = []
        
        for item in iterator:
            buffer.append(item)
            if len(buffer) >= buffer_size:
                buffer.sort()
                runs.append(buffer)
                buffer = []
                
        if buffer:
            buffer.sort()
            runs.append(buffer)
            
        # Phase 2: Merge runs three at a time
        while len(runs) > 1:
            merged_runs = []
            i = 0
            while i < len(runs):
                if i + 2 < len(runs):
                    merged = ThreeWayMergeSort._merge(runs[i], runs[i+1], runs[i+2])
                    merged_runs.append(merged)
                    i += 3
                elif i + 1 < len(runs):
                    merged = KWayMerge.merge_k_sorted_arrays([runs[i], runs[i+1]])
                    merged_runs.append(merged)
                    i += 2
                else:
                    merged_runs.append(runs[i])
                    i += 1
                    
            runs = merged_runs
            
        # Return iterator over final sorted run
        return iter(runs[0] if runs else [])

def example_usage():
    """Demonstrate usage of three-way merge implementations"""
    
    # Example 1: Three-way partitioning
    arr = [5, 2, 9, 1, 7, 6, 3]
    ThreeWayPartition.three_way_quicksort(arr)
    print("Sorted array:", arr)
    
    # Example 2: K-way merge of sorted arrays
    arrays = [
        [1, 4, 7],
        [2, 5, 8],
        [3, 6, 9]
    ]
    merged = KWayMerge.merge_k_sorted_arrays(arrays)
    print("Merged arrays:", merged)
    
    # Example 3: Three-way merge sort
    arr = [9, 3, 7, 1, 8, 2, 6, 4, 5]
    sorted_arr = ThreeWayMergeSort.sort(arr)
    print("Three-way merge sorted:", sorted_arr)
    
    # Example 4: External three-way merge
    large_dataset = iter(np.random.randint(0, 1000, size=10000))
    sorted_iterator = ExternalThreeWayMerge.external_sort(large_dataset, buffer_size=100)
    first_ten = list(sorted_iterator)[:10]
    print("First 10 elements after external sort:", first_ten)

if __name__ == "__main__":
    example_usage()
