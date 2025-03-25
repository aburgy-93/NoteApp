namespace Backend.DTOs

{
    public class NoteDto
    {
        public int NoteId {get; set;}
        public DateTime CreatedAt {get; set;}
        public string? NoteText {get; set;}

        public List<AttributeDto>? NoteAttributes {get; set;}
    }
}