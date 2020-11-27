using API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Net.Mime;
using System.Threading.Tasks;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FileController : Controller
    {
        private IWebHostEnvironment Environment;

        public FileController(IWebHostEnvironment env)
        {
            Environment = env;
        }

        [HttpPost]
        [Route("GetFile")]
        public IActionResult GetFileFromPath(Message pathMessage)
        {
            if (pathMessage.Text == null)
            {
                return StatusCode(500);
            }
            var physicalPath = $"{Environment.ContentRootPath}/Uploads/{pathMessage.Text}";
            var fileStream = new FileStream(physicalPath, FileMode.Open);
            return File(fileStream, "text/html");
        }

        [HttpPost]
        public async Task<IActionResult> UploadFileAsync(IFormFile file)
        {
            if (file == null)
            {
                return StatusCode(500);
            }
            var savedFilePath = await FileManager.SaveFileAsync(file, Environment.ContentRootPath, true);
            return StatusCode(201, savedFilePath);
        }
    }
}
