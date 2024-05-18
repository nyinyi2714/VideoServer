using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using VideoModel;
using VideoServer.DTO;

namespace VideoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(VideoGoldenContext db) : ControllerBase
    {
   
        [HttpGet("{username}")]
        public async Task<ActionResult<IEnumerable<UserProfile>>> GetUserProfile(
            string username, 
            int skip = 0
 
        )
        {
            RegisteredUser? user = await db.RegisteredUsers
                .Include(u => u.Videos)
                .FirstOrDefaultAsync(u => u.Username == username);

            // Check if the user exists
            if (user == null)
            {
                return NotFound($"User with username {username} not found.");
            }

            int numOfVideosToFetch = 5;

            // Fetch the videos related to the user, applying pagination
            IEnumerable<VideoDto> videos = user.Videos
               .OrderByDescending(v => v.Timestamp)
               .Skip(skip * numOfVideosToFetch) 
               .Take(numOfVideosToFetch)
                .Select(v => new VideoDto 
                {
                    VideoId = v.VideoId,
                    Url = v.Url,
                    Title = v.Title,
                    Description = v.Description,
                    Views = v.Views,
                    Timestamp = v.Timestamp,
                    Username = v.Username
                });

            int totalVideos = user.Videos.Count;

            UserProfile userProfile = new()
            {
                Username = username,
                Videos = videos.ToList(),
                TotalVideos = totalVideos,
            };

            // Return the list of VideoDto
            return Ok(userProfile);
        }
    }
}
