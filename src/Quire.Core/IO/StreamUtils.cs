#nullable disable
using System.IO;
using Quire.Mirror;
using Quire.Services;

namespace Quire.IO;

public static partial class StreamUtils
{
    [MirrorVerified("Release IL")]
    public static void CopyStream(Stream source, Stream target)
    {
        byte[] buffer = new byte[4096];
        if (source.CanSeek)
        {
            try
            {
                source.Position = 0L;
            }
            catch (NotSupportedException)
            {
            }
        }
        int count;
        while ((count = source.Read(buffer, 0, 4096)) > 0)
        {
            target.Write(buffer, 0, count);
            target.Flush();
        }
    }

    [MirrorVerified("Release IL")]
    public static byte[] GetStreamHash(Stream stream)
    {
        stream.Position = 0L;
        return AppServiceRegistry.GetService<IHashAlgorithmProvider>().ComputeHash(stream);
    }
}
