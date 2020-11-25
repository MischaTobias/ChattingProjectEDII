using API.Models;
using ChattingDesign.Helpers;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;

namespace ChattingDesign.Controllers
{
    public class ChattingController : Controller
    {
        readonly string BaseUrl = "http://localhost:50489/api";
        // GET: ChattingController
        public ActionResult Index()
        {
            var listOfUsers = GetUsers().Where(user => user.Username != Storage.Instance().CurrentUser.Username);
            return View(listOfUsers);
        }

        public ActionResult Chat(string userName)
        {
            var messages = GetMessages();
            return View();
        }

        private List<User> GetUsers()
        {
            try
            {
                var users = new List<User>();
                using var client = new HttpClient
                {
                    BaseAddress = new System.Uri(BaseUrl)
                };
                var relativeAddress = "api/User";
                var response = client.GetAsync(relativeAddress).Result;
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

        private List<Message> GetMessages()
        {
            try
            {
                var messages = new List<Message>();
                using var client = new HttpClient()
                {
                    BaseAddress = new System.Uri(BaseUrl)
                };
                var relativeAddress = "api/Chat";
                var response = client.GetAsync(relativeAddress).Result;
                if (response.IsSuccessStatusCode)
                {
                    return JsonConvert.DeserializeObject<List<Message>>(response.Content.ReadAsStringAsync().Result);
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
