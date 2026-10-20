#nullable disable
using System.IO;
using System.Text;
using Quire.Mirror;

namespace Quire.Xml;

public static partial class XmlHelper
{
    [MirrorVerified("Release IL")]
    public static StreamWriter CreateStreamWriterForXamlCopy(Stream stream, bool leaveOpen) => new StreamWriter(stream, Encoding.UTF8, 1024, leaveOpen);
}
