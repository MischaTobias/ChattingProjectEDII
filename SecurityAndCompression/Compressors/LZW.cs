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

        public string CompressFile(string path, string filePath, string name)
        {
            var compressedFilePath = $"{path}/Compressions/{name}.lzw";
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
            using var fileToWrite = new FileStream($"{path}/Compressions/{name}.lzw", FileMode.OpenOrCreate);
            using var writer = new BinaryWriter(fileToWrite);
            string compressionCode = "";
            writer.Write(Convert.ToByte(MaxValueLength));
            writer.Write(Convert.ToByte(Differentchar.Count()));
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
        public string DecompressFile(string savingPath, string filePath)
        {
            return string.Empty;
        }
        #endregion
    }
}
