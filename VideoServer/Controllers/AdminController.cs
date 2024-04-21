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
        public async Task<IActionResult> Register(RegisterRequest registerRequest)
        {
            // Check if the email already exists
            if (await userManager.FindByNameAsync(registerRequest.UserName) is not null)
            {
                return Conflict("Username is already registered.");
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

            JwtSecurityToken token = await jwtHandler.GetTokenAsync(user);
            string jwtString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new RegisterResult
            {
                Success = true,
                Token = jwtString,
            });
        }
    }
}
