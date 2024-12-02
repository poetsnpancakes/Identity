using Repository.Interface;
using Infrastructure.Context;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Net;
using Infrastructure.Helpers;
using Infrastructure.Mapper;
using Infrastructure.Entities;
using Infrastructure.RequestModel;
using Infrastructure.ResponseModel;
using Infrastructure.Utilities.Concrete;
using Infrastructure.Utilities.Interface;
//using Microsoft.AspNetCore.Identity;




namespace Repository.Concrete
{
    public class UserRepo :IUserRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private IConfiguration _config;
        private readonly IUserUtility _utility;
        



        public UserRepo(ApplicationDbContext context, IMapper mapper,
            IConfiguration config,IUserUtility utility)
        {
            _context = context;
            _mapper = mapper; 
            _config = config;
            _utility = utility;


        }

        //---------------------------------

        public async Task<List<EmployeeResponse>> GetAllEmployees()
        {

           var res = await _context.Employees.ToListAsync();
           List<EmployeeResponse> result = _mapper.Map<List<EmployeeResponse>>(res);
            /* var result = new List<UserResponse>();
             foreach(var item in res)
              {
                  var temp = new UserResponse();
                  temp.UserName = item.UserName;
                  temp.Email = item.Email;
                  result.Add(temp);

              }*/
         



            return result;

        }
       


        public  Task<EmployeeResponse> GetEmployee(long id)
        {
            // var _Id = request.Id;
           var getItem=  _context.Employees.Where(x => x.Id == id ).FirstOrDefault();


            EmployeeResponse getResponse = _mapper.Map<EmployeeResponse>(getItem);
           /* var getResponse = new UserResponse();
            getResponse.UserName = getItem.UserName;
            getResponse.Email=getItem.Email;*/
           

            return Task.FromResult(getResponse);
         


        }

        public async Task<bool> PutEmployee(long id,EmployeeRequest request)
        {
            /* if (id != request.Id)
             {
                 return BadRequest("Request id can not be null");
             }*/

            bool getItem = _utility.EmployeeExists(id);
            if (getItem == false)
            {
                return false;
            }
            var user = _context.Employees.Where(x => x.Id == id).FirstOrDefault();
            // UserDomain user = _mapper.Map<UserDomain>(request);
            /* if (id != user.Id)
             {
                 return Task.FromResult(false);
             }*/


            if (user != null)
            {
                user.UserName=request.UserName;
                user.Password=request.Password;
                user.Email=request.Email;
                user.IsComplete=true;
                await _context.SaveChangesAsync();
                return true;
            }

            

          
           /* try
            {
                _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RegisterItemExists(id))
                {
                    return Task.FromResult(false);
                }
                else
                {
                    throw;
                }
            }*/

            return false;
        }


        public async Task<BaseResponseModel> CreateEmployee(EmployeeRequest request)
        {

            bool dept = _utility.UniqueDeptID(request.Dept_id);
            if (dept == true)
            {

                Department department = new Department()
                {

                    Name = request.Dept_name
                };
                _context.Departments.Add(department);
                request.Dept_id = _context.Departments.Max(u => u.Id)+1;
                //request.Dept_id = request.Dept_id + 1;


               await _context.SaveChangesAsync();
             }

            //_context.UserDomains.Max(u => u.Id);


            bool user = _utility.UniqueUser(request.UserName);
            if (user == true)
            {
                  // Employee employee = _mapper.Map<Employee>(request);
                Employee registerUser = new Employee()
                {
                    UserName = request.UserName,
                    Password = request.Password,
                    Email = request.Email,
                    IsComplete = request.IsComplete,
                    DepartmentId=request.Dept_id,

                };
                //registerUser.Id += _context.UserDomains.Max(u => u.Id);
                _context.Employees.Add(registerUser);
             
                await _context.SaveChangesAsync();

             /*  bool dept = UniqueDeptID(request.Dept_id);
                if (dept == true)
                {
                    //  Department department = _mapper.Map<Department>(request);
                    Department department = new Department()
                    {
                        Id = request.Dept_id,
                        Name = request.Dept_name
                    };
                    _context.Departments.Add(department);

                }
                _context.SaveChangesAsync();*/




                // UserResponse getResponse = _mapper.Map<UserResponse>(registerUser);
                BaseResponseModel response = new BaseResponseModel()
                {

                    Status = (int)HttpStatusCode.OK,
                    Message = "User created successfully"
                   
                };

                return await Task.FromResult(response);
            }
            BaseResponseModel badresponse = new BaseResponseModel()
            {

                Status = (int)HttpStatusCode.BadRequest,
                Message = "Enter a unique username."

            };
            return await Task.FromResult(badresponse);

        }
        public Task<AppResponseModel<string>> LogEmployee(BaseUserRequest userData)
        {
            if (userData.UserName != null && userData.Password != null)
            {
                bool user =  _utility.GetUser(userData.UserName, userData.Password);
                if (user == false)
                {
                    Helper helper = new Helper(_config);
                    string access_token = helper.GenerateToken(userData);
                   

                    AppResponseModel<string> response = new AppResponseModel<string>
                    {
                       
                        Status = (int)HttpStatusCode.OK,
                        Message = "Logged-In successfully",
                        Data = access_token,
                    };

                    return Task.FromResult(response);
                }

            }
            AppResponseModel<string> badresponse = new AppResponseModel<string>
            {

                Status = (int)HttpStatusCode.BadRequest,
                Message = "Wrong User Credentials",
                Data = ""
            };
            return Task.FromResult(badresponse);
        }

        public Task<bool> DeleteEmployee(long id)
        {
            //var _Id = request.Id;
            var getItem = _context.Employees.Where(x => x.Id == id).FirstOrDefault();

            if (getItem == null)
            {
                return Task.FromResult(false);
            }
            _context.Employees.Remove(getItem);
            _context.SaveChangesAsync();
            return Task.FromResult(true);
        }

            /* var registerItem = await _context.UserDomains.FindAsync(id);
             if (registerItem == null)
             {
                 return NotFound();
             }

             _context.RegisterItems.Remove(registerItem);
             await _context.SaveChangesAsync();

             return NoContent();
         }*/

            //---------------------------------------------

      /*      public bool GetUser(string Username, string password)
        {
            // return _context.UserDomains.FirstOrDefaultAsync(u => u.UserName == Username && u.Password == password);
            return !_context.Employees.Any(u => u.UserName == Username && u.Password == password);
        }
        public Task<UserDomain> GetUserName(string Username)
        {
            return  _context.UserDomains.FirstOrDefaultAsync(u => u.UserName == Username);

        
        }
        public bool UniqueUser(string Username)
        {
            return !_context.Employees.Any(u => u.UserName == Username);
        }


        public bool UniqueDeptID(long id)
        {
            return !_context.Departments.Any(u => u.Id == id);
        }




        public bool EmployeeExists(long id)
        {
            return _context.Employees.Any(e => e.Id == id);
        }
        */
        public async Task<List<DepartmentResponse>> GetAllDepartments()
        {
            var res = await _context.Departments.ToListAsync();
            List<DepartmentResponse> result = _mapper.Map<List<DepartmentResponse>>(res);
            return result;
        }

         
        public  Task<DepartmentResponse> GetDepartment(long id)
        {
            var getItem =  _context.Departments.Where(x => x.Id == id).FirstOrDefault();


            DepartmentResponse getResponse = _mapper.Map<DepartmentResponse>(getItem);
            /* var getResponse = new UserResponse();
             getResponse.UserName = getItem.UserName;
             getResponse.Email=getItem.Email;*/


            return Task.FromResult(getResponse);

        }
      //  public Task<DepartmentResponse> DeleteDepartment(long id);
       public Task<bool> CreateDepartment(DepartmentRequest request)
        {
            bool dept = _utility.UniqueDeptID(request.Id);


            if (dept == true)
            {
                //  Department registerUser = _mapper.Map<Department>(request);
                Department department = new Department()
                {
                    Name = request.Name,
                };
              
               
                _context.Departments.Add(department);
                _context.SaveChangesAsync();

                // UserResponse getResponse = _mapper.Map<UserResponse>(registerUser);
                

                return Task.FromResult(true);
            }
           
            return Task.FromResult(false);

        
         }



    }
}
