using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazDrive.Data;
using BlazDrive.Data.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BlazDrive.Services
{
    public class AccountEditService
    {
        private UserRepository _repoUser { get; set; }

        public AccountEditService(IDbContextFactory<AppDbContext> contextFactory) 
        {
            this._repoUser = new UserRepository(contextFactory);
        }

        public async Task EditAvatar(string avatar)
        {
            
        }

        public async Task EditAccount(string id, string? name, string? email, string? password)
        {
            if(name is not null)
            {

            }
            if (email is not null)
            {

            }
            if (password is not null)
            {

            }
        }
    }
}