using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Text;
using ChattingDesign.Helpers;
using System;

namespace ChattingDesign.Controllers
{
    public class LoginController : Controller
    {
        public LoginController()
        {
            Storage.Instance().EnvironmentPath = Environment.CurrentDirectory;
            Storage.Instance().APIClient = new HttpClient() { BaseAddress = new Uri("http://localhost:50489/api/") };
            Storage.Instance().APIClient.DefaultRequestHeaders.Clear();
            Storage.Instance().APIClient.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
        }

        public ActionResult Login()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(IFormCollection collection)
        {
            try
            {
                var newUser = new User(collection["username"], collection["password"]);
                if (API.Models.User.CheckValidness(newUser))
                {
                    //Mandar a verificar los datos con la API
                    var users = GetUsers().Result.Where(user => user.Username == newUser.Username && user.Password == newUser.Password).ToList();
                    if (users.Count() != 0)
                    {
                        HttpContext.Session.SetString("CurrentUser", newUser.Username);
                        return RedirectToAction("Index", "Chatting");
                    }
                    else
                    {
                        // Mostrar error
                        return View();
                    }
                }
                else
                {
                    // Mostrar error
                    return View();
                }
            }
            catch
            {
                return View();
            }
        }

        [HttpPost]
        public async Task<ActionResult> Register(IFormCollection collection)
        {
            try
            {
                var one = collection["Password"];
                var two = collection["confirmPassword"];
                if (collection["password"] != collection["confirmPassword"])
                {
                    //Mostrar error
                    return View();
                }
                var newUser = new User(collection["Username"], collection["Password"]);
                //Mandar a registrar los datos con la API
                var users = GetUsers().Result.Where(user => user.Username == newUser.Username);
                if (users.Count() == 0)
                {
                    await Storage.Instance().APIClient.PostAsJsonAsync("User", newUser);
                }
                return RedirectToAction("Login");
            }
            catch
            {
                return View();
            }
        }

        private async Task<List<User>> GetUsers()
        {
            try
            {
                var users = new List<User>();
                var response = await Storage.Instance().APIClient.GetAsync("User");
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
    }
}
