using FMInatorul.Models;

namespace FMInatorul.Data;
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationProf, string>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Student> Students { get; set; }
    public DbSet<Profesor> Profesors { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<ApplicationUser>(entity => { entity.ToTable("ApplicationUsers"); });
        builder.Entity<Student>(entity => { entity.ToTable("Students"); });
        builder.Entity<Profesor>(entity => { entity.ToTable("Profesors"); });
    }
}