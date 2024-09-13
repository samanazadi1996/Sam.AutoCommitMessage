using AutoCommitMessage.Models;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoCommitMessage.Helper;

internal class AppConverter
{
    public static List<FileData> ConvertToFileDataLost(string str)
    {

        var lines = Split(str, "\n")
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
                var change = line.Substring(2).Trim();
                var model = new FileData()
                {
                    IsStaged = !line.StartsWith(" ") && !line.StartsWith("??"),
                    Location = Split(change, " -> ").LastOrDefault(),
                    Type = type
                };

                if (model.Type == FileType.Renamed)
                {
                    var fileRename = Split(change, " -> ");
                    model.Text = $"'{ConvertLocationToFilename(fileRename[0])}' was {model.Type.ToString().ToLower()} to `{ConvertLocationToFilename(fileRename[1])}`";
                }
                else
                {
                    model.Text = $"'{ConvertLocationToFilename(model.Location)}' was {model.Type.ToString().ToLower()}";
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
            var parts = Split(file.Location.Replace("../", ""), "/");

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

    private static string ConvertLocationToFilename(string location)
    {
        return Path.GetFileName(location.Replace("\"", "").Trim());
    }
    private static string[] Split(string data, string str)
    {
        return data.Split([str], StringSplitOptions.None);
    }
}