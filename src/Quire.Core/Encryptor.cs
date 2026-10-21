#nullable disable
using System.Security.Cryptography;
using System.Linq;
using Quire.Mirror;

namespace Quire;

public partial class Encryptor
{
    [MirrorVerified("PtcEncryptorMechanicalDifferentialTests")]
    private readonly ICryptoTransform _cryptoTransformer;

    [MirrorVerified("PtcEncryptorMechanicalDifferentialTests")]
    public byte[] Encrypt(string data) => Encrypt(_cryptoTransformer, data);

    [MirrorVerified("PtcEncryptorMechanicalDifferentialTests")]
    public bool IsConnected { get; set; }

    [MirrorVerified("Release IL")]
    public bool MatchesEncrypted(byte[] encryptedData, string data) => encryptedData.SequenceEqual(Encrypt(data));

    [MirrorVerified("Release IL")]
    public bool ShouldUpdateEncryption(Version formatVersion)
    {
        Version key = _legacyDecryptorsDictionary.LastOrDefault().Key;
        return formatVersion.CompareTo(key) <= 0;
    }

    [MirrorVerified("Release IL")]
    public byte[] UpdateEncryption(byte[] encryptedData, Version formatVersion)
    {
        ICryptoTransform decryptorForVersion = GetDecryptorForVersion(formatVersion);
        if (decryptorForVersion == null)
        {
            return encryptedData;
        }
        return Encrypt(Decrypt(decryptorForVersion, encryptedData));
    }
}
