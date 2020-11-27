using System;
using System.Collections.Generic;
using System.Text;

namespace SecurityAndCompression.Incerfaces
{
    interface IEncryptor
    {
        string EncryptFile(string savingPath, string completeFilePath, string key);
        string DecryptFile(string savingPath, string completeFilePath, string key);
        string EncryptString(string text, string key);
        string DecryptString(string text, string key);
    }
}
