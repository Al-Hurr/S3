using Microsoft.EntityFrameworkCore;
using S3.Models;

namespace S3.ApplicationDbContext
{
    public class AppDbContext : DbContext
    {
        public DbSet<S3Object> Orders { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
    }
}