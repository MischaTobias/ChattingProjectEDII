using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

namespace ChattingDesign.Controllers
{
    public class LoginController : Controller
    {
        readonly string BaseUrl = "http://localhost:50489/api";

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
                var newUser = new User(collection["Username"], collection["Password"]);
                if (API.Models.User.CheckValidness(newUser))
                {
                    //Mandar a verificar los datos con la API
                    var users = GetUsers().Where(user => user.Username == newUser.Username && user.Password == newUser.Password).ToList();
                    if (users.Count() != 0)
                    {
                        newUser.IsAuthenticated = true;
                        HttpContext.User = new System.Security.Claims.ClaimsPrincipal(newUser);
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
                var newUser = new User(collection["Username"], collection["Password"]);
                //Mandar a registrar los datos con la API
                var users = GetUsers().Where(user => user.Username.Equals(newUser.Username));
                if (users.Count() == 0)
                {
                    using var client = new HttpClient
                    {
                        BaseAddress = new System.Uri(BaseUrl)
                    };
                    var relativeAddress = "api/User";
                    var jsonUser = JsonConvert.SerializeObject(newUser);
                    var response = await client.PostAsync(relativeAddress, new StringContent(jsonUser, Encoding.UTF8, "application/json"));
                }
                return View();
            }
            catch
            {
                return View();
            }
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
