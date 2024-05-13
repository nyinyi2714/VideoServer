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
    public class VideosController(VideoGoldenContext db) : ControllerBase
    {

        // GET: api/Video/{videoId}
        [HttpGet("{videoId}")]
        public async Task<ActionResult<VideoDto>> GetVideo(int videoId)
        {
            try
            {
                // Retrieve the video from the database
                Video? video = await db.Videos.FindAsync(videoId);

                // Check if the video exists
                if (video == null)
                {
                    return NotFound();
                }

                // Update the view count
                video.Views += 1;

                // Save changes to the database
                await db.SaveChangesAsync();

                // Create a VideoDto object
                VideoDto videoDto = new VideoDto
                {
                    VideoId = video.VideoId,
                    Url = video.Url,
                    Timestamp = video.Timestamp,
                    Description = video.Description,
                    Views = video.Views,
                    Title = video.Title,
                    Username = video.Username
                };

                // Return the VideoDto
                return Ok(videoDto);
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error: {ex.Message}");
            }
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
            string? username = token.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;
            if (username == null)
            {
                return BadRequest("UserName not found in token.");
            }

            RegisteredUser? user = await db.RegisteredUsers
            .Include(u => u.Videos)
                .FirstOrDefaultAsync(u => u.Username == username);

            // Check if the user exists
            if (user == null)
            {
                return NotFound($"User with username {username} not found.");
            }

            Video newVideo = new()
            {
                Url = uploadVideoRequest.Url,
                Title = uploadVideoRequest.Title,
                Description = uploadVideoRequest.Description,
                Timestamp = DateTime.Now,
                Views = 0,
                Username = username,
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
                    Timestamp = v.Timestamp,
                    Views = v.Views,
                    Username = v.Username,
                })
                .ToListAsync();

            return Ok(recentVideos);
        }

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
                    Username = video.Username,
                };

                popularVideosDto.Add(videoDto);
            }

            return Ok(popularVideosDto);
        }
    }
}
