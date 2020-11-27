using SecurityAndCompression.Incerfaces;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using System.Threading.Tasks;

namespace SecurityAndCompression.Compressors
{
    public class LZW : ICompressor
    {
        #region Variables
        Dictionary<string, int> LZWTable = new Dictionary<string, int>();
        Dictionary<int, List<byte>> DecompressLZWTable = new Dictionary<int, List<byte>>();
        List<byte> Differentchar = new List<byte>();
        List<byte> Characters = new List<byte>();
        List<int> NumbersToWrite = new List<int>();
        List<List<byte>> DecompressValues = new List<List<byte>>();
        int MaxValueLength = 0;
        int code = 1;
        string leftoverbits = "";


        private void ResetVariables()
        {
            LZWTable.Clear();
            DecompressLZWTable.Clear();
            Differentchar.Clear();
            Characters.Clear();
            NumbersToWrite.Clear();
            DecompressValues.Clear();
            leftoverbits = string.Empty;
            MaxValueLength = 0;
            code = 1;
        }
        #endregion

        #region Compression
        private void FillDictionary(byte[] Text)
        {
            //First reading through the text
            foreach (var character in Text)
            {
                if (!LZWTable.ContainsKey(character.ToString()))
                {
                    LZWTable.Add(character.ToString(), code);
                    code++;
                    Differentchar.Add(character);
                }
            }
        }

        private void AddValueToDictionary(string key)
        {
            if (!LZWTable.ContainsKey(key))
            {
                LZWTable.Add(key, code);
                code++;
            }
        }

        public string CompressFile(string path, string filePath, string fileName)
        {
            var compressedFilePath = $"{path}/Compressions/{fileName}";
            if (System.IO.File.Exists((compressedFilePath)))
            {
                System.IO.File.Delete((compressedFilePath));
            }
            ResetVariables();

            using var readingFile = new FileStream(filePath, FileMode.Open);

            using var reader = new BinaryReader(readingFile);
            int bufferSize = 2000;
            var buffer = new byte[bufferSize];

            readingFile.Position = readingFile.Seek(0, SeekOrigin.Begin);
            while (readingFile.Position != readingFile.Length)
            {
                buffer = reader.ReadBytes(bufferSize);
                FillDictionary(buffer);
            }

            readingFile.Position = readingFile.Seek(0, SeekOrigin.Begin);
            while (readingFile.Position != readingFile.Length)
            {
                buffer = reader.ReadBytes(bufferSize);
                Characters = buffer.ToList();
                MaxValueLength = 0;
                while (Characters.Count != 0)
                {
                    int codeLength = 0;
                    string Subchain = Characters[codeLength].ToString();
                    codeLength++;
                    while (Subchain.Length != 0)
                    {
                        if (Characters.Count > codeLength)
                        {
                            if (!LZWTable.ContainsKey(Subchain + Characters[codeLength].ToString()))
                            {
                                NumbersToWrite.Add(LZWTable[Subchain]);
                                Subchain += Characters[codeLength].ToString();
                                AddValueToDictionary(Subchain);
                                Subchain = string.Empty;
                                for (int i = 0; i < codeLength; i++)
                                {
                                    Characters.RemoveAt(0);
                                }
                            }
                            else
                            {
                                Subchain += Characters[codeLength].ToString();
                                codeLength++;
                            }
                        }
                        else
                        {
                            if (readingFile.Position != readingFile.Length)
                            {
                                buffer = reader.ReadBytes(bufferSize);
                                List<byte> aux = buffer.ToList();
                                while (Characters.Count > 0)
                                {
                                    aux.Insert(0, Characters[0]);
                                    Characters.RemoveAt(0);
                                }
                                Characters = aux;
                                MaxValueLength = 0;
                            }
                            else
                            {
                                NumbersToWrite.Add(LZWTable[Subchain]);
                                AddValueToDictionary(Subchain);
                                for (int i = 0; i < codeLength; i++)
                                {
                                    Characters.RemoveAt(0);
                                }
                                Subchain = string.Empty;
                            }

                        }
                    }
                }
            }
            reader.Close();
            readingFile.Close();

            MaxValueLength = Convert.ToString(NumbersToWrite.Max(), 2).Length;

            if (!Directory.Exists($"{path}/Compressions"))
            {
                Directory.CreateDirectory($"{path}/Compressions");
            }
            using var fileToWrite = new FileStream($"{path}/Compressions/{fileName}", FileMode.OpenOrCreate);
            using var writer = new BinaryWriter(fileToWrite);
            string compressionCode = "";
            
            int byteDC = Convert.ToInt32(Differentchar.Count());
            int byteCount = 1;
            while (byteDC >= 256) 
            {
                byteDC = byteDC - 255;
                byteCount++;
            }
            writer.Write(Convert.ToByte(byteCount));
            for (int i = 1; i < byteCount; i++)
            {
                writer.Write(Convert.ToByte(255));
            }
            writer.Write(Convert.ToByte(byteDC));
            writer.Write(Convert.ToByte(MaxValueLength));
            foreach (var item in Differentchar)
            {
                writer.Write(item);
            }
            string code = "";
            foreach (var number in NumbersToWrite)
            {
                compressionCode = Convert.ToString(number, 2);
                while (compressionCode.Length < MaxValueLength)
                {
                    compressionCode = "0" + compressionCode;
                }
                code += compressionCode;
                while (code.Length >= 8)
                {
                    writer.Write(Convert.ToByte(code.Substring(0, 8), 2));
                    code = code.Remove(0, 8);
                }
            }
            if (code.Length != 0)
            {
                while (code.Length != 8)
                {
                    code += "0";
                }
                writer.Write(Convert.ToByte(code, 2));
                code = string.Empty;
            }
            writer.Close();
            fileToWrite.Close();
            ResetVariables();
            return compressedFilePath;
        }
        #endregion

        #region Decompression
        private long FillDecompressionDictionary(byte[] text)
        {
            int diffChar = 0;
            for (int i = 0; i < text[0]; i++)
            {
                diffChar += text[i + 1];                
            }
            MaxValueLength = text[1 + text[0]];
            for (int i = 0; i < diffChar; i++)
            {
                DecompressLZWTable.Add(code, new List<byte> { text[2 + text[0] + i]});
                code++;
            }
            return 2 + text[0] + diffChar;
        }

        private bool CompareListofBytes(List<byte> list1, List<byte> list2)
        {
            for (int i = 0; i < list1.Count; i++)
            {
                if (list1[i] != list2[i])
                {
                    return false;
                }
            }
            return true;
        }
        private List<byte> SetValuesForDecompress(List<byte> values)
        {
            List<byte> newList = new List<byte>();
            foreach (var value in values)
            {
                newList.Add(value);
            }
            return newList;
        }

        private bool CheckIfExists(List<byte> actualString)
        {
            foreach (var item in DecompressLZWTable.Values)
            {
                if (actualString.Count == item.Count)
                {
                    if (CompareListofBytes(actualString, item))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public string DecompressFile(string savingPath, string filePath, string fileName)
        {
            var decompressedFilePath = $"{savingPath}/Decompressions/{fileName}";

            if (System.IO.File.Exists((decompressedFilePath)))
            {
                System.IO.File.Delete((decompressedFilePath));
            }

            if (System.IO.File.Exists($"{savingPath}/Decompressions/"))
            {
                System.IO.File.Delete($"{savingPath}/Decompressions/");
            }
            ResetVariables();
            
            using var readingFile = new FileStream(filePath, FileMode.Open);

            using var reader = new BinaryReader(readingFile);
            int bufferSize = 2000;
            var buffer = new byte[bufferSize];
            readingFile.Position = readingFile.Seek(0, SeekOrigin.Begin);
            buffer = reader.ReadBytes(bufferSize);

            readingFile.Position = FillDecompressionDictionary(buffer);

            
            List<int> Codes = new List<int>();
            string binaryNum = "";
            DecompressValues.Add(new List<byte>());
            DecompressValues.Add(new List<byte>());
            DecompressValues.Add(new List<byte>());
            while (readingFile.Position != readingFile.Length)
            {
                buffer = reader.ReadBytes(bufferSize);
                foreach (var item in buffer)
                {
                    string subinaryNum = Convert.ToString(item, 2);
                    while (subinaryNum.Length < 8)
                    {
                        subinaryNum = "0" + subinaryNum;
                    }
                    binaryNum += subinaryNum;
                    while (binaryNum.Length >= MaxValueLength)
                    {
                        var index = Convert.ToInt32(binaryNum.Substring(0, MaxValueLength), 2);
                        binaryNum = binaryNum.Remove(0, MaxValueLength);
                        if (index != 0)
                        {
                            Codes.Add(index);
                            DecompressValues[0] = DecompressValues[1];
                            if (DecompressLZWTable.ContainsKey(index))
                            {
                                DecompressValues[1] = SetValuesForDecompress(DecompressLZWTable[index]);
                                DecompressValues[2].Clear();
                                foreach (var value in DecompressValues[0])
                                {
                                    DecompressValues[2].Add(value);
                                }
                                DecompressValues[2].Add(DecompressValues[1][0]);
                            }
                            else
                            {
                                DecompressValues[1] = DecompressValues[0];
                                DecompressValues[2].Clear();
                                foreach (var value in DecompressValues[0])
                                {
                                    DecompressValues[2].Add(value);
                                }
                                DecompressValues[2].Add(DecompressValues[1][0]);
                                DecompressValues[1] = SetValuesForDecompress(DecompressValues[2]);
                            }
                            if (!CheckIfExists(DecompressValues[2]))
                            {
                                DecompressLZWTable.Add(code, new List<byte>(DecompressValues[2]));
                                code++;
                            }
                        }
                    }
                }
            }
            reader.Close();
            readingFile.Close();

            if (!Directory.Exists($"{savingPath}/Decompressions"))
            {
                Directory.CreateDirectory($"{savingPath}/Decompressions");
            }
            using var fileToWrite = new FileStream($"{savingPath}/Decompressions/{fileName}", FileMode.OpenOrCreate);
            using var writer = new BinaryWriter(fileToWrite);
            foreach (var index in Codes)
            {
                foreach (var value in DecompressLZWTable[index])
                {
                    writer.Write(value);
                }
            }
            writer.Close();
            fileToWrite.Close();
            return decompressedFilePath;
        }
        #endregion
    }
}
