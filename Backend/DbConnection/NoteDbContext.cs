using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Models
{
    public class NoteDbContext : DbContext
    {
        public NoteDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Note> Notes {get; set;} = null!;

        public DbSet<Project> Projects {get; set;} = null!;
        
        public DbSet<User> Users {get; set;} = null!;
    }
}