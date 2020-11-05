using SecurityAndCompression.Incerfaces;
using System.Threading.Tasks;

namespace SecurityAndCompression.Compressors
{
    class LZW : ICompressor
    {
        public Task CompressFile(string savingPath, string filePath)
        {
            throw new System.NotImplementedException();
        }

        public Task DecompressFile(string savingPath, string filePath)
        {
            throw new System.NotImplementedException();
        }
    }
}
