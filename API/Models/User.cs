using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Security.Principal;

namespace API.Models
{
    public class User : IIdentity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }

        public string Name { get; set; }

        public string AuthenticationType { get; set; }

        public bool IsAuthenticated { get; set; }

        public static bool CheckValidness(User user)
        {
            if (user.Username == "" || user.Password == "")
            {
                return false;
            }
            return true;
        }
    }
}
