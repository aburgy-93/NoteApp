using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Backend.Models

{
    public class Note
    {
        [Key]
        public int NoteId {get; set;}

        [Column(TypeName = "datetime")]
        public DateTime CreatedAt {get; set;}

        [Column(TypeName ="nvarchar(255)")]
        public string? NoteText {get; set;} 

        public int? ProjectId {get; set;}
        public Project? Project {get; set;}

        [JsonIgnore]
        public ICollection<NoteAttributeJoin> NoteAttributes { get; set; } = new List<NoteAttributeJoin>();
    }
}