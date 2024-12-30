using Identity_Infrastructure.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity_Infrastructure.Configurations
{
    public class RolePermissionConfiguration : IEntityTypeConfiguration<RolePermission>
    {
        public void Configure(EntityTypeBuilder<RolePermission> builder)
        {
            // Configure the RolePermission table (many-to-many relationship)

            // Composite Key
            builder.HasKey(rp => new { rp.RoleId, rp.ResourceId,rp.PermissionId });

            // Role relationship
            builder.HasOne(rp => rp.Role)
                   .WithMany()
                   .HasForeignKey(rp => rp.RoleId)
                   .OnDelete(DeleteBehavior.Cascade);

            //Resource relationship
            builder.HasOne(rp => rp.ResourceCategory)
                   .WithMany()
                   .HasForeignKey(rp => rp.ResourceId)
                   .OnDelete(DeleteBehavior.Cascade);

            // Permission relationship
            builder.HasOne(rp => rp.Permissions)
                   .WithMany()
                   .HasForeignKey(rp => rp.PermissionId)
                   .OnDelete(DeleteBehavior.Cascade);
          

        }
    }
 }
