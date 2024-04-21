using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VideoModel;
using VideoServer.DTO;

namespace VideoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController(
        VideoGoldenContext context,
        UserManager<VideoUser> userManager
    ) : ControllerBase
    {
        // helper function to retreive user using userManager
        private async Task<UserDto> GetUserDto(string userId)
        {
            VideoUser? user = await userManager.FindByIdAsync(userId);
            var userDto = new UserDto
            {
                UserId = await userManager.GetUserIdAsync(user),
                UserName = await userManager.GetUserNameAsync(user)
            };

            return userDto;
        }

        // GET: api/Videos/recent
        [HttpGet("recent")]
        public async Task<ActionResult<IEnumerable<VideoDto>>> GetRecentVideos()
        {
            // Materialize the query to retrieve the list of videos
            List<Video> recentVideos = await context.Videos
                .OrderByDescending(v => v.Timestamp)
                .Take(4)
                .ToListAsync();

            List<VideoDto> recentVideosDto = new List<VideoDto>();

            foreach (var video in recentVideos)
            {
                // Retrieve the UserDto for the current video
                UserDto userDto = await GetUserDto(video.UserId);

                // Construct the VideoDto object and assign the UserDto
                var videoDto = new VideoDto
                {
                    VideoId = video.VideoId,
                    Url = video.Url,
                    Title = video.Title,
                    Description = video.Description,
                    Views = video.Views,
                    Timestamp = video.Timestamp,
                    User = userDto
                };

                recentVideosDto.Add(videoDto);
            }


            return recentVideosDto;
        }

        /*

        // GET: api/Videos/popular
        [HttpGet("popular")]
        public async Task<ActionResult<IEnumerable<VideoDto>>> GetPopularVideos()
        {
            var popularVideos = await context.Videos
                .OrderByDescending(v => v.Views)
                .Take(4)
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
                    Views = v.Views,
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

        */
    }
}
