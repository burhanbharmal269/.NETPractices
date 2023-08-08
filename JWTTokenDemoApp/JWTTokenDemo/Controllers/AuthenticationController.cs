using JWTTokenDemo.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using System.Text;
using System.Security.Claims;

namespace JWTTokenDemo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthenticationController(IConfiguration config)
        {
            _config = config;
        }

        public Users AuthenticateUser(Users user)
        {
            Users _user = null;
            if(user.UserName == "admin" && user.Password == "12345") 
            {
                _user = new Users { UserName = "Burhan" };
            }
            return _user;
        }

        public string GenerateToken (Users users)
        {
            var secretKey = new SymmetricSecurityKey(
             Encoding.ASCII.GetBytes(
                 _config.GetValue<string>("Authentication:SecretKey")));

            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new();

            claims.Add(new(JwtRegisteredClaimNames.UniqueName, users.UserName));

           
            //TOKEN
            var token = new JwtSecurityToken(
                _config.GetValue<string>("Authentication:Issuer"),
                _config.GetValue<string>("Authentication:Audeience"),
                claims,
                DateTime.UtcNow,//start time
                DateTime.UtcNow.AddMinutes(1),//expiry time
                signingCredentials
                );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Login(Users users)
        {
            IActionResult response = Unauthorized();

            var user = AuthenticateUser(users);
            if(user != null)
            {
                var token = GenerateToken(user);
                response = Ok(new {token = token});
            }
            return response;
        }

    }
}
