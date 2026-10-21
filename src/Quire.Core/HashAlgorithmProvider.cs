#nullable disable
using System.IO;
using System.Security.Cryptography;
using System.Security.Cryptography;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire;

public partial class HashAlgorithmProvider
{
    private static HashAlgorithm _hashAlgorithm;

    private static HashAlgorithm Sha256Service
    {
        get
        {
            if (_hashAlgorithm != null)
            {
                _hashAlgorithm.Initialize();
            }
            else
            {
                _hashAlgorithm = CreateHash();
            }
            return _hashAlgorithm;
        }
    }

    private static HashAlgorithm CreateHash()
    {
        try
        {
            return SHA256.Create();
        }
        catch (Exception)
        {
            DebugTools.FailAssertion(null, Array.Empty<object>());
            return new SHA256Managed();
        }
    }

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

    [MirrorVerified("Release IL")]
    public void Dispose()
    {
        if (_hashAlgorithm == null)
        {
            return;
        }
        _hashAlgorithm.Dispose();
        _hashAlgorithm = null;
    }

}
