﻿using AutoCommitMessage.Helper;
using AutoCommitMessage.Models;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using AppContext = AutoCommitMessage.Helper.AppContext;

namespace AutoCommitMessage
{
    public partial class MyToolWindowControl : UserControl
    {
        public List<FileData> ChangeListData { get; set; }
        public MyToolWindowControl()
        {
            InitializeComponent();
        }

        private void ReloadChangeListData()
        {
            MyTreeViewItem.Header = AppContext.GetOpenedFolder();

            var gitShell = Cmd.Shell("git", "status -s");

            ChangeListData = FileData.GetListData(gitShell).ToList();

            ReloadTreeView();
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

            foreach (var path in FileNode.GetFileNode(ChangeListData))
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
                        ? new SolidColorBrush(Colors.LimeGreen)
                        : new SolidColorBrush(Colors.OrangeRed);


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
            var message = Cmd.Shell("git", $"commit -m \"{CommitMessage.Text}\" -m \"{CommitDescription.Text}\"");

            ReloadChangeListData();

            VS.MessageBox.Show(message);
        }

        private void Push_OnClick(object sender, RoutedEventArgs e)
        {
            var message = Cmd.Shell("git", "push");

            ReloadChangeListData();

            VS.MessageBox.Show(message);
        }

        private void UpdateTextMessage(string text)
        {
            TextMessage.Text = $"{text} at {DateTime.Now:hh:mm:ss}";
        }

        private void Stage_OnClick(object sender, RoutedEventArgs e)
        {
            Cmd.Shell("git", "add .");

            ReloadChangeListData();
        }

        private void Refresh_OnClick(object sender, RoutedEventArgs e)
        {
            ReloadChangeListData();
        }
    }
}