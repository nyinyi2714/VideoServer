using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoModel;
using VideoServer.DTO;

namespace VideoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(VideoGoldenContext context) : ControllerBase
    {
        /*
        // GET: api/Users/{userId}
        [HttpGet("{userId}")]
        public async Task<ActionResult<IEnumerable<VideoDto>>> GetVideosByUser(int userId, int skip = 0, int take = 10)
        {
            var user = await context.Users
                .Include(u => u.Videos)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound();
            }

            var videoDtos = user.Videos
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
                User = new UserDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                }
            }).ToList();

            return videoDtos;
        }

        [HttpGet("video-count/{userId}")]
        public async Task<ActionResult<int>> GetTotalVideosByUser(int userId)
        {
            // Retrieve the user from the database
            var user = await context.Users
                .Include(u => u.Videos)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            // If the user is not found, return NotFound
            if (user == null)
            {
                return NotFound();
            }

            // Return the total number of videos for the user
            return user.Videos.Count;
        }
        */
    }
}
