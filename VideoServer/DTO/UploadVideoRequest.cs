namespace VideoServer.DTO
{
    public class UploadVideoRequest
    {
        public required string Url { get; set; }
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string UserId { get; set; }
    }
}
