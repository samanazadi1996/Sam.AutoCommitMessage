using System.Collections.Generic;
using System.Linq;

namespace AutoCommitMessage.Models;

public class FileNode
{
    public string Name { get; set; }
    public FileType? Type { get; set; }
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

    public static List<FileNode> GetFileNode(List<FileData> files)
    {
        // Creating the root of the tree
        FileNode root = new FileNode("root");

        foreach (var file in files)
        {
            var parts = file.Location.Replace("../", "").Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var current = root;

            // Navigate or create each part of the path
            for (int i = 0; i < parts.Length; i++)
            {
                if (i == parts.Length - 1)
                {
                    // This is the file node
                    var fileNode = new FileNode(parts[i])
                    {
                        IsStaged = file.IsStaged,
                        Type = file.Type,
                        Parent = current
                    };
                    current.AddChild(fileNode);
                }
                else
                {
                    // This is a directory node
                    current = current.FindOrCreateChild(parts[i]);
                }
            }
        }

        return root.Children;
    }

}