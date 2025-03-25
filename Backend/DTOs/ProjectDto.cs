namespace Backend.DTOs
{
    public class ProjectDto
    {
        public int ProjectId {get; set;}
        public required string ProjectName {get; set;}
        public List<NoteDto>? Notes {get; set;}
    }
}