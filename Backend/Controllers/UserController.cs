using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly NoteDbContext _context;
        private readonly PasswordService _passwordService;

        private readonly AuthService _authService;

        public UserController(NoteDbContext context, PasswordService passwordService, AuthService authService)
        {
            _context = context;
            _passwordService = passwordService;
            _authService = authService;
        }

        // GET: api/User
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/User/5
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUser(int id)
        {
            var user = await _context.Users.FindAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        // PUT: api/User/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutUser(int id, User user)
        {
            if (id != user.UserId)
            {
                return BadRequest();
            }

            _context.Entry(user).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/User
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser(UserRegisterDto userRegisterDto)
        {
           // Check if the username already exists
           if(_context.Users.Any(user => user.Username == userRegisterDto.Username)) {
                return BadRequest("Username already exists.");
           }

           // Hash the password
           string hashedPassword = _passwordService.HashPassword(userRegisterDto.PasswordHash);

           var user = new User 
           {
            Username = userRegisterDto.Username,
            PasswordHash = hashedPassword,
            CreatedAt = userRegisterDto.CreatedAt
           };

           _context.Users.Add(user);
           await _context.SaveChangesAsync();

           return CreatedAtAction(nameof(GetUser), new {id = user.UserId}, user);
        }

        [HttpPost("login")]
        public async Task<ActionResult<User>> LoginUser([FromBody] LoginRequestDto loginRequestDto) 
        {
            // Find the user in the database 
            var user = await _context.Users.FirstOrDefaultAsync(user => user.Username == loginRequestDto.Username);
            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            // Verify the password
            bool isPasswordValid = _passwordService.VerifyPassword(loginRequestDto.Password, user.PasswordHash);
            if(!isPasswordValid)
            {
                 return Unauthorized("Invalid username or password.");
            }

            // Generate JWT Token
            string token = _authService.GenerateJwtToken(user.Username);
            return Ok(new {Token = token});
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool UserExists(int id)
        {
            return _context.Users.Any(e => e.UserId == id);
        }
    }
}
