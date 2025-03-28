using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace Backend.Models
{
    public class NoteDbContext : DbContext
    {
        public NoteDbContext(DbContextOptions options) : base(options)
        {
        }

        // Defining the databse tables for the application.
        public DbSet<Note> Notes {get; set;} = null!;

        public DbSet<Project> Projects {get; set;} = null!;
        
        public DbSet<User> Users {get; set;} = null!;

        public DbSet<AttributeEntity> Attributes {get; set;} = null!;

        public DbSet<NoteAttributeJoin> NoteAttributes {get; set;} = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            
           // Unique Constraints for User, Project, and Attribute entities.
           modelBuilder.Entity<User>()
            .HasIndex(user => user.Username)
            .IsUnique();

            modelBuilder.Entity<Project>()
                .HasIndex(project => project.ProjectName)
                .IsUnique();

            modelBuilder.Entity<AttributeEntity>()
                .HasIndex(attr => attr.AttributeName)
                .IsUnique();

            // One to Many Relationship (Project --> Notes). 
            // If a project is deleted, its associated notes will remain in the 
            // database, but their ProjectId will be set to null.
            modelBuilder.Entity<Note>()
                .HasOne(note => note.Project)
                .WithMany(project => project.Notes)
                .HasForeignKey(note => note.ProjectId)
                .OnDelete(DeleteBehavior.SetNull);
            
            // Many to Many Relationship (Notes <--> Attributes)
            // The join table ensures that each note can have multiple attributes,
            // and each attribute can be assigned to multiple notes. 

            // Define composite key for NoteAttributeJoin (ensuring unique note-attribute pairs).
            modelBuilder.Entity<NoteAttributeJoin>()
                .HasKey(noteAttr => new {noteAttr.NoteId, noteAttr.AttributeId});

            // Define the relationship of the join table for notes and attributes.
            // If a note is deleted, its related NoteAttributeJoin entries are also deleted.
            modelBuilder.Entity<NoteAttributeJoin>()
                .HasOne(noteAttr => noteAttr.Note)
                .WithMany(note => note.NoteAttributes)
                .HasForeignKey(noteAttr => noteAttr.NoteId)
                .OnDelete(DeleteBehavior.Cascade);
            
            // Define the relationship between the join table and the attribute table. 
            modelBuilder.Entity<NoteAttributeJoin>()
                .HasOne(noteAttr => noteAttr.Attribute)
                .WithMany(attr => attr.NoteAttributes)
                .HasForeignKey(noteAttr => noteAttr.AttributeId);
        }   
    }
}