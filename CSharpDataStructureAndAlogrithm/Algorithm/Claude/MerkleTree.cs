using System.Security.Cryptography;
using System.Text;
/// <summary>
/// A Merkle tree (or hash tree) is a data structure where each leaf node contains the hash of a data block, and each non-leaf node contains the hash of its child nodes. This allows efficient and secure verification of content in large data structures. They're widely used in distributed systems like Git, Bitcoin, and Cassandra to efficiently compare data across nodes.
/// </summary>
public class MerkleTree
{
    public class Node
    {
        public string Hash { get; set; }
        public Node Left { get; set; }
        public Node Right { get; set; }
    }

    private readonly SHA256 sha256 = SHA256.Create();

    public Node BuildTree(List<string> dataBlocks)
    {
        List<Node> leaves = dataBlocks
            .Select(block => new Node { Hash = ComputeHash(block) })
            .ToList();

        return BuildTreeRecursive(leaves);
    }

    private Node BuildTreeRecursive(List<Node> nodes)
    {
        if (nodes.Count == 1)
            return nodes[0];

        List<Node> parents = new List<Node>();

        for (int i = 0; i < nodes.Count; i += 2)
        {
            Node left = nodes[i];
            Node right = (i + 1 < nodes.Count) ? nodes[i + 1] : left;

            string combinedHash = ComputeHash(left.Hash + right.Hash);
            parents.Add(new Node
            {
                Hash = combinedHash,
                Left = left,
                Right = right
            });
        }

        return BuildTreeRecursive(parents);
    }

    private string ComputeHash(string data)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(data);
        byte[] hashBytes = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hashBytes);
    }
}