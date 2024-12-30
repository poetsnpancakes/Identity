using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity_Infrastructure.Entity
{
    public class RolePermission
    {
       
            
            public string RoleId { get; set; } // FK to IdentityRole
            public IdentityRole Role { get; set; }
            public int ResourceId { get; set; } // FK to PermissionResourceCategory
            public PermissionResourceCategory ResourceCategory { get; set; }

            public int PermissionId { get; set; } // FK to PermissionAction
            public PermissionAction Permissions { get; set; } 
        

    }
}
