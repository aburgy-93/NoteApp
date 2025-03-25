namespace Backend.DTOs

{
    public class LoginRequestDto
    {
        public required string Username {get; set;}
        public required string Password {get; set;}
        public DateTime LastLoginAt {get; set;}
    }
}