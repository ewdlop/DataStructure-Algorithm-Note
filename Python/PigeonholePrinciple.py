from typing import List, Set, Dict, Tuple, Any, Optional
from collections import defaultdict
import math
import random

class PigeonholePrinciple:
    """Implementation of various pigeonhole principle applications"""
    
    @staticmethod
    def find_duplicate(arr: List[int], range_start: int, range_end: int) -> Optional[int]:
        """
        Find a duplicate in array where elements are in range [range_start, range_end]
        Returns duplicate if exists, None otherwise
        Uses pigeonhole principle: if n+1 items are put into n holes, at least one hole has 2+ items
        """
        n = range_end - range_start + 1
        if len(arr) <= n:
            return None
            
        # Count occurrences
        counts = defaultdict(int)
        for num in arr:
            counts[num] += 1
            if counts[num] > 1:
                return num
        
        return None
    
    @staticmethod
    def find_same_sum_subsequence(arr: List[int]) -> Tuple[List[int], List[int]]:
        """
        Find two different subsequences with the same sum
        Uses pigeonhole: if n numbers taken from range [1,m], 
        there must be two with same sum if n > m
        """
        n = len(arr)
        # Generate all possible subsequences sums
        sums = {}  # sum -> subsequence mapping
        
        # We'll use binary representation to generate subsequences
        for mask in range(1, 2**n):
            subsequence = []
            total = 0
            for i in range(n):
                if mask & (1 << i):
                    subsequence.append(arr[i])
                    total += arr[i]
            
            if total in sums:
                # Found two subsequences with same sum
                return sums[total], subsequence
            sums[total] = subsequence
            
        return [], []
    
    @staticmethod
    def find_common_element_sequences(sequences: List[List[int]]) -> Optional[int]:
        """
        Find an element that appears in all sequences
        Uses pigeonhole: if total elements > (k-1)*n where k is number of sequences,
        there must be a common element
        """
        element_counts = defaultdict(int)
        
        for sequence in sequences:
            # Use set to count unique elements in each sequence
            seen = set()
            for num in sequence:
                if num not in seen:
                    element_counts[num] += 1
                    seen.add(num)
        
        # Check if any element appears in all sequences
        k = len(sequences)
        for element, count in element_counts.items():
            if count == k:
                return element
                
        return None
    
    @staticmethod
    def find_monotonic_subsequence(arr: List[int]) -> List[int]:
        """
        Find monotonic subsequence of length at least sqrt(n)+1
        Uses Erdős–Szekeres theorem based on pigeonhole principle
        """
        n = len(arr)
        if n < 2:
            return arr
            
        # For each position, store length of longest increasing and decreasing
        # subsequences ending at that position
        inc = [1] * n
        dec = [1] * n
        
        # Compute lengths
        for i in range(1, n):
            for j in range(i):
                if arr[i] > arr[j]:
                    inc[i] = max(inc[i], inc[j] + 1)
                elif arr[i] < arr[j]:
                    dec[i] = max(dec[i], dec[j] + 1)
        
        # Find maximum length and corresponding sequence
        max_inc = max(inc)
        max_dec = max(dec)
        
        # Reconstruct the longer sequence
        if max_inc >= max_dec:
            # Reconstruct increasing sequence
            sequence = []
            curr_len = max_inc
            curr_val = float('inf')
            for i in range(n-1, -1, -1):
                if inc[i] == curr_len and arr[i] < curr_val:
                    sequence.append(arr[i])
                    curr_len -= 1
                    curr_val = arr[i]
            return sequence[::-1]
        else:
            # Reconstruct decreasing sequence
            sequence = []
            curr_len = max_dec
            curr_val = float('-inf')
            for i in range(n-1, -1, -1):
                if dec[i] == curr_len and arr[i] > curr_val:
                    sequence.append(arr[i])
                    curr_len -= 1
                    curr_val = arr[i]
            return sequence[::-1]
    
    @staticmethod
    def find_repeated_sum_modulo(arr: List[int], m: int) -> Tuple[int, List[int]]:
        """
        Find subarray with sum divisible by m
        Uses pigeonhole: if we have m+1 prefix sums modulo m, 
        two must have same remainder
        """
        prefix_sum = 0
        prefix_sums = {0: -1}  # sum modulo m -> index mapping
        
        for i, num in enumerate(arr):
            prefix_sum = (prefix_sum + num) % m
            
            if prefix_sum in prefix_sums:
                # Found subarray with sum divisible by m
                start_idx = prefix_sums[prefix_sum] + 1
                return prefix_sum, arr[start_idx:i+1]
            
            prefix_sums[prefix_sum] = i
            
        return 0, []

def example_usage():
    """Demonstrate pigeonhole principle applications"""
    
    # Example 1: Find duplicate in range
    arr = [1, 2, 3, 4, 2, 5]
    duplicate = PigeonholePrinciple.find_duplicate(arr, 1, 5)
    print(f"Duplicate in {arr}: {duplicate}")
    
    # Example 2: Find subsequences with same sum
    arr = [1, 2, 3, 4, 5]
    seq1, seq2 = PigeonholePrinciple.find_same_sum_subsequence(arr)
    print(f"\nSubsequences with same sum in {arr}:")
    print(f"Sequence 1: {seq1}, Sum: {sum(seq1)}")
    print(f"Sequence 2: {seq2}, Sum: {sum(seq2)}")
    
    # Example 3: Find common element in sequences
    sequences = [
        [1, 2, 3, 4],
        [2, 4, 6, 8],
        [2, 3, 4, 5]
    ]
    common = PigeonholePrinciple.find_common_element_sequences(sequences)
    print(f"\nCommon element in sequences: {common}")
    
    # Example 4: Find monotonic subsequence
    arr = [5, 2, 8, 6, 3, 9, 1, 7, 4]
    monotonic = PigeonholePrinciple.find_monotonic_subsequence(arr)
    print(f"\nMonotonic subsequence in {arr}: {monotonic}")
    
    # Example 5: Find subarray with sum divisible by m
    arr = [1, 3, 2, 6, 4, 5]
    m = 3
    remainder, subarray = PigeonholePrinciple.find_repeated_sum_modulo(arr, m)
    print(f"\nSubarray with sum divisible by {m} in {arr}:")
    print(f"Subarray: {subarray}, Sum: {sum(subarray)}, Remainder: {remainder}")

if __name__ == "__main__":
    example_usage()
