using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity_Infrastructure.Models.ResponseModel
{
    public class UserResponse
    {
        public string UserName { get; set; } = "NA";
        public string Email { get; set; } = "NA";
        public string User_Role { get; set; } = "NA";
    }
}
