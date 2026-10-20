#nullable disable
using System.IO;
using Quire.Mirror;
using Quire.Services;

namespace Quire.IO;

public static partial class StreamUtils
{
    [MirrorVerified("Release IL")]
    public static byte[] GetStreamHash(Stream stream)
    {
        stream.Position = 0L;
        return AppServiceRegistry.GetService<IHashAlgorithmProvider>().ComputeHash(stream);
    }
}
