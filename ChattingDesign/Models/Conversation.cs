﻿using API.Models;
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
        public List<Message> Files { get; set; }

        public string SearchedMessage { get; set; }

        public Conversation(List<Message> messages, List<Message> files, string receiver)
        {
            if (messages == null)
            {
                Messages = new List<Message>();
            }
            else
            {
                Messages = messages;
            }
            if (files == null)
            {
                Files = new List<Message>();
            }
            else
            {
                Files = files;
            }
            Receiver = receiver;
        }

        public Conversation(List<Message> messages, string receiver, string searched)
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
            SearchedMessage = searched;
        }
    }
}
