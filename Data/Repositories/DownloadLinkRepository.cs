using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazDrive.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazDrive.Data.Repositories
{
    public class DownloadLinkRepository : AbstractRepository, IRepository<DownloadLink>
    {
        public DownloadLinkRepository(IDbContextFactory<AppDbContext> contextFactory) : base(contextFactory)
        {
        }

        public async Task AddAsync(DownloadLink entity)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                await context.DownloadLinks.AddAsync(entity);
                await context.SaveChangesAsync();
            }
        }

        public async void DeleteAsync(DownloadLink entity)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                context.DownloadLinks.Remove(entity);
                await context.SaveChangesAsync();
            }        
        }

        public async Task DeleteByIdAsync(Guid id)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                context.DownloadLinks.Remove(await context.DownloadLinks.FirstOrDefaultAsync(f => f.Id == id));
                await context.SaveChangesAsync();
            }     
        }

        public async Task<IEnumerable<DownloadLink>> GetAllAsync()
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                return await context.DownloadLinks.ToListAsync();
            }     
        }

        public async Task<DownloadLink?> GetByIdAsync(Guid id)
        {
            using (var context  = _contextFactory.CreateDbContext())
            {
                return await context.DownloadLinks.FirstOrDefaultAsync(x => x.Id == id);
            }
        }

        public Task UpdateAsync(DownloadLink entity)
        {
            throw new NotImplementedException();
        }
    }
}