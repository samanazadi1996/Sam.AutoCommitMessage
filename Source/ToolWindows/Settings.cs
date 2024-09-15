using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.VisualStudio.Imaging;

namespace AutoCommitMessage;

public class Settings : BaseToolWindow<Settings>
{
    public override string GetTitle(int toolWindowId) => "Auto Commit Message - Settings";

    public override Type PaneType => typeof(Pane);

    public override Task<FrameworkElement> CreateAsync(int toolWindowId, CancellationToken cancellationToken)
    {
        return Task.FromResult<FrameworkElement>(new SettingsControl());
    }

    [Guid("27ca1f56-77b8-4887-972f-f7f9e6aa9800")]
    internal class Pane : ToolkitToolWindowPane
    {
        public Pane()
        {
            BitmapImageMoniker = KnownMonikers.ToolWindow;
        }
    }
}