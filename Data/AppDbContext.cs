using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazDrive.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazDrive.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Folder> Folders { get; set; }
        public DbSet<User> Users { get; set; }
        // public DbSet<Models.Entities.File> Files { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
    }
}