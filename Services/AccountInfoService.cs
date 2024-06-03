using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Caching.Memory;

namespace BlazDrive.Services
{
    public class AccountInfoService
    {
        private AuthenticationStateProvider _authStateProvider;
        private IMemoryCache _cache;

        public AccountInfoService(AuthenticationStateProvider authenticationStateProvider, IMemoryCache memoryCache)
        {
            _authStateProvider = authenticationStateProvider;
            _cache = memoryCache;
        }

        public async Task<Guid> GetGuid()
        {
            return Guid.Parse((await _authStateProvider.GetAuthenticationStateAsync()).User.FindFirstValue("Id"));
        }

        public async Task<string> GetName()
        {
            return (await _authStateProvider.GetAuthenticationStateAsync()).User.FindFirstValue(ClaimTypes.Name);
        }

        public async Task<string>  GetEmail()
        {
            return (await _authStateProvider.GetAuthenticationStateAsync()).User.FindFirstValue(ClaimTypes.Email);
        }

        public async Task<string> GetAvatar()
        {
            var avatarKey = (await _authStateProvider.GetAuthenticationStateAsync()).User.FindFirstValue("AvatarKey");
            return _cache.Get(Guid.Parse(avatarKey))?.ToString();
        }

        public async Task<string> GetAvatarKey()
        {
            return (await _authStateProvider.GetAuthenticationStateAsync()).User.FindFirstValue("AvatarKey");
        }


        public async Task<string> GetRootFolderId()
        {
            return (await _authStateProvider.GetAuthenticationStateAsync()).User.FindFirstValue("RootFolderId");
        }

        public async Task<string> GetEncryptionKey()
        {
            var key =  (await _authStateProvider.GetAuthenticationStateAsync()).User.FindFirstValue("EncryptionKey");
            return _cache.Get(Guid.Parse(key))?.ToString();
        }
    }
}