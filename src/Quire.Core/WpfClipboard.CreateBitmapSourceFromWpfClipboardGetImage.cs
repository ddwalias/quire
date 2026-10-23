#nullable disable
using Quire.Mirror;
using Quire.Wpf;

namespace Quire;

public partial class WpfClipboard
{
    [MirrorVerified("Release IL")]
    private static System.Windows.Media.Imaging.BitmapSource CreateBitmapSourceFromWpfClipboardGetImage()
    {
        System.Windows.Media.Imaging.BitmapSource bitmapSource = null;
        if (System.Windows.Clipboard.ContainsImage())
        {
            System.Windows.Media.Imaging.BitmapSource bitmap = System.Windows.Clipboard.GetImage();
            if (bitmap != null)
            {
                string path = System.IO.Path.GetTempFileName();
                using (System.IO.FileStream stream = new System.IO.FileStream(path, System.IO.FileMode.Create))
                {
                    ImageUtils.EncodePngImage(bitmap, stream);
                }
                bitmapSource = new System.Windows.Media.Imaging.BitmapImage(new System.Uri(path, (System.UriKind)1));
            }
        }
        return bitmapSource;
    }
}
