
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Identity_Infrastructure.Entity;
using Identity_Infrastructure.Utilities;
using Identity_Infrastructure.Utilities.Interface;
using Identity_Infrastructure.Context.SeedData;
using System.Data;
using Identity_Infrastructure.Authentication;

namespace Identity_Infrastructure.Helpers
{
    public class Helper
    {
        private IConfiguration _config;
        private IUtility _utility;

        public Helper(IConfiguration config, IUtility utility)
        {
            _config = config;
            _utility = utility;
        }


        public async Task<string> GenerateToken(ApplicationUser user, IList<string> roles, IList<Claim> claims)
        {
            // var UserRole = _utility.GetUserPermissions(user.Id);
            /*   var claims = new List<Claim>
                        {                        
                           new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                           new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()),
                           new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                           new Claim(ClaimTypes.Name,user.UserName),
                           new Claim(ClaimTypes.Email, user.Email),   
                           new Claim(ClaimTypes.Role,role)
                         };
               */

            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTime.UtcNow.ToString()));
            claims.Add(new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()));
            claims.Add(new Claim(ClaimTypes.Name, user.UserName));
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
         /*   foreach (var role in roles)
            {
                var permissions = await _utility.GetUserPermissions(role);
                foreach (var permission in permissions)
                {
                    claims.Add(new Claim(CustomClaims.Permissions, permission));
                }
            }*/
           
            // claims.Add(new Claim(ClaimTypes.Role, role));   // roles.First() for IList roles

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
            var signIn = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(60000),
                Issuer = _config["Jwt:Issuer"],
                Audience = _config["Jwt:Audience"],
                SigningCredentials = signIn
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return  tokenHandler.WriteToken(token);
        }
    }
}
