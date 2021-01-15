using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DotHome.Definitions;
using DotHome.Definitions.Tools;
using DotHome.Config.Tools;
using DotHome.Config.Views;
using DotHome.ProgrammingModel;
using DotHome.ProgrammingModel.Tools;
using MessageBox.Avalonia;
using MessageBox.Avalonia.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DotHome.Config
{
    public class MainWindow : Window
    {
        private ProjectView ProjectView { get => (ProjectView)projectContentControl.Content; set => projectContentControl.Content = value; }
        private string Path { get; set; }

        private ContentControl projectContentControl;

        private Command NewProjectCommand { get; }
        private Command OpenProjectCommand { get; }
        private Command CloseProjectCommand { get; }
        private Command SaveProjectCommand { get; }
        private Command SaveProjectAsCommand { get; }
        private Command ExitCommand { get; }

        private Command CancelCommand { get; }
        private Command SelectAllCommand { get; }
        private Command CopyCommand { get; }
        private Command CutCommand { get; }
        private Command PasteCommand { get; }
        private Command DeleteCommand { get; }

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif
            projectContentControl = this.FindControl<ContentControl>("projectContentControl");

            NewProjectCommand = new Command(() => ProjectView == null, () => { ProjectView = ConfigTools.NewProjectView(); Path = null; });
            OpenProjectCommand = new Command(() => ProjectView == null, OpenProject_Executed);
            CloseProjectCommand = new Command(() => ProjectView != null, CloseProject_Executed);
            SaveProjectCommand = new Command(() => ProjectView != null, SaveProject_Executed);
            SaveProjectAsCommand = new Command(() => ProjectView != null, SaveProjectAs_Executed);
            ExitCommand = new Command(() => true, () => Close());

            CancelCommand = new Command(() => ProjectView?.SelectedPage != null, () => ProjectView.SelectedPage.Cancel());
            SelectAllCommand = new Command(() => ProjectView?.SelectedPage != null, () => ProjectView.SelectedPage.SelectAll());
            CopyCommand = new Command(() => ProjectView?.SelectedPage != null && ProjectView.SelectedPage.Blocks.Any(b => b.Selected), () => ProjectView.SelectedPage.Copy());
            CutCommand = new Command(() => ProjectView?.SelectedPage != null && ProjectView.SelectedPage.Blocks.Any(b => b.Selected), () => ProjectView.SelectedPage.Cut());
            PasteCommand = new Command(() => ProjectView?.SelectedPage != null, () => ProjectView.SelectedPage.Paste());
            DeleteCommand = new Command(() => ProjectView?.SelectedPage != null && ProjectView.SelectedPage.Blocks.Any(b => b.Selected), () => ProjectView.SelectedPage.Delete());

            DataContext = this;
        }

        private async Task SaveProjectAs_Executed()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog() { Filters = { new FileDialogFilter() { Name = "JSON", Extensions = { "json" } } }, DefaultExtension = "json" };
            string path = await saveFileDialog.ShowAsync(this);
            if (path != null)
            {
                Path = path;
                File.WriteAllText(Path, ModelSerializer.SerializeProject(ModelConverter.ProjectViewToProject(ProjectView)));
            }
        }

        private async Task SaveProject_Executed()
        {
            if (Path == null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog() { Filters = { new FileDialogFilter() { Name = "JSON", Extensions = { "json" } } }, DefaultExtension = "json" };
                string path = await saveFileDialog.ShowAsync(this);
                if (path != null)
                {
                    Path = path;
                    File.WriteAllText(Path, ModelSerializer.SerializeProject(ModelConverter.ProjectViewToProject(ProjectView)));
                }
            }
            else
            {
                File.WriteAllText(Path, ModelSerializer.SerializeProject(ModelConverter.ProjectViewToProject(ProjectView)));
            }
        }

        private async Task CloseProject_Executed()
        {
            var result = await MessageBoxManager.GetMessageBoxStandardWindow("Close Project", "Do you want to save project?", ButtonEnum.YesNoCancel, MessageBox.Avalonia.Enums.Icon.Warning).ShowDialog(this);
            if (result == ButtonResult.No)
            {
                ProjectView = null;
            }
            else if (result == ButtonResult.Yes)
            {
                if (Path == null)
                {
                    SaveFileDialog saveFileDialog = new SaveFileDialog() { Filters = { new FileDialogFilter() { Name = "JSON", Extensions = { "json" } } } };
                    string path = await saveFileDialog.ShowAsync(this);
                    if (path != null)
                    {
                        Path = path;
                        File.WriteAllText(Path, ModelSerializer.SerializeProject(ModelConverter.ProjectViewToProject(ProjectView)));
                        ProjectView = null;
                    }
                }
                else
                {
                    File.WriteAllText(Path, ModelSerializer.SerializeProject(ModelConverter.ProjectViewToProject(ProjectView)));
                    ProjectView = null;
                }
            }
        }


        private async Task OpenProject_Executed()
        {
            OpenFileDialog saveFileDialog = new OpenFileDialog() { Filters = { new FileDialogFilter() { Name = "JSON", Extensions = { "json" } } } };
            string[] paths = await saveFileDialog.ShowAsync(this);
            if(paths.Length == 1 && paths[0] != null)
            {
                Path = paths[0];
                ProjectView = ModelConverter.ProjectToProjectView(ModelSerializer.DeserializeProject(File.ReadAllText(Path)));
            }
        }

        private async void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(ProjectView != null)
            {
                e.Cancel = true;
                var result = await MessageBoxManager.GetMessageBoxStandardWindow("Exit", "Do you want to save project?", ButtonEnum.YesNoCancel, MessageBox.Avalonia.Enums.Icon.Warning).ShowDialog(this);
                if(result == ButtonResult.No)
                {
                    ProjectView = null;
                    Close();
                }
                else if(result == ButtonResult.Yes)
                {
                    if(Path == null)
                    {
                        OpenFileDialog saveFileDialog = new OpenFileDialog() { Filters = { new FileDialogFilter() { Name = "JSON", Extensions = { "json" } } }, AllowMultiple = false };
                        string[] paths = await saveFileDialog.ShowAsync(this);
                        if (paths.Length == 1 && paths[0] != null)
                        {
                            Path = paths[0];
                            ProjectView = ModelConverter.ProjectToProjectView(ModelSerializer.DeserializeProject(File.ReadAllText(Path)));
                            ProjectView = null;
                            Close();
                        }
                    }
                    else
                    {
                        File.WriteAllText(Path, ModelSerializer.SerializeProject(ModelConverter.ProjectViewToProject(ProjectView)));
                        ProjectView = null;
                        Close();
                    }
                }
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
