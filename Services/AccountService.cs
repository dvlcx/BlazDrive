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
namespace BlazDrive.Services
{
    public class AccountService
    {
        private UserRepository _repoUser { get; set; }
        private FolderRepository _repoFolder { get; set; }

        public AccountService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _repoUser = new UserRepository(contextFactory);
            _repoFolder = new FolderRepository(contextFactory);
        }

        public async Task<bool> SignUp(string name, string email, string password, HttpContext httpContext)
        {
            var guidUser = Guid.NewGuid();
            var passwordHashed = BCrypt.Net.BCrypt.HashPassword(password + guidUser.ToString());
            var guidFolder = Guid.NewGuid();

            await _repoFolder.AddAsync(new Folder(guidFolder, name + "_root", null, DateTime.Now));
            await _repoUser.AddAsync(new User(guidUser, name, email, passwordHashed, guidFolder));
            return await LogIn(email, password, httpContext);
        }

        public async Task<bool> LogIn(string email, string password, HttpContext httpContext)
        {
            var account = await _repoUser.GetByEmail(email);
            if (account is null) return false;
            if (!BCrypt.Net.BCrypt.Verify(password + account.Id.ToString(), account.Password)) return false;
            
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, account.Name),
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            
            //await httpContext.SignInAsync(principal);

            return true;
        }
        public async Task<bool> CheckEmail(string email)
        {
            return await _repoUser.EmailExists(email);
        }
    }
}