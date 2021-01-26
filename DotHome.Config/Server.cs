using DotHome.Config.Tools;
using DotHome.Definitions.Tools;
using DotHome.ProgrammingModel;
using DotHome.ProgrammingModel.Tools;
using System;
using System.Collections.Generic;
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
            string text = await httpClient.GetStringAsync($"https://{Host}/config/download");
            return ModelSerializer.DeserializeProject(text, DefinitionsCreator.CreateDefinitions(AppConfig.Configuration["AssembliesPath"]));
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
    }
}
