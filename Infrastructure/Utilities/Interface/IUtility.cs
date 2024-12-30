using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Identity_Infrastructure.Entity;
using Identity_Infrastructure.Models.ResponseModel;

namespace Identity_Infrastructure.Utilities.Interface
{
    public interface IUtility
    {
        public Task<string> GetUserRole(ApplicationUser user);
        public  Task<List<PermissionResource>> GetUserPermissions(string role);
    }
}
