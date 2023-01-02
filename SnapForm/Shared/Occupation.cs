using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SnapForm.Shared
{
    public class Occupation
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Name { get; set; }
    }
}