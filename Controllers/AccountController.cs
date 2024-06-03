using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace BlazDrive.Controllers
{
    [ApiController]
    [Route("")]
    public class AccountController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        public AccountController(IMemoryCache cache)
        {
            this._cache = cache;
        }

        [Route("/login/{key}")]
        public async Task<IActionResult> LogIn(Guid key)
        {
            var pr = _cache.Get<ClaimsPrincipal>(key);
            await HttpContext.SignInAsync(pr);
            return Redirect("/");
        } 
        
        [Route("/re-login/{key}")]
        public async Task<IActionResult> SettingsReLogIn(Guid key)
        {
            var pr = _cache.Get<ClaimsPrincipal>(key);
            await HttpContext.SignInAsync(pr);
            return Redirect("/settings");
        }
        
        [Route("/logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync();
            return Redirect("/");
        }
    }
}