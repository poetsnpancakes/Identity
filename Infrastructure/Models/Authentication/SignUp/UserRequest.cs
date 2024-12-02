using System.ComponentModel.DataAnnotations;

namespace Infrastructure.Models.Authentication.SignUp
{
    public class UserRequest
    {
        [Required (ErrorMessage ="UserName is Required")]
        public string? UserName { get; set; }
        [Required(ErrorMessage = "Password is Required")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Email is Required")]
        public string? Email { get; set; }
    }
}
