using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using SecurityAndCompression.Ciphers;
using System;
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

        public int SecretNumber { get; set; }

        public int PublicKey { get; set; }

        public string AuthenticationType { get; set; }

        public bool IsAuthenticated { get; set; }

        public User(string name, string password)
        {
            Username = name;
            Name = name;
            SecretNumber = 0;
            while (SecretNumber < 20)
            {
                SecretNumber = new Random().Next(0, 502);
            }
            PublicKey = SDES.GetPublicKey(SecretNumber);
            var cipher = new Cesar();
            Password = cipher.EncryptString(password, "pass");
        }

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
