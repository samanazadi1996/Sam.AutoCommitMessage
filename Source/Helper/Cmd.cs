using System.Diagnostics;

namespace AutoCommitMessage.Helper
{
    internal class Cmd
    {
        public static string Shell(string app, string arg)
        {
            string directory = AppContext.GetOpenedFolder();
            if (string.IsNullOrEmpty(directory))
                return "Invalid directory";

            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                WorkingDirectory = directory,
                FileName = app,
                Arguments = arg,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using (Process process = new Process())
            {
                process.StartInfo = startInfo;
                process.Start();

                string result = process.StandardOutput.ReadToEnd();
                string error = process.StandardError.ReadToEnd();

                process.WaitForExit();

                if (!string.IsNullOrEmpty(error))
                {
                    return $"Git error: {error}";
                }

                return result;
            }
        }

    }
}
