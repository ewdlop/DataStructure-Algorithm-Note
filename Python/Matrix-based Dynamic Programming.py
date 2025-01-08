import numpy as np
from typing import List, Tuple, Optional
import heapq
from collections import deque

class MatrixDP:
    """Matrix-based Dynamic Programming Solutions"""

    @staticmethod
    def min_path_sum(matrix: List[List[int]]) -> Tuple[int, List[Tuple[int, int]]]:
        """
        Find minimum path sum from top-left to bottom-right
        Returns (min_sum, path)
        Only down and right movements allowed
        """
        if not matrix or not matrix[0]:
            return 0, []
            
        rows, cols = len(matrix), len(matrix[0])
        dp = [[float('inf')] * cols for _ in range(rows)]
        dp[0][0] = matrix[0][0]
        
        # Fill dp table
        for i in range(rows):
            for j in range(cols):
                if i > 0:
                    dp[i][j] = min(dp[i][j], dp[i-1][j] + matrix[i][j])
                if j > 0:
                    dp[i][j] = min(dp[i][j], dp[i][j-1] + matrix[i][j])
        
        # Reconstruct path
        path = []
        i, j = rows-1, cols-1
        while i > 0 or j > 0:
            path.append((i, j))
            if i > 0 and dp[i][j] == dp[i-1][j] + matrix[i][j]:
                i -= 1
            else:
                j -= 1
        path.append((0, 0))
        
        return dp[-1][-1], path[::-1]

    @staticmethod
    def longest_increasing_path(matrix: List[List[int]]) -> Tuple[int, List[Tuple[int, int]]]:
        """
        Find longest increasing path in matrix
        Can move in all four directions
        Returns (length, path)
        """
        if not matrix or not matrix[0]:
            return 0, []
            
        rows, cols = len(matrix), len(matrix[0])
        dp = [[0] * cols for _ in range(rows)]
        next_cell = [[None] * cols for _ in range(rows)]
        
        def dfs(i: int, j: int) -> int:
            if dp[i][j] != 0:
                return dp[i][j]
                
            directions = [(0,1), (1,0), (0,-1), (-1,0)]
            max_len = 1
            
            for di, dj in directions:
                ni, nj = i + di, j + dj
                if (0 <= ni < rows and 0 <= nj < cols and 
                    matrix[ni][nj] > matrix[i][j]):
                    length = 1 + dfs(ni, nj)
                    if length > max_len:
                        max_len = length
                        next_cell[i][j] = (ni, nj)
                        
            dp[i][j] = max_len
            return max_len
        
        # Find longest path starting from each cell
        max_length = 0
        start_cell = (0, 0)
        
        for i in range(rows):
            for j in range(cols):
                length = dfs(i, j)
                if length > max_length:
                    max_length = length
                    start_cell = (i, j)
        
        # Reconstruct path
        path = []
        current = start_cell
        while current is not None:
            path.append(current)
            current = next_cell[current[0]][current[1]]
            
        return max_length, path

    @staticmethod
    def max_square_submatrix(matrix: List[List[int]]) -> Tuple[int, List[Tuple[int, int]]]:
        """
        Find largest square submatrix of all 1s
        Returns (size, top_left_coordinate)
        """
        if not matrix or not matrix[0]:
            return 0, []
            
        rows, cols = len(matrix), len(matrix[0])
        dp = [[0] * cols for _ in range(rows)]
        max_size = 0
        max_pos = (0, 0)
        
        # Initialize first row and column
        for i in range(rows):
            dp[i][0] = matrix[i][0]
        for j in range(cols):
            dp[0][j] = matrix[0][j]
            
        # Fill dp table
        for i in range(1, rows):
            for j in range(1, cols):
                if matrix[i][j] == 1:
                    dp[i][j] = min(dp[i-1][j], dp[i][j-1], dp[i-1][j-1]) + 1
                    if dp[i][j] > max_size:
                        max_size = dp[i][j]
                        max_pos = (i - max_size + 1, j - max_size + 1)
        
        # Get submatrix coordinates
        i, j = max_pos
        coords = [(i+di, j+dj) for di in range(max_size) 
                 for dj in range(max_size)]
                 
        return max_size, coords

    @staticmethod
    def matrix_chain_multiplication(dimensions: List[int]) -> Tuple[int, str]:
        """
        Find optimal way to multiply chain of matrices
        Returns (minimum operations, parenthesization)
        """
        n = len(dimensions) - 1
        dp = [[float('inf')] * n for _ in range(n)]
        splits = [[0] * n for _ in range(n)]
        
        # Initialize diagonal
        for i in range(n):
            dp[i][i] = 0
            
        # Fill table
        for length in range(2, n + 1):
            for i in range(n - length + 1):
                j = i + length - 1
                for k in range(i, j):
                    cost = (dp[i][k] + dp[k+1][j] + 
                           dimensions[i] * dimensions[k+1] * dimensions[j+1])
                    if cost < dp[i][j]:
                        dp[i][j] = cost
                        splits[i][j] = k
        
        # Get parenthesization
        def get_parenthesis(i: int, j: int) -> str:
            if i == j:
                return f"M{i+1}"
            k = splits[i][j]
            left = get_parenthesis(i, k)
            right = get_parenthesis(k+1, j)
            return f"({left} × {right})"
            
        return dp[0][n-1], get_parenthesis(0, n-1)

    @staticmethod
    def edit_distance_matrix(str1: str, str2: str) -> Tuple[int, List[str]]:
        """
        Find minimum edit distance between two strings with operations
        Returns (distance, list of operations)
        """
        m, n = len(str1), len(str2)
        dp = [[0] * (n + 1) for _ in range(m + 1)]
        ops = [[[] for _ in range(n + 1)] for _ in range(m + 1)]
        
        # Initialize first row and column
        for i in range(m + 1):
            dp[i][0] = i
            if i > 0:
                ops[i][0] = ops[i-1][0] + [f"Delete {str1[i-1]}"]
                
        for j in range(n + 1):
            dp[0][j] = j
            if j > 0:
                ops[0][j] = ops[0][j-1] + [f"Insert {str2[j-1]}"]
        
        # Fill table
        for i in range(1, m + 1):
            for j in range(1, n + 1):
                if str1[i-1] == str2[j-1]:
                    dp[i][j] = dp[i-1][j-1]
                    ops[i][j] = ops[i-1][j-1]
                else:
                    delete_cost = dp[i-1][j] + 1
                    insert_cost = dp[i][j-1] + 1
                    replace_cost = dp[i-1][j-1] + 1
                    
                    min_cost = min(delete_cost, insert_cost, replace_cost)
                    dp[i][j] = min_cost
                    
                    if min_cost == delete_cost:
                        ops[i][j] = ops[i-1][j] + [f"Delete {str1[i-1]}"]
                    elif min_cost == insert_cost:
                        ops[i][j] = ops[i][j-1] + [f"Insert {str2[j-1]}"]
                    else:
                        ops[i][j] = ops[i-1][j-1] + [f"Replace {str1[i-1]} with {str2[j-1]}"]
                        
        return dp[m][n], ops[m][n]

def example_usage():
    """Demonstrate usage of matrix DP algorithms"""
    
    # Example 1: Minimum Path Sum
    matrix = [
        [1, 3, 1],
        [1, 5, 1],
        [4, 2, 1]
    ]
    min_sum, path = MatrixDP.min_path_sum(matrix)
    print(f"Minimum path sum: {min_sum}")
    print(f"Path: {path}")
    
    # Example 2: Longest Increasing Path
    matrix = [
        [9, 9, 4],
        [6, 6, 8],
        [2, 1, 1]
    ]
    length, path = MatrixDP.longest_increasing_path(matrix)
    print(f"\nLongest increasing path length: {length}")
    print(f"Path: {path}")
    
    # Example 3: Maximum Square Submatrix
    matrix = [
        [1, 0, 1, 0, 0],
        [1, 0, 1, 1, 1],
        [1, 1, 1, 1, 1],
        [1, 0, 0, 1, 0]
    ]
    size, coords = MatrixDP.max_square_submatrix(matrix)
    print(f"\nMaximum square size: {size}")
    print(f"Coordinates: {coords}")
    
    # Example 4: Matrix Chain Multiplication
    dimensions = [30, 35, 15, 5, 10, 20, 25]
    operations, parenthesis = MatrixDP.matrix_chain_multiplication(dimensions)
    print(f"\nMinimum operations: {operations}")
    print(f"Optimal parenthesization: {parenthesis}")
    
    # Example 5: Edit Distance
    str1, str2 = "sunday", "saturday"
    distance, operations = MatrixDP.edit_distance_matrix(str1, str2)
    print(f"\nEdit distance: {distance}")
    print(f"Operations: {operations}")

if __name__ == "__main__":
    example_usage()
