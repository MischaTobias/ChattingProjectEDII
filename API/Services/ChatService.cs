using API.Models;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services
{
    public class ChatService
    {
        private readonly IMongoCollection<Message> _chat;

        public ChatService(IChatDatabaseSettings settings)
        {
            var client = new MongoClient(settings.ConnectionString);
            var database = client.GetDatabase(settings.DatabaseName);

            _chat = database.GetCollection<Message>(settings.ChatCollectionName);
        }

        public List<Message> Get() => _chat.Find(message => true).ToList();

        public Message Get(string id) => _chat.Find(message => message.Id == id).FirstOrDefault();

        public Message Create(Message message)
        {
            _chat.InsertOne(message);
            return message;
        }

        public void Update(string id, Message messageIn) => _chat.ReplaceOne(message => message.Id == id, messageIn);

        public void Remove(Message messageIn) => _chat.DeleteOne(message => message.Id == messageIn.Id);

        public void Remove(string id) => _chat.DeleteOne(message => message.Id == id);
    }
}
