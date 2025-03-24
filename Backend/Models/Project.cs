using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Backend.Models
{
    public class Project
    {
        [Key]
        public int ProjectId {get; set;}

        [Column(TypeName ="nvarchar(25)")]
        public required string ProjectName {get; set;}

        // TODO: Array of notes
    }
}