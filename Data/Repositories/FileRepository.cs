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
                await context.Files.AddAsync(entity);
                await context.SaveChangesAsync();
            }
        }

        public async void DeleteAsync(Drive.File entity)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                context.Files.Remove(entity);
                await context.SaveChangesAsync();
            }
        }

        public async Task DeleteByIdAsync(Guid id)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                var file = await context.Files.FirstOrDefaultAsync(f => f.Id == id);
                if (file is null)
                    return;
                context.Files.Remove(file);
                await context.SaveChangesAsync();
            }        
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

        public async Task<Drive.File?> GetByIdAsync(Guid id)
        {
            using (var context  = _contextFactory.CreateDbContext())
            {
                return await context.Files.FirstOrDefaultAsync(x => x.Id == id);
            }
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