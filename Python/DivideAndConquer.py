import numpy as np
from typing import List, Tuple, Any, Dict, Optional
from concurrent.futures import ProcessPoolExecutor, ThreadPoolExecutor
import heapq
from collections import defaultdict
from functools import lru_cache
import time

class DivideAndConquer:
    """Implementation of Divide and Conquer algorithms"""
    
    @staticmethod
    def merge_sort(arr: List[int]) -> List[int]:
        """Classic merge sort implementation"""
        if len(arr) <= 1:
            return arr
            
        mid = len(arr) // 2
        left = DivideAndConquer.merge_sort(arr[:mid])
        right = DivideAndConquer.merge_sort(arr[mid:])
        
        return DivideAndConquer._merge(left, right)
    
    @staticmethod
    def _merge(left: List[int], right: List[int]) -> List[int]:
        """Helper function for merge sort"""
        result = []
        i = j = 0
        
        while i < len(left) and j < len(right):
            if left[i] <= right[j]:
                result.append(left[i])
                i += 1
            else:
                result.append(right[j])
                j += 1
                
        result.extend(left[i:])
        result.extend(right[j:])
        return result
    
    @staticmethod
    def closest_pair(points: List[Tuple[float, float]]) -> Tuple[Tuple[float, float], Tuple[float, float], float]:
        """Find closest pair of points in 2D space"""
        def distance(p1: Tuple[float, float], p2: Tuple[float, float]) -> float:
            return ((p1[0] - p2[0])**2 + (p1[1] - p2[1])**2)**0.5
        
        def closest_split_pair(px: List[Tuple[float, float]], py: List[Tuple[float, float]], d: float) -> Tuple[Optional[Tuple[float, float]], Optional[Tuple[float, float]], float]:
            mid_x = px[len(px)//2][0]
            sy = [p for p in py if mid_x - d <= p[0] <= mid_x + d]
            
            best = d
            best_pair = (None, None)
            
            for i in range(len(sy)):
                for j in range(i+1, min(i+7, len(sy))):
                    dist = distance(sy[i], sy[j])
                    if dist < best:
                        best = dist
                        best_pair = (sy[i], sy[j])
                        
            return best_pair[0], best_pair[1], best
        
        # Base cases
        if len(points) <= 1:
            return None, None, float('inf')
        if len(points) == 2:
            return points[0], points[1], distance(points[0], points[1])
            
        # Sort points by x and y coordinates
        px = sorted(points, key=lambda p: p[0])
        py = sorted(points, key=lambda p: p[1])
        
        # Divide
        mid = len(points) // 2
        left_x = px[:mid]
        right_x = px[mid:]
        
        left_y = [p for p in py if p in left_x]
        right_y = [p for p in py if p in right_x]
        
        # Recursive calls
        p1_l, p2_l, d_l = DivideAndConquer.closest_pair(left_x)
        p1_r, p2_r, d_r = DivideAndConquer.closest_pair(right_x)
        
        # Find minimum distance
        d = min(d_l, d_r)
        if d_l < d_r:
            p1, p2 = p1_l, p2_l
        else:
            p1, p2 = p1_r, p2_r
            
        # Check split pair
        p1_s, p2_s, d_s = closest_split_pair(px, py, d)
        
        if d_s < d:
            return p1_s, p2_s, d_s
        return p1, p2, d

class ParallelProcessing:
    """Implementation of Embarrassingly Parallel algorithms"""
    
    @staticmethod
    def parallel_map(func: callable, items: List[Any], num_workers: int = 4) -> List[Any]:
        """Apply function to items in parallel"""
        with ProcessPoolExecutor(max_workers=num_workers) as executor:
            return list(executor.map(func, items))
    
    @staticmethod
    def parallel_matrix_multiply(A: np.ndarray, B: np.ndarray, num_workers: int = 4) -> np.ndarray:
        """Parallel matrix multiplication"""
        if A.shape[1] != B.shape[0]:
            raise ValueError("Matrix dimensions don't match")
            
        result = np.zeros((A.shape[0], B.shape[1]))
        
        def compute_element(params):
            i, j = params
            return i, j, sum(A[i,k] * B[k,j] for k in range(A.shape[1]))
        
        # Generate all (i,j) coordinates
        coords = [(i,j) for i in range(A.shape[0]) for j in range(B.shape[1])]
        
        with ProcessPoolExecutor(max_workers=num_workers) as executor:
            for i, j, value in executor.map(compute_element, coords):
                result[i,j] = value
                
        return result

class GreedyAlgorithms:
    """Implementation of Greedy algorithms"""
    
    @staticmethod
    def activity_selection(start: List[int], finish: List[int]) -> List[int]:
        """Select maximum number of non-overlapping activities"""
        n = len(start)
        activities = sorted(range(n), key=lambda x: finish[x])
        selected = [activities[0]]
        last_finish = finish[activities[0]]
        
        for i in range(1, n):
            idx = activities[i]
            if start[idx] >= last_finish:
                selected.append(idx)
                last_finish = finish[idx]
                
        return selected
    
    @staticmethod
    def fractional_knapsack(values: List[float], weights: List[float], capacity: float) -> Tuple[List[float], float]:
        """Solve fractional knapsack problem"""
        n = len(values)
        # Calculate value per unit weight
        value_per_weight = [(values[i]/weights[i], i) for i in range(n)]
        value_per_weight.sort(reverse=True)
        
        total_value = 0
        fractions = [0] * n
        remaining_capacity = capacity
        
        for vpw, i in value_per_weight:
            if weights[i] <= remaining_capacity:
                fractions[i] = 1
                total_value += values[i]
                remaining_capacity -= weights[i]
            else:
                fraction = remaining_capacity / weights[i]
                fractions[i] = fraction
                total_value += values[i] * fraction
                break
                
        return fractions, total_value

class DynamicProgramming:
    """Implementation of Dynamic Programming algorithms"""
    
    @staticmethod
    def longest_common_subsequence(s1: str, s2: str) -> str:
        """Find longest common subsequence of two strings"""
        m, n = len(s1), len(s2)
        dp = [[0] * (n + 1) for _ in range(m + 1)]
        
        # Fill dp table
        for i in range(1, m + 1):
            for j in range(1, n + 1):
                if s1[i-1] == s2[j-1]:
                    dp[i][j] = dp[i-1][j-1] + 1
                else:
                    dp[i][j] = max(dp[i-1][j], dp[i][j-1])
        
        # Reconstruct the subsequence
        lcs = []
        i, j = m, n
        while i > 0 and j > 0:
            if s1[i-1] == s2[j-1]:
                lcs.append(s1[i-1])
                i -= 1
                j -= 1
            elif dp[i-1][j] > dp[i][j-1]:
                i -= 1
            else:
                j -= 1
                
        return ''.join(reversed(lcs))
    
    @staticmethod
    def knapsack_01(values: List[int], weights: List[int], capacity: int) -> Tuple[int, List[int]]:
        """Solve 0/1 knapsack problem"""
        n = len(values)
        dp = [[0] * (capacity + 1) for _ in range(n + 1)]
        
        # Fill dp table
        for i in range(1, n + 1):
            for w in range(capacity + 1):
                if weights[i-1] <= w:
                    dp[i][w] = max(dp[i-1][w],
                                 dp[i-1][w-weights[i-1]] + values[i-1])
                else:
                    dp[i][w] = dp[i-1][w]
        
        # Reconstruct solution
        selected = []
        w = capacity
        for i in range(n, 0, -1):
            if dp[i][w] != dp[i-1][w]:
                selected.append(i-1)
                w -= weights[i-1]
                
        return dp[n][capacity], selected

def example_usage():
    """Demonstrate usage of different algorithmic paradigms"""
    
    # Divide and Conquer Example
    arr = [64, 34, 25, 12, 22, 11, 90]
    sorted_arr = DivideAndConquer.merge_sort(arr)
    print("Merge Sort:", sorted_arr)
    
    points = [(0,0), (1,1), (2,2), (3,3), (0.5, 0.5)]
    p1, p2, dist = DivideAndConquer.closest_pair(points)
    print(f"Closest pair: {p1}, {p2} with distance {dist}")
    
    # Parallel Processing Example
    def square(x): return x * x
    numbers = list(range(1000))
    result = ParallelProcessing.parallel_map(square, numbers)
    print("Parallel map first 5 results:", result[:5])
    
    # Greedy Example
    start = [1, 3, 0, 5, 8, 5]
    finish = [2, 4, 6, 7, 9, 9]
    activities = GreedyAlgorithms.activity_selection(start, finish)
    print("Selected activities:", activities)
    
    # Dynamic Programming Example
    s1, s2 = "ABCDGH", "AEDFHR"
    lcs = DynamicProgramming.longest_common_subsequence(s1, s2)
    print("Longest Common Subsequence:", lcs)

if __name__ == "__main__":
    example_usage()
