using NuGet.Packaging.Signing;

namespace VideoServer.DTO
{
    public class VideoDto
    {
        public required int VideoId { get; set; }
        public required string Url { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public int Views { get; set; }
        public required DateTime Timestamp { get; set; }
        public required string Username { get; set; }
    }
}
