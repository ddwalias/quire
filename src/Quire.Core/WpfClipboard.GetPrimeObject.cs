#nullable disable
using Quire.Mirror;
using Quire.Undo;

namespace Quire;

public partial class WpfClipboard
{
    [MirrorVerified("Release IL")]
    public object GetPrimeObject()
    {
        if (!System.Windows.Clipboard.ContainsData(_primeXamlStreamCompressed))
        {
            return null;
        }
        System.IO.Stream stream = GetPrimeXamlStream();
        if (DebugTools.AssertNotNull(stream, null).Failed)
        {
            return null;
        }
        object result;
        using (CommandScope commandScope = new CommandScope(new CompoundCommand()))
        {
            using (System.IO.Compression.DeflateStream deflateStream = new System.IO.Compression.DeflateStream(stream, System.IO.Compression.CompressionMode.Decompress))
            {
                try
                {
                    stream.Position = 0L;
                    System.Windows.Markup.ParserContext parserContext = new System.Windows.Markup.ParserContext();
                    parserContext.XmlSpace = "preserve";
                    result = System.Windows.Markup.XamlReader.Load(deflateStream, parserContext);
                }
                catch (System.Exception exception)
                {
                    result = null;
                    DebugTools.FailAssertionDoNotThrow(exception);
                }
            }
        }
        return result;
    }
}
