using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;
using System.IO;

namespace AutoCommitMessage.Helper
{
    internal class Cmd
    {
        public static string Shell(string app,string arg)
        {
            string directory = GetOpenedFolder();
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
            string GetOpenedFolder()
            {
                ThreadHelper.ThrowIfNotOnUIThread();

                var solutionService = Package.GetGlobalService(typeof(SVsSolution)) as IVsSolution;
                if (solutionService == null)
                    return null;

                solutionService.GetSolutionInfo(out string solutionDir, out string solutionFile, out string userOptsFile);

                if (!string.IsNullOrEmpty(solutionFile) && Directory.Exists(solutionDir))
                {
                    return solutionDir;
                }

                return null;
            }


        }

    }
}
