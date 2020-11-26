using API.Models;
using ChattingDesign.Helpers;
using ChattingDesign.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SecurityAndCompression.Ciphers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChattingDesign.Controllers
{
    public class ChattingController : Controller
    {
        // GET: ChattingController
        public ActionResult Index()
        {
            var listOfUsers = GetUsers().Where(user => user.Username != HttpContext.Session.GetString("CurrentUser"));
            return View(listOfUsers);
        }

        public ActionResult Chat(string receiver)
        {
            if (receiver != null)
            {
                HttpContext.Session.SetString("CurrentReceiver", receiver);
            }
            else
            {
                receiver = HttpContext.Session.GetString("CurrentReceiver");
            }
            var conversation = new Conversation(GetMessages(HttpContext.Session.GetString("CurrentUser"), receiver), receiver);
            return View(conversation);
        }

        [HttpPost]
        public async Task<ActionResult> Chat(IFormCollection collection)
        {
            try
            {
                var message = collection["Message"];
                if (message == string.Empty)
                {
                    return RedirectToAction("Chat");
                }
                var currentUser = HttpContext.Session.GetString("CurrentUser");
                var receiver = HttpContext.Session.GetString("CurrentReceiver");
                var SDESKey = SDES.GetSecretKey(GetUserSecretNumber(currentUser), GetUserPublicKey(receiver));
                var cipher = new SDES();
                var cipheredMessage = cipher.EncryptString(message, Convert.ToString(SDESKey, 2));
                var messageForUpload = new Message() { Receiver = receiver, Sender = currentUser, Text = cipheredMessage };
                await Storage.Instance().APIClient.PostAsJsonAsync("Chat", messageForUpload);
                return RedirectToAction("Chat");
            }
            catch
            {
                return RedirectToAction("Chat");
            }
        }

        public ActionResult SearchMessages(string receiver, string searchedValue)
        {
            if (receiver != null)
            {
                HttpContext.Session.SetString("CurrentReceiver", receiver);
            }
            else
            {
                receiver = HttpContext.Session.GetString("CurrentReceiver");
            }
            var searchedMessages = GetMessages(HttpContext.Session.GetString("CurrentUser"), receiver);
            searchedMessages = searchedMessages.Where(message => message.Text.Contains(searchedValue)).ToList();
            var conversation = new Conversation(searchedMessages, receiver, searchedValue);
            return View(conversation);
        }

        [HttpPost]
        public ActionResult SearchMessages(IFormCollection collection)
        {
            try
            {
                var searched = collection["SearchedValue"];
                if (searched == string.Empty)
                {
                    return RedirectToAction("Chat");
                }
                var receiverUser = HttpContext.Session.GetString("CurrentReceiver");
                return RedirectToAction("SearchMessages", new { receiver = receiverUser, searchedValue = searched });
            }
            catch
            {
                return RedirectToAction("Chat");
            }
        }

        [HttpPost]
        public ActionResult UploadFile(IFormFile file)
        {
            var uploadedFile = file;
            return RedirectToAction("Chat");
        }

        private List<User> GetUsers()
        {
            try
            {
                var users = new List<User>();
                var response = Storage.Instance().APIClient.GetAsync("User").Result;
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<List<User>>(response.Content.ReadAsStringAsync().Result);
                }
                return new List<User>();
            }
            catch
            {
                return new List<User>();
            }
        }

        private int GetUserPublicKey(string username)
        {
            try
            {
                var user = GetUsers().FirstOrDefault(user => user.Username == username);
                return user.PublicKey;
            }
            catch 
            {
                return 0;
            }
        }

        private int GetUserSecretNumber(string username)
        {
            try
            {
                var user = GetUsers().FirstOrDefault(user => user.Username == username);
                return user.SecretNumber;
            }
            catch
            {
                return 0;
            }
        }

        private List<Message> GetMessages(string currentUser, string receiver)
        {
            try
            {
                var messages = new List<Message>();
                var response = Storage.Instance().APIClient.GetAsync("Chat").Result;
                if (response.IsSuccessStatusCode)
                {
                    var messageList = JsonConvert.DeserializeObject<List<Message>>(response.Content.ReadAsStringAsync().Result);
                    var conversationMessages = messageList.Where(m => (m.Sender == currentUser && m.Receiver == receiver) || (m.Sender == receiver && m.Receiver == currentUser)).ToList();
                    if (conversationMessages.Count != 0)
                    {
                        var SDESKey = SDES.GetSecretKey(GetUserSecretNumber(currentUser), GetUserPublicKey(receiver));
                        var cipher = new SDES();
                        foreach (var message in conversationMessages)
                        {
                            message.Text = cipher.DecryptString(message.Text, Convert.ToString(SDESKey, 2));
                        }
                        return conversationMessages;
                    }
                    else
                    {
                        return new List<Message>();
                    }
                }
                return new List<Message>();
            }
            catch
            {
                return new List<Message>();
            }
        }
    }
}
