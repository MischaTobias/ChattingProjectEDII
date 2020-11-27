using API.Models;
using ChattingDesign.Helpers;
using ChattingDesign.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SecurityAndCompression.Ciphers;
using SecurityAndCompression.Compressors;
using System;
using System.Collections.Generic;
using System.IO;
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
            var messages = GetMessages(HttpContext.Session.GetString("CurrentUser"), receiver, false);
            var files = GetMessages(HttpContext.Session.GetString("CurrentUser"), receiver, true);
            var conversation = new Conversation(messages, files, receiver);
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
                var messageForUpload = new Message() { 
                    Receiver = receiver, 
                    IsFile = false, 
                    Sender = currentUser, 
                    Text = cipheredMessage
                };
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
            var searchedMessages = GetMessages(HttpContext.Session.GetString("CurrentUser"), receiver, false);
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
        public async Task<ActionResult> UploadFileAsync(IFormFile file)
        {
            try
            {
                if (file == null)
                {
                    return RedirectToAction("Chat");
                }
                var currentUser = HttpContext.Session.GetString("CurrentUser");
                var receiver = HttpContext.Session.GetString("CurrentReceiver");
                var savedFileRoute = await FileManager.SaveFileAsync(file, Storage.Instance().EnvironmentPath, false);
                var compressor = new LZW();
                //var compressedFilePath = compressor.CompressFile(Storage.Instance().EnvironmentPath, savedFileRoute, Path.GetFileNameWithoutExtension(savedFileRoute));
                var fileStream = System.IO.File.OpenRead(savedFileRoute);
                var multiForm = new MultipartFormDataContent
                {
                    { new StreamContent(fileStream), "file", Path.GetFileName(savedFileRoute) }
                };
                var response = await Storage.Instance().APIClient.PostAsync("File", multiForm);
                var fileNameInAPI = await response.Content.ReadAsStringAsync();
                fileNameInAPI = fileNameInAPI.Remove(0, 1);
                fileNameInAPI = fileNameInAPI.Remove(fileNameInAPI.Length - 1, 1);
                //var SDESKey = SDES.GetSecretKey(GetUserSecretNumber(currentUser), GetUserPublicKey(receiver));
                //var cipher = new SDES();
                //var cipheredMessage = cipher.EncryptString(fileNameInAPI, Convert.ToString(SDESKey, 2));
                var pathMessage = new Message() { Text = fileNameInAPI, IsFile = true, Sender = currentUser, Receiver = receiver };
                await Storage.Instance().APIClient.PostAsJsonAsync("Chat", pathMessage);
                return RedirectToAction("Chat");
            }
            catch
            {
                return RedirectToAction("Chat");
            }
        }

        [Route("DownloadFile")]
        public async Task<ActionResult> DownloadFileAsync(string message)
        {
            try
            {
                var newMessage = new Message() { Text = message };
                var result = await Storage.Instance().APIClient.PostAsJsonAsync("File/GetFile", newMessage);
                var fileForDownloading = await result.Content.ReadAsStreamAsync();
                var route = await FileManager.SaveDownloadedStream(fileForDownloading, Storage.Instance().EnvironmentPath, newMessage.Text);
                route = Path.GetFullPath(route);
                var fileArray = System.IO.File.ReadAllBytes(route);
                return File(fileArray, "text/plain", message);
            }
            catch
            {
                return RedirectToAction("Chat");
            }
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

        private List<Message> GetMessages(string currentUser, string receiver, bool isFile)
        {
            try
            {
                var messages = new List<Message>();
                var response = Storage.Instance().APIClient.GetAsync("Chat").Result;
                if (response.IsSuccessStatusCode)
                {
                    var messageList = JsonConvert.DeserializeObject<List<Message>>(response.Content.ReadAsStringAsync().Result);
                    var conversationMessages = new List<Message>();
                    if (isFile)
                    {
                        conversationMessages = messageList.Where(m => ((m.Sender == currentUser && m.Receiver == receiver) || (m.Sender == receiver && m.Receiver == currentUser)) && m.IsFile).ToList();
                    }
                    else
                    {
                        conversationMessages = messageList.Where(m => (m.Sender == currentUser && m.Receiver == receiver) || (m.Sender == receiver && m.Receiver == currentUser) && !m.IsFile).ToList();
                    }

                    if (conversationMessages.Count != 0 && !isFile)
                    {
                        var SDESKey = SDES.GetSecretKey(GetUserSecretNumber(currentUser), GetUserPublicKey(receiver));
                        var cipher = new SDES();
                        foreach (var message in conversationMessages)
                        {
                            message.Text = cipher.DecryptString(message.Text, Convert.ToString(SDESKey, 2));
                        }
                        return conversationMessages;
                    }
                    else if (conversationMessages.Count != 0)
                    {
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
