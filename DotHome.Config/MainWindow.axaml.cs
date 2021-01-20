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
using Avalonia.Data;

namespace DotHome.Config
{
    public partial class MainWindow : Window
    {

        private ProjectView projectView;

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
            //projectContentControl = this.FindControl<ContentControl>("projectContentControl");

            NewProjectCommand = new Command(() => Project == null, () => { Project = ConfigTools.NewProject(); Path = null; });
            OpenProjectCommand = new Command(() => Project == null, OpenProject_Executed);
            CloseProjectCommand = new Command(() => Project != null, CloseProject_Executed);
            SaveProjectCommand = new Command(() => Project != null, SaveProject_Executed);
            SaveProjectAsCommand = new Command(() => Project != null, SaveProjectAs_Executed);
            ExitCommand = new Command(() => true, () => Close());

            CancelCommand = new Command(() => true, () => projectView?.SelectedPageView?.Cancel());
            SelectAllCommand = new Command(() => true, () => projectView?.SelectedPageView?.SelectAll());
            CopyCommand = new Command(() => true, () => projectView?.SelectedPageView?.Copy());
            CutCommand = new Command(() => true, () => projectView?.SelectedPageView?.Cut());
            PasteCommand = new Command(() => true, () => projectView?.SelectedPageView?.Paste(Project.Definitions));
            DeleteCommand = new Command(() => true, () => projectView?.SelectedPageView?.Delete());

            var b = new Binding("SelectedPage") { Source = Project };

            DataContext = this;

            
        }

        private async Task SaveProjectAs_Executed()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog() { Filters = { new FileDialogFilter() { Name = "JSON", Extensions = { "json" } } }, DefaultExtension = "json" };
            string path = await saveFileDialog.ShowAsync(this);
            if (path != null)
            {
                Path = path;
                File.WriteAllText(Path, ModelSerializer.SerializeProject(Project));
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
                    File.WriteAllText(Path, ModelSerializer.SerializeProject(Project));
                }
            }
            else
            {
                File.WriteAllText(Path, ModelSerializer.SerializeProject(Project));
            }
        }

        private async Task CloseProject_Executed()
        {
            var result = await MessageBoxManager.GetMessageBoxStandardWindow("Close Project", "Do you want to save project?", ButtonEnum.YesNoCancel, MessageBox.Avalonia.Enums.Icon.Warning).ShowDialog(this);
            if (result == ButtonResult.No)
            {
                Project = null;
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
                        File.WriteAllText(Path, ModelSerializer.SerializeProject(Project));
                        Project = null;
                    }
                }
                else
                {
                    File.WriteAllText(Path, ModelSerializer.SerializeProject(Project));
                    Project = null;
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
                Project = ModelSerializer.DeserializeProject(File.ReadAllText(Path));
            }
        }

        private async void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if(Project != null)
            {
                e.Cancel = true;
                var result = await MessageBoxManager.GetMessageBoxStandardWindow("Exit", "Do you want to save project?", ButtonEnum.YesNoCancel, MessageBox.Avalonia.Enums.Icon.Warning).ShowDialog(this);
                if(result == ButtonResult.No)
                {
                    Project = null;
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
                            File.WriteAllText(Path, ModelSerializer.SerializeProject(Project));
                            Project = null;
                            Close();
                        }
                    }
                    else
                    {
                        File.WriteAllText(Path, ModelSerializer.SerializeProject(Project));
                        Project = null;
                        Close();
                    }
                }
            }
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        private void Project_AttachedToVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            projectView = (ProjectView)sender;
        }

        private void Project_DetachedFromVisualTree(object sender, VisualTreeAttachmentEventArgs e)
        {
            project = null;
        }
    }
}
