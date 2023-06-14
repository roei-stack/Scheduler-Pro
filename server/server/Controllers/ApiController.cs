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
using Microsoft.AspNetCore.Cors.Infrastructure;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Forms.v1;
using Google.Apis.Forms.v1.Data;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using System;
using System.Collections.Generic;
using System.IO;
using Google.Apis.Util.Store;
using System.IO;
using System.Threading;
using System.Runtime.Serialization.Formatters;

namespace server.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly APIDbContext dbContext;
        private readonly KeyManager keyManager;

        public ApiController(APIDbContext dbContext, KeyManager keyManager)
        {
            this.dbContext = dbContext;
            this.keyManager = keyManager;
        }

        [HttpPost("Account/signup")]
        public async Task<IActionResult> SignUp([FromBody] User model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            try
            {
                // currently
                model.NeedSetup = model.IsInstitution;
                await dbContext.Users.AddAsync(model);

                if (model.IsInstitution)
                {
                    var institutionData = new InstitutionData { Username = model.Username };
                    await dbContext.InstitutionsData.AddAsync(institutionData);
                }
                await dbContext.SaveChangesAsync();
                return Ok();
            }
            catch (Exception ex)
            {
                return Unauthorized(ex.Message);
            }
        }

        [HttpPost("Account/signin")]
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
                return Ok(new 
                { 
                    token = token,
                    username = user.Username,
                    isInstitution = user.IsInstitution,
                    needSetup = user.NeedSetup
                });
            }
            else
            {
                return Unauthorized();
            }
        }

        [Authorize]
        [HttpPost("Account/checkInstitutionName")]
        public async Task<IActionResult> CheckInstitutionName([FromBody] string name)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // check that user is an institution
            var username = User.FindFirst("username")?.Value;
            var user = await dbContext.Users.FindAsync(username);

            if (user == null || user.IsInstitution == false)
            {
                return Unauthorized();
            }

            bool isNameAvailable = !this.dbContext.InstitutionsData.Any(i => i.InstitutionName == name);

            if (isNameAvailable)
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [Authorize]
        [HttpPost("Account/institutionSetup/{institutionName}")]
        public async Task<IActionResult> InstitutionSetup(string institutionName, [FromBody] InstitutionSetupViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var username = User.FindFirst("username")?.Value;
            var user = await dbContext.Users.FindAsync(username);
            
            if (user == null || user.IsInstitution == false || user.NeedSetup == false)
            {
                return BadRequest();
            }
            var institutionData = await this.dbContext.InstitutionsData.FirstOrDefaultAsync(i => i.Username == username);
            if (institutionData == null)
            {
                // we should not be here
                return StatusCode(500);
            }
            // todo save and verify data...SETUP
            user.NeedSetup = false;
            // create form links
            institutionData.StaffFormId = $"{Guid.NewGuid()}";
            institutionData.StudentFormId = $"{Guid.NewGuid()}";
            await dbContext.SaveChangesAsync();

            var newToken = RefreshToken(user);
            return Ok(new { token = newToken });
        }
        
        [Authorize]
        [HttpGet("Account/getFormsLinks")]
        public async Task<IActionResult> getFormsLinks()
        {
            var username = User.FindFirst("username")?.Value;
            var user = await dbContext.Users.FindAsync(username);

            if (user == null || user.IsInstitution == false || user.NeedSetup == true)
            {
                return Unauthorized();
            }

            var institutionData = await this.dbContext.InstitutionsData.FirstOrDefaultAsync(i => i.Username == username);
            if (institutionData == null)
            {
                // we should not be here
                return StatusCode(500);
            }

            if (institutionData.StudentFormId == null || institutionData.StaffFormId == null)
            {
                return BadRequest();
            }

            return Ok(new
            {
                staffFormId = institutionData.StaffFormId,
                studentFormId = institutionData.StudentFormId
            });
        }

        [HttpGet("checkFormId/{formId}")]
        public IActionResult CheckFormId(string formId)
        {
            var i = dbContext.InstitutionsData.Find("biu");
            if (dbContext.InstitutionsData.Any(i => i.StudentFormId == formId))
            {
                return Ok( new { formType = "student" } );
            }
            if (dbContext.InstitutionsData.Any(i => i.StaffFormId == formId))
            {
                return Ok(new { formType = "staff" });
            }
            return BadRequest();
        }

        private string RefreshToken(User user)
        {
            var currentExpiration = User.FindFirst("exp")?.Value;
            // Parse the current expiration date
            var expiration = DateTime.UnixEpoch.AddSeconds(long.Parse(currentExpiration));
            return GenerateJWTToken(user, expiration);
        }

        private string GenerateJWTToken(User user, DateTime expiration)
        {
            // Define secret key and issuer
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyManager.GetSecretKey()));
            var issuer = "Scheduler-Pro";

            // Create claims for the token
            var claims = new[]
            {
                new Claim("username", user.Username),
                new Claim("isInstitution", user.IsInstitution ? "true" : "false"),
                new Claim("isSetupNeeded", user.NeedSetup ? "true" : "false")
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
