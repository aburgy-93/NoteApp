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

        public DbSet<AttributeEntity> Attributes {get; set;} = null!;

        public DbSet<NoteAttributeJoin> NoteAttributes {get; set;} = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
           // Unique Constraints 
           modelBuilder.Entity<User>()
            .HasIndex(user => user.Username)
            .IsUnique();

            modelBuilder.Entity<Project>()
                .HasIndex(project => project.ProjectName)
                .IsUnique();

            modelBuilder.Entity<AttributeEntity>()
                .HasIndex(attr => attr.AttributeName)
                .IsUnique();

            // One to Many Relationship (Project --> Notes)
            modelBuilder.Entity<Note>()
                .HasOne(note => note.Project)
                .WithMany(project => project.Notes)
                .HasForeignKey(note => note.ProjectId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Many to Many Relationship (Notes --> Attributes)
            // Composite key
            modelBuilder.Entity<NoteAttributeJoin>()
                .HasKey(noteAttr => new {noteAttr.NoteId, noteAttr.AttributeId});

            modelBuilder.Entity<NoteAttributeJoin>()
                .HasOne(noteAttr => noteAttr.Note)
                .WithMany(note => note.NoteAttributes)
                .HasForeignKey(noteAttr => noteAttr.NoteId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<NoteAttributeJoin>()
                .HasOne(noteAttr => noteAttr.Attribute)
                .WithMany(attr => attr.NoteAttributes)
                .HasForeignKey(noteAttr => noteAttr.AttributeId);
        }   
    }
}