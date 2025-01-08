from typing import Optional, Any, List, Tuple
import math
from dataclasses import dataclass
from enum import Enum

class EntryStatus(Enum):
    """Status of a hash table entry"""
    EMPTY = 0
    OCCUPIED = 1
    DELETED = 2  # Tombstone for lazy deletion

@dataclass
class Entry:
    """Hash table entry with status"""
    key: Any
    value: Any
    status: EntryStatus = EntryStatus.EMPTY

class QuadraticHashTable:
    """
    Hash table using quadratic probing for collision resolution
    Implements dynamic resizing and load factor maintenance
    """
    
    def __init__(self, initial_size: int = 16, max_load_factor: float = 0.75):
        """
        Initialize hash table
        
        Args:
            initial_size: Initial table size (should be power of 2)
            max_load_factor: Maximum load factor before resizing
        """
        # Ensure initial size is power of 2
        self.size = 1 << (initial_size - 1).bit_length()
        self.max_load_factor = max_load_factor
        self.table: List[Entry] = [Entry(None, None) for _ in range(self.size)]
        self.num_entries = 0
        self.num_active = 0  # Includes OCCUPIED and DELETED
        
        # Constants for quadratic probing
        self.c1 = 1  # Linear coefficient
        self.c2 = 1  # Quadratic coefficient
    
    def _hash(self, key: Any) -> int:
        """Primary hash function"""
        if isinstance(key, str):
            # DJB2 hash for strings
            hash_value = 5381
            for char in key:
                hash_value = ((hash_value << 5) + hash_value) + ord(char)
            return hash_value % self.size
        # Default hash for other types
        return hash(key) % self.size
    
    def _probe(self, key: Any, i: int) -> int:
        """
        Quadratic probe function: h(k,i) = (h'(k) + c1*i + c2*i^2) mod m
        where h'(k) is the primary hash function
        """
        h = self._hash(key)
        return (h + self.c1 * i + self.c2 * i * i) % self.size
    
    def _should_resize(self) -> bool:
        """Check if table needs resizing"""
        return self.num_active / self.size > self.max_load_factor
    
    def _resize(self, new_size: int = None) -> None:
        """
        Resize hash table
        
        Args:
            new_size: New size (if None, doubles current size)
        """
        if new_size is None:
            new_size = self.size * 2
            
        old_table = self.table
        self.size = new_size
        self.table = [Entry(None, None) for _ in range(self.size)]
        self.num_entries = 0
        self.num_active = 0
        
        # Reinsert all OCCUPIED entries
        for entry in old_table:
            if entry.status == EntryStatus.OCCUPIED:
                self.insert(entry.key, entry.value)
    
    def insert(self, key: Any, value: Any) -> bool:
        """
        Insert key-value pair into hash table
        
        Args:
            key: Key to insert
            value: Value to insert
            
        Returns:
            bool: True if insertion successful
        """
        if self._should_resize():
            self._resize()
            
        i = 0
        first_deleted = None
        
        while i < self.size:
            probe_idx = self._probe(key, i)
            entry = self.table[probe_idx]
            
            if entry.status == EntryStatus.EMPTY:
                # Insert at first deleted position if found
                insert_idx = first_deleted if first_deleted is not None else probe_idx
                self.table[insert_idx] = Entry(key, value, EntryStatus.OCCUPIED)
                self.num_entries += 1
                if first_deleted is None:
                    self.num_active += 1
                return True
                
            elif entry.status == EntryStatus.DELETED:
                if first_deleted is None:
                    first_deleted = probe_idx
                    
            elif entry.status == EntryStatus.OCCUPIED:
                if entry.key == key:  # Update existing key
                    self.table[probe_idx].value = value
                    return True
                    
            i += 1
            
        return False  # Table is full or no slot found
    
    def get(self, key: Any) -> Optional[Any]:
        """
        Retrieve value for key
        
        Args:
            key: Key to look up
            
        Returns:
            Value if found, None otherwise
        """
        i = 0
        while i < self.size:
            probe_idx = self._probe(key, i)
            entry = self.table[probe_idx]
            
            if entry.status == EntryStatus.EMPTY:
                return None
                
            if entry.status == EntryStatus.OCCUPIED and entry.key == key:
                return entry.value
                
            i += 1
            
        return None
    
    def delete(self, key: Any) -> bool:
        """
        Delete key from hash table
        
        Args:
            key: Key to delete
            
        Returns:
            bool: True if deletion successful
        """
        i = 0
        while i < self.size:
            probe_idx = self._probe(key, i)
            entry = self.table[probe_idx]
            
            if entry.status == EntryStatus.EMPTY:
                return False
                
            if entry.status == EntryStatus.OCCUPIED and entry.key == key:
                self.table[probe_idx].status = EntryStatus.DELETED
                self.num_entries -= 1
                return True
                
            i += 1
            
        return False
    
    def load_factor(self) -> float:
        """Calculate current load factor"""
        return self.num_entries / self.size
    
    def probe_statistics(self) -> Tuple[float, float]:
        """
        Calculate probe sequence statistics
        
        Returns:
            Tuple[float, float]: (average_probe_length, max_probe_length)
        """
        total_probes = 0
        max_probes = 0
        num_lookups = 0
        
        for entry in self.table:
            if entry.status == EntryStatus.OCCUPIED:
                probes = 0
                key = entry.key
                i = 0
                while i < self.size:
                    probe_idx = self._probe(key, i)
                    probes += 1
                    if self.table[probe_idx].key == key:
                        break
                    i += 1
                total_probes += probes
                max_probes = max(max_probes, probes)
                num_lookups += 1
                
        avg_probes = total_probes / num_lookups if num_lookups > 0 else 0
        return avg_probes, max_probes

def example_usage():
    """Demonstrate usage of quadratic hash table"""
    
    # Create hash table
    hash_table = QuadraticHashTable(initial_size=16)
    
    # Insert some key-value pairs
    test_data = [
        ("apple", 1),
        ("banana", 2),
        ("cherry", 3),
        ("date", 4),
        ("elderberry", 5)
    ]
    
    for key, value in test_data:
        hash_table.insert(key, value)
        
    # Retrieve values
    print(f"Value for 'apple': {hash_table.get('apple')}")
    print(f"Value for 'banana': {hash_table.get('banana')}")
    
    # Delete an entry
    hash_table.delete("cherry")
    print(f"Value for 'cherry' after deletion: {hash_table.get('cherry')}")
    
    # Get statistics
    load_factor = hash_table.load_factor()
    avg_probes, max_probes = hash_table.probe_statistics()
    
    print(f"Current load factor: {load_factor:.2f}")
    print(f"Average probes per lookup: {avg_probes:.2f}")
    print(f"Maximum probes required: {max_probes}")

if __name__ == "__main__":
    example_usage()
