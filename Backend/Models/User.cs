using System.ComponentModel.DataAnnotations;

namespace Backend.Models

{
    public class User
    {
        [Key]
        public int UserId {get; set;}

        public required string Username {get; set;}

        public required string PasswordHash {get; set;}

        public DateTime CreatedAt {get; set;}

        public DateTime LastLoginAt {get; set;}
    }
}