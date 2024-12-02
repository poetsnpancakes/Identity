using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using AutoMapper;

namespace Infrastructure.Context.SeedData
{
    public class SeedUserRole
    {
        private readonly IMapper _mapper;
        public SeedUserRole(IMapper mapper)
        {
            _mapper = mapper;
        }


        public async Task SeedRoles(ApplicationDbContext context)
        {
          
            //var roles = new Dictionary<string, string>() { { "1", "Admin" }, { "2", "User" } };

            if (!context.Roles.Any())
            {

                // Deserialize JSON to object
                string jsonString = File.ReadAllText("C:\\Users\\Ayush\\source\\repos\\Infrastructure\\Context\\SeedData\\UserRole.json");
                // var roles = JsonSerializer.Deserialize<Dictionary<string, string>>(jsonString);
                var roles = JsonSerializer.Deserialize<List<UserRole>>(jsonString);
                List<IdentityRole> user_roles = _mapper.Map<List<IdentityRole>>(roles);
                foreach (IdentityRole role in user_roles)
                {
                   /* var user_roles = new IdentityRole()
                    {
                        Name = role.Name,
                        ConcurrencyStamp = role.ConcurrencyStamp,
                        NormalizedName = role.NormalizedName
                    };*/
                  
                   await context.Roles.AddAsync(role);
                 
                }

                //context.ChangeTracker.DetectChanges();
              
                //context.SaveChanges();
            }
            if(context.ChangeTracker.HasChanges()==true)
            {
                context.SaveChanges();
            }



            /* if (!await _role.Roles.AnyAsync())
             {
                 foreach (KeyValuePair<string, string> role in roles)
                 {
                     // if (!await _role.RoleExistsAsync(role))
                     // (++a).ToString()
                     await _role.CreateAsync(new IdentityRole() { Name = role.Value, ConcurrencyStamp = role.Key });

                 }
             }*/

        }
    }
}

/*
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Entity
{
    public class UserRole
    {
        private readonly RoleManager<IdentityRole> _role;
        public UserRole(RoleManager<IdentityRole> role)
        {
            _role = role;
        }

        public async Task SeedRoles()
        {
            //  string[] roles = { "Admin", "User" };
            var roles = new Dictionary<string, string>() { { "1", "Admin" }, { "2", "User" } };

            if (!await _role.Roles.AnyAsync())
            {
                foreach (KeyValuePair<string, string> role in roles)
                {
                    // if (!await _role.RoleExistsAsync(role))
                    // (++a).ToString()
                    await _role.CreateAsync(new IdentityRole() { Name = role.Value, ConcurrencyStamp = role.Key });

                }
            }
        }

        /* public void SeedRoles(ModelBuilder builder)
         {
             builder.Entity<IdentityRole>().HasData
                 (
                 new IdentityRole() { Name = "Admin", ConcurrencyStamp = "1", NormalizedName = "Admin" },
                 new IdentityRole() { Name = "User", ConcurrencyStamp = "2", NormalizedName = "User" },
                 new IdentityRole() { Name = "HR", ConcurrencyStamp = "3", NormalizedName = "HR" });
         }


    }
}*/