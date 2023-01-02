using System;
using System.Collections.Generic;
using System.Text;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace SnapForm.Shared
{
    public class SnapFormUserEntry
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string UserId { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string InputId { get; set; }
        public string InputType { get; set; }
        public string Input { get; set; }
        public List<SnapFormAlternativeEntry> AlternativeEntries { get; set; }
        public DateTime DateCreated { get; set; } = DateTime.Now;
        public DateTime DateUpdated { get; set; } = DateTime.Now;
        public Type Type() => InputTypes.GetType(InputType);
    }

    public class SnapFormAlternativeEntry
    {
        public string Input { get; set; }
        [BsonRepresentation(BsonType.ObjectId)]
        public string PresetId { get; set; }
    }

    public class SnapFormLocationEntry
    {
        public int Lat { get; set; }
        public int Lng { get; set; }
        public Address Address { get; set; }
    }

    public class Address
    {
        public string StreetAddress { get; set; }
        public string Suburb { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public string Country { get; set; }
    }
}