namespace AutoCommitMessage.Models;

public class FileData
{
    public string Text { get; set; }
    public string Location { get; set; }
    public FileType Type { get; set; }
    public bool IsStaged { get; set; }
}