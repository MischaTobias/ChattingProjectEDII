using SecurityAndCompression.Incerfaces;
using System;

namespace SecurityAndCompression.Ciphers
{
    public class SDES : IEncryptor
    {
        #region Variables
        private string K1;
        private string K2;
        private static int PrimeNumber = 1021;
        private static int GeneratorNumber = 503;
        #endregion

        #region KeyGenerator&Permutations
        public static int GetSecretKey(int userSecretRandom, int destinyPublicKey)
        {
            int secretKey = destinyPublicKey;
            for (int i = 0; i < userSecretRandom; i++)
            {
                secretKey *= destinyPublicKey;
                secretKey %= PrimeNumber;
            }
            return secretKey;
        }

        public static int GetPublicKey(int userSecretRandom)
        {
            int publicKey = GeneratorNumber;
            for (int i = 0; i < userSecretRandom; i++)
            {
                publicKey *= GeneratorNumber;
                publicKey %= PrimeNumber;
            }
            return publicKey;
        }

        private void GenerateKeys(string key)
        {
            while (key.Length < 10)
            {
                key = "0" + key;
            }
            var Left = LeftShift(key.Substring(0, key.Length / 2));
            var Right = LeftShift(key.Substring(key.Length / 2, key.Length / 2));
            K1 = PEigth(Left + Right);
            for (int i = 0; i < 2; i++)
            {
                Left = LeftShift(Left);
                Right = LeftShift(Right);
            }
            K2 = PEigth(Left + Right);
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
            while (key.Length < 8)
            {
                key = '0' + key;
            }
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

        private string XOR(string key1, string key2)
        {
            var result = string.Empty;
            for (int i = 0; i < key1.Length; i++)
            {
                if (key1[i] != key2[i] && (key1[i] == '1' || key2[i] == '1'))
                {
                    result += "1";
                }
                else
                {
                    result += "0";
                }
            }
            return result;
        }
        #endregion

        #region FileEncryption
        public string EncryptFile(string savingPath, string completeFilePath, string key)
        {
            throw new NotImplementedException();
        }

        public string DecryptFile(string savingPath, string completeFilePath, string key)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region StringEncryption
        public string EncryptString(string text, string key)
        {
            string result = string.Empty;
            GenerateKeys(key);
            foreach (var character in text)
            {
                var IPResult = InitialPermutation(Convert.ToString(character, 2));
                var Left = IPResult.Substring(0, IPResult.Length / 2);
                var Right = IPResult.Substring(IPResult.Length / 2, IPResult.Length / 2);
                var EPResult = ExpAndSwap(Right);
                var XORK = XOR(EPResult, K1);
                var SwapBoxResult = SwapBoxLeft(XORK.Substring(0, XORK.Length / 2)) + SwapBoxRight(XORK.Substring(XORK.Length / 2, XORK.Length / 2));
                var XORLeft = XOR(SwapBoxResult, Left);
                var SwapResult = Swap(XORLeft + Right);
                Left = SwapResult.Substring(0, SwapResult.Length / 2);
                Right = SwapResult.Substring(SwapResult.Length / 2, SwapResult.Length / 2);
                EPResult = ExpAndSwap(Right);
                XORK = XOR(EPResult, K2);
                SwapBoxResult = SwapBoxLeft(XORK.Substring(0, XORK.Length / 2)) + SwapBoxRight(XORK.Substring(XORK.Length / 2, XORK.Length / 2));
                XORLeft = XOR(SwapBoxResult, Left);
                result += (char)Convert.ToByte(InverseInitialPermutation(XORLeft + Right), 2);
            }
            return result;
        }

        public string DecryptString(string text, string key)
        {
            string result = string.Empty;
            GenerateKeys(key);
            foreach (var character in text)
            {
                var IPResult = InitialPermutation(Convert.ToString(character, 2));
                var Left = IPResult.Substring(0, IPResult.Length / 2);
                var Right = IPResult.Substring(IPResult.Length / 2, IPResult.Length / 2);
                var EPResult = ExpAndSwap(Right);
                var XORK = XOR(EPResult, K2);
                var SwapBoxResult = SwapBoxLeft(XORK.Substring(0, XORK.Length / 2)) + SwapBoxRight(XORK.Substring(XORK.Length / 2, XORK.Length / 2));
                var XORLeft = XOR(SwapBoxResult, Left);
                var SwapResult = Swap(XORLeft + Right);
                Left = SwapResult.Substring(0, SwapResult.Length / 2);
                Right = SwapResult.Substring(SwapResult.Length / 2, SwapResult.Length / 2);
                EPResult = ExpAndSwap(Right);
                XORK = XOR(EPResult, K1);
                SwapBoxResult = SwapBoxLeft(XORK.Substring(0, XORK.Length / 2)) + SwapBoxRight(XORK.Substring(XORK.Length / 2, XORK.Length / 2));
                XORLeft = XOR(SwapBoxResult, Left);
                result += (char)Convert.ToByte(InverseInitialPermutation(XORLeft + Right), 2);
            }
            return result;
        }
        #endregion
    }
}
