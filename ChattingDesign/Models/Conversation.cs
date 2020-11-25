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
    }
}
