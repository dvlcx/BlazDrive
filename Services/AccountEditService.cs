using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using BCrypt.Net;
using BlazDrive.Data;
using BlazDrive.Data.Repositories;
using BlazDrive.Models.Entities;
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
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var cacheKey = Guid.NewGuid();
            _cache.Set(cacheKey, principal);
            return cacheKey;
        }

        public async Task EditAccountAsync(string id, string name, string email, string password)
        {
            var user = await _repoUser.GetByIdAsync(Guid.Parse(id));
            if(user is null) return;
            password = BCrypt.Net.BCrypt.HashPassword(password + id);
            if(name is not null || name != user.Name)
            {

            }
            if (email is not null || email != user.Email)
            {

            }
            if (password is not null || BCrypt.Net.BCrypt.Verify(password, user.Password))
            {

            }
        }
    }
}