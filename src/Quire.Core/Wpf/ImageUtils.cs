#nullable disable
using System.IO;
using System.Linq;
using System.IO;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Interop;
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

    public static Uri CopyImageToTempBitmap(Uri sourceUri)
    {
        MemoryStream memoryStream = new MemoryStream();
        CopyImageToStream(sourceUri, memoryStream, null);
        memoryStream.Flush();
        return CopyImageToTempBitmap(memoryStream);
    }

    private static Uri CopyStreamToTempFile(Stream source)
    {
        string tempFileName = Path.GetTempFileName();
        using (FileStream target = File.Open(tempFileName, FileMode.Create))
        {
            global::Quire.IO.StreamUtils.CopyStream(source, target);
        }
        return new Uri(tempFileName, UriKind.Absolute);
    }

    public static Uri CopyImageToTempBitmap(ImageSource imageSource)
    {
        if (!(imageSource is BitmapSource bitmapSource))
        {
            return null;
        }
        Uri uri = new Uri(((object)bitmapSource).ToString());
        string extension = Path.GetExtension(uri.LocalPath);
        extension = string.IsNullOrEmpty(extension) ? global::Quire.Xml.xmlWorksheetNodeNames.defaultImageExtention : extension.Substring(1).ToLower();
        MemoryStream memoryStream = new MemoryStream();
        CopyImageToStream(extension, uri, memoryStream, bitmapSource, null);
        memoryStream.Flush();
        return CopyImageToTempBitmap(memoryStream);
    }

    [MirrorVerified("Release IL")]
    public static int BytesNeeded(int bits) => (bits + 7) / 8;

    public static byte[] GetImageHash(BitmapSource source)
    {
        byte[] result;
        using (global::Quire.MindTouch.IO.ChunkedMemoryStream chunkedMemoryStream = new global::Quire.MindTouch.IO.ChunkedMemoryStream())
        {
            try
            {
                CopyPixelsToStream(source, chunkedMemoryStream, null);
                chunkedMemoryStream.Flush();
                chunkedMemoryStream.Seek(0L, SeekOrigin.Begin);
                result = global::Quire.IO.StreamUtils.GetStreamHash(chunkedMemoryStream);
            }
            catch (Exception ex)
            {
                result = null;
                DebugTools.FailAssertion(ex.Message);
            }
        }
        return result;
    }

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

    [MirrorVerified("Release IL")]
    public static Size GetScreenDpiForApplication()
    {
        Size val = DpiScaleForApplication();
        return new Size(val.Width * 96.0, val.Height * 96.0);
    }

    [MirrorVerified("Release IL")]
    public static List<Visual> GetAncestors(DependencyObject obj)
    {
        List<Visual> list = new List<Visual>();
        while (true)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            if (parent == null)
            {
                break;
            }
            list.Add((Visual)(object)obj);
            obj = parent;
        }
        return list;
    }

    [MirrorVerified("Release IL")]
    public static Visual GetRoot(DependencyObject obj)
    {
        while (true)
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);
            if (parent == null)
            {
                return obj as Visual;
            }
            obj = parent;
        }
    }

    [MirrorVerified("Release IL")]
    public static AdornerDecorator GetAdornerDecoratorAncestor(DependencyObject obj)
    {
        while (obj != null && !(obj is AdornerDecorator))
        {
            obj = VisualTreeHelper.GetParent(obj);
        }
        return obj as AdornerDecorator;
    }

    [MirrorVerified("Release IL")]
    public static BitmapImage GetBitmapImage(Stream stream)
    {
        BitmapImage bitmapImage = new BitmapImage();
        bitmapImage.BeginInit();
        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapImage.StreamSource = stream;
        bitmapImage.EndInit();
        return bitmapImage;
    }

    [MirrorVerified("Release IL")]
    public static bool IsStreamContentEqual(Stream sourceStream, Stream targetStream)
    {
        if (sourceStream.Length != targetStream.Length)
        {
            return false;
        }
        sourceStream.Position = 0L;
        targetStream.Position = 0L;
        int num;
        int num2;
        while ((num = sourceStream.ReadByte()) != -1 && (num2 = targetStream.ReadByte()) != -1)
        {
            if (num != num2)
            {
                return false;
            }
        }
        return true;
    }

    [MirrorVerified("Release IL")]
    public static Size DpiScaleForVisual(Visual visual)
    {
        HwndSource compositionTargetOwner;
        return DpiScaleForVisual(visual, out compositionTargetOwner);
    }

    [MirrorVerified("Release IL")]
    public static Size DpiScaleForVisual(Visual visual, out HwndSource compositionTargetOwner)
    {
        if (DebugTools.AssertNotNull(visual, null).Failed)
        {
            compositionTargetOwner = null;
            return Size.Empty;
        }
        compositionTargetOwner = PresentationSource.FromVisual(visual) as HwndSource;
        if (compositionTargetOwner == null || compositionTargetOwner.CompositionTarget == null)
        {
            return Size.Empty;
        }
        Matrix transformToDevice = compositionTargetOwner.CompositionTarget.TransformToDevice;
        return new Size(transformToDevice.M11, transformToDevice.M22);
    }

    [MirrorVerified("Release IL")]
    public static Size DpiScaleForApplication()
    {
        if (Application.Current == null || Application.Current.MainWindow == null)
        {
            return new Size(1.0, 1.0);
        }
        return DpiScaleForVisual(Application.Current.MainWindow);
    }

    [MirrorVerified("Release IL")]
    public static ImageSource GetImageSourceFromIcon(System.Drawing.Icon icon, int imageSize = 0)
    {
        if (icon == null)
        {
            return null;
        }
        int num = ((imageSize <= 0) ? icon.Width : imageSize);
        int num2 = ((imageSize <= 0) ? icon.Height : imageSize);
        return Imaging.CreateBitmapSourceFromHIcon(icon.Handle, new Int32Rect(0, 0, num, num2), BitmapSizeOptions.FromEmptyOptions());
    }

    [MirrorVerified("Release IL")]
    public static System.Drawing.Icon MessageBoxImageToIcon(MessageBoxImage image)
    {
        System.Drawing.Icon result = null;
        if (image == MessageBoxImage.Asterisk)
        {
            result = System.Drawing.SystemIcons.Asterisk;
        }
        else if (image == MessageBoxImage.Asterisk)
        {
            result = System.Drawing.SystemIcons.Information;
        }
        else if (image == MessageBoxImage.Hand || image == MessageBoxImage.Hand)
        {
            result = System.Drawing.SystemIcons.Error;
        }
        else if (image == MessageBoxImage.Hand)
        {
            result = System.Drawing.SystemIcons.Hand;
        }
        else if (image == MessageBoxImage.Exclamation)
        {
            result = System.Drawing.SystemIcons.Exclamation;
        }
        else if (image == MessageBoxImage.Exclamation)
        {
            result = System.Drawing.SystemIcons.Warning;
        }
        else if (image == MessageBoxImage.Question)
        {
            result = System.Drawing.SystemIcons.Question;
        }
        else if (image == MessageBoxImage.None)
        {
            result = null;
        }
        return result;
    }

    [MirrorVerified("Release IL")]
    public static bool CompareBitmapsHash(BitmapSource bm1, BitmapSource bm2, bool convertIfNeeded = false)
    {
        if (bm1 == null || bm2 == null)
        {
            DebugTools.FailAssertion(null, Array.Empty<object>());
            return bm1 == bm2;
        }
        if (bm1.PixelWidth != bm2.PixelWidth || bm1.PixelHeight != bm2.PixelHeight)
        {
            return false;
        }
        BitmapSource[] array = new BitmapSource[2] { bm1, bm2 };
        PixelFormat format = bm1.Format;
        PixelFormat format2 = bm2.Format;
        if (format != format2)
        {
            if (!convertIfNeeded || format.BitsPerPixel > 32 || format2.BitsPerPixel > 32)
            {
                return false;
            }
            PixelFormat bgra = PixelFormats.Bgra32;
            for (int i = 0; i < array.Length; i++)
            {
                BitmapSource bitmapSource = array[i];
                if (bitmapSource.Format != bgra)
                {
                    FormatConvertedBitmap formatConvertedBitmap = new FormatConvertedBitmap();
                    formatConvertedBitmap.BeginInit();
                    formatConvertedBitmap.Source = bitmapSource;
                    formatConvertedBitmap.DestinationFormat = bgra;
                    formatConvertedBitmap.EndInit();
                    array[i] = formatConvertedBitmap;
                }
            }
        }
        byte[] imageHash = GetImageHash(array[0]);
        byte[] imageHash2 = GetImageHash(array[1]);
        return CompareHashes(imageHash, imageHash2);
    }
}
