using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.OpenApi.Writers;
using NuGet.Packaging.Signing;
using System.IdentityModel.Tokens.Jwt;
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

        // GET: api/Video/{videoId}
        [HttpGet("{videoId}")]
        public async Task<ActionResult<VideoDto>> GetVideo(int videoId)
        {
            Video? video = await db.Videos.FindAsync(videoId);

            if (video == null)
            {
                return NotFound();
            }

            VideoDto videoDto = new()
            {
                VideoId = video.VideoId,
                Url = video.Url,
                Timestamp = video.Timestamp,
                Description = video.Description,
                Title = video.Title,
                Username = video.Username,
            };

            return Ok(videoDto);
        }

        [Authorize]
        [HttpPost("Upload")]
        public async Task<ActionResult> UploadVideo(UploadVideoRequest uploadVideoRequest)
        {
            // Get the Authorization header
            string? authHeader = HttpContext.Request.Headers.Authorization;
            if (string.IsNullOrEmpty(authHeader) || !authHeader.StartsWith("Bearer "))
            {
                return Unauthorized("Authorization header missing or invalid.");
            }

            // Extract the JWT token
            string tokenString = authHeader["Bearer ".Length..].Trim();
            var tokenHandler = new JwtSecurityTokenHandler();
            JwtSecurityToken token = tokenHandler.ReadJwtToken(tokenString);


            // Find the claim that represents the user ID
            string? userName = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (userName == null)
            {
                return BadRequest("UserName not found in token.");
            }

            // Check if the user exists
            VideoUser? identityUser = await userManager.FindByNameAsync(userName);
            if (identityUser == null)
            {
                return NotFound($"User with Username '{userName}' not found.");
            }

            Video newVideo = new()
            {
                Url = uploadVideoRequest.Url,
                Title = uploadVideoRequest.Title,
                Description = uploadVideoRequest.Description,
                Timestamp = DateTime.Now,
                Views = 0,
                Username = userName,
            };

            db.Videos.Add(newVideo);
            await db.SaveChangesAsync();

            return CreatedAtAction(nameof(GetVideo), new { videoId = newVideo.VideoId }, "Upload Successful");
        }

        // GET: api/Videos/recent
        [HttpGet("Recent")]
        public async Task<ActionResult<IEnumerable<VideoDto>>> GetRecentVideos()
        {
            // Materialize the query to retrieve the list of videos
            List<VideoDto> recentVideos = await db.Videos
                .OrderByDescending(v => v.Timestamp)
                .Take(4)
                .Select(v => new VideoDto
                {
                    VideoId = v.VideoId,
                    Url = v.Url,
                    Title = v.Title,
                    Description = v.Description,
                    Timestamp = DateTime.Now,
                    Views = v.Views,
                    Username = v.Username,
                })
                .ToListAsync();

            return Ok(recentVideos);
        }

        /*
        // GET: api/Videos/popular
        [HttpGet("Popular")]
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
        */
    }
}
