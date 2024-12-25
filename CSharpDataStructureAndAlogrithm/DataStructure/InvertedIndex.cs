using System;

namespace DataStructure;

public class InvertedIndex
{
    public Dictionary<string, HashSet<int>> Index { get; } = new();
    public void Add(string term, int documentId)
    {
        if (Index.TryGetValue(term, out HashSet<int>? value))
        {
            value.Add(documentId);
        }
        else
        {
            Index[term] = [documentId];
        }
    }
    public void Remove(string term, int documentId)
    {
        if (Index.TryGetValue(term, out HashSet<int>? value))
        {
            value.Remove(documentId);
            if (value.Count == 0)
            {
                Index.Remove(term);
            }
        }
    }

    public void AddDocument(int docId, string content)
    {
        string[] terms = content.Split([' ', '.', ',', '!', '?'], StringSplitOptions.RemoveEmptyEntries);
        foreach (string term in terms)
        {
            string termLower = term.ToLower();
            if (!Index.TryGetValue(termLower, out HashSet<int>? value))
            {
                Index[termLower] = new HashSet<int> { docId };
            }
            else
            {
                value.Add(docId);
            }
        }
    }

    // Retrieving documents by term
    public HashSet<int>? GetDocuments(string term)
    {
        return Index.TryGetValue(term.ToLower(), out HashSet<int>? value) ? value : ([]);
    }
}