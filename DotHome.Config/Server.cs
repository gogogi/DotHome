using DotHome.Config.Tools;
using DotHome.Definitions.Tools;
using DotHome.ProgrammingModel;
using DotHome.ProgrammingModel.Tools;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace DotHome.Config
{
    public class Server
    {
        private HttpClient httpClient = new HttpClient();
        private HubConnection hubConnection;
        private Project project;

        public string Host { get; private set; }

        public static async Task<Server> Connect(string host, string username, string password)
        {
            try
            {
                HttpClient httpClient = new HttpClient();
                var res = await httpClient.PostAsJsonAsync($"https://{host}/config/login", new Dictionary<string, string>() { ["Username"] = username, ["Password"] = password });
                if (res.IsSuccessStatusCode)
                {
                    return new Server() { Host = host, httpClient = httpClient };
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

        private Server() { }

        public async Task Disconnect()
        {
            var res = await httpClient.PostAsync($"https://{Host}/config/logout", null);
        }

        public async Task<Project> DownloadProject()
        {
            var res = await httpClient.GetAsync($"https://{Host}/config/download");
            if(res.IsSuccessStatusCode)
            {
                string text = await res.Content.ReadAsStringAsync();
                return ModelSerializer.DeserializeProject(text, DefinitionsCreator.CreateDefinitions(AppConfig.Configuration["AssembliesPath"]));
            }
            else
            {
                return new Project() { Definitions =  DefinitionsCreator.CreateDefinitions(AppConfig.Configuration["AssembliesPath"]) };
            }
        }

        public async Task StopCore()
        {
            var res = await httpClient.PostAsync($"https://{Host}/config/stopcore", null);
        }

        public async Task StartCore()
        {
            var res = await httpClient.PostAsync($"https://{Host}/config/startcore", null);
        }

        public async Task UploadProject(Project project)
        {
            using (var stream = new MemoryStream(Encoding.Default.GetBytes(ModelSerializer.SerializeProject(project))))
            {
                var res = await httpClient.PostAsync($"https://{Host}/config/upload", new StreamContent(stream));
            }
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

        public async Task<bool> ChangeCredentials(string oldUsername, string oldPassword, string newUsername, string newPassword)
        {
            var res = await httpClient.PostAsJsonAsync($"https://{Host}/config/changecredentials", new Dictionary<string, string>() { ["OldUsername"] = oldUsername, ["OldPassword"] = oldPassword, ["NewUsername"] = newUsername, ["NewPassword"] = newPassword });
            return res.IsSuccessStatusCode;
        }

        public async Task StartDebugging(Project project)
        {
            this.project = project;
            hubConnection = new HubConnectionBuilder().WithAutomaticReconnect().WithUrl($"https://{Host}/debug").AddNewtonsoftJsonProtocol().Build();
            hubConnection.On<int, int, object>("Input", DebugInput);
            hubConnection.On<int, int, object>("Output", DebugOutput);
            await hubConnection.StartAsync();
        }

        public async Task StopDebugging()
        {
            await hubConnection.StopAsync();
            await hubConnection.DisposeAsync();
            hubConnection = null;
            foreach(Page p in project.Pages)
            {
                foreach(Block b in p.Blocks)
                {
                    foreach (Input i in b.Inputs) i.DebugValue = null;
                    foreach (Output o in b.Outputs) o.DebugValue = null;
                }
            }
            project = null;
        }

        private void DebugOutput(int blockId, int outputIndex, object value)
        {
            GetBlockById(blockId).Outputs[outputIndex].DebugValue = value;
            Debug.WriteLine("output " + blockId + " " + outputIndex + " " + value);
        }

        private void DebugInput(int blockId, int inputIndex, object value)
        {
            GetBlockById(blockId).Inputs[inputIndex].DebugValue = value;
            Debug.WriteLine("input " + blockId + " " + inputIndex + " " + value);
        }

        private Block GetBlockById(int id)
        {
            foreach(Page page in project.Pages)
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
