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
using System.Reflection;
using Identity_Infrastructure.Entity;
using Identity_Infrastructure.Authentication;

namespace Identity_Infrastructure.Context.SeedData
{
    public class SeedUserData
    {
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;
        public SeedUserData(IMapper mapper, RoleManager<IdentityRole> roleManager)
        {
            _mapper = mapper;
            _roleManager = roleManager;
        }


        public async Task SeedData(ApplicationDbContext context)
        {
            if (!context.PermissionActions.Any())
            {
                IEnumerable<PermissionAction> permissions =
                Enum.GetValues<PermissionActionEnum>()
                .Select(p => new PermissionAction
                {
                    // Id = (int)p,
                    Name = p.ToString(),
                    Value = (int)p,
                });
                foreach (PermissionAction permission in permissions)
                {
                    await context.PermissionActions.AddAsync(permission);
                }
            }
            if (!context.PermissionResourceCategories.Any())
            {
                IEnumerable<PermissionResourceCategory> recources =
                Enum.GetValues<PermissionResourceEnum>()
                .Select(p => new PermissionResourceCategory
                {
                    // Id = (int)p,
                    Name = p.ToString(),
                    Value = (int)p,
                });
                foreach (PermissionResourceCategory resource in recources)
                {
                    await context.PermissionResourceCategories.AddAsync(resource);
                }
            }
            if (context.ChangeTracker.HasChanges() == true)
            {
                context.SaveChanges();
            }





            //var roles = new Dictionary<string, string>() { { "1", "Admin" }, { "2", "User" } };

            if (!context.Roles.Any())
            {
              //  string filePath = Path.Combine(Directory.GetCurrentDirectory(), @"wwwroot\Files", "UserRole.json");
              //  string filePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Context\SeedData", "UserRole.json");

                // Deserialize JSON to object
               string jsonString = File.ReadAllText("C:\\Users\\Ayush\\source\\repos\\Infrastructure\\Context\\SeedData\\UserRole.json");
                //string jsonString = File.ReadAllText(filePath);
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
                if (context.ChangeTracker.HasChanges() == true)
                {
                    context.SaveChanges();
                }

                var resources = await context.PermissionResourceCategories.ToListAsync();

                foreach (IdentityRole role in user_roles)
                {
                    string role_id = _roleManager.GetRoleIdAsync(role).Result;
                    if (role.NormalizedName == "ADMIN")
                    {
                        foreach(PermissionResourceCategory resource in resources)
                        {
                            
                            for (int i = 1; i <= 4; i++) //for all PermissionActions
                            {
                                await context.RolePermissions.AddAsync(new RolePermission { RoleId = role_id, ResourceId =resource.Id,PermissionId = i });
                                /*   await context.RolePermissions.AddAsync(new RolePermission { RoleId = _roleManager.GetRoleIdAsync(role).Result, PermissionId = 2 });
                                     await context.RolePermissions.AddAsync(new RolePermission { RoleId = _roleManager.GetRoleIdAsync(role).Result, PermissionId = 3 });
                                     await context.RolePermissions.AddAsync(new RolePermission { RoleId = _roleManager.GetRoleIdAsync(role).Result, PermissionId = 4 });
                                     await context.RolePermissions.AddAsync(new RolePermission { RoleId = _roleManager.GetRoleIdAsync(role).Result, PermissionId = 5 });*/
                            }
                        }
                       
                    }
                    else if(role.NormalizedName == "USER")
                    {
                        foreach (PermissionResourceCategory resource in resources)
                        {
                            //Only Access to Read-PermissionAction
                            await context.RolePermissions.AddAsync(new RolePermission { RoleId = role_id, ResourceId = resource.Id, PermissionId = 2 });
                                
                            
                        }

                    }


                }


                //context.ChangeTracker.DetectChanges();

                //context.SaveChanges();
            }
           

         
            if (context.ChangeTracker.HasChanges() == true)
            {
                context.SaveChanges();
            }


        }
    }
}

