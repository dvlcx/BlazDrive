using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazDrive.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazDrive.Data.Repositories
{
    public class FolderRepository : AbstractRepository, IRepository<Folder>
    {
        public FolderRepository(IDbContextFactory<AppDbContext> contextFactory) : base(contextFactory)
        {

        }

        public async Task AddAsync(Folder entity)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                await context.Folders.AddAsync(entity);
                await context.SaveChangesAsync();
            }
        }

        public void Delete(Folder entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Folder>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Folder> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Folder entity)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                context.Update(entity);
            }
        }

        public async Task SaveAsync()
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                await context.SaveChangesAsync();
            }
        }
    }
}