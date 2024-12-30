using Identity_Infrastructure.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity_Infrastructure.Entity
{
    public class PermissionAction
    {
        public int Id { get; set; }
        public string Name { get; set; } 
        public int Value { get; set; }


      //  public string Description { get; set; } 
    }
}
