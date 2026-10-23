#nullable disable
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Quire;

public partial class Encryptor
{
    internal static byte[] Encrypt(ICryptoTransform encryptor, string data)
    {
        if (data.IsNullOrEmpty<char>())
        {
            return null;
        }
        byte[] result;
        using (MemoryStream memoryStream = new MemoryStream())
        {
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
            {
                byte[] bytes = new UTF8Encoding().GetBytes(data);
                cryptoStream.Write(bytes, 0, bytes.Length);
                cryptoStream.FlushFinalBlock();
                result = memoryStream.ToArray();
            }
            return result;
        }
    }

    internal static string Decrypt(ICryptoTransform decryptor, byte[] encryptedData)
    {
        if (encryptedData.IsNullOrEmpty<byte>())
        {
            return null;
        }
        string result;
        using (MemoryStream memoryStream = new MemoryStream())
        {
            memoryStream.Write(encryptedData, 0, encryptedData.Length);
            memoryStream.Flush();
            memoryStream.Seek(0L, SeekOrigin.Begin);
            using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
            {
                using (StreamReader streamReader = new StreamReader(cryptoStream))
                {
                    result = streamReader.ReadToEnd();
                }
                return result;
            }
        }
    }
}
