using System.Security.Cryptography;
using System.Text;

namespace Algorithm.Claude
{
    /// <summary>
    /// Consistent hashing is a technique that distributes data across a cluster in a way that minimizes reorganization when nodes are added or removed. When the number of nodes changes, only K/n keys need to be remapped (where K is the number of keys and n is the number of nodes), rather than remapping almost all keys as in traditional hash tables. This makes it ideal for distributed caches and databases.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ConsistentHash<T>
    {
        private readonly SortedDictionary<int, T> _ring = new SortedDictionary<int, T>();
        private readonly int _replicas;
        private readonly HashAlgorithm _hashAlgorithm;

        public ConsistentHash(int replicas, IEnumerable<T> nodes)
        {
            _replicas = replicas;
            _hashAlgorithm = MD5.Create();

            foreach (T node in nodes)
                AddNode(node);
        }

        public void AddNode(T node)
        {
            for (int i = 0; i < _replicas; i++)
            {
                string key = $"{node}:{i}";
                int hashKey = GetHash(key);
                _ring[hashKey] = node;
            }
        }

        public void RemoveNode(T node)
        {
            for (int i = 0; i < _replicas; i++)
            {
                string key = $"{node}:{i}";
                int hashKey = GetHash(key);
                _ring.Remove(hashKey);
            }
        }

        public T GetNode(string key)
        {
            if (_ring.Count == 0)
                throw new InvalidOperationException("No nodes available");

            int hash = GetHash(key);

            if (!_ring.ContainsKey(hash))
            {
                var keys = _ring.Keys;
                var firstGreaterKey = keys.FirstOrDefault(k => k >= hash);
                hash = firstGreaterKey == 0 ? keys.First() : firstGreaterKey;
            }

            return _ring[hash];
        }

        private int GetHash(string key)
        {
            byte[] keyBytes = Encoding.UTF8.GetBytes(key);
            byte[] hashedBytes = _hashAlgorithm.ComputeHash(keyBytes);
            return BitConverter.ToInt32(hashedBytes, 0);
        }
    }
}