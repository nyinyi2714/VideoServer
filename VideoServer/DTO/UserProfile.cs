namespace VideoServer.DTO
{
    public class UserProfile
    {
        public required string Username { get; set; }
        public int TotalVideos { get; set; }
        public required List<VideoDto> Videos { get; set; }
    }
}
