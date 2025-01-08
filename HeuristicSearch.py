import numpy as np
from typing import List, Tuple, Dict, Set, Optional
from collections import defaultdict
import itertools
import time
from scipy.optimize import linprog

class HeuristicSearch:
    """Implementation of Heuristic Search Algorithms"""
    
    @staticmethod
    def simulated_annealing(initial_state: List[int], 
                           cost_func: callable,
                           neighbor_func: callable,
                           temp_schedule: callable,
                           max_iter: int = 1000) -> Tuple[List[int], float]:
        """
        Simulated Annealing optimization
        
        Args:
            initial_state: Starting solution
            cost_func: Function to evaluate solution cost
            neighbor_func: Function to generate neighbor solution
            temp_schedule: Temperature scheduling function
            max_iter: Maximum iterations
        """
        current_state = initial_state.copy()
        current_cost = cost_func(current_state)
        best_state = current_state.copy()
        best_cost = current_cost
        
        for i in range(max_iter):
            T = temp_schedule(i)
            if T <= 0:
                break
                
            neighbor = neighbor_func(current_state)
            neighbor_cost = cost_func(neighbor)
            
            delta_E = neighbor_cost - current_cost
            if delta_E < 0 or np.random.random() < np.exp(-delta_E / T):
                current_state = neighbor
                current_cost = neighbor_cost
                
                if current_cost < best_cost:
                    best_state = current_state.copy()
                    best_cost = current_cost
                    
        return best_state, best_cost
    
    @staticmethod
    def hill_climbing(initial_state: List[int],
                     cost_func: callable,
                     neighbor_func: callable,
                     max_iter: int = 1000) -> Tuple[List[int], float]:
        """Hill Climbing with random restarts"""
        def single_climb(state):
            current_state = state.copy()
            current_cost = cost_func(current_state)
            
            for _ in range(max_iter):
                neighbor = neighbor_func(current_state)
                neighbor_cost = cost_func(neighbor)
                
                if neighbor_cost >= current_cost:
                    break
                    
                current_state = neighbor
                current_cost = neighbor_cost
                
            return current_state, current_cost
        
        # Multiple random restarts
        best_state, best_cost = single_climb(initial_state)
        
        for _ in range(5):  # Number of restarts
            new_state = [np.random.randint(0, 100) for _ in range(len(initial_state))]
            state, cost = single_climb(new_state)
            if cost < best_cost:
                best_state, best_cost = state, cost
                
        return best_state, best_cost

class Backtracking:
    """Implementation of Backtracking Algorithms"""
    
    @staticmethod
    def n_queens(n: int) -> List[List[int]]:
        """
        Solve N-Queens problem using backtracking
        Returns all valid placements
        """
        def is_safe(board: List[int], row: int, col: int) -> bool:
            # Check previous queens placements
            for prev_row in range(row):
                prev_col = board[prev_row]
                if (prev_col == col or 
                    abs(prev_col - col) == abs(prev_row - row)):
                    return False
            return True
        
        def solve(board: List[int], row: int) -> None:
            if row == n:
                solutions.append(board[:])
                return
                
            for col in range(n):
                if is_safe(board, row, col):
                    board[row] = col
                    solve(board, row + 1)
            
        solutions = []
        initial_board = [-1] * n
        solve(initial_board, 0)
        return solutions
    
    @staticmethod
    def subset_sum(numbers: List[int], target: int) -> List[List[int]]:
        """Find all subsets that sum to target"""
        def backtrack(start: int, target: int, current: List[int]) -> None:
            if target == 0:
                solutions.append(current[:])
                return
                
            for i in range(start, len(numbers)):
                if target - numbers[i] >= 0:
                    current.append(numbers[i])
                    backtrack(i + 1, target - numbers[i], current)
                    current.pop()
        
        solutions = []
        backtrack(0, target, [])
        return solutions

class BruteForce:
    """Implementation of Brute Force Algorithms"""
    
    @staticmethod
    def traveling_salesman(distances: List[List[float]]) -> Tuple[List[int], float]:
        """
        Solve TSP using brute force
        Returns optimal tour and distance
        """
        n = len(distances)
        cities = list(range(n))
        min_distance = float('inf')
        best_tour = None
        
        for tour in itertools.permutations(cities[1:]):
            tour = (0,) + tour  # Start from city 0
            distance = 0
            
            for i in range(n-1):
                distance += distances[tour[i]][tour[i+1]]
            distance += distances[tour[-1]][tour[0]]  # Return to start
            
            if distance < min_distance:
                min_distance = distance
                best_tour = list(tour)
                
        return best_tour, min_distance
    
    @staticmethod
    def string_matching(text: str, pattern: str) -> List[int]:
        """Find all occurrences of pattern in text"""
        n, m = len(text), len(pattern)
        positions = []
        
        for i in range(n - m + 1):
            if text[i:i+m] == pattern:
                positions.append(i)
                
        return positions

class LinearProgramming:
    """Implementation of Linear Programming Solutions"""
    
    @staticmethod
    def solve_production_planning(profits: List[float],
                                constraints: List[List[float]],
                                bounds: List[float]) -> Tuple[List[float], float]:
        """
        Solve production planning LP problem
        
        Args:
            profits: Profit per unit for each product
            constraints: Resource constraints matrix
            bounds: Resource availability bounds
        """
        # Minimize negative profits (for maximization)
        c = [-p for p in profits]
        
        # Solve using scipy's linprog
        result = linprog(c, A_ub=constraints, b_ub=bounds, method='simplex')
        
        if result.success:
            return result.x, -result.fun
        return None, None
    
    @staticmethod
    def solve_transportation(supply: List[float],
                           demand: List[float],
                           costs: List[List[float]]) -> Tuple[List[List[float]], float]:
        """
        Solve transportation problem
        
        Args:
            supply: Supply at each source
            demand: Demand at each destination
            costs: Transportation costs matrix
        """
        m, n = len(supply), len(demand)
        
        # Prepare for scipy.linprog format
        num_vars = m * n
        c = [cost for row in costs for cost in row]
        
        # Supply constraints
        A_eq = []
        b_eq = []
        
        # Each source constraint
        for i in range(m):
            row = [0] * num_vars
            for j in range(n):
                row[i*n + j] = 1
            A_eq.append(row)
            b_eq.append(supply[i])
            
        # Each destination constraint
        for j in range(n):
            row = [0] * num_vars
            for i in range(m):
                row[i*n + j] = 1
            A_eq.append(row)
            b_eq.append(demand[j])
            
        # Solve
        result = linprog(c, A_eq=A_eq, b_eq=b_eq, method='simplex')
        
        if result.success:
            # Reshape solution into matrix
            solution = np.reshape(result.x, (m, n))
            return solution, result.fun
        return None, None

def example_usage():
    """Demonstrate usage of different algorithmic approaches"""
    
    # Heuristic Search Example
    def cost_func(x): return sum(x)  # Minimize sum
    def neighbor_func(x): 
        x = x.copy()
        i = np.random.randint(0, len(x))
        x[i] += np.random.randint(-10, 11)
        return x
    def temp_schedule(i): return 100 * (0.95 ** i)
    
    initial = [50] * 10
    result, cost = HeuristicSearch.simulated_annealing(
        initial, cost_func, neighbor_func, temp_schedule)
    print("Simulated Annealing Result:", result, "Cost:", cost)
    
    # Backtracking Example
    queens_solutions = Backtracking.n_queens(4)
    print("4-Queens Solutions:", queens_solutions)
    
    numbers = [1, 2, 3, 4, 5]
    subset_solutions = Backtracking.subset_sum(numbers, 7)
    print("Subset Sum Solutions for target 7:", subset_solutions)
    
    # Brute Force Example
    distances = [
        [0, 10, 15, 20],
        [10, 0, 35, 25],
        [15, 35, 0, 30],
        [20, 25, 30, 0]
    ]
    tour, distance = BruteForce.traveling_salesman(distances)
    print("TSP Solution:", tour, "Distance:", distance)
    
    # Linear Programming Example
    profits = [40, 30]  # Profit per unit for two products
    constraints = [
        [2, 1],  # Resource 1 usage
        [1, 3]   # Resource 2 usage
    ]
    bounds = [100, 90]  # Resource availability
    
    production, total_profit = LinearProgramming.solve_production_planning(
        profits, constraints, bounds)
    print("Optimal Production:", production, "Total Profit:", total_profit)

if __name__ == "__main__":
    example_usage()
