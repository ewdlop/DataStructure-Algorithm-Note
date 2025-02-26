using System.Collections;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// A Bloom filter is a space-efficient probabilistic data structure used to test whether an element is a member of a set. It may yield false positives but never false negatives. Bloom filters are used in distributed systems to reduce unnecessary network requests by quickly determining if data might exist before making expensive lookups.
/// </summary>
public class BloomFilter
{
    private readonly BitArray _bits;
    private readonly int _size;
    private readonly int _hashFunctions;
    private readonly HashAlgorithm _hashAlgorithm;

    public BloomFilter(int size, int hashFunctions)
    {
        _size = size;
        _hashFunctions = hashFunctions;
        _bits = new BitArray(size);
        _hashAlgorithm = MD5.Create();
    }

    public void Add(string item)
    {
        byte[] baseHash = _hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(item));

        // Create multiple hash functions using the double hashing technique
        for (int i = 0; i < _hashFunctions; i++)
        {
            int hashValue = Math.Abs((BitConverter.ToInt32(baseHash, 0) + i * BitConverter.ToInt32(baseHash, 4)) % _size);
            _bits[hashValue] = true;
        }
    }

    public bool MightContain(string item)
    {
        byte[] baseHash = _hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(item));

        for (int i = 0; i < _hashFunctions; i++)
        {
            int hashValue = Math.Abs((BitConverter.ToInt32(baseHash, 0) + i * BitConverter.ToInt32(baseHash, 4)) % _size);
            if (!_bits[hashValue])
                return false; // Definitely not in the set
        }

        return true; // Might be in the set
    }
}