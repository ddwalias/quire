#nullable disable
using System.IO;
using System.Linq;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class ImageUtils
{
    [MirrorVerified("PtcWpfImageUtilsMechanicalDifferentialTests")]
    public static BitmapSource CreateBitmapFromVisual(double dpiX, double dpiY, Rect box, Visual visual) => CreateBitmapFromVisual(dpiX, dpiY, box, visual, false, null);

    [MirrorVerified("PtcWpfImageUtilsMechanicalDifferentialTests")]
    public static void EncodePngImage(BitmapSource source, Stream target) => EncodeImage(source, "png", target);

    [MirrorVerified("PtcWpfImageUtilsMechanicalDifferentialTests")]
    public ImageUtils() { }

    [MirrorVerified("Release IL")]
    public static Size GetArchitectureIndependentSize(BitmapSource bitmapSource) => new Size(bitmapSource.PixelWidth * 96.0 / bitmapSource.DpiX, bitmapSource.PixelHeight * 96.0 / bitmapSource.DpiY);

    [MirrorVerified("Release IL")]
    public static Uri CopyImageToTempBitmap(Stream sourceStream)
    {
        DebugTools.DevAssert(sourceStream.Seek(0L, (SeekOrigin)0) == 0L, null, Array.Empty<object>());
        return CopyStreamToTempFile(sourceStream);
    }

    [MirrorVerified("Release IL")]
    public static int BytesNeeded(int bits) => (bits + 7) / 8;

    [MirrorVerified("Release IL")]
    public static bool CompareHashes(byte[] hash1, byte[] hash2)
    {
        if (hash1 != null && hash2 != null)
        {
            return hash1.SequenceEqual(hash2);
        }
        return false;
    }

    [MirrorVerified("Release IL")]
    public static BitmapImage CreateBitmapImageWithCacheOptions(Uri uri)
    {
        BitmapImage bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.CreateOptions = BitmapCreateOptions.IgnoreImageCache;
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.UriSource = uri;
        bitmapImage.EndInit();
        return bitmapImage;
    }

    [MirrorVerified("Release IL")]
    public static VisualBrush CreateVisualBrush(Visual visual, Rect box, bool stretch = false)
    {
        return new VisualBrush(visual)
        {
            ViewboxUnits = BrushMappingMode.Absolute,
            Stretch = (stretch ? Stretch.Uniform : Stretch.None),
            Viewbox = box
        };
    }
}
