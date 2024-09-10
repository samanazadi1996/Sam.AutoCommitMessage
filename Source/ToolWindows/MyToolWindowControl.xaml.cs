using AutoCommitMessage.Helper;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace AutoCommitMessage
{
    public partial class MyToolWindowControl : UserControl
    {
        public MyToolWindowControl()
        {
            InitializeComponent();

        }
        private void GenerateMessageButton_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var newList = Cmd.Shell("git", "status")
                    .Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries)
                    .Where(p => p.StartsWith("\t"))
                    .Select(p =>
                    {
                        var trimmed = p.Replace("\t", "").Trim();
                        return trimmed.Contains(":  ") ? trimmed : $"added:   {trimmed}";
                    })
                    .ToList();

                CommitMessage.Text = GetCommitMessage(newList);
                CommitDescription.Text = string.Join(Environment.NewLine, newList);

                UpdateTextMessage("Generate Message");
            }
            catch
            {
                UpdateTextMessage("Error");
            }
            string GetCommitMessage(List<string> newList)
            {
                var fileChanges = newList.Select(p => p.Split(':'))
                    .Select(parts => new
                    {
                        Type = parts[0].Trim(),
                        FileName = Path.GetFileName(parts[1].Trim())
                    });

                var detailedMessage = string.Join(" and ", fileChanges.Select(change => $"'{change.FileName}' was {change.Type.ToLower()}"));

                if (detailedMessage.Length > 150)
                {
                    var summarizedMessage = fileChanges
                        .GroupBy(change => change.Type)
                        .Select(group => $"{group.Count()} {(group.Count() > 1 ? "files" : "file")} {group.Key}");

                    detailedMessage = string.Join(" and ", summarizedMessage);
                }

                return detailedMessage;

            }
        }


        private void Commit_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                var newList = Cmd.Shell("git", "add .");

                UpdateTextMessage("Commit");
            }
            catch
            {
                UpdateTextMessage("Error");
            }
        }

        private void Push_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {

                UpdateTextMessage("Push");
            }
            catch
            {
                UpdateTextMessage("Error");
            }
        }

        private void UpdateTextMessage(string text)
        {
            TextMessage.Text = $"{text} at {DateTime.Now:hh:mm:ss}";
        }

        private void Stage_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                Cmd.Shell("git", "add .");

                UpdateTextMessage("Stage");
            }
            catch
            {
                UpdateTextMessage("Error");
            }
        }
    }
}