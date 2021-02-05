using DotHome.Config.Tools;
using DotHome.Definitions.Tools;
using DotHome.ProgrammingModel;
using DotHome.ProgrammingModel.Tools;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DotHome.Config
{
    public class Server : INotifyPropertyChanged
    {
        private CookieContainer cookies;
        private HttpClient httpClient = new HttpClient();
        private HubConnection hubConnection;
        private Timer timer;
        private double averageUsage, maxUsage;

        public event PropertyChangedEventHandler PropertyChanged;

        public Project Project { get; private set; }

        public string Host { get; private set; }

        public bool IsDebugging { get; private set; }

        public double AverageUsage { get => averageUsage; private set { averageUsage = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(AverageUsage))); } }

        public double MaxUsage { get => maxUsage; private set { maxUsage = value; PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MaxUsage))); } }


        public static async Task<Server> Connect(string host, string username, string password)
        {
            try
            {
                CookieContainer cookies = new CookieContainer();
                HttpClientHandler handler = new HttpClientHandler();
                handler.CookieContainer = cookies;
                HttpClient httpClient = new HttpClient(handler);
                var res = await httpClient.PostAsJsonAsync($"https://{host}/config/login", new Dictionary<string, string>() 
                { 
                    ["Username"] = username, 
                    ["Password"] = password 
                });
                if (res.IsSuccessStatusCode)
                {
                    var server = new Server() { Host = host, httpClient = httpClient, cookies = cookies };
                    await server.DownloadProject();
                    return server;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        private Server() 
        {
            timer = new Timer(UsageTimer_Callback, null, 0, 5_000);
        }

        private async void UsageTimer_Callback(object state)
        {
            var result = await httpClient.GetAsync($"https://{Host}/config/getusages");
            if(result.IsSuccessStatusCode)
            {
                var tuple = await result.Content.ReadFromJsonAsync<Tuple<double, double>>();
                AverageUsage = tuple.Item1;
                MaxUsage = tuple.Item2;
            }
        }

        public async Task Disconnect()
        {
            await StopDebugging();
            var res = await httpClient.PostAsync($"https://{Host}/config/logout", null);
            timer.Dispose();
        }

        public async Task UploadProject(Project project)
        {
            using (var stream = new MemoryStream(Encoding.Default.GetBytes(ModelSerializer.SerializeProject(project))))
            {
                var res = await httpClient.PostAsync($"https://{Host}/config/upload", new StreamContent(stream));
            }
            Project = project;
        }

        public async Task UploadAssemblies()
        {   
            foreach (string dll in Directory.GetFiles(AppConfig.Configuration["AssembliesPath"], "*.dll"))
            {
                using(var stream = File.OpenRead(dll))
                {
                    var content = new StreamContent(stream);
                    content.Headers.Add("filename", Path.GetFileName(dll));
                    var res = await httpClient.PostAsync($"https://{Host}/config/dllupload", content);
                }
            }
        }

        public async Task<Stream> DownloadAssemblies()
        {
            var res = await httpClient.GetAsync($"https://{Host}/config/dlldownload");
            return await res.Content.ReadAsStreamAsync();
        }

        public async Task<Project> DownloadProject()
        {
            var res = await httpClient.GetAsync($"https://{Host}/config/download");
            if (res.IsSuccessStatusCode)
            {
                string text = await res.Content.ReadAsStringAsync();
                Project = ModelSerializer.DeserializeProject(text, DefinitionsCreator.CreateDefinitions(AppConfig.Configuration["AssembliesPath"]));
            }
            else
            {
                Project = new Project() { Definitions = DefinitionsCreator.CreateDefinitions(AppConfig.Configuration["AssembliesPath"]) };
            }
            return Project;
        }

        public async Task<bool> ChangeCredentials(string oldUsername, string oldPassword, string newUsername, string newPassword)
        {
            var res = await httpClient.PostAsJsonAsync($"https://{Host}/config/changecredentials", new Dictionary<string, string>() 
            { 
                ["OldUsername"] = oldUsername, 
                ["OldPassword"] = oldPassword, 
                ["NewUsername"] = newUsername, 
                ["NewPassword"] = newPassword 
            });
            return res.IsSuccessStatusCode;
        }

        private void DebugBlock(int blockId, string text)
        {
            var block = GetBlockById(blockId);
            if (block != null) block.DebugString = text;
        }

        private void BlockException(int blockId, Exception exception)
        {
            var block = GetBlockById(blockId);
            if (block != null) block.Exception = exception;
        }

        private void DebugOutput(int blockId, int outputIndex, object value)
        {
            var block = GetBlockById(blockId);
            if (block != null) block.Outputs[outputIndex].DebugValue = value;
        }

        private void DebugInput(int blockId, int inputIndex, object value)
        {
            var block = GetBlockById(blockId);
            if (block != null) block.Inputs[inputIndex].DebugValue = value;
        }

        public async Task StartDebugging()
        {
            hubConnection = new HubConnectionBuilder().WithAutomaticReconnect().WithUrl($"https://{Host}/debug", o => o.Cookies = cookies).AddNewtonsoftJsonProtocol().Build();
            hubConnection.On<int, int, object>("Input", DebugInput);
            hubConnection.On<int, int, object>("Output", DebugOutput);
            hubConnection.On<int, string>("Block", DebugBlock);
            hubConnection.On<int, Exception>("BlockException", BlockException);
            await hubConnection.StartAsync();
            IsDebugging = true;
        }

        public async Task StopDebugging()
        {
            if(IsDebugging)
            {
                await hubConnection.StopAsync();
                await hubConnection.DisposeAsync();
                hubConnection = null;
                foreach (Page p in Project.Pages)
                {
                    foreach (Block b in p.Blocks)
                    {
                        foreach (Input i in b.Inputs) i.DebugValue = null;
                        foreach (Output o in b.Outputs) o.DebugValue = null;
                        b.Exception = null;
                    }
                }
                IsDebugging = false;
            }
        }

        private Block GetBlockById(int id)
        {
            foreach(Page page in Project.Pages)
            {
                foreach(Block block in page.Blocks)
                {
                    if(block.Id == id)
                    {
                        return block;
                    }
                }
            }
            return null;
        }
    }
}
