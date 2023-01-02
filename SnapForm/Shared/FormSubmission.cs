using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SnapForm.Shared
{
    public class FormSubmission
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public UserBase User { get; set; }
        public EnterpriseForm Form { get; set; }
        public List<SnapFormField> Fields { get; set; }
        public List<SnapFormUserEntry> Entries { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}
