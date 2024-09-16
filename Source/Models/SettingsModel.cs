using System.IO;
using System.Text.Json;
using System.Windows.Media;

namespace AutoCommitMessage.Models;

public class SettingsModel
{
    private const string FileSettingsName = "AutoCommitMessage_v1.json";
    public static event Action SettingsChanged;
    private static string OldPath { get; set; }
    public static Model Data { get; set; }

    public static void Init(string path)
    {
        if (OldPath == path) return;
        if (!string.IsNullOrWhiteSpace(path))
        {

            var dataPath = Path.Combine(path, ".vs", FileSettingsName);
            if (File.Exists(dataPath))
            {
                Data = JsonSerializer.Deserialize<Model>(File.ReadAllText(dataPath));

                OldPath = path;
                return;
            }
        }

        Data = new Model()
        {
            StagedFileColor = Colors.LimeGreen,
            UnStagedFileColor = Colors.OrangeRed,
            CommitButton = true,
            GenerateMessageButton = true,
            PullButton = true,
            PushButton = true,
            RefreshButton = true,
            StageAllButton = true
        };
        OldPath = path;

    }
    public static void Save(string path, Model model)
    {
        if (string.IsNullOrWhiteSpace(path)) return;

        var dataPathDir = Path.Combine(path, ".vs");
        if (!Directory.Exists(dataPathDir))
            Directory.CreateDirectory(dataPathDir);

        var dataPath = Path.Combine(path, ".vs", FileSettingsName);

        File.WriteAllText(dataPath, JsonSerializer.Serialize(model));

        Data = model;
        SettingsChanged?.Invoke();

    }
    public class Model
    {
        public Color StagedFileColor { get; set; }
        public Color UnStagedFileColor { get; set; }
        public bool PullButton { get; set; }
        public bool RefreshButton { get; set; }
        public bool StageAllButton { get; set; }
        public bool GenerateMessageButton { get; set; }
        public bool CommitButton { get; set; }
        public bool PushButton { get; set; }
    }
}
