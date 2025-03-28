using Backend.Models;

namespace Backend.DTOs

{
    public class NoteDto
    {
        public int NoteId {get; set;}
        public DateTime CreatedAt {get; set;}
        public string? NoteText {get; set;}
        public int? ProjectId {get; set;}

        // Unsure if this is required, but will keep in case I may need it.
        // public List<int>? NoteAttributes {get; set;}
        public List<string>? NoteAttributeNames {get; set;} = new List<string>();
    }
}