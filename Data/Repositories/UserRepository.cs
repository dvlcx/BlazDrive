using BlazDrive.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazDrive.Data.Repositories
{
    public class UserRepository : AbstractRepository, IRepository<User>
    {
        public UserRepository(IDbContextFactory<AppDbContext> contextFactory) : base(contextFactory)
        {
     
        }

        public async Task AddAsync(User entity)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                await context.Users.AddAsync(entity);
                await context.SaveChangesAsync();
            }
        }

        public void Delete(User entity)
        {
            throw new NotImplementedException();
        }

        public Task DeleteByIdAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            using (var context  = _contextFactory.CreateDbContext())
            {
                return await context.Users.FirstOrDefaultAsync(x => x.Id == id);
            }
        }
        public async Task<User?> GetByEmail(string email)
        {
            using (var context  = _contextFactory.CreateDbContext())
            {
                return await context.Users.FirstOrDefaultAsync(x => x.Email == email);
            }
        }

        public async Task<bool> EmailExists(string email)
        {
            using (var context  = _contextFactory.CreateDbContext())
            {
                return await context.Users.AnyAsync(x => x.Email == email);
            }
        }

        public async Task UpdateAsync(User entity)
        {
            using (var context = _contextFactory.CreateDbContext())
            {
                context.Update(entity); 
                await context.SaveChangesAsync();
            }
        }
    }
}