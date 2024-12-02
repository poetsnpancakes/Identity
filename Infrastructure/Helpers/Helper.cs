
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Infrastructure.Entity;
using Infrastructure.Utilities;
using Infrastructure.Utilities.Interface;
using Infrastructure.Context.SeedData;

namespace Infrastructure.Helpers
{
    public class Helper
    {
        private IConfiguration _config;


        public Helper(IConfiguration config)
        {
            _config = config;

        }


        public string GenerateToken(ApplicationUser user,string role)
        {
           // var UserRole = _utility.GetUserRole(user.Id);
            var claims = new List<Claim>
                     {                        
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                        new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                        new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                        new Claim(ClaimTypes.Name,user.UserName),
                        new Claim(ClaimTypes.Email, user.Email),   
                        new Claim(ClaimTypes.Role,role)
                      };
            
               // claims.Add(new Claim(ClaimTypes.Role, role));   // roles.First() for IList roles
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(600),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = signIn
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
