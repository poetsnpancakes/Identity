using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity_Infrastructure.Models.ResponseModel
{
    public class PermissionResource
    {
        public string Resource { get; set; } 
        public List<ActionResponse> Actions { get; set; } 
    }
}
