#nullable disable
#pragma warning disable

namespace Quire;

partial interface IHashAlgorithmProvider
{
    byte[] ComputeHash(global::System.IO.Stream stream);
    byte[] ComputeHash(byte[] sourceBytes);
    global::System.Security.Cryptography.HashAlgorithm GetDisposableHash();
}
