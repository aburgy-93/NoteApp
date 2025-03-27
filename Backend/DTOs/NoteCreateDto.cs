using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Backend.DTOs
{
    public class NoteCreateDto
    {
        public string? NoteText {get; set;}
        public int? ProjectId {get; set;}
        public List<int>? NoteAttributes {get; set;}
    }
}