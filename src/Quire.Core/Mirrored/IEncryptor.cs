#nullable disable
#pragma warning disable

namespace Quire;

partial interface IEncryptor
{
    byte[] Encrypt(string data);
    bool MatchesEncrypted(byte[] encryptedData, string data);
    bool ShouldUpdateEncryption(global::System.Version formatVersion);
    byte[] UpdateEncryption(byte[] encryptedData, global::System.Version formatVersion);
}
