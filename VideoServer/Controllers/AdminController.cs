﻿using Microsoft.AspNetCore.Http;
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
            VideoUser? user = await userManager.FindByEmailAsync(loginRequest.Email);
            if (user == null)
            {
                return Unauthorized("Incorrect Email or Password");
            }

            bool success = await userManager.CheckPasswordAsync(user, loginRequest.Password);
            if (!success)
            {
                return Unauthorized("Incorrect Email or Password");
            }

            JwtSecurityToken token = await jwtHandler.GetTokenAsync(user);
            string jwtString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new LoginResult
            {
                Success = true,
                Token = jwtString
            });

        }

        [HttpPost("Register")]
        public async Task<ActionResult> Register(RegisterRequest registerRequest)
        {
            // Check if the email already exists
            if (await userManager.FindByEmailAsync(registerRequest.Email) is not null)
            {
                return Conflict("Email address is already registered.");
            }


            VideoUser user = new()
            {
                UserName = registerRequest.UserName,
                Email = registerRequest.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
            };

            IdentityResult result = await userManager.CreateAsync(user, registerRequest.Password);

            if (!result.Succeeded) 
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Failed to create user.");
            }

            await db.SaveChangesAsync();
            return Ok("User registered successfully.");
        }

        [HttpPost("SeedUser")]
        public async Task<ActionResult> SeedUser()
        {
            (string name, string email) = ("user1", "comp584@csun.edu");
            VideoUser user = new()
            {
                UserName = name,
                Email = email,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            if (await userManager.FindByNameAsync(name) is not null)
            {
                user.UserName = "user2";
            }
            var result = await userManager.CreateAsync(user, "Abcde1$");
            if (!result.Succeeded)
            {
                var errorMessages = string.Join(", ", result.Errors.Select(e => e.Description));
                return BadRequest($"Error(s): {errorMessages}");
            }
            user.EmailConfirmed = true;
            user.LockoutEnabled = false;
            await db.SaveChangesAsync();

            return Ok();
        }
    }
}
