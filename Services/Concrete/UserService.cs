using Microsoft.EntityFrameworkCore;
using Services.Interface;
using Repository.Interface;
using Infrastructure.RequestModel;
using Infrastructure.ResponseModel;


namespace Services.Concrete
{
    public class UserService :IUserService
    {
        private readonly IUserRepo _repo;
       
        public UserService(IUserRepo repo)
        {
            _repo = repo;
            
        }
        public Task<List<EmployeeResponse>> GetAllEmployees() => _repo.GetAllEmployees();
        public Task<EmployeeResponse> GetEmployee(long id) => _repo.GetEmployee(id);
        public Task<bool> PutEmployee(long id, EmployeeRequest request) => _repo.PutEmployee(id,request);

        public Task<BaseResponseModel> CreateEmployee(EmployeeRequest request) => _repo.CreateEmployee(request);
        public Task<AppResponseModel<string>> LogEmployee(BaseUserRequest userData) => _repo.LogEmployee(userData);
        public Task<bool> DeleteEmployee(long id)=> _repo.DeleteEmployee(id);

        public Task<bool> CreateDepartment(DepartmentRequest request) => _repo.CreateDepartment(request);
        public Task<List<DepartmentResponse>> GetAllDepartments() => _repo.GetAllDepartments();

        public Task<DepartmentResponse> GetDepartment(long id) => _repo.GetDepartment(id);













    }
}
