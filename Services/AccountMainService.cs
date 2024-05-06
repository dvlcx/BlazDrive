using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazDrive.Components.Pages;
using BlazDrive.Data;
using BlazDrive.Data.Repositories;
using BlazDrive.Models.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using BCrypt.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using System.Drawing;
namespace BlazDrive.Services
{
    public class AccountMainService
    {
        private UserRepository _repoUser { get; set; }
        private FolderRepository _repoFolder { get; set; }
        private IMemoryCache _cache { get; set; }
        public AccountMainService(IDbContextFactory<AppDbContext> contextFactory, IMemoryCache cache)
        {
            _repoUser = new UserRepository(contextFactory);
            _repoFolder = new FolderRepository(contextFactory);
            _cache = cache;
        }

        public async Task<Guid?> SignUp(string name, string email, string password)
        {
            var guidUser = Guid.NewGuid();
            var passwordHashed = BCrypt.Net.BCrypt.HashPassword(password + guidUser.ToString());
            var guidFolder = Guid.NewGuid();

            await _repoFolder.AddAsync(new Folder(guidFolder, name + "_root", null, DateTime.Now));
            await _repoUser.AddAsync(new User(guidUser, name, email, passwordHashed, guidFolder));
            return await LogIn(email, password);
        }

        public async Task<Guid?> LogIn(string email, string password)
        {
            var account = await _repoUser.GetByEmail(email);
            if (account is null) return null;
            if (!BCrypt.Net.BCrypt.Verify(password + account.Id.ToString(), account.Password)) return null;
            
            Guid avatarKey = Guid.NewGuid();
            _cache.Set<string>(avatarKey, account.Avatar is null ?
            Convert.ToBase64String(System.IO.File.ReadAllBytes("wwwroot/Images/default_avatar.png")) :
            Convert.ToBase64String(System.IO.File.ReadAllBytes("wwwroot/Images/default_avatar.png")));


            var claims = new List<Claim>
            {

                new Claim("Id", account.Id.ToString()),
                new Claim(ClaimTypes.Name, account.Name),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim("AvatarKey", avatarKey.ToString()),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            
            var cacheKey = Guid.NewGuid();
            _cache.Set(cacheKey, principal);
            return cacheKey;
        }
        public async Task<bool> CheckEmail(string email)
        {
            return await _repoUser.EmailExists(email);
        }
    }
}