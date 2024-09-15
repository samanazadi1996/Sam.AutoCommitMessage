using AutoCommitMessage.EventHandlers;
using AutoCommitMessage.Helper;
using AutoCommitMessage.Models;
using Microsoft.VisualStudio.Shell.Interop;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace AutoCommitMessage;

public partial class SettingsControl : UserControl
{
    public SettingsControl()
    {
        InitializeComponent();

    }

    private async void Back_OnClick(object sender, RoutedEventArgs e)
    {
        await AutoCommitMessage.Settings.HideAsync();
    }
}

