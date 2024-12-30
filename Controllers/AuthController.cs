using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using System.Net;
using System.ComponentModel.DataAnnotations;
using Identity_Infrastructure.Models.ResponseModel;
using Identity_Infrastructure.Models.Authentication.LogIn;
using Identity_Infrastructure.Models.Authentication.SignUp;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Identity_Infrastructure.Context.SeedData;
using Identity_Infrastructure.Entity;
using Identity_Service.Interface;
using Identity_Infrastructure.Authentication;


namespace Identity.Controllers
{
 //   [HasPermission(PermissionEnum.AccessMember)]
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

      //  [HasPermission(PermissionResourceEnum.Member,PermissionActionEnum.Create)]
        [HttpPost("user/register")]
        public async Task<IActionResult> RegisterUser (UserRequest request,string role)
        {

            if (request == null || role==null)
            {
                throw new ArgumentNullException(nameof(request));
            }
            var result = await _service.RegisterUser(request,role);
            return Ok(result);
          
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


        
        //[Authorize(Roles ="Admin")]
        [HttpGet("users/role")]
        [HasPermission(PermissionResourceEnum.Member, PermissionActionEnum.Read)]

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


        // [Authorize(Roles ="Admin,User")]
        [HttpGet("me")]
        public async Task<ActionResult<UserPermissionResponse>> GetUser()
        {
            //var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            var UserId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            var result = await _service.ConfirmEmail(userId, token);
            if (result == false)
            {
                return BadRequest("User not found.");
            }
            else
            {
                return Ok("Email-Confirmed");
            }
        }

    }
}
