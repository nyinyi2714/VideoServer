using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

            string? username = await userManager.GetUserNameAsync(identityUser);

            if (username == null)
            {
                return NotFound("Username not found");
            }

            JwtSecurityToken token = await jwtHandler.GetTokenAsync(identityUser);
            string jwtString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new LoginResult
            {
                Success = true,
                Username = username,
                Token = jwtString
            });

        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            // Check if the email or username already exists
            bool isUsernameUnique = await userManager.FindByNameAsync(registerRequest.Username) is null;
            bool isEmailUnique = await userManager.FindByEmailAsync(registerRequest.Email) is null;
           
            if (!isUsernameUnique || !isEmailUnique)
            {
                return Conflict("Username and/or Email is already registered.");
            }

            // Identity User for authentication
            VideoUser identityUser = new()
            {
                UserName = registerRequest.Username,
                Email = registerRequest.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };
  
            IdentityResult result = await userManager.CreateAsync(identityUser, registerRequest.Password);

            if (!result.Succeeded) 
            {
                var errors = result.Errors.Select(e => e.Description);
                // Return detailed error information
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create user.");
            }

            // RegisteredUser for business logic
            RegisteredUser user = new()
            {
                Username = registerRequest.Username,
            };

            db.RegisteredUsers.Add(user);
            await db.SaveChangesAsync();

            JwtSecurityToken token = await jwtHandler.GetTokenAsync(identityUser);
            string jwtString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new RegisterResult
            {
                Success = true,
                Username = registerRequest.Username,
                Token = jwtString,
            });
        }
        
        [Authorize]
        [HttpGet("CheckToken")]
        public async Task<ActionResult> CheckToken()
        {
            // Get the Authorization header
            string? authHeader = HttpContext.Request.Headers.Authorization;
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Authorization header missing or invalid.");
            }

            // Extract the JWT token
            string tokenString = authHeader["Bearer ".Length..].Trim();
            var tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = tokenHandler.ReadJwtToken(tokenString);


            // Find the claim that represents the user ID
            string? username = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (username == null)
            {
                return BadRequest("Username not found in token.");
            }

            RegisteredUser? user = await db.RegisteredUsers
            .Include(u => u.Videos)
                .FirstOrDefaultAsync(u => u.Username == username);

            // Check if the user exists
            if (user == null)
            {
                return NotFound($"User with username {username} not found.");
            }

            CheckTokenResult checkTokenResult = new()
            {
                IsTokenValid = true,
                Username = username,
            };

            // User exists and token is valid
            return Ok(checkTokenResult);
        }
    }
}
