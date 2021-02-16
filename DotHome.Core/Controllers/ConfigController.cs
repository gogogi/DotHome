using DotHome.Core.Services;
using DotHome.ProgrammingModel.Tools;
using DotHome.RunningModel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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

        public ConfigController(IConfiguration configuration, IProgramCore programCore, PageReloader pageReloader)
        {
            this.configuration = configuration;
            this.programCore = programCore;
            this.pageReloader = pageReloader;
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
    }
}