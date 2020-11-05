using SecurityAndCompression.Incerfaces;
using System;

namespace SecurityAndCompression.Ciphers
{
    class SDES : IEncryptor
    {
        private string K1;
        private string K2;

        #region KeyGenerator
        private void GenerateKeys(string key)
        {

        }

        private string PTen(string key)
        {
            var result = string.Empty;
            for (int i = 0; i < key.Length - 1; i += 2)
            {
                result += key[i + 1] + key[i];
            }
            return result;
        }

        private string PEigth(string key)
        {
            var result = key[4].ToString() + key[7].ToString() + key[8].ToString() + key[5].ToString()
                + key[6].ToString() + key[1].ToString() + key[2].ToString() + key[0].ToString();
            return result;
        }

        private string PFour(string key)
        {
            var result = key[2].ToString() + key[0].ToString() + key[3].ToString() + key[1].ToString();
            return result;
        }

        private string ExpAndSwap(string key)
        {
            var result = key[1].ToString() + key[3].ToString() + key[2].ToString() + key[0].ToString() + key[0].ToString() +
                key[2].ToString() + key[3].ToString() + key[1].ToString();
            return result;
        }

        private string InitialPermutation(string key)
        {
            var result = key[4].ToString() + key[5].ToString() + key[7].ToString() + key[6].ToString() + key[3].ToString() +
                key[1].ToString() + key[0].ToString() + key[2].ToString();
            return result;
        }

        private string InverseInitialPermutation(string key)
        {
            var result = key[6].ToString() + key[5].ToString() + key[7].ToString() + key[4].ToString() + key[0].ToString() +
                key[1].ToString() + key[3].ToString() + key[2].ToString();
            return result;
        }

        private string LeftShift(string key)
        {
            var tempKey = key.Substring(0, 1);
            var result = key.Remove(0, 1);
            return result + tempKey;
        }

        private string Swap(string key)
        {
            var result = key.Substring(0, 4);
            return key.Remove(0, 4) + result;
        }

        private string SwapBoxLeft(string key)
        {
            var matrix = new string[4, 4] {
                { "01", "00", "11", "10" },
                { "11", "10", "01", "00" },
                { "00", "10", "01", "11" },
                { "11", "01", "11", "10" } 
            };
            var rows = Convert.ToInt32(key[0].ToString() + key[3].ToString(), 2);
            var columns = Convert.ToInt32(key[1].ToString() + key[2].ToString(), 2);
            return matrix[rows, columns];
        }

        private string SwapBoxRight(string key)
        {
            var matrix = new string[4, 4]{
                { "00", "01", "10", "11" },
                { "10", "00", "01", "11" },
                { "11", "00", "01", "00" },
                { "10", "01", "00", "11" }
            };
            var rows = Convert.ToInt32(key[0].ToString() + key[3].ToString(), 2);
            var columns = Convert.ToInt32(key[1].ToString() + key[2].ToString(), 2);
            return matrix[rows, columns];
        }
        #endregion

        public string EncryptFile(string savingPath, string completeFilePath, string key)
        {
            throw new NotImplementedException();
        }

        public string DecryptFile(string savingPath, string completeFilePath, string key)
        {
            throw new NotImplementedException();
        }
    }
}
