using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using VideoModel;
using VideoServer.DTO;
using WeatherServer;

namespace VideoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController(
        VideoGoldenContext db,
        UserManager<VideoUser> userManager,
        JwtHandler jwtHandler
    ) : ControllerBase
    {
        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest loginRequest)
        {
            VideoUser? identityUser = await userManager.FindByEmailAsync(loginRequest.Email);
            if (identityUser == null)
            {
                return Unauthorized("Incorrect Email or Password");
            }

            bool success = await userManager.CheckPasswordAsync(identityUser, loginRequest.Password);
            if (!success)
            {
                return Unauthorized("Incorrect Email or Password");
            }

            JwtSecurityToken token = await jwtHandler.GetTokenAsync(identityUser);
            string jwtString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new LoginResult
            {
                Success = true,
                Token = jwtString
            });

        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            // Check if the email already exists
            if (await userManager.FindByNameAsync(registerRequest.UserName) is not null)
            {
                return Conflict("Username is already registered.");
            }


            VideoUser identityUser = new()
            {
                UserName = registerRequest.UserName,
                Email = registerRequest.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            IdentityResult result = await userManager.CreateAsync(identityUser, registerRequest.Password);

            if (!result.Succeeded) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create user.");
            }

            RegisteredUser user = new()
            {
                Username = registerRequest.UserName,
            };

            db.RegisteredUsers.Add(user);
            await db.SaveChangesAsync();

            JwtSecurityToken token = await jwtHandler.GetTokenAsync(identityUser);
            string jwtString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new RegisterResult
            {
                Success = true,
                Token = jwtString,
            });
        }
    }
}
