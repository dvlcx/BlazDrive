using System.Security.Claims;
using BlazDrive.Data;
using BlazDrive.Data.Repositories;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;

namespace BlazDrive.Services
{
    public class AccountEditService
    {
        private UserRepository _repoUser { get; set; }
        private IMemoryCache _cache { get; set; }
        public AccountEditService(IDbContextFactory<AppDbContext> contextFactory, IMemoryCache cache) 
        {
            this._repoUser = new UserRepository(contextFactory);
            this._cache = cache;
        }

        public async Task<Guid> EditAvatarAsync(string id, byte[] avatar)
        {
            var user = await _repoUser.GetByIdAsync(Guid.Parse(id));
            if (user is null) return Guid.Empty;
            user.Avatar = avatar;
            await _repoUser.UpdateAsync(user);

            var avatarKey = Guid.NewGuid();
            _cache.Set<string>(avatarKey, Convert.ToBase64String(avatar));

            var claims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("AvatarKey", avatarKey.ToString()),
                new Claim("RootFolderId", user.RootFolderId.ToString()),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var cacheKey = Guid.NewGuid();
            _cache.Set(cacheKey, principal);
            return cacheKey;
        }

        public async Task<Guid> EditAccountAsync(string id, string name, string email, string password, string avatarKey)
        {
            var user = await _repoUser.GetByIdAsync(Guid.Parse(id));
            if(user is null) return Guid.Empty;
            password = BCrypt.Net.BCrypt.HashPassword(password + id);
            if(name is not null && name != user.Name)
            {
                user.Name = name;
            }
            if (email is not null && email != user.Email)
            {
                user.Email = email;
            }
            if (password is not null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                user.Password = password;
            }
            await _repoUser.UpdateAsync(user);

            var claims = new List<Claim>
            {
                new Claim("Id", user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("AvatarKey", avatarKey.ToString()),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var cacheKey = Guid.NewGuid();
            _cache.Set(cacheKey, principal);
            return cacheKey;
        }
    }
}
