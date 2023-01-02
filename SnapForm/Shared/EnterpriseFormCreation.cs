using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SnapForm.Shared
{
    public class EnterpriseFormCreation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        [Required, StringLength(35, ErrorMessage = "Name is too long, max 35 characters.")]
        public string Name { get; set; }
        [Required, StringLength(280, ErrorMessage = "Description is too long, max 35 characters.")]
        public string Description { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> FormInputIds { get; set; }
    }
}
