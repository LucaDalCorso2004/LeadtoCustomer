using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace LeadtoCustomer.Model
{
    public class LoginModel
    {
        public const string GUID = "396B5DD9-CC75-411C-9401-5B6E1F391B89";
        public static string CreateJWT(UserModel userModel)
        {
            var secretkey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(GUID)); 
            var credentials = new SigningCredentials(secretkey, SecurityAlgorithms.HmacSha256);

            var claims = new[] 
			{
                new Claim(ClaimTypes.Name, userModel.Username),
				new Claim(JwtRegisteredClaimNames.Sub, userModel.Username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")),
             new Claim("role", userModel.Role.ToString())
            };

            var token = new JwtSecurityToken(issuer: "domain.com", audience: "domain.com", claims: claims, expires: DateTime.Now.AddMinutes(60), signingCredentials: credentials); // NOTE: USE THE REAL DOMAIN NAME
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
        public string Username { get; set; }

        public string Password { get; set; }    

    }
}
