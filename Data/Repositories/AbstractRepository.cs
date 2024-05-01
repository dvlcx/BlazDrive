using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;

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