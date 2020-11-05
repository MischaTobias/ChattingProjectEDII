using ChattingDesign.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ChattingDesign.Controllers
{
    public class ChattingController : Controller
    {
        // GET: ChattingController
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(IFormCollection collection)
        {
            try
            {
                var newUser = new User() { Username = collection["Username"] };
                newUser.SetPassword(collection["Password"]);
                return View();
            }
            catch
            {
                return View();
            }
        }
    }
}
