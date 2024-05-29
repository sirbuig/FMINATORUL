using FMInatorul.Data;
using FMInatorul.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

public static class SeedData
{
    public static void Initialize(IServiceProvider
        serviceProvider)
    {
        using (var context = new ApplicationDbContext(
                   serviceProvider.GetRequiredService
                       <DbContextOptions<ApplicationDbContext>>()))
        {
            if (context.Roles.Any()) return; // baza de date contine deja roluri
            // CREAREA ROLURILOR IN BD
            // daca nu contine roluri, acestea se vor crea
            context.Roles.AddRange(
                new IdentityRole
                { Id = "2c5e174e-3b0e-446f-86af483d56fd7210", Name = "Admin", NormalizedName = "Admin".ToUpper() },
                new IdentityRole
                {
                    Id = "2c5e174e-3b0e-446f-86af483d56fd7211",
                    Name = "Profesor",
                    NormalizedName = "Profesor".ToUpper()
                },
                new IdentityRole
                {
                    Id = "2c5e174e-3b0e-446f-86af483d56fd7212",
                    Name = "Student",
                    NormalizedName = "Student".ToUpper()
                }
            );

            var hasher = new PasswordHasher<ApplicationUser>();
            // CREAREA USERILOR IN BD
            // Se creeaza cate un user pentru fiecare rol
            context.Users.AddRange(
                new ApplicationUser
                {
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb0",
                    // primary key
                    UserName = "admin@test.com",
                    FirstName = "Admin",
                    LastName = "Admin",
                    EmailConfirmed = true,
                    NormalizedEmail = "ADMIN@TEST.COM",
                    Email = "admin@test.com",
                    NormalizedUserName = "ADMIN@TEST.COM",
                    PasswordHash = hasher.HashPassword(null,
                        "Admin1!")
                },
                new ApplicationUser
                {
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb1",
                    // primary key
                    UserName = "profesor@unibuc.ro",
                    FirstName = "Profesor",
                    LastName = "Profesor",
                    EmailConfirmed = true,
                    NormalizedEmail = "PROFESOR@TEST.COM",
                    Email = "profesor@test.com",
                    NormalizedUserName = "PROFESOR@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "Profesor1!")
                },
                new ApplicationUser
                {
                    Id = "8e445865-a24d-4543-a6c6-9443d048cdb2",
                    // primary key
                    UserName = "student@s.unibuc.ro",
                    FirstName = "Student",
                    LastName = "Student",
                    EmailConfirmed = true,
                    NormalizedEmail = "STUDENT@TEST.COM",
                    Email = "student@test.com",
                    NormalizedUserName = "STUDENT@TEST.COM",
                    PasswordHash = hasher.HashPassword(null, "Student1!")
                }
            );
            // ASOCIEREA USER-ROLE
            context.UserRoles.AddRange(
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af483d56fd7210",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb0"
                },
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af483d56fd7211",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb1"
                },
                new IdentityUserRole<string>
                {
                    RoleId = "2c5e174e-3b0e-446f-86af483d56fd7212",
                    UserId = "8e445865-a24d-4543-a6c6-9443d048cdb2"
                }
            );
            context.SaveChanges();
        }
    }
}