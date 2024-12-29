using System;

namespace DataStructure;

public class InvertedIndex
{
    public virtual Dictionary<string, HashSet<int>> Index { get; } = [];
    
    public virtual void Add(string term, int documentId)
    {
        if (Index.TryGetValue(term, out HashSet<int>? value))
        {
            (value ??= []).Add(documentId);
        }
        else
        {
            Index[term] = [documentId];
        }
    }

    public virtual bool TryAdd(string term, int documentId)
    {
        if (Index.TryGetValue(term, out HashSet<int>? value))
        {
            return (value ??= []).Add(documentId);
        }
        Index[term] = [documentId];
        return true;
    }

    public virtual void Remove(string term, int documentId)
    {
        if (Index.TryGetValue(term, out HashSet<int>? value))
        {
            value?.Remove(documentId);
            if (value?.Count == 0)
            {
                Index.Remove(term);
            }
        }
    }

    public virtual bool TryRemove(string term, int documentId)
    {
        if (Index.TryGetValue(term, out HashSet<int>? value))
        {
            if(value is null) return false;
            value.Remove(documentId);
            if (value.Count == 0)
            {
                Index.Remove(term);
            }
            return true;
        }
        return false;
    }

    public virtual void AddDocument(int docId, string content)
    {
        string[] terms = content.Split([' ', '.', ',', '!', '?'], StringSplitOptions.RemoveEmptyEntries);
        foreach (string term in terms)
        {
            string termLower = term.ToLower();
            if (!Index.TryGetValue(termLower, out HashSet<int>? value))
            {
                Index[termLower] = [docId];
            }
            else
            {
                (value ??= []).Add(docId);
            }
        }
    }

    // Retrieving documents by term
    public virtual HashSet<int>? GetDocuments(string term)
    {
        return Index.TryGetValue(term.ToLower(), out HashSet<int>? value) ? value : ([]);
    }
}