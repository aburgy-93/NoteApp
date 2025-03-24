using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    public class UserRegisterDto
    {
        public required string Username {get; set;}

        public required string PasswordHash {get; set;}

        public DateTime CreatedAt {get; set;}
    }
}