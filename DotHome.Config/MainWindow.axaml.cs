using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using DotHome.Config.Tools;
using DotHome.Config.Views;
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
using System.Text;
using DotHome.Config.Windows;
using System.Security.Cryptography;
using DotHome.Model.Tools;

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

        private Command ConnectCommand { get; }
        private Command DisconnectCommand { get; }
        private Command DownloadProjectCommand { get; }
        private Command UploadProjectCommand { get; }
        private Command DownloadDllsCommand { get; }
        private Command ChangeCredentialsCommand { get; }
        private Command StartDebuggingCommand { get; }
        private Command StopDebuggingCommand { get; }
        private Command PauseCommand { get; }
        private Command ContinueCommand { get; }
        private Command StepCommand { get; }

        public MainWindow()
        {
            InitializeComponent();
#if DEBUG
            this.AttachDevTools();
#endif

            NewProjectCommand = new Command(() => Project == null, NewProject_Executed);
            OpenProjectCommand = new Command(() => Project == null, OpenProject_Executed);
            CloseProjectCommand = new Command(() => Project != null, CloseProject_Executed);
            SaveProjectCommand = new Command(() => Project != null, SaveProject_Executed);
            SaveProjectAsCommand = new Command(() => Project != null, SaveProjectAs_Executed);
            ExitCommand = new Command(() => true, () => Close());

            CancelCommand = new Command(() => projectView?.SelectedPageView?.Page?.SelectedBlocks?.Any() ?? false, () => projectView?.SelectedPageView?.Cancel());
            SelectAllCommand = new Command(() => projectView?.SelectedPageView?.Page?.Blocks.Any(b => !b.Selected) ?? false, () => projectView?.SelectedPageView?.SelectAll());
            CopyCommand = new Command(() => projectView?.SelectedPageView?.Page?.SelectedBlocks?.Any() ?? false, () => projectView?.SelectedPageView?.Copy());
            CutCommand = new Command(() => projectView?.SelectedPageView?.Page?.SelectedBlocks?.Any() ?? false, () => projectView?.SelectedPageView?.Cut());
            PasteCommand = new Command(async () => ContainerSerializer.TryDeserializeContainer(await Application.Current.Clipboard.GetTextAsync()) != null, () => projectView?.SelectedPageView?.Paste());
            DeleteCommand = new Command(() => (projectView?.SelectedPageView?.Page?.SelectedBlocks?.Any() ?? false) && projectView.SelectedPageView.IsFocused, () => projectView?.SelectedPageView?.Delete());

            ConnectCommand = new Command(() => Server == null, Connect_Executed);
            DisconnectCommand = new Command(() => Server != null, Disconnect_Executed);
            DownloadProjectCommand = new Command(() => Server != null && Project == null, DownloadProject_Executed);
            UploadProjectCommand = new Command(() => Server != null && Project != null, UploadProject_Executed);
            DownloadDllsCommand = new Command(() => Server != null, DownloadDlls_Executed);
            ChangeCredentialsCommand = new Command(() => Server != null && !Server.IsDebugging, ChangeCredentials_Executed);
            StartDebuggingCommand = new Command(() => Server != null && Server.Project == Project && !Server.IsDebugging, StartDebugging_Executed);
            StopDebuggingCommand = new Command(() => Server != null && Server.IsDebugging, StopDebugging_Executed);
            PauseCommand = new Command(() => Server != null && Server.IsDebugging && !Server.IsPaused, Pause_Executed);
            ContinueCommand = new Command(() => Server != null && Server.IsDebugging && Server.IsPaused, Continue_Executed);
            StepCommand = new Command(() => Server != null && Server.IsDebugging && Server.IsPaused, Step_Executed);

            DataContext = this;
        }

        private async Task Step_Executed()
        {
            await Server.Step();
        }

        private async Task Continue_Executed()
        {
            await Server.Continue();
        }

        private async Task Pause_Executed()
        {
            await Server.Pause();
        }

        private async Task StopDebugging_Executed()
        {
            await Server.StopDebugging();
        }

        private async Task StartDebugging_Executed()
        {
            await Server.StartDebugging();
        }

        private async Task ChangeCredentials_Executed()
        {
            ChangeCredentialsWindow changeCredentialsWindow = new ChangeCredentialsWindow(Server);
            await changeCredentialsWindow.ShowDialog(this);
        }

        private async Task DownloadDlls_Executed()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog() { Title = "Download DLLs", Filters = { new FileDialogFilter() { Name = "ZIP", Extensions = { "zip" } } }, DefaultExtension = "zip" };
            string path = await saveFileDialog.ShowAsync(this);
            if (path != null)
            {
                using (var stream = await Server.DownloadAssemblies())
                using (var file = File.Create(path))
                {
                    await stream.CopyToAsync(file);
                }
            }
        }

        private async Task DownloadProject_Executed()
        {
            Project = await Server.DownloadProject();
            Path = null;
        }

        private async Task UploadProject_Executed()
        {
            ModelTools.SetBlockIds(Project);
            await Server.UploadAssemblies();
            await Server.UploadProject(Project);
        }

        private async Task Disconnect_Executed()
        {
            await Server.Disconnect();
            Server = null;
        }

        private async Task Connect_Executed()
        {
            Server = await new ConnectWindow().ShowDialog<Server>(this);
        }

        private async Task SaveProjectAs_Executed()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog() { Filters = { new FileDialogFilter() { Name = "JSON", Extensions = { "json" } } }, DefaultExtension = "json" };
            string path = await saveFileDialog.ShowAsync(this);
            if (path != null)
            {
                Path = path;
                ModelTools.SetBlockIds(Project);
                //File.WriteAllText(Path, ModelSerializer.SerializeProject(Project));
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
                    ModelTools.SetBlockIds(Project);
                    //File.WriteAllText(Path, ModelSerializer.SerializeProject(Project));
                }
            }
            else
            {
                ModelTools.SetBlockIds(Project);
                //File.WriteAllText(Path, ModelSerializer.SerializeProject(Project));
            }
        }

        private async Task CloseProject_Executed()
        {
            var result = await MessageBoxManager.GetMessageBoxStandardWindow("Close Project", "Do you want to save project?", ButtonEnum.YesNoCancel, MessageBox.Avalonia.Enums.Icon.Warning).ShowDialog(this);
            if (result == ButtonResult.No)
            {
                if(Server != null) await Server.StopDebugging();
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
                        //File.WriteAllText(Path, ModelSerializer.SerializeProject(Project));
                        Project = null;
                        await Server?.StopDebugging();
                    }
                }
                else
                {
                    //File.WriteAllText(Path, ModelSerializer.SerializeProject(Project));
                    Project = null;
                    await Server?.StopDebugging();
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
                //Project = ModelSerializer.DeserializeProject(File.ReadAllText(Path), DefinitionsCreator.CreateDefinitions(AppConfig.Configuration["AssembliesPath"]));
            }
        }

        private void NewProject_Executed()
        {
            Project = ConfigTools.NewProject();
            Path = null;
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
                    Server?.StopDebugging();
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
                            //File.WriteAllText(Path, ModelSerializer.SerializeProject(Project));
                            Project = null;
                            Server?.StopDebugging();
                            Close();
                        }
                    }
                    else
                    {
                        //File.WriteAllText(Path, ModelSerializer.SerializeProject(Project));
                        Project = null;
                        Server?.StopDebugging();
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
            projectView = null;
        }
    }
}
