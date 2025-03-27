using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend.Models

{
    public class Project
    {
        [Key]
        public int ProjectId {get; set;}

        [Column(TypeName ="nvarchar(25)")]
        [Required]
        public required string ProjectName {get; set;}
        
        public ICollection<Note> Notes {get; set;} = new List<Note>();
    }
}