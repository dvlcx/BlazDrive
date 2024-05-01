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

        public Task DeleteByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Drive.File>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public Task<Drive.File> GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public void Update(Drive.File entity)
        {
            throw new NotImplementedException();
        }
    }
}