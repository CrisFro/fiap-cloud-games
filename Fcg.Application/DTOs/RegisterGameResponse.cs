namespace Fcg.Application.DTOs
{
    public class RegisterGameResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public Guid? GameId { get; set; }
    }
}