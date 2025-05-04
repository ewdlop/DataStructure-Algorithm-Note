import heapq
from collections import deque
from typing import List, Tuple, Dict, Set

class SokobanState:
    """Represents a state in the Sokoban game"""
    def __init__(self, player_pos: Tuple[int, int], boxes: Set[Tuple[int, int]], level: List[List[str]]):
        self.player_pos = player_pos
        self.boxes = frozenset(boxes)  # Use frozenset for hashing
        self.level = level
        self.goals = self._find_goals()
        
    def _find_goals(self) -> Set[Tuple[int, int]]:
        """Find all goal positions in the level"""
        goals = set()
        for y, row in enumerate(self.level):
            for x, cell in enumerate(row):
                if cell in '.+*':
                    goals.add((x, y))
        return goals
    
    def is_goal_state(self) -> bool:
        """Check if all boxes are on goal positions"""
        return all(box in self.goals for box in self.boxes)
    
    def __hash__(self):
        return hash((self.player_pos, self.boxes))
    
    def __eq__(self, other):
        return self.player_pos == other.player_pos and self.boxes == other.boxes

class SokobanSolver:
    """Solves Sokoban puzzles using A* search algorithm"""
    
    def __init__(self, level: List[str]):
        self.level = self._parse_level(level)
        self.initial_state = self._create_initial_state()
        self.moves = [(0, 1), (1, 0), (0, -1), (-1, 0)]  # down, right, up, left
        self.move_names = ['D', 'R', 'U', 'L']
        
    def _parse_level(self, level: List[str]) -> List[List[str]]:
        """Convert string level to 2D list"""
        return [list(row) for row in level]
    
    def _create_initial_state(self) -> SokobanState:
        """Create initial state from level"""
        player_pos = None
        boxes = set()
        
        for y, row in enumerate(self.level):
            for x, cell in enumerate(row):
                if cell in '@+':
                    player_pos = (x, y)
                if cell in '$*':
                    boxes.add((x, y))
        
        return SokobanState(player_pos, boxes, self.level)
    
    def _is_wall(self, x: int, y: int) -> bool:
        """Check if position is a wall"""
        return self.level[y][x] == '#'
    
    def _is_valid_position(self, x: int, y: int) -> bool:
        """Check if position is within bounds and not a wall"""
        if x < 0 or x >= len(self.level[0]) or y < 0 or y >= len(self.level):
            return False
        return not self._is_wall(x, y)
    
    def _calculate_heuristic(self, state: SokobanState) -> int:
        """Calculate heuristic (Manhattan distance from boxes to nearest goals)"""
        if not state.goals:
            return 0
            
        total_distance = 0
        for box in state.boxes:
            min_distance = float('inf')
            for goal in state.goals:
                distance = abs(box[0] - goal[0]) + abs(box[1] - goal[1])
                min_distance = min(min_distance, distance)
            total_distance += min_distance
        
        return total_distance
    
    def _get_successors(self, state: SokobanState) -> List[Tuple[SokobanState, str]]:
        """Get possible next states and the moves that lead to them"""
        successors = []
        
        for i, (dx, dy) in enumerate(self.moves):
            # Calculate new player position
            new_x = state.player_pos[0] + dx
            new_y = state.player_pos[1] + dy
            
            if not self._is_valid_position(new_x, new_y):
                continue
            
            # Check if player is pushing a box
            box_pos = (new_x, new_y)
            if box_pos in state.boxes:
                # Calculate new box position
                new_box_x = new_x + dx
                new_box_y = new_y + dy
                
                # Check if box can be pushed
                if not self._is_valid_position(new_box_x, new_box_y):
                    continue
                    
                # Check if there's already a box at the new position
                new_box_pos = (new_box_x, new_box_y)
                if new_box_pos in state.boxes:
                    continue
                
                # Create new state with pushed box
                new_boxes = set(state.boxes)
                new_boxes.remove(box_pos)
                new_boxes.add(new_box_pos)
                new_state = SokobanState((new_x, new_y), new_boxes, state.level)
            else:
                # Create new state with moved player
                new_state = SokobanState((new_x, new_y), state.boxes, state.level)
            
            successors.append((new_state, self.move_names[i]))
        
        return successors
    
    def solve(self) -> List[str]:
        """Solve the Sokoban puzzle using A* search"""
        frontier = []
        heapq.heappush(frontier, (0, 0, self.initial_state, []))
        explored = set()
        explored.add(self.initial_state)
        
        while frontier:
            _, cost, current_state, moves = heapq.heappop(frontier)
            
            if current_state.is_goal_state():
                return moves
            
            for next_state, move in self._get_successors(current_state):
                if next_state not in explored:
                    new_cost = cost + 1
                    priority = new_cost + self._calculate_heuristic(next_state)
                    heapq.heappush(frontier, (priority, new_cost, next_state, moves + [move]))
                    explored.add(next_state)
        
        return []  # No solution found

# Example usage
def print_level(level):
    """Print the level"""
    for row in level:
        print(''.join(row))
    print()

def apply_moves(level, moves):
    """Apply a sequence of moves to the level"""
    # Create a copy of the level as list of lists
    current_level = [list(row) for row in level]
    
    # Find player position
    player_pos = None
    for y, row in enumerate(current_level):
        for x, cell in enumerate(row):
            if cell in '@+':
                player_pos = (x, y)
                break
        if player_pos:
            break
    
    # Define move directions
    directions = {'D': (0, 1), 'R': (1, 0), 'U': (0, -1), 'L': (-1, 0)}
    
    for move in moves:
        dx, dy = directions[move]
        new_x, new_y = player_pos[0] + dx, player_pos[1] + dy
        
        # Check if player is pushing a box
        if current_level[new_y][new_x] in '$*':
            box_new_x, box_new_y = new_x + dx, new_y + dy
            
            # Update box position
            if current_level[new_y][new_x] == '$':
                current_level[new_y][new_x] = ' '
            else:  # current_level[new_y][new_x] == '*'
                current_level[new_y][new_x] = '.'
            
            if current_level[box_new_y][box_new_x] == '.':
                current_level[box_new_y][box_new_x] = '*'
            else:
                current_level[box_new_y][box_new_x] = '$'
        
        # Update player position
        if current_level[player_pos[1]][player_pos[0]] == '@':
            current_level[player_pos[1]][player_pos[0]] = ' '
        else:  # current_level[player_pos[1]][player_pos[0]] == '+'
            current_level[player_pos[1]][player_pos[0]] = '.'
        
        if current_level[new_y][new_x] == '.':
            current_level[new_y][new_x] = '+'
        else:
            current_level[new_y][new_x] = '@'
        
        player_pos = (new_x, new_y)
        
        print(f"Move: {move}")
        print_level(current_level)
    
    return current_level

# Test with a simple level
if __name__ == "__main__":
    # Simple example level
    level = [
        "#####",
        "#@ $#",
        "# . #",
        "#   #",
        "#####"
    ]
    
    print("Initial level:")
    print_level(level)
    
    solver = SokobanSolver(level)
    solution = solver.solve()
    
    if solution:
        print(f"Solution found: {solution}")
        print("\nApplying solution:")
        final_level = apply_moves(level, solution)
    else:
        print("No solution found!")
