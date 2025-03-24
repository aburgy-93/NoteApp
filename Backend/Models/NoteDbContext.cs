using Microsoft.EntityFrameworkCore;

namespace Backend.Models
{
    public class NoteDbContext : DbContext
    {
        public NoteDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Note> Notes {get; set;} = null!;
    }
}