using API.Models;
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
            var listOfUsers = GetUsers().Where(user => user.Username != HttpContext.User.Identity.Name);
            return View(listOfUsers);
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
                    var list = JsonConvert.DeserializeObject<List<User>>(response.Content.ReadAsStringAsync().Result);
                    return list;
                }
                return new List<User>();
            }
            catch
            {
                return new List<User>();
            }
        }
    }
}
