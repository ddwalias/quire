#nullable disable

namespace Quire.Wpf;

public static partial class WpfUtils
{
    // Keep these unfinished declarations after ExecuteTask to preserve Mathcad's callback ordinal.
    public static void Blink(global::System.Collections.Generic.IEnumerable<global::System.Windows.UIElement> elements, int period) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public static void SetPropertyAndNotify<T>(ref T property, T value, global::System.ComponentModel.INotifyPropertyChanged propertyOwner, global::System.ComponentModel.PropertyChangedEventHandler propertyChanged, string propertyName)
    {
        if (object.Equals(property, value))
        {
            return;
        }
        property = value;
        propertyChanged?.Invoke(propertyOwner, new global::System.ComponentModel.PropertyChangedEventArgs(propertyName));
    }

    public static void DoEvents(global::System.Windows.Threading.Dispatcher dispatcher, global::System.Windows.Threading.DispatcherPriority priority) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public static bool WaitForIdle(global::System.Threading.Thread thread, global::System.TimeSpan timeout) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public static void TextRangeCopy(global::System.Windows.Documents.TextRange source, global::System.Windows.Documents.TextRange destination, string dataFormat)
    {
        using global::System.IO.MemoryStream memoryStream = new global::System.IO.MemoryStream();
        source.Save(memoryStream, dataFormat, preserveTextElements: true);
        memoryStream.Flush();
        memoryStream.Seek(0L, global::System.IO.SeekOrigin.Begin);
        global::Quire.Serialization.TextRangeExtensionMethods.LoadWithCustomContext(destination, memoryStream, true);
    }

    public static global::System.Uri CombineRelativeUris(global::System.Uri baseUri, global::System.Uri rel)
    {
        global::Quire.DebugTools.DevAssert(!baseUri.IsAbsoluteUri, null);
        global::System.Uri uri = new global::System.Uri(new global::System.Uri(new global::System.Uri("http://invalid", global::System.UriKind.Absolute), baseUri), rel);
        return new global::System.Uri(uri.PathAndQuery, global::System.UriKind.Relative);
    }

    public static void ExecuteOnLoad(global::System.Windows.FrameworkElement element, global::System.Action action, bool condition = false) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
}
