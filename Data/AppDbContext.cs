using Microsoft.EntityFrameworkCore;
using EFCore.Models;
namespace ST10263027_PROG6212_POE.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }

        public DbSet<Lecturer> Lecturers { get; set; }

    }
}
