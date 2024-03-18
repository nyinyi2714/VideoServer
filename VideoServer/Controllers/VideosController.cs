using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VideoModel;
using VideoServer.DTO;

namespace VideoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController(VideoSourceContext context) : ControllerBase
    {

        // GET: api/Videos/recent
        [HttpGet("recent")]
        public async Task<ActionResult<IEnumerable<VideoDto>>> GetRecentVideos()
        {
            var recentVideos = await context.Videos
                .OrderByDescending(v => v.Timestamp)
                .Take(8)
                .Select(v => new VideoDto
                {
                    VideoId = v.VideoId,
                    Url = v.Url,
                    Title = v.Title,
                    Description = v.Description,
                    Likes = v.Likes,
                    Timestamp = v.Timestamp,
                    User = new UserDto
                    {
                        UserId = v.User.UserId,
                        Username = v.User.Username,
                    }
                })
                .ToListAsync();

            return recentVideos;
        }

        // GET: api/Videos/popular
        [HttpGet("popular")]
        public async Task<ActionResult<IEnumerable<VideoDto>>> GetPopularVideos()
        {
            var popularVideos = await context.Videos
                .OrderByDescending(v => v.Likes)
                .Take(8)
                .Select(v => new VideoDto
                {
                    VideoId = v.VideoId,
                    Url = v.Url,
                    Title = v.Title,
                    Description = v.Description,
                    Likes = v.Likes,
                    Timestamp = v.Timestamp,
                    User = new UserDto
                    {
                        UserId = v.User.UserId,
                        Username = v.User.Username,
                    }
                })
                .ToListAsync();

            return popularVideos;
        }

        // GET: api/Videos/by-user/{userId}
        [HttpGet("by-user/{userId}")]
        public async Task<ActionResult<IEnumerable<VideoDto>>> GetVideosByUser(int userId)
        {
            var user = await context.Users
                .Include(u => u.Videos)
                .FirstOrDefaultAsync(u => u.UserId == userId);

            if (user == null)
            {
                return NotFound();
            }

            var videoDtos = user.Videos.Select(v => new VideoDto
            {
                VideoId = v.VideoId,
                Url = v.Url,
                Title = v.Title,
                Description = v.Description,
                Likes = v.Likes,
                Timestamp = v.Timestamp,
                User = new UserDto
                {
                    UserId = user.UserId,
                    Username = user.Username,
                }
            }).ToList();

            return videoDtos;
        }


        // DELETE: api/Videos/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVideo(int id)
        {
            var video = await context.Videos.FindAsync(id);
            if (video == null)
            {
                return NotFound();
            }

            context.Videos.Remove(video);
            await context.SaveChangesAsync();

            return NoContent();
        }
    }
}
