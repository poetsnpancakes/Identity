using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Identity_Infrastructure.Entity;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using Identity_Infrastructure.Context;
using Microsoft.Identity.Client;

namespace Identity_Infrastructure.Configurations
{
   

    public class PermissionConfiguration : IEntityTypeConfiguration<PermissionAction>
    {
        public ApplicationDbContext Context { get; set; }
        public void Configure(EntityTypeBuilder<PermissionAction> builder)
        {
            
            // Configure the Permission table
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
            //  builder.Property(p => p.Description).HasMaxLength(250);

           
              /*  IEnumerable<Permission> permissions =
                  Enum.GetValues<Authentication.PermissionEnum>()
                  .Select(p => new Permission
                  {
                      Id = (int)p,
                      Name = p.ToString()
                  });

                builder.HasData(permissions);*/
            
        }
    }

}
