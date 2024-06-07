using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using BlazDrive.Models.OutputModels;
using BlazDrive.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace BlazDrive.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class DownloadController : Controller
    {
        private IMemoryCache _cache;
        private AccountInfoService _accountInfoService;

        public DownloadController(IMemoryCache cache, AccountInfoService accountInfoService)
        {
            _cache = cache;
            _accountInfoService = accountInfoService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }

        [HttpGet("file/{key}")]
        public async Task<IActionResult> GetFileOnce(Guid key)
        {
            var fm = _cache.Get<OutputFile>(key);
            // if (fm is null) return StatusCode(500);
            // _cache.Remove(key);
            if (fm.UserId != Guid.Parse(HttpContext.User.FindFirstValue("Id"))) return BadRequest();

            // return File(fm.File, "application/octet-stream", fm.FileName);
            var content = "<!DOCTYPE html>" +
            "<html><head></head><body>" +                          
            $"<a style=\"display:none\" id=\"linker\" href=\"data:application/octet-stream;charset=utf-8;base64,{Convert.ToBase64String(fm.File)}\" download=\"{fm.FileName}\"></a>" +
            "</body>" +
            "<script>document.addEventListener('DOMContentLoaded', function(){document.getElementById('linker').click(); window.location='/blazdrive'}, false);</script>" +
            "</html>";
            return Content(content, "text/html");
        }
    }
}