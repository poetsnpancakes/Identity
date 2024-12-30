using Identity_Infrastructure.Utilities.Interface;
using Identity_Infrastructure.Context;
using Identity_Infrastructure.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Identity_Infrastructure.Models.ResponseModel;

namespace Identity_Infrastructure.Utilities.Concrete
{
    public class UtilityUser : IUtility
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public UtilityUser(UserManager<ApplicationUser> userManager, ApplicationDbContext context,
            RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _context = context;
            _roleManager = roleManager;
            _signInManager = signInManager;

        }

        public async Task<string> GetUserRole(ApplicationUser user)
        {
            //var roles = await _userManager.GetRolesAsync(user);
            //var userId= await _userManager.GetUserIdAsync(user);
            var role = await _context.UserRoles.Where(u => u.UserId == user.Id).FirstOrDefaultAsync();
            var user_role = await _roleManager.FindByIdAsync(role.RoleId);
            string role_name= user_role.Name;
            return role_name;
            //return roles;



        }
        public async Task<List<PermissionResource>> GetUserPermissions(string role)
        {

            /* var permissions = await _context.RolePermissions
             .Where(rp => _context.Roles
                 .Where(r => role.Contains(r.NormalizedName))
                 .Select(r => r.Id)
                 .Contains(rp.RoleId))
             .Select(rp => rp.Permissions.Name)
             .Distinct()
             .ToListAsync();

             return permissions;*/

            var rolePermissions = await _context.RolePermissions
      .Where(rp => _context.Roles
          .Where(r => role.Contains(r.NormalizedName))
          .Select(r => r.Id)
          .Contains(rp.RoleId))
      .Select(rp => new
      {
          ResourceName = rp.ResourceCategory.Name,  // Rename to ResourceName
          PermissionName = rp.Permissions.Name,     // Rename to PermissionName
      })
      .ToListAsync();

            // Group by resource and assign actions to the corresponding resources
            var permissionsGrouped = rolePermissions
                .GroupBy(rp => rp.ResourceName)
                .Select(g => new PermissionResource
                {
                    Resource = g.Key,  // Resource (Category Name)
                    Actions = g.Select(a => new ActionResponse
                    {
                        Type = a.PermissionName,  // Use renamed property
                    }).ToList()
                }).ToList();

            // Prepare the response model
          /*  return new PermissionResource
            {

                Permissions = permissionsGrouped
            };*/
          return permissionsGrouped;
        }
    }
}
