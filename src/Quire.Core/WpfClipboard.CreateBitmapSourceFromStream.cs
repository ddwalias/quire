#nullable disable
using Quire.Mirror;
using Quire.Wpf;

namespace Quire;

public partial class WpfClipboard
{
    [MirrorVerified("Release IL")]
    private static System.Windows.Media.Imaging.BitmapSource CreateBitmapSourceFromStream(System.IO.Stream stream)
    {
        if (stream == null)
        {
            return null;
        }
        string path = System.IO.Path.GetTempFileName();
        using (System.IO.FileStream fileStream = System.IO.File.Open(path, System.IO.FileMode.Create))
        {
            CreateBitmapFromDib(stream, fileStream);
        }
        return new System.Windows.Media.Imaging.BitmapImage(new System.Uri(path, (System.UriKind)1));
    }
}
