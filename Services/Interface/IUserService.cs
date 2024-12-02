/*using Registration_Login.DB_Model;
using Registration_Login.Models.RequestModel;
using Registration_Login.Models.ResponseModel;*/
using Infrastructure.RequestModel;
using Infrastructure.ResponseModel;


namespace Services.Interface
{
    public interface IUserService
    {
        public Task<List<EmployeeResponse>> GetAllEmployees();
        public Task<EmployeeResponse> GetEmployee(long id);
        public Task<bool> PutEmployee(long id, EmployeeRequest request);
        public Task<bool> DeleteEmployee(long id);
        public Task<BaseResponseModel> CreateEmployee(EmployeeRequest request);

        public Task<AppResponseModel<string>> LogEmployee(BaseUserRequest userData);

       public Task<bool> CreateDepartment(DepartmentRequest request);
        public  Task<List<DepartmentResponse>> GetAllDepartments();
        public Task<DepartmentResponse> GetDepartment(long id);

    }
}
