using AutoCommitMessage.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoCommitMessage.Helper;

internal class AppConverter
{
    public static List<FileData> ConvertToFileDataLost(string str)
    {
        var lines = str
            .Split(new[] { "\n" }, StringSplitOptions.None)
            .Where(p => !string.IsNullOrEmpty(p));

        var types = new Dictionary<string, FileType>()
        {
            {"A",FileType.Added},
            {"??",FileType.Added},
            {"M",FileType.Modified},
            {"D",FileType.Deleted},
            {"R",FileType.Renamed}
        };
        List<FileData> result = [];

        foreach (var line in lines)
        {
            if (types.TryGetValue(line.Substring(0, 3).Trim(), out var type))
            {
                var model = new FileData()
                {
                    IsStaged = !line.StartsWith(" ") && !line.StartsWith("??"),
                    Location = line.Substring(2).Trim(),
                    Type = type
                };

                if (model.Type == FileType.Renamed)
                {
                    var fileRename = model.Location.Split(new[] { " -> " }, StringSplitOptions.None);
                    model.Text = $"'{Path.GetFileName(fileRename[0])}' was {model.Type.ToString().ToLower()} to `{Path.GetFileName(fileRename[1])}`";
                }
                else
                {
                    model.Text = $"'{Path.GetFileName(model.Location)}' was {model.Type.ToString().ToLower()}";
                }

                result.Add(model);
            }

        }

        return result;
    }

    public static List<FileNode> ConvertToFileNodeList(List<FileData> files)
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