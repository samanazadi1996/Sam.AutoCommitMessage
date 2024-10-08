using AutoCommitMessage.EventHandlers;
using AutoCommitMessage.Helper;
using AutoCommitMessage.Models;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutoCommitMessage;

public partial class MyToolWindowControl : UserControl
{
    public List<FileData> ChangeListData { get; set; }
    public MyToolWindowControl()
    {
        InitializeComponent();

        SettingsModel.SettingsChanged += LoadSettings;
        SolutionEventHandle.OnAfterOpenSolutionAction += LoadSettings;
    }


    private void LoadSettings()
    {
        ReloadChangeListData();

        UpdateVisibility(PullButton, SettingsModel.Data.PullButton);
        UpdateVisibility(RefreshButton, SettingsModel.Data.RefreshButton);
        UpdateVisibility(StageAllButton, SettingsModel.Data.StageAllButton);
        UpdateVisibility(GenerateMessageButton, SettingsModel.Data.GenerateMessageButton);
        UpdateVisibility(CommitButton, SettingsModel.Data.CommitButton);
        UpdateVisibility(PushButton, SettingsModel.Data.PushButton);
        return;

        void UpdateVisibility(Button button, bool visibility)
        {
            button.Visibility = visibility ? Visibility.Visible : Visibility.Collapsed;
        }

    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
        LoadSettings();
        base.OnGotFocus(e);
    }

    private void ReloadChangeListData()
    {
        try
        {
            var cFolder = ApplicationContext.GetOpenedFolder();

            SettingsModel.Init(cFolder);


            MyTreeViewItem.Header = cFolder;

            var gitShell = Cmd.Shell("git", "status -s");

            ChangeListData = AppConverter.ConvertToFileDataLost(gitShell);

            //_fileChangeWatcher.StartWatching();

            ReloadTreeView();
        }
        catch
        {
            // ignored
        }
    }
    private void GenerateMessageButton_OnClick(object sender, RoutedEventArgs e)
    {
        try
        {
            ReloadChangeListData();

            var stagedItems = ChangeListData.Where(p => p.IsStaged).ToList();

            if (stagedItems.Any())
            {
                CommitMessage.Text = GetCommitMessage(stagedItems);
                CommitDescription.Text = string.Join(Environment.NewLine, stagedItems.Select(p => $"{p.Type} : {p.Location}"));

                UpdateTextMessage("Generate Message");
            }
            else
            {
                CommitDescription.Text = string.Empty;

                UpdateTextMessage("No Change on Stage");
            }

        }
        catch
        {
            UpdateTextMessage("Error");
        }

        return;

        string GetCommitMessage(List<FileData> changeListData)
        {
            var detailedMessage = string.Join(" and ", changeListData.Select(p => p.Text));

            if (detailedMessage.Length <= 150) return detailedMessage;

            var summarizedMessage = changeListData
                .GroupBy(change => change.Type)
                .Select(group => $"{group.Count()} {(group.Count() > 1 ? "files" : "file")} {group.Key.ToString().ToLower()}");

            return string.Join(" and ", summarizedMessage);

        }
    }

    private void ReloadTreeView()
    {
        MyTreeViewItem.Items.Clear();

        foreach (var path in AppConverter.ConvertToFileNodeList(ChangeListData))
        {
            AddFileToTree(MyTreeViewItem, path);
        }

        MyTreeViewItem.ExpandSubtree();
        return;

        void AddFileToTree(TreeViewItem rootItem, FileNode fileNode)
        {

            var newItem = new TreeViewItem
            {
                Header = fileNode.Name,
            };

            if (fileNode.IsStaged.HasValue)
            {
                newItem.Foreground = fileNode.IsStaged.Value
                    ? new SolidColorBrush(SettingsModel.Data.StagedFileColor)
                    : new SolidColorBrush(SettingsModel.Data.UnStagedFileColor);


                newItem.MouseDoubleClick += (s, e) => { ToggleStage(fileNode); };
            }
            rootItem.Items.Add(newItem);

            if (!fileNode.Children.Any()) return;

            foreach (var itemChild in fileNode.Children)
            {
                AddFileToTree(newItem, itemChild);
            }
        }
        void ToggleStage(FileNode file)
        {
            var name = file.Name;
            GetParentName(file.Parent);

            var find = ChangeListData.FirstOrDefault(p => p.Location.EndsWith(name));
            if (find is not null)
            {
                find.IsStaged = !find.IsStaged;
                var fileName = find.Location.Split(new[] { " -> " }, StringSplitOptions.None)[0];

                Cmd.Shell("git", find.IsStaged ? $"add \"{fileName}\"" : $"restore --staged \"{fileName}\"");
            }
            ReloadTreeView();
            return;

            void GetParentName(FileNode fn)
            {
                while (true)
                {
                    if (fn == null) return;
                    name = fn.Name + "/" + name;

                    fn = fn.Parent;
                }
            }
        }
    }

    private void Commit_OnClick(object sender, RoutedEventArgs e)
    {
        if (string.IsNullOrWhiteSpace(CommitMessage.Text))
        {
            ShowVsMessageBox("Commit message is required");
            return;
        }

        var cli = "commit" + AddMessage(CommitMessage.Text) + AddMessage(CommitDescription.Text);

        var message = Cmd.Shell("git", cli);

        ReloadChangeListData();
        ClearMessages();

        ShowVsMessageBox(message);

        return;

        string AddMessage(string msg)
            => string.IsNullOrWhiteSpace(msg) ? string.Empty : $" -m \"{msg}\"";
    }

    private void Push_OnClick(object sender, RoutedEventArgs e)
    {
        var message = Cmd.Shell("git", "push");

        ReloadChangeListData();

        if (string.IsNullOrWhiteSpace(message))
            message = "git push successful";

        ShowVsMessageBox(message);
    }
    private void ClearMessages()
    {
        CommitMessage.Text = string.Empty;
        CommitDescription.Text = string.Empty;
    }

    private void UpdateTextMessage(string text)
    {
        TextMessage.Text = $"{text} at {DateTime.Now:hh:mm:ss}";
    }

    private void Refresh_OnClick(object sender, RoutedEventArgs e)
    {
        ReloadChangeListData();
    }

    private void Pull_OnClick(object sender, RoutedEventArgs e)
    {
        var message = Cmd.Shell("git", "pull");

        UpdateTextMessage("Pull");

        ReloadChangeListData();

        ShowVsMessageBox(message);
    }

    private void StageAll_OnClick(object sender, RoutedEventArgs e)
    {
        Cmd.Shell("git", "add .");

        ReloadChangeListData();
    }

    private async void ShowSettings_OnClick(object sender, RoutedEventArgs e)
    {
        await Settings.ShowAsync();
    }

    public void ShowVsMessageBox(string message)
    {
        if (string.IsNullOrWhiteSpace(message)) return;

        VS.MessageBox.Show(message, buttons: OLEMSGBUTTON.OLEMSGBUTTON_OK);
    }
}

