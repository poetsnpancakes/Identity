using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Identity_Infrastructure.Entity;
using Identity_Infrastructure.Configurations;



namespace Identity_Infrastructure.Context
{

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
        public DbSet<PermissionAction> PermissionActions { get; set; }
        public DbSet<RolePermission> RolePermissions { get; set; }
        public DbSet<PermissionResourceCategory> PermissionResourceCategories { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Apply configurations
            builder.ApplyConfiguration(new PermissionConfiguration());
            builder.ApplyConfiguration(new RolePermissionConfiguration());
            // builder.ApplyConfiguration(new RoleConfiguration());

           
        }
        /*  protected override void OnModelCreating(ModelBuilder builder)
          {
              base.OnModelCreating(builder);
              //this.SeedUsers(builder);

              UserRole role = new UserRole();
              role.SeedRoles(builder);
          }*/

        /* private void SeedRoles(ModelBuilder builder)
         {
             builder.Entity<IdentityRole>().HasData
                 (
                 new IdentityRole() { Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
                 new IdentityRole() { Name = "User", ConcurrencyStamp = "2", NormalizedName = "User" },
                 new IdentityRole() { Name = "HR", ConcurrencyStamp = "3", NormalizedName = "HR" });



         }*/
    }
}
