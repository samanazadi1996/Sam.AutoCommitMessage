namespace AutoCommitMessage;

[Command(PackageIds.MyCommand2)]
internal sealed class SettingsCommand : BaseCommand<SettingsCommand>
{
    protected override Task ExecuteAsync(OleMenuCmdEventArgs e)
    {
        return Settings.ShowAsync();
    }
}