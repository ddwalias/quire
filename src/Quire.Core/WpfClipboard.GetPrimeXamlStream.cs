#nullable disable
using Quire.Mirror;

namespace Quire;

public partial class WpfClipboard
{
    [MirrorVerified("Release IL")]
    public System.IO.Stream GetPrimeXamlStream()
    {
        object data = System.Windows.Clipboard.GetData(_primeXamlStreamCompressed);
        System.IO.Stream stream = data as System.IO.Stream;
        if (stream != null)
        {
            return stream;
        }
        byte[] bytes = data as byte[];
        if (bytes != null)
        {
            return new System.IO.MemoryStream(bytes);
        }
        return null;
    }
}
