using System.IO;
using System.Text.Json;
using System.Windows.Media;

namespace AutoCommitMessage.Models;

public class SettingsModel
{
    private static string oldPath { get; set; }
    public static Model Data { get; set; }

    public static void Init(string path)
    {
        if (oldPath == path) return;
        if (!string.IsNullOrWhiteSpace(path))
        {

            var dataPath = Path.Combine(path, ".vs", "AutoCommitMessage.json");
            if (File.Exists(dataPath))
            {
                Data = JsonSerializer.Deserialize<Model>(File.ReadAllText(dataPath));

                oldPath = path;
                return;
            }
        }

        Data = new Model()
        {
            StagedFileColor = Colors.LimeGreen,
            UnStagedFileColor = Colors.OrangeRed
        };
        oldPath = path;

    }
    public static void Save(string path, Model model)
    {
        if (string.IsNullOrWhiteSpace(path)) return;

        var dataPathDir = Path.Combine(path, ".vs");
        if (!Directory.Exists(dataPathDir))
            Directory.CreateDirectory(dataPathDir);

        var dataPath = Path.Combine(path, ".vs", "AutoCommitMessage.json");

        File.WriteAllText(dataPath, JsonSerializer.Serialize(model));

        Data = model;
    }
    public class Model
    {
        public Color StagedFileColor { get; set; }
        public Color UnStagedFileColor { get; set; }

    }
}
