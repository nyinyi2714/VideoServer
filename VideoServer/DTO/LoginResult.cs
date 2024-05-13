namespace VideoServer.DTO
{
    public class LoginResult
    {
        public bool Success { get; set; }
        public required string Username { get; set; }
        public required string Token { get; set; }
    }
}
