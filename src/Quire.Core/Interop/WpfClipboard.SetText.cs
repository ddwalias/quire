#nullable disable
using Quire.Mirror;

namespace Quire;

public partial class WpfClipboard
{
    [MirrorVerified("Release IL")]
    public static bool CopyPrimeXamlToDataObject(System.Windows.IDataObject dataObject, object item, int itemsCount)
    {
        byte[] serializedData;
        bool canPasteIntoTextRegion;
        System.IO.MemoryStream memoryStream;
        System.IO.Compression.DeflateStream deflateStream;
        System.IO.StreamWriter streamWriter;
        bool success;
        System.ApplicationException applicationException;
        try
        {
            memoryStream = new System.IO.MemoryStream();
            try
            {
                deflateStream = new System.IO.Compression.DeflateStream(memoryStream, System.IO.Compression.CompressionMode.Compress, true);
                try
                {
                    streamWriter = new System.IO.StreamWriter(deflateStream);
                    try
                    {
                        System.Windows.Markup.XamlWriter.Save(item, streamWriter);
                    }
                    finally
                    {
                        if (streamWriter != null)
                        {
                            ((System.IDisposable)streamWriter).Dispose();
                        }
                    }
                }
                finally
                {
                    if (deflateStream != null)
                    {
                        ((System.IDisposable)deflateStream).Dispose();
                    }
                }
                serializedData = memoryStream.ToArray();
            }
            finally
            {
                if (memoryStream != null)
                {
                    ((System.IDisposable)memoryStream).Dispose();
                }
            }
            dataObject.SetData(_primeXamlStreamCompressed, serializedData);
            canPasteIntoTextRegion = itemsCount < 2;
            dataObject.SetData(_canPasteIntoTextRegion, canPasteIntoTextRegion);
            success = true;
            goto Success;
        }
        catch (System.Reflection.TargetInvocationException exception)
        {
            applicationException = exception.InnerException as System.ApplicationException;
            if (applicationException != null)
            {
                Quire.Services.AppServiceRegistry.GetService<Quire.Wpf.IMessageBoxManager>().ShowWarning(applicationException.Message, Quire.Wpf.WpfUtils.ApplicationFullName);
            }
        }
        catch
        {
        }
        return false;
    Success:
        return success;
    }

    [MirrorVerified("Release IL")]
    private static void CreateBitmapFromDib(System.IO.Stream dib, System.IO.Stream outStream)
    {
        int dibHeaderSize = new System.IO.BinaryReader(dib).ReadInt32();
        int dataSize = (int)dib.Length - dibHeaderSize;
        int fileSize = 14 + dibHeaderSize + dataSize;
        System.IO.BinaryWriter writer = new System.IO.BinaryWriter(outStream);
        writer.Write((byte)66);
        writer.Write((byte)77);
        writer.Write(fileSize);
        writer.Write(0);
        writer.Write(14 + dibHeaderSize);
        dib.Position = 0L;
        Quire.IO.StreamUtils.CopyStream(dib, outStream);
    }

    [MirrorVerified("Release IL")]
    public static bool CopyItemAsImageToDataObject(System.Windows.DataObject dataObject, object whiteboardItem)
    {
        System.Windows.FrameworkElement element = whiteboardItem as System.Windows.FrameworkElement;
        if (element == null)
        {
            return false;
        }
        System.Windows.Media.Imaging.BitmapSource bitmapSource;
        bool result;
        try
        {
            bitmapSource = Quire.Wpf.ImageUtils.CreatePngFromElement(element);
            dataObject.SetImage(bitmapSource);
            result = true;
            goto Success;
        }
        catch
        {
        }
        return false;
    Success:
        return result;
    }

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    public void Clear() => System.Windows.Clipboard.Clear();

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    public string GetText() => System.Windows.Clipboard.GetText();

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    public bool HasText() => System.Windows.Clipboard.ContainsText();

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    public string GetSExpression() => System.Windows.Clipboard.GetData(_mathNodeSExpression) as string;

    [MirrorVerified("Release IL")]
    public void SetText(string text) => SafeClipboardAction(() => System.Windows.Clipboard.SetText(text));

    [MirrorVerified("PtcWpfClipboardMechanicalDifferentialTests")]
    public bool HasSExpression() => System.Windows.Clipboard.ContainsData(_mathNodeSExpression);

    [MirrorVerified("Release IL")]
    public static void SetDataObjectToClipboard(System.Windows.IDataObject dataObject, bool copy = false) => SafeClipboardAction(() => System.Windows.Clipboard.SetDataObject(dataObject, copy));

}
