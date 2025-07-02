namespace Fcg.Application.DTOs
{
    public class RegisterUserResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? UserId { get; set; }
    }
}