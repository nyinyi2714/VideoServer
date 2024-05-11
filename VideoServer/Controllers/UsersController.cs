using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<ActionResult<IEnumerable<VideoDto>>> GetVideosByUser(
            string username, 
            int skip = 0, 
            int take = 10
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

            // Fetch the videos related to the user, applying pagination
            var videos = user.Videos
               .OrderByDescending(v => v.Timestamp)
               .Skip(skip) 
               .Take(take)
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

            // Return the list of VideoDto
            return Ok(videos);
        }


        [HttpGet("VideoCount/{username}")]
        public async Task<ActionResult<int>> GetTotalVideosByUser(string username)
        {
            RegisteredUser? user = await db.RegisteredUsers
                .Include(u => u.Videos) // Ensure that the Videos navigation property is included
                .FirstOrDefaultAsync(u => u.Username == username);

            // If the user is not found, return NotFound
            if (user == null)
            {
                return NotFound();
            }

            // Return the total number of videos for the user
            return Ok(user.Videos.Count);
        }
    }
}
