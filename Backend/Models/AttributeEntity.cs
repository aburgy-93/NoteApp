using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Backend.Models

{
    public class AttributeEntity
    {
        [Key]
        public int AttributeId {get; set;}

        public required string AttributeName {get; set;}

        [JsonIgnore]
         public ICollection<NoteAttributeJoin> NoteAttributes { get; set; } = new List<NoteAttributeJoin>();
    }
}