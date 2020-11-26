using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace API.Models
{
    public class FileManager
    {
        public static async Task<string> SaveFileAsync(IFormFile file, string path)
        {
            if (Directory.Exists($"{path}/Uploads"))
            {
                if (File.Exists($"{path}/Uploads/{file.FileName}"))
                {
                    File.Delete($"{path}/Uploads/{file.FileName}");
                }
            }
            else
            {
                Directory.CreateDirectory($"{path}/Uploads");
            }
            using var saver = new FileStream($"{path}/Uploads/{file.FileName}", FileMode.OpenOrCreate);
            await file.CopyToAsync(saver);
            saver.Close();
            return $"{path}/Uploads/{file.FileName}";
        }
    }
}
