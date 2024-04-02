using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
                .Take(4)
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
                .Take(4)
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

        // GET: api/Video/{videoId}
        [HttpGet("{videoId}")]
        public async Task<ActionResult<VideoDto>> GetVideo(int videoId)
        {
            var video = await context.Videos
                .Where(v => v.VideoId == videoId)
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
                .FirstOrDefaultAsync();

            if (video == null)
            {
                return NotFound();
            }

            return video;
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
