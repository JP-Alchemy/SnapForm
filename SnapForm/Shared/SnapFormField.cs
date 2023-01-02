using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SnapForm.Shared
{
    public class SnapFormField
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Category { get; set; }
        [Required, StringLength(35, ErrorMessage = "Title is too long, max 35 characters.")]
        public string Title { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public string InputType { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        [Required]
        public bool IsRequired { get; set; }
        public List<string> Options { get; set; }
        public bool SelectMultiple { get; set; }
        public bool UseDiagram { get; set; }
        public bool UseDateRange { get; set; }
        public string DiagramImage { get; set; }
        public string SearchType { get; set; }
    }
}
