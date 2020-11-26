using API.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult GetFileFromPath(string path)
        {
            return PhysicalFile(path, MediaTypeNames.Text.Plain);
        }

        [HttpPost]
        public async Task<IActionResult> UploadFileAsync(IFormFile file)
        {
            var savedFilePath = await FileManager.SaveFileAsync(file, Environment.ContentRootPath);
            return StatusCode(201, savedFilePath);
        }
    }
}
