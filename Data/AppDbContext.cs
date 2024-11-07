using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ST10263027_PROG6212_POE.Models;
using System.Security.Claims;
//this file contains the DBSets for the Lecturer database which has tables for lecturers, coordinators, managers and the claim
namespace ST10263027_PROG6212_POE.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser, IdentityRole, string>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Lecturer> Lecturers { get; set; } // This represents the database table for Lecturer entities which links to the Lecturer
        public DbSet<ProgrammeCoordinator> ProgrammeCoordinators { get; set; } // This represents the database table for Programme Coordinators entities which links to ProgrammeCoordinators model
        public DbSet<AcademicManager> AcademicManagers { get; set; } // This represents the database table for Academic Managers entities which links to AcademicManager model
        public DbSet<ST10263027_PROG6212_POE.Models.Claim> Claims { get; set; } // This represents the database table for Claims entities which links to Claims model

    }
}
//**************************************************end of file***********************************************//
