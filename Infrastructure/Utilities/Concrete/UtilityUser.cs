using Infrastructure.Utilities.Interface;
using Infrastructure.Context;
using Infrastructure.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Utilities.Concrete
{
    public class UtilityUser:IUtility
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
    }
}
