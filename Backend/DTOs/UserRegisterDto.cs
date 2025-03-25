namespace Backend.DTOs

{
    public class UserRegisterDto
    {
        public required string Username {get; set;}

        public required string PasswordHash {get; set;}

        public DateTime CreatedAt {get; set;}
    }
}