using Microsoft.AspNetCore.Mvc;
using server.ViewModels;
using server.Models;
using server.Data;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;

namespace server.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UsersAPIDbContext dbContext;
        private readonly KeyManager keyManager;
        public AccountController(UsersAPIDbContext dbContext, KeyManager keyManager)
        {
            this.dbContext = dbContext;
            this.keyManager = keyManager;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUp([FromBody] User model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                await dbContext.Users.AddAsync(model);
                await dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await dbContext.Users.FindAsync(model.Username); // todo maybe only db.find()
            // todo hash passwords
            if (user != null && user.Password == model.Password)
            {
                var tokenExpiration = model.RememberMe ? DateTime.UtcNow.AddDays(7) : DateTime.UtcNow.AddHours(1);
                string token = GenerateJWTToken(user, tokenExpiration);
                if (model.RememberMe)
                {
                    Response.Cookies.Append("rememberMeToken", token, new CookieOptions() 
                    {
                        HttpOnly = true, 
                        Expires = tokenExpiration 
                    });
                }
                return Ok(new { token = token, username = user.Username, isInstitution = user.IsInstitution });
            }
            else
            {
                return Unauthorized();
            }
        }


        [Authorize]
        [HttpGet("verifyToken")]
        public IActionResult VerifyToken()
        {
            return Ok();
        }


        private string GenerateJWTToken(User user, DateTime expiration)
        {
            // Define secret key and issuer
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyManager.GetSecretKey()));
            var issuer = "Scheduler-Pro";

            // Create claims for the token
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.IsInstitution ? "Institution" : "User")
            };

            var token = new JwtSecurityToken (
                issuer: issuer,
                audience: issuer,
                claims: claims,
                expires: expiration,
                signingCredentials: new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
            );

            // Return token as a string
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
