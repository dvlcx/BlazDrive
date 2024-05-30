using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Drive = BlazDrive.Models.Entities; 

namespace BlazDrive.Data.Repositories
{
    public class FileRepository : AbstractRepository, IRepository<Drive.File>
    {
        public FileRepository(IDbContextFactory<AppDbContext> contextFactory) : base(contextFactory)
        {

        }

        public async Task AddAsync(Drive.File entity)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                // await context.Files.AddAsync(entity);
            }
        }

        public void Delete(Drive.File entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Drive.File>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Drive.File>> GetByFolderId(Guid parentId)
        {
            using (var context  = _contextFactory.CreateDbContext())
            {
                return await context.Files.Where(x => x.ParentFolderId == parentId).ToListAsync();
            }
        }

        public Task<Drive.File> GetByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task UpdateAsync(Drive.File entity)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                context.Update(entity);
                await context.SaveChangesAsync();
            }
        }
    }
}