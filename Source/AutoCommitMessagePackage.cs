global using Community.VisualStudio.Toolkit;
global using Microsoft.VisualStudio.Shell;
global using System;
global using Task = System.Threading.Tasks.Task;
using AutoCommitMessage.EventHandlers;
using Microsoft.VisualStudio.Shell.Interop;
using System.Runtime.InteropServices;
using System.Threading;

namespace AutoCommitMessage;

[PackageRegistration(UseManagedResourcesOnly = true, AllowsBackgroundLoading = true)]
[InstalledProductRegistration(Vsix.Name, Vsix.Description, Vsix.Version)]
[ProvideToolWindow(typeof(MyToolWindow.Pane), Style = VsDockStyle.Tabbed, Window = WindowGuids.SolutionExplorer)]
[ProvideMenuResource("Menus.ctmenu", 1)]
[Guid(PackageGuids.AutoCommitMessageString)]
public sealed class AutoCommitMessagePackage : ToolkitPackage
{
    protected override async Task InitializeAsync(CancellationToken cancellationToken, IProgress<ServiceProgressData> progress)
    {
        await this.RegisterCommandsAsync();

        this.RegisterToolWindows();
        // Add your initialization code here
        await JoinableTaskFactory.SwitchToMainThreadAsync(cancellationToken);
        var solutionService = await GetServiceAsync(typeof(SVsSolution)) as IVsSolution;
        var solutionEventHandle = new SolutionEventHandle(solutionService);

    }
}

