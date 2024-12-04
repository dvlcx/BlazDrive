using BlazDrive.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BlazDrive.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Folder> Folders { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Models.Entities.File> Files { get; set; } = null!;
        public DbSet<DownloadLink> DownloadLinks { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
    }
}