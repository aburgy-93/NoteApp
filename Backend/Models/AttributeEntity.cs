using System.ComponentModel.DataAnnotations;

namespace Backend.Models

{
    public class AttributeEntity
    {
        [Key]
        public int AttributeId {get; set;}

        public required string AttributeName {get; set;}

         public ICollection<NoteAttributeJoin>? NoteAttributes { get; set; } = new List<NoteAttributeJoin>();
    }
}