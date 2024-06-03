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

        public async void Delete(Folder entity)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                context.Folders.Remove(entity);
                await context.SaveChangesAsync();
            }
        }

        public Task DeleteByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Folder>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<Folder?> GetByIdAsync(Guid id)
        {
            using (var context  = _contextFactory.CreateDbContext())
            {
                return await context.Folders.FirstOrDefaultAsync(x => x.Id == id);
            }
        }

        public async Task<IEnumerable<Folder>> GetByParentId(Guid parentId)
        {
            using (var context  = _contextFactory.CreateDbContext())
            {
                return await context.Folders.Where(x => x.ParentFolderId == parentId).ToListAsync();
            }
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