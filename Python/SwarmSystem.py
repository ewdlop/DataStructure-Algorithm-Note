"""
===================================================================================
            Swarm Behavior and Game AI Algorithms Library
===================================================================================
Created by: Claude (Anthropic AI Assistant)
Model: Claude 3.5 Sonnet
Version: 1.0
Created: 2024

A comprehensive library implementing various swarm behaviors and game AI algorithms.
===================================================================================
"""

import numpy as np
from typing import List, Tuple, Optional
import random
import math

class Vector2D:
    """2D vector class for movement calculations."""
    def __init__(self, x: float, y: float):
        self.x = x
        self.y = y
    
    def __add__(self, other):
        return Vector2D(self.x + other.x, self.y + other.y)
    
    def __sub__(self, other):
        return Vector2D(self.x - other.x, self.y - other.y)
    
    def __mul__(self, scalar):
        return Vector2D(self.x * scalar, self.y * scalar)
    
    def magnitude(self) -> float:
        return math.sqrt(self.x**2 + self.y**2)
    
    def normalize(self):
        mag = self.magnitude()
        if mag > 0:
            return Vector2D(self.x/mag, self.y/mag)
        return Vector2D(0, 0)
    
    def limit(self, max_magnitude: float):
        mag = self.magnitude()
        if mag > max_magnitude:
            return self.normalize() * max_magnitude
        return self

class Boid:
    """Base class for flocking entities."""
    def __init__(self, x: float, y: float):
        self.position = Vector2D(x, y)
        self.velocity = Vector2D(random.uniform(-1, 1), random.uniform(-1, 1))
        self.acceleration = Vector2D(0, 0)
        self.max_force = 0.5
        self.max_speed = 4.0
        
    def apply_force(self, force: Vector2D):
        """Apply a force to the boid."""
        self.acceleration = self.acceleration + force
        
    def update(self):
        """Update boid's position based on velocity and acceleration."""
        self.velocity = (self.velocity + self.acceleration).limit(self.max_speed)
        self.position = self.position + self.velocity
        self.acceleration = self.acceleration * 0

class FlockingSystem:
    """Implements classic Boids flocking behavior."""
    def __init__(self):
        self.boids: List[Boid] = []
        self.separation_weight = 1.5
        self.alignment_weight = 1.0
        self.cohesion_weight = 1.0
        self.perception_radius = 50
        
    def add_boid(self, x: float, y: float):
        """Add a new boid to the flock."""
        self.boids.append(Boid(x, y))
        
    def separation(self, boid: Boid) -> Vector2D:
        """Calculate separation force to avoid crowding."""
        steering = Vector2D(0, 0)
        total = 0
        
        for other in self.boids:
            d = math.sqrt((boid.position.x - other.position.x)**2 + 
                         (boid.position.y - other.position.y)**2)
            if other != boid and d < self.perception_radius:
                diff = (boid.position - other.position).normalize()
                diff = diff * (1.0/d if d > 0 else 1.0)
                steering = steering + diff
                total += 1
        
        if total > 0:
            steering = steering * (1.0/total)
            steering = steering.normalize() * self.max_speed
            steering = steering - boid.velocity
            steering = steering.limit(self.max_force)
            
        return steering
    
    def alignment(self, boid: Boid) -> Vector2D:
        """Calculate alignment force to steer towards average heading."""
        steering = Vector2D(0, 0)
        total = 0
        
        for other in self.boids:
            d = math.sqrt((boid.position.x - other.position.x)**2 + 
                         (boid.position.y - other.position.y)**2)
            if other != boid and d < self.perception_radius:
                steering = steering + other.velocity
                total += 1
        
        if total > 0:
            steering = steering * (1.0/total)
            steering = steering.normalize() * self.max_speed
            steering = steering - boid.velocity
            steering = steering.limit(self.max_force)
            
        return steering
    
    def cohesion(self, boid: Boid) -> Vector2D:
        """Calculate cohesion force to steer towards center of mass."""
        steering = Vector2D(0, 0)
        total = 0
        
        for other in self.boids:
            d = math.sqrt((boid.position.x - other.position.x)**2 + 
                         (boid.position.y - other.position.y)**2)
            if other != boid and d < self.perception_radius:
                steering = steering + other.position
                total += 1
        
        if total > 0:
            steering = steering * (1.0/total)
            steering = steering - boid.position
            steering = steering.normalize() * self.max_speed
            steering = steering - boid.velocity
            steering = steering.limit(self.max_force)
            
        return steering
    
    def update(self):
        """Update all boids in the flock."""
        for boid in self.boids:
            separation = self.separation(boid) * self.separation_weight
            alignment = self.alignment(boid) * self.alignment_weight
            cohesion = self.cohesion(boid) * self.cohesion_weight
            
            boid.apply_force(separation)
            boid.apply_force(alignment)
            boid.apply_force(cohesion)
            boid.update()

class ParticleSwarmOptimization:
    """Implements PSO algorithm for optimization problems."""
    def __init__(self, num_particles: int, num_dimensions: int):
        self.num_particles = num_particles
        self.num_dimensions = num_dimensions
        self.particles = []
        self.global_best_position = None
        self.global_best_score = float('inf')
        
        for _ in range(num_particles):
            self.particles.append({
                'position': np.random.rand(num_dimensions),
                'velocity': np.zeros(num_dimensions),
                'best_position': None,
                'best_score': float('inf')
            })
    
    def optimize(self, objective_func, num_iterations: int):
        """Run PSO optimization."""
        w = 0.729  # Inertia weight
        c1 = 1.49  # Cognitive weight
        c2 = 1.49  # Social weight
        
        for _ in range(num_iterations):
            for particle in self.particles:
                # Evaluate current position
                score = objective_func(particle['position'])
                
                # Update personal best
                if score < particle['best_score']:
                    particle['best_score'] = score
                    particle['best_position'] = particle['position'].copy()
                
                # Update global best
                if score < self.global_best_score:
                    self.global_best_score = score
                    self.global_best_position = particle['position'].copy()
                
                # Update velocity and position
                r1, r2 = np.random.rand(2)
                cognitive = c1 * r1 * (particle['best_position'] - particle['position'])
                social = c2 * r2 * (self.global_best_position - particle['position'])
                
                particle['velocity'] = w * particle['velocity'] + cognitive + social
                particle['position'] = particle['position'] + particle['velocity']

class GameSwarmAI:
    """Implements game-specific swarm behaviors."""
    
    class Unit:
        def __init__(self, x: float, y: float, unit_type: str):
            self.position = Vector2D(x, y)
            self.velocity = Vector2D(0, 0)
            self.unit_type = unit_type
            self.health = 100
            self.target = None
    
    def __init__(self):
        self.units: List[GameSwarmAI.Unit] = []
        self.formation_spacing = 30
        
    def add_unit(self, x: float, y: float, unit_type: str):
        """Add a new unit to the swarm."""
        self.units.append(self.Unit(x, y, unit_type))
    
    def move_to_formation(self, center: Vector2D, formation_type: str):
        """Move units into specified formation."""
        if formation_type == "circle":
            radius = len(self.units) * self.formation_spacing / (2 * math.pi)
            for i, unit in enumerate(self.units):
                angle = (2 * math.pi * i) / len(self.units)
                target_x = center.x + radius * math.cos(angle)
                target_y = center.y + radius * math.sin(angle)
                unit.target = Vector2D(target_x, target_y)
        
        elif formation_type == "square":
            side_length = math.ceil(math.sqrt(len(self.units)))
            for i, unit in enumerate(self.units):
                row = i // side_length
                col = i % side_length
                target_x = center.x + (col - side_length/2) * self.formation_spacing
                target_y = center.y + (row - side_length/2) * self.formation_spacing
                unit.target = Vector2D(target_x, target_y)
    
    def update_combat_behavior(self, enemies: List[Vector2D]):
        """Update unit behavior in combat situations."""
        for unit in self.units:
            if unit.target:
                # Move towards target while maintaining formation
                direction = (unit.target - unit.position).normalize()
                unit.velocity = direction * 2.0
                
                # Check for nearby enemies and adjust behavior
                for enemy_pos in enemies:
                    distance = (enemy_pos - unit.position).magnitude()
                    if distance < 100:  # Combat range
                        if unit.unit_type == "ranged":
                            # Maintain distance for ranged units
                            retreat_vector = (unit.position - enemy_pos).normalize()
                            unit.velocity = unit.velocity + (retreat_vector * 1.5)
                        else:
                            # Close in for melee units
                            attack_vector = (enemy_pos - unit.position).normalize()
                            unit.velocity = unit.velocity + (attack_vector * 1.5)
            
            # Update position
            unit.position = unit.position + unit.velocity

def main():
    """Example usage of swarm algorithms."""
    
    # Flocking example
    flock = FlockingSystem()
    for _ in range(50):
        flock.add_boid(random.uniform(0, 800), random.uniform(0, 600))
    
    # Game swarm example
    game_swarm = GameSwarmAI()
    for i in range(20):
        unit_type = "ranged" if i < 10 else "melee"
        game_swarm.add_unit(random.uniform(0, 800), random.uniform(0, 600), unit_type)
    
    # Move units into circle formation
    center = Vector2D(400, 300)
    game_swarm.move_to_formation(center, "circle")
    
    # Example enemy positions
    enemies = [Vector2D(600, 400), Vector2D(550, 350)]
    game_swarm.update_combat_behavior(enemies)
    
    # PSO example
    def objective_function(x):
        return np.sum(x**2)  # Simple sphere function
        
    pso = ParticleSwarmOptimization(30, 2)
    pso.optimize(objective_function, 100)

if __name__ == "__main__":
    main()
