using Identity_Repository.Interface;
using Identity_Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Identity_Infrastructure.Models.ResponseModel;
using Identity_Infrastructure.Models.Authentication.LogIn;
using Identity_Infrastructure.Models.Authentication.SignUp;

namespace Identity_Service.Concrete
{
    public class UserService:IUserService
    {
        private readonly IUserRepo _repo;

        public UserService(IUserRepo repo)
        {
           _repo= repo;
        }

        public  Task<BaseResponse> RegisterUser(UserRequest request,string role) =>  _repo.RegisterUser(request,role);
        public  Task<AppResponse<string>> LogInUser(UserCredentialsRequest request)=> _repo.LogInUser(request);
        public  Task<List<UserResponse>> GetByRole(string role) =>  _repo.GetByRole(role);
        public Task<UserPermissionResponse> GetUser(string userId) => _repo.GetUser(userId);
        public Task<bool> ConfirmEmail(string userid, string token) => _repo.ConfirmEmail(userid,token);

    }
}
