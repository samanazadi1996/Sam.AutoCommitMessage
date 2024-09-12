using System.Collections.Generic;
using System.Linq;

namespace AutoCommitMessage.Models;

public class FileNode
{
    public string Name { get; set; }
    public bool? IsStaged { get; set; }
    public List<FileNode> Children { get; set; } = new List<FileNode>();
    public FileNode Parent { get; set; }

    public FileNode(string name)
    {
        Name = name;
    }

    public void AddChild(FileNode child)
    {
        Children.Add(child);
    }

    public FileNode FindOrCreateChild(string name)
    {
        var child = Children.FirstOrDefault(c => c.Name == name);
        if (child == null)
        {
            child = new FileNode(name);
            AddChild(child);
        }
        return child;
    }
}