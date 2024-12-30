namespace DataStructure;

public class Trie
{
    public class TrieNode
    {
        public Dictionary<char, TrieNode> Children = new Dictionary<char, TrieNode>();
        public bool IsEndOfWord;
    }

    private TrieNode root;

    public Trie()
    {
        root = new TrieNode();
    }

    public void Insert(string word)
    {
        var node = root;
        foreach (var ch in word)
        {
            if (!node.Children.ContainsKey(ch))
                node.Children[ch] = new TrieNode();
            node = node.Children[ch];
        }
        node.IsEndOfWord = true;
    }

    public bool Search(string word)
    {
        var node = root;
        foreach (var ch in word)
        {
            if (!node.Children.ContainsKey(ch))
                return false;
            node = node.Children[ch];
        }
        return node.IsEndOfWord;
    }

    public bool StartsWith(string prefix)
    {
        var node = root;
        foreach (var ch in prefix)
        {
            if (!node.Children.ContainsKey(ch))
                return false;
            node = node.Children[ch];
        }
        return true;
    }

    public void Delete(string word)
    {
        Delete(root, word, 0);
    }

    private bool Delete(TrieNode node, string word, int index)
    {
        if (index == word.Length)
        {
            if (!node.IsEndOfWord)
                return false;
            node.IsEndOfWord = false;
            return node.Children.Count == 0;
        }
        if (!node.Children.ContainsKey(word[index]))
            return false;
        var shouldDeleteCurrentNode = Delete(node.Children[word[index]], word, index + 1);
        if (shouldDeleteCurrentNode)
        {
            node.Children.Remove(word[index]);
            return node.Children.Count == 0;
        }
        return false;
    }

    public static void Test()
    {
        var trie = new Trie();
        trie.Insert("apple");
        Console.WriteLine(trie.Search("apple")); // True
        Console.WriteLine(trie.Search("app")); // False
        Console.WriteLine(trie.StartsWith("app")); // True
        trie.Insert("app");
        Console.WriteLine(trie.Search("app")); // True
        trie.Delete("app");
        Console.WriteLine(trie.Search("app")); // False
    }

    public static void Main(string[] args)
    {
        Test();
    }

    public void Display(TrieNode node, string word)
    {
        if (node.IsEndOfWord)
            Console.WriteLine(word);
        foreach (var child in node.Children)
            Display(child.Value, word + child.Key);
    }

    public void AutoComplete(string prefix)
    {
        var node = root;
        foreach (var ch in prefix)
        {
            if (!node.Children.ContainsKey(ch))
                return;
            node = node.Children[ch];
        }
        Display(node, prefix);
    }

    public void AutoCompleteTest()
    {
        var trie = new Trie();
        trie.Insert("apple");
        trie.Insert("app");
        trie.Insert("apricot");
        trie.Insert("banana");
        trie.Insert("cherry");
        trie.Insert("date");
        trie.Insert("grape");

        Console.WriteLine("Autocomplete suggestions for 'ap':");
        AutoComplete("ap");
        Console.WriteLine(
            "Autocomplete suggestions for 'ban':");
        AutoComplete("ban");
        Console.WriteLine(
            "Autocomplete suggestions for 'z':");
    }

    public void AutoCompleteMain(string[] args)
    {
        AutoCompleteTest();
        AutoComplete("ap");
    }
}