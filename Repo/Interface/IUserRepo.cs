using Identity_Infrastructure.Models.Authentication.SignUp;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Identity_Infrastructure.Models.ResponseModel;
using Identity_Infrastructure.Models.Authentication.LogIn;

namespace Identity_Repository.Interface
{
    public interface IUserRepo
    {
        public Task<BaseResponse> RegisterUser(UserRequest request,string role);
        public Task<AppResponse<string>> LogInUser(UserCredentialsRequest request);
        public Task<List<UserResponse>> GetByRole(string role);
        public Task<UserPermissionResponse> GetUser(string user_id);

        public Task<bool> ConfirmEmail(string userid, string token);
    }
}
