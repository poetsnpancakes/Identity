using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.ComponentModel.DataAnnotations;
using Services.Interface;
using Infrastructure.Models.ResponseModel;
using Infrastructure.Models.Authentication.LogIn;
using Infrastructure.Models.Authentication.SignUp;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Infrastructure.Context.SeedData;
using Infrastructure.Entity;


namespace Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _service;
        private readonly IHttpContextAccessor _httpContextAccessor;

      

        public AuthController(IUserService service,IHttpContextAccessor httpContextAccessor)
        {
           _service = service;
            _httpContextAccessor = httpContextAccessor;


        }
        protected string UserId
        {
            get
            {
                return _httpContextAccessor?.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            }
        }


        [HttpPost("user/register")]
        public async Task<IActionResult> RegisterUser (UserRequest request,string role)
        {

            if (request == null || role==null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var result = await _service.RegisterUser(request,role);
            return Ok(result);
           /* //Check User Exist
            var usercheck= await _userManager.FindByNameAsync(request.UserName);
            if (usercheck != null)
            {
                BaseResponse badresponse = new BaseResponse()
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = "User Already Exists"
                };
                return BadRequest(badresponse);
            }
            // Add User
            IdentityUser user = new IdentityUser()
            {
                UserName = request.UserName,
                Email = request.Email,
            };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                BaseResponse goodresponse = new BaseResponse()
                {
                    Status = (int)HttpStatusCode.OK,
                    Message = "User Created Successfully"
                };
                return Ok(goodresponse);

            }
            else
            {
                BaseResponse badresponse = new BaseResponse()
                {
                    Status = (int)HttpStatusCode.BadRequest,
                    Message = "User Failed To Create." 
      
                };
                return BadRequest(badresponse);
            }

                //Assign Role*/
        }

        [HttpPost("user/login")]
        public async Task<IActionResult> LogInUser(UserCredentialsRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var result= await _service.LogInUser(request);
            return Ok(result);
            
        }


        [HttpGet("users/role")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<UserResponse>>> GetByRole(string role)
        {
            if (role == null)
            {
                return BadRequest("Role can not be NULL");
            }
            var result = await _service.GetByRole(role);
            if (result != null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("User-Role Does Not Exist.");
            }
        }


        [Authorize(Roles ="Admin")]
        [HttpGet("me")]
        public async Task<ActionResult<UserResponse>> GetUser()
        {
          //  var UserId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier).Value;
            //var UserId = "70d80b6f-d2d2-449b-8d6c-fb9e046e867e";
            if (UserId == null)
            {
                return NotFound();
            }
           var result = await _service.GetUser(UserId);
            if (result!=null)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest("Invalid User-ID");
            }

        
        }
    }
}
