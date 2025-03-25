using System.Text.Json.Serialization;

namespace Backend.Models

{
    public class NoteAttributeJoin
    {
        public int NoteId {get; set;}

        [JsonIgnore]
        public Note? Note {get; set;}

        public int AttributeId {get; set;}
         public AttributeEntity? Attribute { get; set; }
    }
}