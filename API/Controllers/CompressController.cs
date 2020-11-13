using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class CompressController : Controller
    {
        // GET: CompressController
        public ActionResult Index()
        {
            return View();
        }

        // GET: CompressController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CompressController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: CompressController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CompressController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: CompressController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: CompressController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: CompressController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
