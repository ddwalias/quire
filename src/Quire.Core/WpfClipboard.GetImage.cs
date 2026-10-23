#nullable disable
using Quire.Mirror;

namespace Quire;

public partial class WpfClipboard
{
    [MirrorVerified("Release IL")]
    public System.Windows.Media.Imaging.BitmapSource GetImage()
    {
        System.Windows.Media.Imaging.BitmapSource bitmapSource = null;
        System.Windows.IDataObject dataObject = System.Windows.Clipboard.GetDataObject();
        if (dataObject != null)
        {
            try
            {
                try
                {
                    bitmapSource = CreateBitmapSourceFromStream(dataObject.GetData(System.Windows.DataFormats.Dib, false) as System.IO.Stream);
                }
                catch (System.OutOfMemoryException)
                {
                    bitmapSource = CreateBitmapSourceFromStream(dataObject.GetData(System.Windows.DataFormats.Bitmap, false) as System.IO.Stream);
                }
            }
            catch (System.Runtime.InteropServices.COMException)
            {
                bitmapSource = CreateBitmapSourceFromWpfClipboardGetImage();
            }
        }
        return bitmapSource;
    }
}
