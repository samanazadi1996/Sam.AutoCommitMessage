using AutoCommitMessage.Helper;
using AutoCommitMessage.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutoCommitMessage;

public partial class SettingsControl : UserControl
{ 
    public SettingsControl()
    {
        InitializeComponent();

        LoadSettings();
    }

    private async void Back_OnClick(object sender, RoutedEventArgs e)
    {
        await AutoCommitMessage.Settings.HideAsync();
    }
    private async void Save_OnClick(object sender, RoutedEventArgs e)
    {
        var model = new SettingsModel.Model
        {
            StagedFileColor = StagedColorPicker.SelectedColor ?? Colors.LimeGreen,
            UnStagedFileColor = UnstagedColorPicker.SelectedColor ?? Colors.OrangeRed,
            CommitButton = CommitButton.IsChecked == true,
            GenerateMessageButton = GenerateMessageButton.IsChecked == true,
            PullButton = PullButton.IsChecked == true,
            PushButton = PushButton.IsChecked == true,
            RefreshButton = RefreshButton.IsChecked == true,
            StageAllButton = StageAllButton.IsChecked == true
        };

        SettingsModel.Save(ApplicationContext.GetOpenedFolder(), model);
        await AutoCommitMessage.Settings.HideAsync();
    }

    private void Refresh_OnClick(object sender, RoutedEventArgs e)
    {
        LoadSettings();
    }
    private void LoadSettings()
    {
        SettingsModel.Init(ApplicationContext.GetOpenedFolder());

        if (SettingsModel.Data is null) return;

        StagedColorPicker.SelectedColor = SettingsModel.Data.StagedFileColor;
        UnstagedColorPicker.SelectedColor = SettingsModel.Data.UnStagedFileColor;

        CommitButton.IsChecked = SettingsModel.Data.CommitButton;
        GenerateMessageButton.IsChecked = SettingsModel.Data.GenerateMessageButton;
        PullButton.IsChecked = SettingsModel.Data.PullButton;
        PushButton.IsChecked = SettingsModel.Data.PushButton;
        RefreshButton.IsChecked = SettingsModel.Data.RefreshButton;
        StageAllButton.IsChecked = SettingsModel.Data.StageAllButton;
    }
}