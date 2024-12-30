using System.ComponentModel.DataAnnotations;

namespace Identity_Infrastructure.Models.Authentication.LogIn
{
    public class UserCredentialsRequest
    {
        [Required(ErrorMessage = "Email is Required")]
        public string? UserName { get; set; }

        [Required(ErrorMessage = "Password is Required")]
        public string? Password { get; set; }

      
    }
}
