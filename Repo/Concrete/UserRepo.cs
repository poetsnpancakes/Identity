using Identity_Repository.Interface;
using Identity_Infrastructure.Context;
using Microsoft.Extensions.Configuration;
using Identity_Infrastructure.Models.ResponseModel;
using Identity_Infrastructure.Models.Authentication.LogIn;
using Identity_Infrastructure.Models.Authentication.SignUp;
using Microsoft.AspNetCore.Identity;
using System.Net;
using Identity_Infrastructure.Helpers;
using Identity_Infrastructure.Entity;
using System;
using System.IO;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Azure;
using System.Xml.Linq;
using Identity_Infrastructure.Context.SeedData;
using Identity_Infrastructure.Utilities.Interface;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using Azure.Core;
using Microsoft.AspNetCore.Identity.UI.Services;
using Identity_Infrastructure.Configurations.EmailConfiguration;


namespace Identity_Repository.Concrete
{
    public class UserRepo:IUserRepo
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IMapper _mapper;
        private IUtility _utility;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IEmailService _emailService;
        public UserRepo(ApplicationDbContext context, UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager, IConfiguration config,
            SignInManager<ApplicationUser> signInManager, IMapper mapper, IUtility utility
            ,IHttpContextAccessor httpContextAccessor,IEmailService emailService)
        {
            _config = config;
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _mapper = mapper;
            _utility = utility;
            _httpContextAccessor = httpContextAccessor;
            _emailService = emailService;
        }

        public async Task<BaseResponse> RegisterUser(UserRequest request,string role)
        {
            //CheckUser
            var usercheck = await _userManager.FindByNameAsync(request.UserName);
            if (usercheck != null)
            {
                BaseResponse badresponse = new BaseResponse()
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = "User Already Exists"
                };
                return await Task.FromResult(badresponse);
            }
            //Fetch-User
            ApplicationUser user = _mapper.Map<ApplicationUser>(request);

                /* ApplicationUser user = new ApplicationUser()
                 {
                     UserName = request.UserName,
                     Email = request.Email,
                 };*/
                //CheckRole
                if (await _roleManager.RoleExistsAsync(role))
                {
                // Add User
                var result = await _userManager.CreateAsync(user, request.Password);
                if (!result.Succeeded)
                {
                    BaseResponse badresponse = new BaseResponse()
                    {
                        Status = (int)HttpStatusCode.BadRequest,
                        Message = "User Failed To Create."

                    };
                    return await Task.FromResult(badresponse);

                }
                //AddRole
                await _userManager.AddToRoleAsync(user,role);

                //Add Claim
                var claim = new Claim("Role", role);
                await _userManager.AddClaimAsync(user, claim);

                    //Confirm-Email
                    var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                    // var serverIpAddress = "https://192.168.1.7";
                    var requestcontext = _httpContextAccessor.HttpContext.Request;
                    // var confirmationLink = $"{requestcontext.Scheme}://{serverIpAddress}/api/auth/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";
                    var confirmationLink = $"{requestcontext.Scheme}://{requestcontext.Host}/api/auth/confirm-email?userId={user.Id}&token={Uri.EscapeDataString(token)}";
                    //  await _emailService.SendEmailAsync(user.Email, "Confirm Your Email",
                    //       $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>.");
                    var emailSentSuccessfully = await SendConfirmationEmail(user.Email, confirmationLink);
                if (emailSentSuccessfully)
                { 
                    BaseResponse goodresponse = new BaseResponse()
                    {
                        Status = (int)HttpStatusCode.OK,
                        Message = "User Created Successfully.Please check your Email for Confirmation."
                    };
                    return await Task.FromResult(goodresponse);
                }
                else
                {
                    BaseResponse badresponse = new BaseResponse()
                    {
                        Status = (int)HttpStatusCode.BadRequest,
                        Message = "Invalid Email-address!"

                    };
                    return await Task.FromResult(badresponse);
                }
            }
            else
            {
                BaseResponse badresponse = new BaseResponse()
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = "User-Role Does Not Exist."

                };
                return await Task.FromResult(badresponse);
            }

            
        }
        private async Task<bool> SendConfirmationEmail(string email, string confirmationLink)
        {
            try
            {
                // Attempt to send the email
                await _emailService.SendEmailAsync(email, "Confirm Your Email", $"Please confirm your email by clicking <a href='{confirmationLink}'>here</a>.");
                return true; // If the email was sent successfully
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as necessary
                // Log.Error($"Error sending email: {ex.Message}");
                return false; // If there was an error sending the email
            }
        }
        public async Task<bool> ConfirmEmail(string userid, string token)
        {
            var user = await _userManager.FindByIdAsync(userid);
            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            return false;
        }
        public async Task<AppResponse<string>> LogInUser(UserCredentialsRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
            {
                AppResponse<string> badresponse = new AppResponse<string>()
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = "Invalid User-Name or Password.",
                    Data = "NA",
                };
                return await Task.FromResult(badresponse);
            }

            //check credentials
           /* var result = await _signInManager.PasswordSignInAsync(request.UserName,
                           request.Password, isPersistent: false, lockoutOnFailure: false);*/

            var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
            //generate token
            if (result.Succeeded)
            {
              if(!await _userManager.IsEmailConfirmedAsync(user))
                {
                    AppResponse<string> badresponse = new AppResponse<string>()
                    {
                        Status = (int)HttpStatusCode.BadRequest,
                        Message = "Please verify your email!",
                        Data = "NA",
                    };
                    return await Task.FromResult(badresponse);
                }
            
                var roles = await _userManager.GetRolesAsync(user);
               // var UserRole = _utility.GetUserRole(user).Result;

                IList<Claim> claims = await _userManager.GetClaimsAsync(user);
                /* return Ok(new
                 {
                     result = result,
                     username = user.UserName,
                     email = user.Email,
                     token = _jwtToken.GenerateToken(user, roles, claims)
                 });*/

                //Generate-Token
                Helper helper = new Helper(_config,_utility);
                string access_token = await helper.GenerateToken(user,roles,claims);

                AppResponse<string> response = new AppResponse<string>
                {

                    Status = (int)HttpStatusCode.OK,
                    Message = "Logged-In successfully",
                    Data = access_token,
                };
               
                return await Task.FromResult(response);
            }
            else
            {
               AppResponse<string> badresponse = new AppResponse<string>()
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = "Invalid User-Name or Password.",
                    Data = "NA",
                };
                return await Task.FromResult(badresponse);
            }
            //verify user and password
            //claimlist-creation
            //Add-roles to the list
            //Generate Token with Claims
            //return Token
        }
        public async Task<List<UserResponse>> GetByRole(string role)
        {
            if (await _roleManager.RoleExistsAsync(role))
            {
                var result = await _userManager.GetUsersInRoleAsync(role);
                var users =  result.ToList();
                /*  var users = new List<UserResponse>();
                  foreach (var user in response)
                  {
                      var temp = new UserResponse();
                      temp.UserName = user.UserName;
                      temp.Email = user.Email;
                      users.Add(temp);

                  };*/
                List<UserResponse> response = _mapper.Map<List<UserResponse>>(users);
                return response;
            }
            else
            {

                return null;
            }
             
            
        }
        public async Task<UserPermissionResponse> GetUser(string userId)   //me-API
        {
            var result= await _userManager.FindByIdAsync(userId);
            var role =  await _context.UserRoles.Where(u => u.UserId == userId).FirstOrDefaultAsync();//Get object from IdentityUserRoles
            var user_role = await _roleManager.FindByIdAsync(role.RoleId);//Get role from IdentityRoles
            var permissions_list = await _utility.GetUserPermissions(user_role.NormalizedName);
           /* List<string> permissions = new();
            foreach (var permission in permissions_list)
            {
                permissions.Add(permission);
            }*/
            if (result!= null )
            {
                UserPermissionResponse user = new UserPermissionResponse()
                {
                    UserName = result.UserName,
                    User_Role = user_role.NormalizedName,
                    Email = result.Email,
                    Permissions = permissions_list
                    

            };
                return user;
            }
            return null;


            /* var result = await _context.Users.ToListAsync();
             // List<UserResponse> result = _mapper.Map<List<UserResponse>>(users);
             var users = new List<UserResponse>();
             foreach (var user in result)
             {
                 var temp = new UserResponse();
                 temp.UserName = user.UserName;
                 temp.Email = user.Email;
                 var temp_user = await _userManager.FindByNameAsync(user.UserName);
                 var temp_User_Role = await _userManager.GetRolesAsync(temp_user);
                 temp.User_Role = temp_User_Role.ToString();
                 users.Add(temp);

             }; 
             return users;
            */

        }
    }
}
