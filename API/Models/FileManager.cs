using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading.Tasks;

namespace API.Models
{
    public class FileManager
    {
        public static async Task<string> SaveFileAsync(IFormFile file, string path, bool getFileName)
        {
            var name = Path.GetFileNameWithoutExtension(file.FileName);
            var originalName = Path.GetFileNameWithoutExtension(file.FileName);
            var ext = Path.GetExtension(file.FileName);
            var cont = 1;
            if (Directory.Exists($"{path}/Uploads"))
            {
                while (File.Exists($"{path}/Uploads/{name}{ext}"))
                {
                    name = originalName;
                    name += $" ({cont})";
                    cont++;
                }
            }
            else
            {
                Directory.CreateDirectory($"{path}/Uploads");
            }
            using var saver = new FileStream($"{path}/Uploads/{name}{ext}", FileMode.OpenOrCreate);
            await file.CopyToAsync(saver);
            saver.Close();
            if (getFileName)
            {
                return $"{name}{ext}";
            }
            return $"{path}/Uploads/{name}{ext}";

        }
    }
}
