#nullable disable
using System.Security.Cryptography;
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
}
