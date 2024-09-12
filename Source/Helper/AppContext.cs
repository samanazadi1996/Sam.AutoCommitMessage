using Microsoft.VisualStudio.Shell.Interop;
using System.IO;

namespace AutoCommitMessage.Helper;

internal class AppContext
{
    public static string GetOpenedFolder()
    {
        ThreadHelper.ThrowIfNotOnUIThread();

        if (Package.GetGlobalService(typeof(SVsSolution)) is not IVsSolution solutionService)
            return null;

        solutionService.GetSolutionInfo(out var solutionDir, out var solutionFile, out var userOptsFile);

        if (!string.IsNullOrEmpty(solutionFile) && Directory.Exists(solutionDir))
        {
            return solutionDir;
        }

        return null;
    }

}