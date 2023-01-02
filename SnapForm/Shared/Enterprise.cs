using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SnapForm.Shared
{
    public class Enterprise
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string ID { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string Created_by { get; set; }
        public string Industry { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public string Name { get; set; }
    }
}
