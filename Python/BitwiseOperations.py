from typing import List, Dict, Any, Union, Optional
import mmh3  # MurmurHash3
import math
from bitarray import bitarray
import numpy as np

class HashFunctions:
    """Various hash function implementations"""
    
    @staticmethod
    def djb2_hash(string: str) -> int:
        """
        DJB2 hash function - simple but effective string hashing
        """
        hash_value = 5381
        for char in string:
            hash_value = ((hash_value << 5) + hash_value) + ord(char)
            hash_value = hash_value & 0xFFFFFFFF  # Keep 32 bits
        return hash_value
    
    @staticmethod
    def fnv1a_hash(data: bytes) -> int:
        """
        FNV-1a hash function - good for arbitrary byte sequences
        """
        FNV_PRIME = 0x01000193
        FNV_OFFSET = 0x811C9DC5
        
        hash_value = FNV_OFFSET
        for byte in data:
            hash_value ^= byte
            hash_value *= FNV_PRIME
            hash_value = hash_value & 0xFFFFFFFF  # Keep 32 bits
        return hash_value
    
    @staticmethod
    def murmur3_hash(key: Union[str, bytes], seed: int = 0) -> int:
        """
        MurmurHash3 - excellent distribution and performance
        """
        if isinstance(key, str):
            key = key.encode('utf-8')
        return mmh3.hash(key, seed)

class BloomFilter:
    """Bloom filter for probabilistic set membership"""
    
    def __init__(self, size: int, num_hash_functions: int):
        self.size = size
        self.num_hash_functions = num_hash_functions
        self.bit_array = bitarray(size)
        self.bit_array.setall(0)
        
    def _get_hash_values(self, item: str) -> List[int]:
        hash_values = []
        for seed in range(self.num_hash_functions):
            hash_value = mmh3.hash(str(item), seed) % self.size
            hash_values.append(hash_value)
        return hash_values
    
    def add(self, item: str) -> None:
        """Add an item to the Bloom filter"""
        for index in self._get_hash_values(item):
            self.bit_array[index] = 1
    
    def contains(self, item: str) -> bool:
        """Check if item might be in the set"""
        return all(self.bit_array[index] for index in self._get_hash_values(item))
    
    def estimate_false_positive_rate(self) -> float:
        """Estimate current false positive rate"""
        num_bits_set = self.bit_array.count(1)
        return (1 - math.exp(-self.num_hash_functions * num_bits_set / self.size)) ** self.num_hash_functions

class ConsistentHashing:
    """Consistent hashing implementation for distributed systems"""
    
    def __init__(self, num_replicas: int = 100):
        self.num_replicas = num_replicas
        self.hash_ring = {}  # Virtual node -> physical node
        self.nodes = set()
    
    def add_node(self, node: str) -> None:
        """Add a node to the hash ring"""
        self.nodes.add(node)
        for i in range(self.num_replicas):
            replica_key = f"{node}:{i}"
            hash_value = HashFunctions.murmur3_hash(replica_key)
            self.hash_ring[hash_value] = node
            
    def remove_node(self, node: str) -> None:
        """Remove a node from the hash ring"""
        self.nodes.remove(node)
        for i in range(self.num_replicas):
            replica_key = f"{node}:{i}"
            hash_value = HashFunctions.murmur3_hash(replica_key)
            self.hash_ring.pop(hash_value, None)
            
    def get_node(self, key: str) -> Optional[str]:
        """Get node responsible for key"""
        if not self.hash_ring:
            return None
            
        hash_value = HashFunctions.murmur3_hash(key)
        nodes = sorted(self.hash_ring.keys())
        
        # Binary search for the first node with hash >= key's hash
        idx = bisect.bisect(nodes, hash_value)
        if idx == len(nodes):
            idx = 0
        return self.hash_ring[nodes[idx]]

class BitwiseOperations:
    """Utility class for bitwise operations"""
    
    @staticmethod
    def count_set_bits(n: int) -> int:
        """Count number of 1s in binary representation"""
        count = 0
        while n:
            n &= (n - 1)  # Clear least significant set bit
            count += 1
        return count
    
    @staticmethod
    def is_power_of_two(n: int) -> bool:
        """Check if number is power of 2"""
        return n > 0 and (n & (n - 1)) == 0
    
    @staticmethod
    def get_bit(num: int, i: int) -> bool:
        """Get i-th bit"""
        return bool(num & (1 << i))
    
    @staticmethod
    def set_bit(num: int, i: int) -> int:
        """Set i-th bit to 1"""
        return num | (1 << i)
    
    @staticmethod
    def clear_bit(num: int, i: int) -> int:
        """Clear i-th bit"""
        return num & ~(1 << i)
    
    @staticmethod
    def update_bit(num: int, i: int, value: bool) -> int:
        """Update i-th bit to given value"""
        mask = ~(1 << i)
        return (num & mask) | (value << i)
    
    @staticmethod
    def reverse_bits(n: int, bit_length: int = 32) -> int:
        """Reverse bits in integer"""
        result = 0
        for i in range(bit_length):
            result = (result << 1) | (n & 1)
            n >>= 1
        return result
    
    @staticmethod
    def find_missing_number(arr: List[int]) -> int:
        """Find missing number using XOR"""
        n = len(arr) + 1
        xor_sum = 0
        
        # XOR all numbers from 1 to n
        for i in range(1, n + 1):
            xor_sum ^= i
            
        # XOR with all array elements
        for num in arr:
            xor_sum ^= num
            
        return xor_sum

class BitVector:
    """Efficient bit vector implementation"""
    
    def __init__(self, size: int):
        self.size = size
        self.vector = bytearray((size + 7) // 8)
    
    def set_bit(self, index: int) -> None:
        """Set bit at index to 1"""
        if 0 <= index < self.size:
            byte_index = index // 8
            bit_index = index % 8
            self.vector[byte_index] |= (1 << bit_index)
    
    def clear_bit(self, index: int) -> None:
        """Set bit at index to 0"""
        if 0 <= index < self.size:
            byte_index = index // 8
            bit_index = index % 8
            self.vector[byte_index] &= ~(1 << bit_index)
    
    def get_bit(self, index: int) -> bool:
        """Get bit value at index"""
        if 0 <= index < self.size:
            byte_index = index // 8
            bit_index = index % 8
            return bool(self.vector[byte_index] & (1 << bit_index))
        return False
    
    def count_set_bits(self) -> int:
        """Count number of set bits"""
        return sum(bin(byte).count('1') for byte in self.vector)

def example_usage():
    """Demonstrate usage of hash functions and bitwise operations"""
    
    # Hash Functions
    text = "Hello, World!"
    print(f"DJB2 hash: {HashFunctions.djb2_hash(text)}")
    print(f"FNV-1a hash: {HashFunctions.fnv1a_hash(text.encode())}")
    print(f"MurmurHash3: {HashFunctions.murmur3_hash(text)}")
    
    # Bloom Filter
    bloom = BloomFilter(size=1000, num_hash_functions=3)
    bloom.add("apple")
    bloom.add("banana")
    print(f"'apple' in filter: {bloom.contains('apple')}")
    print(f"'orange' in filter: {bloom.contains('orange')}")
    
    # Bitwise Operations
    num = 42
    print(f"Set bits in {num}: {BitwiseOperations.count_set_bits(num)}")
    print(f"Is power of 2: {BitwiseOperations.is_power_of_two(num)}")
    
    # Bit Vector
    bv = BitVector(100)
    bv.set_bit(5)
    bv.set_bit(10)
    print(f"Bit 5 is set: {bv.get_bit(5)}")
    print(f"Number of set bits: {bv.count_set_bits()}")
    
    # Find missing number
    arr = [1, 2, 4, 5, 6]
    missing = BitwiseOperations.find_missing_number(arr)
    print(f"Missing number: {missing}")

if __name__ == "__main__":
    example_usage()
