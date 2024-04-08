using FMInatorul.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography.X509Certificates;

namespace FMInatorul.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    { }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public DbSet<Profesor> Profesors { get; set; }
    public DbSet<Student> Students { get; set; }
}