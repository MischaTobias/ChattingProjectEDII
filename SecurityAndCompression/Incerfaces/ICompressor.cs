using System.Threading.Tasks;

namespace SecurityAndCompression.Incerfaces
{
    interface ICompressor
    {
        string CompressFile(string savingPath, string filePath, string name);
        string DecompressFile(string savingPath, string filePath);
    }
}
