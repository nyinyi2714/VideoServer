using Firebase.Auth;
using Firebase.Storage;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Storage.v1.Data;
using Google.Apis.Upload;
using Google.Cloud.Storage.V1;
using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Identity.Client.Extensions.Msal;
using Microsoft.OpenApi.Writers;
using NuGet.Packaging.Signing;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using VideoModel;
using VideoServer.DTO;

namespace VideoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VideosController : ControllerBase
    {
        private static readonly string ApiKey = "AIzaSyDwFJ1ZdiJ3wPcpqJJNsMbJg9JM3r3Nm0Q"; // Replace with your Firebase project ID
        private static readonly string BucketName = "video-upload-e0f8d.appspot.com"; // Replace with your Firebase Storage bucket name
        private static readonly string AuthEmail = "videoserver@gmail.com"; // Replace with your service account email
        private static readonly string AuthPassword = "nyinyi2714";

        private readonly VideoGoldenContext _db;
        private readonly FirebaseAuthProvider _authProvider;
        private readonly FirebaseStorage _storage;
        public VideosController(VideoGoldenContext db)
        {
            _db = db;
            _authProvider = new FirebaseAuthProvider(new FirebaseConfig(ApiKey));
            _storage = new FirebaseStorage(
                BucketName,
                new FirebaseStorageOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(_authProvider.SignInWithEmailAndPasswordAsync(AuthEmail, AuthPassword).Result.FirebaseToken)
                });
        }

       // [Authorize]
        [RequestFormLimits(ValueLengthLimit = int.MaxValue, MultipartBodyLengthLimit = int.MaxValue)]
        [DisableRequestSizeLimit]
        [Consumes("multipart/form-data")]
        [HttpPost("UploadVideo")]
        public async Task<IActionResult> UploadVideo(
            IFormFile videoFile, 
            string username, 
            string title, 
            string description
        )
        {
            try
            {
                using Stream stream = videoFile.OpenReadStream();
                string uniqueFilename = GenerateUniqueFilename(videoFile.FileName);

                // you can use CancellationTokenSource to cancel the upload midway
                CancellationTokenSource cancellation = new();

                // Create Firebase storage reference
                var storageReference = _storage.Child("upload").Child(uniqueFilename);

                // Upload the video stream with cancellation support
                string downloadUrl = await storageReference.PutAsync(stream, cancellation.Token);

                // Find the user in the database
                RegisteredUser? user = await _db.RegisteredUsers
                    .Include(u => u.Videos)
                    .FirstOrDefaultAsync(u => u.Username == username);

                // Check if the user exists
                if (user == null)
                {
                    return NotFound($"User with username {username} not found.");
                }

                // Store the new video in the database
                var newVideo = new Video
                {
                    Url = downloadUrl,
                    Filename = uniqueFilename,
                    Title = title,
                    Description = description,
                    Timestamp = DateTime.Now,
                    Views = 0,
                    Username = username,
                };

                _db.Videos.Add(newVideo);
                await _db.SaveChangesAsync();

                return Ok("Upload successful");

            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Upload failed: {ex.Message}");
            }
        }

        [HttpDelete("DeleteVideo/{videoId}")]
        public async Task<IActionResult> DeleteVideo(int videoId)
        {
            try
            {
                // Find the video in the database
                var video = await _db.Videos.FindAsync(videoId);

                if (video == null)
                {
                    return NotFound($"Video with ID {videoId} not found.");
                }

                // Delete the video from Firebase Storage
                var storageReference = _storage.Child("upload").Child(video.Filename);
                await storageReference.DeleteAsync();

                // Remove the video from the database
                _db.Videos.Remove(video);
                await _db.SaveChangesAsync();

                return Ok("Video deleted successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Delete failed: {ex.Message}");
            }
        }

        // GET: api/Video/{videoId}
        [HttpGet("{videoId}")]
        public async Task<ActionResult<VideoDto>> GetVideo(int videoId)
        {
            try
            {
                // Retrieve the video from the database
                Video? video = await _db.Videos.FindAsync(videoId);

                // Check if the video exists
                if (video == null)
                {
                    return NotFound();
                }

                // Update the view count
                video.Views += 1;

                // Save changes to the database
                await _db.SaveChangesAsync();

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

        [HttpGet("Test")]
        public async Task<ActionResult<string>> Test()
        {
            return Ok("server running");
        }

        // GET: api/Videos/recent
        [HttpGet("Recent")]
        public async Task<ActionResult<IEnumerable<VideoDto>>> GetRecentVideos()
        {
            // Materialize the query to retrieve the list of videos
            List<VideoDto> recentVideos = await _db.Videos
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
            var popularVideos = await _db.Videos
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

        static string GenerateUniqueFilename(string originalFilename)
        {
            // Get current timestamp in a specific format (adjust if needed)
            var timestamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssfff");

            // Combine timestamp with original extension for a unique name
            return $"{timestamp}-{originalFilename}";
        }
    }
}
