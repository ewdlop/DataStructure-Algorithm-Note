import time
import heapq
from typing import Any, Callable, Dict, List, Optional, Tuple
from dataclasses import dataclass
from collections import deque
import random
import threading
from queue import Queue
import numpy as np
from enum import Enum

class TimeoutState(Enum):
    PENDING = "pending"
    EXPIRED = "expired"
    CANCELLED = "cancelled"

@dataclass
class TimeoutEvent:
    """Represents a timeout event"""
    id: str
    deadline: float
    callback: Callable
    state: TimeoutState = TimeoutState.PENDING

class TimeoutManager:
    """Manages timeouts with priority queue"""
    
    def __init__(self):
        self.timeouts: List[Tuple[float, str, TimeoutEvent]] = []  # (deadline, id, event)
        self.events: Dict[str, TimeoutEvent] = {}
        self._lock = threading.Lock()
    
    def set_timeout(self, callback: Callable, delay: float, timeout_id: Optional[str] = None) -> str:
        """Set a timeout with callback"""
        with self._lock:
            timeout_id = timeout_id or str(random.randint(0, 1000000))
            deadline = time.time() + delay
            
            event = TimeoutEvent(
                id=timeout_id,
                deadline=deadline,
                callback=callback
            )
            
            heapq.heappush(self.timeouts, (deadline, timeout_id, event))
            self.events[timeout_id] = event
            
            return timeout_id
    
    def cancel_timeout(self, timeout_id: str) -> bool:
        """Cancel a pending timeout"""
        with self._lock:
            if timeout_id in self.events:
                event = self.events[timeout_id]
                if event.state == TimeoutState.PENDING:
                    event.state = TimeoutState.CANCELLED
                    return True
            return False
    
    def process_timeouts(self) -> None:
        """Process all expired timeouts"""
        with self._lock:
            current_time = time.time()
            
            while self.timeouts and self.timeouts[0][0] <= current_time:
                deadline, timeout_id, event = heapq.heappop(self.timeouts)
                
                if event.state == TimeoutState.PENDING:
                    event.state = TimeoutState.EXPIRED
                    try:
                        event.callback()
                    except Exception as e:
                        print(f"Error in timeout callback: {e}")

class QueueTheory:
    """Implementation of queue theory models"""
    
    @staticmethod
    def mm1_queue_metrics(arrival_rate: float, service_rate: float) -> Dict[str, float]:
        """
        Calculate M/M/1 queue metrics
        
        Args:
            arrival_rate: Average arrival rate (λ)
            service_rate: Average service rate (μ)
            
        Returns:
            Dictionary with queue metrics
        """
        if arrival_rate >= service_rate:
            raise ValueError("System unstable: arrival rate must be less than service rate")
            
        rho = arrival_rate / service_rate  # Utilization
        
        metrics = {
            'utilization': rho,
            'avg_customers_system': rho / (1 - rho),  # L
            'avg_customers_queue': rho**2 / (1 - rho),  # Lq
            'avg_wait_time': 1 / (service_rate - arrival_rate),  # W
            'avg_queue_time': rho / (service_rate - arrival_rate)  # Wq
        }
        
        return metrics
    
    @staticmethod
    def mmk_queue_metrics(arrival_rate: float, service_rate: float, k: int) -> Dict[str, float]:
        """
        Calculate M/M/k queue metrics
        
        Args:
            arrival_rate: Average arrival rate (λ)
            service_rate: Average service rate per server (μ)
            k: Number of servers
            
        Returns:
            Dictionary with queue metrics
        """
        rho = arrival_rate / (k * service_rate)
        if rho >= 1:
            raise ValueError("System unstable")
            
        # Calculate p0 (probability of empty system)
        sum_term = sum([(k*rho)**n / np.math.factorial(n) for n in range(k)])
        p0 = 1 / (sum_term + (k*rho)**k / (np.math.factorial(k) * (1-rho)))
        
        # Calculate metrics
        Lq = (p0 * (arrival_rate/service_rate)**k * rho) / (np.math.factorial(k) * (1-rho)**2)
        L = Lq + arrival_rate/service_rate
        W = L/arrival_rate
        Wq = Lq/arrival_rate
        
        return {
            'utilization': rho,
            'avg_customers_system': L,
            'avg_customers_queue': Lq,
            'avg_wait_time': W,
            'avg_queue_time': Wq,
            'empty_prob': p0
        }

class QueueSimulator:
    """Simulate queue behavior"""
    
    def __init__(self, arrival_rate: float, service_rate: float, num_servers: int = 1):
        self.arrival_rate = arrival_rate
        self.service_rate = service_rate
        self.num_servers = num_servers
        self.queue = Queue()
        self.servers = [False] * num_servers  # False = idle, True = busy
        self.timeout_manager = TimeoutManager()
        
        # Statistics
        self.total_customers = 0
        self.total_wait_time = 0
        self.max_queue_length = 0
        
    def start_simulation(self, duration: float):
        """Run simulation for specified duration"""
        end_time = time.time() + duration
        
        # Schedule first arrival
        self.timeout_manager.set_timeout(
            self._handle_arrival,
            self._generate_interarrival_time()
        )
        
        # Main simulation loop
        while time.time() < end_time:
            self.timeout_manager.process_timeouts()
            time.sleep(0.01)  # Prevent CPU hogging
            
        # Print statistics
        self._print_statistics()
    
    def _handle_arrival(self):
        """Handle customer arrival"""
        self.total_customers += 1
        arrival_time = time.time()
        
        # Find idle server
        idle_server = None
        for i, busy in enumerate(self.servers):
            if not busy:
                idle_server = i
                break
        
        if idle_server is not None:
            # Server available
            self.servers[idle_server] = True
            service_time = self._generate_service_time()
            
            # Schedule service completion
            self.timeout_manager.set_timeout(
                lambda: self._handle_departure(idle_server),
                service_time
            )
        else:
            # All servers busy, add to queue
            self.queue.put((arrival_time, self._generate_service_time()))
            self.max_queue_length = max(self.max_queue_length, self.queue.qsize())
        
        # Schedule next arrival
        self.timeout_manager.set_timeout(
            self._handle_arrival,
            self._generate_interarrival_time()
        )
    
    def _handle_departure(self, server_id: int):
        """Handle customer departure"""
        if not self.queue.empty():
            # Get next customer from queue
            arrival_time, service_time = self.queue.get()
            self.total_wait_time += time.time() - arrival_time
            
            # Schedule service completion
            self.timeout_manager.set_timeout(
                lambda: self._handle_departure(server_id),
                service_time
            )
        else:
            self.servers[server_id] = False
    
    def _generate_interarrival_time(self) -> float:
        """Generate random interarrival time"""
        return random.expovariate(self.arrival_rate)
    
    def _generate_service_time(self) -> float:
        """Generate random service time"""
        return random.expovariate(self.service_rate)
    
    def _print_statistics(self):
        """Print simulation statistics"""
        print("\nSimulation Statistics:")
        print(f"Total customers served: {self.total_customers}")
        if self.total_customers > 0:
            avg_wait = self.total_wait_time / self.total_customers
            print(f"Average wait time: {avg_wait:.2f} seconds")
        print(f"Maximum queue length: {self.max_queue_length}")

def example_usage():
    """Demonstrate timeout handling and queue theory"""
    
    # Timeout Manager Example
    print("Testing Timeout Manager:")
    tm = TimeoutManager()
    
    def callback1():
        print("Timeout 1 expired!")
    
    def callback2():
        print("Timeout 2 expired!")
    
    # Set timeouts
    id1 = tm.set_timeout(callback1, 2)
    id2 = tm.set_timeout(callback2, 3)
    
    # Cancel one timeout
    tm.cancel_timeout(id1)
    
    # Process timeouts
    time.sleep(4)
    tm.process_timeouts()
    
    # Queue Theory Example
    print("\nTesting Queue Theory:")
    
    # M/M/1 queue
    metrics = QueueTheory.mm1_queue_metrics(arrival_rate=2, service_rate=3)
    print("\nM/M/1 Queue Metrics:")
    for metric, value in metrics.items():
        print(f"{metric}: {value:.3f}")
    
    # M/M/k queue
    metrics = QueueTheory.mmk_queue_metrics(arrival_rate=4, service_rate=3, k=2)
    print("\nM/M/2 Queue Metrics:")
    for metric, value in metrics.items():
        print(f"{metric}: {value:.3f}")
    
    # Queue Simulation
    print("\nRunning Queue Simulation:")
    simulator = QueueSimulator(arrival_rate=2, service_rate=3, num_servers=2)
    simulator.start_simulation(duration=10)

if __name__ == "__main__":
    example_usage()
