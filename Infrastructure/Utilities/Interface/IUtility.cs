using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Infrastructure.Entity;

namespace Infrastructure.Utilities.Interface
{
    public interface IUtility
    {
        public Task<string> GetUserRole(ApplicationUser user);
    }
}
