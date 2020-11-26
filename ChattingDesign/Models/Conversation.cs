using API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ChattingDesign.Models
{
    public class Conversation
    {
        public List<Message> Messages { get; set; }
        public string Receiver { get; set; }

        public Conversation(List<Message> messages, string receiver)
        {
            if (messages == null)
            {
                Messages = new List<Message>();
            }
            else
            {
                Messages = messages;
            }
            Receiver = receiver;
        }
    }
}
