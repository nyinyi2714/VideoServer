using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoModel;
using VideoServer.DTO;

namespace VideoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(VideoGoldenContext db, UserManager<VideoUser> userManager) : ControllerBase
    {

        // GET: api/Users/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<VideoDto>>> GetVideosByUser(
            string userId, 
            int skip = 0, 
            int take = 10
        )
        {
            //VideoUser? user = await userManager.FindByIdAsync(userId);
            VideoUser? user = await db.Users.FindAsync( userId );

            if (user == null)
            {
                return NotFound();
            }

            List<Video> videoDtos = user.Videos
                .OrderByDescending(v => v.Timestamp)
                .Skip(skip)
                .Take(take)
                .ToList();

            return Ok(videoDtos);
        }

        [HttpGet("video-count/{userId}")]
        public async Task<ActionResult<int>> GetTotalVideosByUser(string userId)
        {
            // Retrieve the user from the database
            var user = await userManager.FindByIdAsync(userId);

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
