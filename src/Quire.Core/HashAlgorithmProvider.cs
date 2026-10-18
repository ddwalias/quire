#nullable disable
using System.Security.Cryptography;
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
}
