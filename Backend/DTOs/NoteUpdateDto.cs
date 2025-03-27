namespace Backend.DTOs

{
    public class NoteUpdateDto
    {
        public string? NoteText {get; set;}
        public int? ProjectId {get; set;}
        public List<int>? NoteAttributes {get; set;}
    }
}