using DotHome.Core.Services;
using DotHome.Definitions;
using DotHome.ProgrammingModel.Tools;
using DotHome.RunningModel;
using DotHome.RunningModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace DotHome.Core.Controllers
{
    [ApiController]
    [Route("config")]
    [Authorize(Roles = "Administrator")]
    public class ConfigController : ControllerBase
    {
        private IConfiguration configuration;
        private IProgramCore programCore;
        private PageReloader pageReloader;
        private BlocksActivator blocksActivator;
        private ModelLoader modelLoader;

        public ConfigController(IConfiguration configuration, IProgramCore programCore, PageReloader pageReloader, BlocksActivator blocksActivator, ModelLoader modelLoader)
        {
            this.configuration = configuration;
            this.programCore = programCore;
            this.pageReloader = pageReloader;
            this.blocksActivator = blocksActivator;
            this.modelLoader = modelLoader;
        }

        [AllowAnonymous, HttpPost("login")]
        public async Task Login(Dictionary<string, string> dictionary)
        {
            if (dictionary.TryGetValue("Username", out string username)
                && dictionary.TryGetValue("Password", out string password)
                && username == configuration["AdminUsername"]
                && password == configuration["AdminPassword"])
            {
                await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, username),
                        new Claim(ClaimTypes.Role, "Administrator")
                    }, "Password")));
            }
            else
            {
                Response.StatusCode = 401;
            }
        }

        [HttpPost("logout")]
        public async Task Logout()
        {
            await HttpContext.SignOutAsync();
        }

        [HttpPost("changecredentials")]
        public async Task ChangeCredentials(Dictionary<string, string> dictionary)
        {
            if (dictionary.TryGetValue("OldUsername", out string oldUsername)
                && dictionary.TryGetValue("OldPassword", out string oldPassword)
                && dictionary.TryGetValue("NewUsername", out string newUsername)
                && dictionary.TryGetValue("NewPassword", out string newPassword)
                && oldUsername == configuration["AdminUsername"]
                && oldPassword == configuration["AdminPassword"])
            {
                configuration["AdminUsername"] = newUsername;
                configuration["AdminPassword"] = newPassword;
                await HttpContext.SignOutAsync();
                await HttpContext.SignInAsync(new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, newUsername),
                        new Claim(ClaimTypes.Role, "Administrator")
                    }, "Password")));
            }
            else
            {
                Response.StatusCode = 401;
            }
        }

        [HttpGet("download")]
        public IActionResult DownloadProject()
        {
            if (System.IO.File.Exists(configuration["ProjectPath"]))
                return File(System.IO.File.OpenRead(configuration["ProjectPath"]), "application/json", "project.json");
            else
                return NotFound();
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadProject()
        {
            using (FileStream fs = System.IO.File.Create(configuration["ProjectPath"]))
            {
                await Request.Body.CopyToAsync(fs);
            }
            programCore.Restart();
            pageReloader.ForceReload();
            return Ok();
        }

        [HttpGet("dlldownload"), AllowAnonymous]
        public IActionResult DownloadDll()
        {
            ZipFile.CreateFromDirectory(configuration["AssembliesPath"], "dlls.zip");

            MemoryStream ms = new MemoryStream();
            using (FileStream fs = System.IO.File.OpenRead("dlls.zip"))
            {
                fs.CopyTo(ms);
            }
            ms.Seek(0, SeekOrigin.Begin);
            System.IO.File.Delete("dlls.zip");
            return File(ms, "application/x-zip-compressed", "dlls.zip");
        }

        [HttpPost("dllupload")]
        public async Task<IActionResult> UploadDll()
        {
            using (FileStream fs = System.IO.File.Create(Path.Combine(configuration["AssembliesPath"], Request.Headers["filename"])))
            {
                await Request.Body.CopyToAsync(fs);
            }
            return Ok();
        }

        [HttpGet("getusages")]
        public Tuple<double, double> GetUsages()
        {
            return new Tuple<double, double>(programCore.AverageCpuUsage, programCore.MaxCpuUsage);
        }

        [HttpPost("searchdevices")]
        public List<string> SearchForGenericDevices([FromBody] string typename)
        {
            BlockDefinition blockDefinition = modelLoader.LoadDefinitions().GetBlockDefinitionByFullName(typename);
            Type communicationProviderType = blockDefinition.Type.Assembly.GetTypes().SingleOrDefault(t => !t.IsGenericType && !t.IsAbstract && t.IsAssignableTo(typeof(CommunicationProvider<>).MakeGenericType(blockDefinition.Type)));
            if(communicationProviderType != null)
            {
                var communicationProvider = blocksActivator.GetService(communicationProviderType);
                if(communicationProvider != null)
                {
                    return ((CommunicationProvider)communicationProvider).SearchDevices().Select(gd => ModelSerializer.Serialize(GenericDeviceToProgrammingBlock(gd, blockDefinition))).ToList();
                }
            }
            return null;
        }

        private ProgrammingModel.Block GenericDeviceToProgrammingBlock(GenericDevice genericDevice, BlockDefinition blockDefinition)
        {
            ProgrammingModel.Block block = new ProgrammingModel.Block(blockDefinition);
            foreach(ProgrammingModel.Parameter parameter in block.Parameters)
            {
                parameter.Value = parameter.Definition.PropertyInfo.GetValue(genericDevice);
            }
            foreach (DeviceValue rValue in genericDevice.RValues)
            {
                var outputDefinition = new OutputDefinition() { Name = rValue.Name, Type = rValue.Type };
                block.Outputs.Add(new ProgrammingModel.Output(outputDefinition));
            }
            foreach (DeviceValue wValue in genericDevice.WValues)
            {
                var inputDefinition = new InputDefinition() { Name = wValue.Name, Type = wValue.Type };
                block.Inputs.Add(new ProgrammingModel.Input(inputDefinition));
            }
            return block;
        }

    }
}