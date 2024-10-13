using Microsoft.EntityFrameworkCore;
using ST10263027_PROG6212_POE.Models;
using System.Security.Claims;

namespace ST10263027_PROG6212_POE.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options) { }
        public AppDbContext() { }
        public DbSet<Lecturer> Lecturers { get; set; }
        public DbSet<ProgrammeCoordinator> ProgrammeCoordinators { get; set; }
        public DbSet<AcademicManager> AcademicManagers { get; set; }
        public DbSet<ST10263027_PROG6212_POE.Models.Claim> Claims { get; set; }

    }
}
