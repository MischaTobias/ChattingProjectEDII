using System.Threading.Tasks;

namespace SecurityAndCompression.Incerfaces
{
    interface ICompressor
    {
        Task CompressFile(string savingPath, string filePath);
        Task DecompressFile(string savingPath, string filePath);
    }
}
