using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using static System.Collections.Specialized.BitVector32;

namespace Identity_Infrastructure.Authentication
{
    public sealed class HasPermissionAttribute : AuthorizeAttribute
    {
        public HasPermissionAttribute(PermissionResourceEnum resource,PermissionActionEnum action)
          //:base(policy: $"{resource}:{action}")//policy:resouce.ToString()
        {
           
            Policy = $"{resource}:{action}";

        }
    }
}
