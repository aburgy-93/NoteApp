using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Backend.Models;
using Backend.DTOs;
using Backend.Services;

namespace Backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly NoteDbContext _context;
        private readonly PasswordService _passwordService;

        private readonly AuthService _authService;

        public UserController(NoteDbContext context, 
            PasswordService passwordService, 
            AuthService authService)
        {
            _context = context;
            _passwordService = passwordService;
            _authService = authService;
        }

        // GET: api/User
        /*
            Get the users.
            Not currently being used, but could be implemented for an Admin console.
        */
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> GetUsers()
        {
            return await _context.Users.ToListAsync();
        }

        // GET: api/User/5
        /*
            Get the user by id.
            Not currently being used, but could be implemented for an Admin console.
        */
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
        /*
            Update the user based on id.
            Not currently being used, but could be implemented for an Admin console.
            TODO: fix being able to update the lastLoginAt
        */
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
        /*
            Create a new user. 
        */
        [HttpPost("register")]
        public async Task<ActionResult<User>> RegisterUser(UserRegisterDto userRegisterDto)
        {
           // Check if the username already exists
           if(_context.Users.Any(user => user.Username == userRegisterDto.Username)) {
                return BadRequest("Username already exists.");
           }

           // Hash the password
           string hashedPassword = _passwordService
            .HashPassword(userRegisterDto.PasswordHash);

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
        /*
            Log the user in by checking the username and password. 
        */
        public async Task<ActionResult<User>> LoginUser([FromBody] 
            LoginRequestDto loginRequestDto) 
        {
            // Find the user in the database 
            var user = await _context.Users
                .FirstOrDefaultAsync(user => user.Username == loginRequestDto.Username);

            if (user == null)
            {
                return Unauthorized("Invalid username or password");
            }

            // Verify the password
            bool isPasswordValid = _passwordService
                .VerifyPassword(loginRequestDto.Password, user.PasswordHash);

            if(!isPasswordValid)
            {
                 return Unauthorized("Invalid username or password.");
            }

            // Generate JWT Token
            string token = _authService.GenerateJwtToken(user.Username);
            return Ok(new {Token = token});
        }

        // DELETE: api/User/5
        /*
            Delete the user based on the passed in id.
            Not currently being used, but could be implemented for an Admin console.
        */
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
