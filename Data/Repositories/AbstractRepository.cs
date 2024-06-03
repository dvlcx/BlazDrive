using Microsoft.EntityFrameworkCore;

namespace BlazDrive.Data.Repositories
{
    public abstract class AbstractRepository
    {
        protected readonly IDbContextFactory<AppDbContext> _contextFactory;
        protected AbstractRepository(IDbContextFactory<AppDbContext> contextFactory)
        {
            this._contextFactory = contextFactory;
        }
    }
}