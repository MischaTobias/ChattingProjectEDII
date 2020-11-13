using Microsoft.AspNetCore.Mvc;

namespace ChattingDesign.Controllers
{
    public class ChattingController : Controller
    {
        readonly string BaseUrl = "ngrok.io/...";
        // GET: ChattingController
        public ActionResult Index()
        {
            return View();
        }
    }
}
