using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Writers;
using NuGet.Packaging.Signing;
using System.Security.Claims;
using VideoModel;
using VideoServer.DTO;

namespace VideoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController(
        VideoGoldenContext db,
        UserManager<VideoUser> userManager
    ) : ControllerBase
    {
        // helper function to retreive user using userManager
        private async Task<UserDto?> GetUserDto(string userId)
        {
            VideoUser? user = await userManager.FindByIdAsync(userId);
            if(user == null) { return null; }

            string? UserName = await userManager.GetUserNameAsync(user);
            if(UserName == null) { return null; }

            var userDto = new UserDto
            {
                UserId = await userManager.GetUserIdAsync(user),
                UserName = UserName,
            };

            return userDto;
        }

        // GET: api/Videos/recent
        [HttpGet("recent")]
        public async Task<ActionResult<IEnumerable<VideoDto>>> GetRecentVideos()
        {
            // Materialize the query to retrieve the list of videos
            List<Video> recentVideos = await db.Videos
                .OrderByDescending(v => v.Timestamp)
                .Take(4)
                .ToListAsync();

            List<VideoDto> recentVideosDto = [];

            foreach (Video video in recentVideos)
            {
                // Retrieve the UserDto for the current video
                UserDto? userDto = await GetUserDto(video.UserId);

                if (userDto == null)
                {
                    return NotFound($"User not found for Video ID: {video.VideoId}");
                }

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

        // GET: api/Videos/popular
        [HttpGet("popular")]
        public async Task<ActionResult<IEnumerable<VideoDto>>> GetPopularVideos()
        {
            // Materialize the query to retrieve the list of videos
            var popularVideos = await db.Videos
                .OrderByDescending(v => v.Views)
                .Take(4)
                .ToListAsync();

            List<VideoDto> popularVideosDto = [];

            foreach (Video video in popularVideos)
            {
                // Retrieve the UserDto for the current video
                UserDto? userDto = await GetUserDto(video.UserId);

                if(userDto == null)
                {
                    return NotFound($"User not found for Video ID: {video.VideoId}");
                }

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

                popularVideosDto.Add(videoDto);
            }

            return Ok(popularVideosDto);
        }
        

        // GET: api/Video/{videoId}
        [HttpGet("{videoId}")]
        public async Task<ActionResult<VideoDto>> GetVideo(int videoId)
        {
            Video? video = await db.Videos.FindAsync(videoId);

            if (video == null)
            {
                return NotFound();
            }

            // Retrieve the UserDto for the current video
            UserDto? userDto = await GetUserDto(video.UserId);

            if (userDto == null)
            {
                return NotFound($"User not found for Video ID: {video.VideoId}");
            }

            VideoDto videoDto = new()
            {
                VideoId = video.VideoId,
                Url = video.Url,
                Timestamp = video.Timestamp,
                Description = video.Description,
                Title = video.Title,
                User = userDto,
            };

            return Ok(videoDto);
        }

        //[Authorize]
        [HttpPost("upload")]
        public async Task<ActionResult> UploadVideo(UploadVideoRequest uploadVideoRequest)
        {
            /*
            // Retrieve the token from the Authorization header
            string? authHeader = HttpContext.Request.Headers.Authorization;
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("No valid token provided.");
            }

            // Extract the token part (remove "Bearer " prefix)
            string token = authHeader["Bearer ".Length..].Trim();

            // Extract userId from the claims
            string? userIdClaim = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            if (userIdClaim == null)
            {
                return BadRequest("userId claim not found.");
            }
            */

            // Check if the user exists
            VideoUser? user = await userManager.FindByIdAsync(uploadVideoRequest.UserId);
            if (user == null)
            {
                return NotFound($"User with ID {uploadVideoRequest.UserId} not found.");
            }


            Video newVideo = new()
            {
                Url = uploadVideoRequest.Url,
                Title = uploadVideoRequest.Title,
                Description = uploadVideoRequest.Description,
                UserId = uploadVideoRequest.UserId,
                Timestamp = DateTime.Now,
                Views = 0,
            };

            db.Videos.Add(newVideo);
            await db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVideo), new { videoId = newVideo.VideoId }, "Upload Successful");
        }

    }
}
