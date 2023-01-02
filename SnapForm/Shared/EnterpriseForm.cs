using System;
using System.Collections.Generic;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SnapForm.Shared
{
    public class EnterpriseForm
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public List<string> FormInputIds{ get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string EnterpriseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        [BsonRepresentation(BsonType.ObjectId)]
        public string Created_by { get; set; }
        public long SubmissionCount { get; set; }
        public List<SnapFormField> Fields { get; set; } = new List<SnapFormField>();
        public DateTime DateUpdated { get; set; } = DateTime.Now;
    }
}
