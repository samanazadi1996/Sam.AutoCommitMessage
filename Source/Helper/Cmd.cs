using System.Diagnostics;

namespace AutoCommitMessage.Helper;

internal class Cmd
{
    public static string Shell(string app, string arg)
    {
        var directory = AppContext.GetOpenedFolder();
        if (string.IsNullOrEmpty(directory))
            return "Invalid directory";

        var startInfo = new ProcessStartInfo
        {
            WorkingDirectory = directory,
            FileName = app,
            Arguments = arg,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        using var process = new Process();
        process.StartInfo = startInfo;
        process.Start();

        var result = process.StandardOutput.ReadToEnd();

        process.WaitForExit();

        return result;
    }

}

