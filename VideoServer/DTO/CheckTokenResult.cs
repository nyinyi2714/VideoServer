namespace VideoServer.DTO
{
    public class CheckTokenResult
    {
        public bool IsTokenValid { get; set; }
        public required string Username { get; set; }

    }
}
