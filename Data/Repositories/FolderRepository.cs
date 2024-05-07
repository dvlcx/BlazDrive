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

        public Task DeleteByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Folder>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Folder> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Folder entity)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                context.Update(entity);
                await context.SaveChangesAsync();
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