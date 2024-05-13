namespace VideoServer.DTO
{
    public class RegisterResult
    {
        public bool Success { get; set; }
        public required string Username { get; set; }

        public required string Token { get; set; }
    }
}
