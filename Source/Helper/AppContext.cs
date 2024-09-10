using System.IO;
using Microsoft.VisualStudio.Shell.Interop;

namespace AutoCommitMessage.Helper;

internal class AppContext
{
    public static string GetOpenedFolder()
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