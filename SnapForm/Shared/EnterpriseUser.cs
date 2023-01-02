using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SnapForm.Shared
{
    public class EnterpriseUser : UserSecure
    {
        [BsonRepresentation(BsonType.ObjectId)]
        public string EnterpriseId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Role Role { get; set; } = Role.User;
    }

    public enum Role
    {
        User,
        Admin
    }
}
