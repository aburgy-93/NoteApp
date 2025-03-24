using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

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

        // TODO: ProjectId

        // TODO: Attribute Array

    }
}