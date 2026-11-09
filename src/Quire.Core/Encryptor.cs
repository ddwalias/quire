#nullable disable
using System.Collections.Generic;
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

    [MirrorVerified("Release IL")]
    private ICryptoTransform GetDecryptorForVersion(Version formatVersion)
    {
        return _legacyDecryptorsDictionary?.FirstOrDefault((KeyValuePair<Version, ICryptoTransform> el) => formatVersion.CompareTo(el.Key) <= 0).Value;
    }

    [MirrorVerified("Release IL")]
    public Encryptor(global::Quire.License.ILicenseManager licenseManager)
    {
        _cryptoTransformer = new global::System.Security.Cryptography.AesCryptoServiceProvider().CreateEncryptor(licenseManager.GetEncryptionKey(global::Quire.License.EncryptorVersion.Current), licenseManager.GetEncryptionInitVector(global::Quire.License.EncryptorVersion.Current));
        DebugTools.DevAssert(_cryptoTransformer.CanReuseTransform, null);
        _legacyDecryptorsDictionary = new global::System.Collections.Generic.SortedDictionary<global::System.Version, ICryptoTransform>
        {
            {
                new global::System.Version(11, 0, 0, 0),
                new global::System.Security.Cryptography.TripleDESCryptoServiceProvider().CreateDecryptor(licenseManager.GetEncryptionKey(global::Quire.License.EncryptorVersion.PrePrime11), licenseManager.GetEncryptionInitVector(global::Quire.License.EncryptorVersion.PrePrime11))
            }
        };
        DebugTools.DevAssert(_legacyDecryptorsDictionary.First().Value.CanReuseTransform, null);
    }

    private SortedDictionary<Version, ICryptoTransform> _legacyDecryptorsDictionary;
}
