namespace DataStructure;

public class TreeNode
{
    public string Data { get; set; }
    public List<string> Children { get; set; }

    public TreeNode(string data)
    {
        Data = data;
        Children = new List<string>();
    }
}
