using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AutoCommitMessage.Models;

public class FileData
{
    public string Text { get; set; }
    public string Location { get; set; }
    public FileType Type { get; set; }
    public bool IsStaged { get; set; }
    public static IEnumerable<FileData> GetListData(string str)
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

                yield return model;
            }

        }
    }

}