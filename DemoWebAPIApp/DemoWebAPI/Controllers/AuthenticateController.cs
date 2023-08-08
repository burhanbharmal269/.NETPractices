using DemoWebAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using JwtRegisteredClaimNames = System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames;

namespace DemoWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticateController : ControllerBase
    {
        private readonly IConfiguration _config;
        private readonly PersonDbContext _context;

        public AuthenticateController(IConfiguration config, PersonDbContext context)
        {
            _config = config;
            _context = context;
        }
        public string ValidateUser(Users users)
        {
            var connectionString = "Server=LAPTOP-HV667SQK\\SQLEXPRESS;Database=PersonEFDb;Trusted_Connection=True;TrustServerCertificate=True";

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand("SP_ValidateUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("UserName", users.UserName);
                    cmd.Parameters.AddWithValue("Password", users.Password);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        string username = reader.GetString(0);
                        return username;
                    }
                    
                }
                 
            }
            return null;
        }
        public string GenerateToken(Users user)
        {
            var secretKey = new SymmetricSecurityKey(
                Encoding.ASCII.GetBytes(
                    _config.GetValue<string>("Authentication:SecretKey")));

            var signingCredentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            List<Claim> claims = new();

            claims.Add(new(JwtRegisteredClaimNames.Sub, user.UserName));

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
        [HttpPost("PostUser")]
        public async Task<ActionResult<Users>> CreteUser(Users users)
        {
            _context.Users.Add(users);
            await _context.SaveChangesAsync();
            return Ok(users);
        }

        [AllowAnonymous]
        [HttpPost("token")]
        public ActionResult<string> Authenticate([FromBody] Users user)
        {
            var data = ValidateUser(user);

            if (data is null)
            {
                return Unauthorized();
            }
            var token = GenerateToken(user);

            return Ok(token);

        }


    }
}

        

