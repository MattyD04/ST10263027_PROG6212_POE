using Microsoft.EntityFrameworkCore;
using ST10263027_PROG6212_POE.Models;

namespace ST10263027_PROG6212_POE.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }
        public AppDbContext() { }
        public DbSet<Lecturer> Lecturers { get; set; }

    }
}
