using System.Linq;
using System.Reflection;
using AutoCommitMessage.Helper;
using AutoCommitMessage.Models;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutoCommitMessage;

public partial class SettingsControl : UserControl
{
    public string[] AvailableColors { get; set; }
    public SettingsControl()
    {
        InitializeComponent();

        LoadColors();

        PopulateColorDropdowns();

        LoadButtonstates();
    }

    private void LoadButtonstates()
    {
        CommitButton.IsChecked = SettingsModel.Data.CommitButton;
        GenerateMessageButton.IsChecked = SettingsModel.Data.GenerateMessageButton;
        PullButton.IsChecked = SettingsModel.Data.PullButton;
        PushButton.IsChecked = SettingsModel.Data.PushButton;
        RefreshButton.IsChecked = SettingsModel.Data.RefreshButton;
        StageAllButton.IsChecked = SettingsModel.Data.StageAllButton;
    }
    private void LoadColors()
    {
        AvailableColors = typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public)
            .Where(p => p.PropertyType == typeof(Color))
            .Select(p => p.Name)
            .ToArray();
    }

    private void PopulateColorDropdowns()
    {
        // Clear any existing items
        StagedColorComboBox.Items.Clear();
        UnStagedColorComboBox.Items.Clear();

        // Add available colors to both ComboBoxes using the helper method
        foreach (var color in AvailableColors)
        {
            AddColorToComboBox(StagedColorComboBox, color);
            AddColorToComboBox(UnStagedColorComboBox, color);
        }

        // Set default values for both ComboBoxes
        SetDefaultColor(StagedColorComboBox, SettingsModel.Data.StagedFileColor);
        SetDefaultColor(UnStagedColorComboBox, SettingsModel.Data.UnStagedFileColor);
        void SetDefaultColor(ComboBox comboBox, Color defaultColor)
        {
            var defaultColorName = ColorToName(defaultColor);

            var defaultItem = comboBox.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Tag.ToString() == defaultColorName);

            if (defaultItem != null)
            {
                comboBox.SelectedItem = defaultItem;
            }
            return;

            string ColorToName(Color color)
            {
                var colorProperty = typeof(Colors).GetProperties(BindingFlags.Static | BindingFlags.Public)
                    .FirstOrDefault(p => ((Color)p.GetValue(null)).Equals(color));

                return colorProperty?.Name ?? color.ToString();
            }
        }
        void AddColorToComboBox(ComboBox comboBox, string color)
        {
            var colorPanel = new StackPanel { Orientation = Orientation.Horizontal };

            var colorRectangle = new System.Windows.Shapes.Rectangle
            {
                Width = 16,
                Height = 16,
                Fill = new SolidColorBrush((Color)ColorConverter.ConvertFromString(color)),
                Margin = new Thickness(0, 0, 5, 0) // Add some space between the rectangle and the text
            };

            var colorName = new TextBlock { Text = color };

            colorPanel.Children.Add(colorRectangle);
            colorPanel.Children.Add(colorName);

            comboBox.Items.Add(new ComboBoxItem { Content = colorPanel, Tag = color });
        }
    }
    private async void Back_OnClick(object sender, RoutedEventArgs e)
    {
        await AutoCommitMessage.Settings.HideAsync();
    }
    private async void Save_OnClick(object sender, RoutedEventArgs e)
    {
        // Parse the selected colors
        Color stagedFileColor = (Color)ColorConverter.ConvertFromString((string)((ComboBoxItem)StagedColorComboBox.SelectedItem).Tag);
        Color unStagedFileColor = (Color)ColorConverter.ConvertFromString((string)((ComboBoxItem)UnStagedColorComboBox.SelectedItem).Tag);

        // Create model and save
        var model = new SettingsModel.Model
        {
            StagedFileColor = stagedFileColor,
            UnStagedFileColor = unStagedFileColor,
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

}

