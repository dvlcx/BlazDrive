using BlazDrive.Data;
using BlazDrive.Data.Repositories;
using BlazDrive.Models.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Extensions.Caching.Memory;
using SimpleCrypto;

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

            await _repoFolder.AddAsync(new Folder(guidFolder, name + "_root", null, DateTime.Now, guidFolder.ToString()));
            Directory.CreateDirectory("Storage/" + guidFolder.ToString());
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
            Convert.ToBase64String(account.Avatar));

            PBKDF2 crypt = new()
            {
                PlainText = account.Password,
                Salt = account.Id.ToString(),
            };
            Guid cryptKey = Guid.NewGuid();
            var key = new string(crypt.Compute().Take(43).ToArray()) + "=";
            var iv = new string(crypt.Compute().Skip(30).Take(22).ToArray()) + "==";
            var c = $"<?xml version=\"1.0\" encoding=\"utf-16\"?>\n<AesKeyValue xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\">\n  <key>{key}</key>\n  <iv>{iv}</iv>\n</AesKeyValue>";

            _cache.Set(cryptKey, c);

            var claims = new List<Claim>
            {
                new Claim("Id", account.Id.ToString()),
                new Claim(ClaimTypes.Name, account.Name),
                new Claim(ClaimTypes.Email, account.Email),
                new Claim("AvatarKey", avatarKey.ToString()),
                new Claim("RootFolderId", account.RootFolderId.ToString()),
                new Claim("EncryptionKey", cryptKey.ToString())
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