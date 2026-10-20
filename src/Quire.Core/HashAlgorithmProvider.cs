#nullable disable
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire;

public partial class HashAlgorithmProvider
{
    [MirrorVerified("PtcHashAlgorithmProviderMechanicalDifferentialTests")]
    public HashAlgorithm GetDisposableHash() => CreateHash();

    [MirrorVerified("PtcHashAlgorithmProviderMechanicalDifferentialTests")]
    public HashAlgorithmProvider() { }

    [MirrorVerified("PtcHashAlgorithmProviderMechanicalDifferentialTests")]
    public bool IsConnected { get; set; }

    [MirrorVerified("Release IL")]
    public byte[] ComputeHash(Stream stream) => Sha256Service.ComputeHash(stream);

    [MirrorVerified("Release IL")]
    public byte[] ComputeHash(byte[] sourceBytes) => Sha256Service.ComputeHash(sourceBytes);
}
