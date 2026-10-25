#nullable disable
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Quire.Core;
using Quire.Mirror;

namespace Quire.Wpf;

public partial class ImageUtils
{
    static ImageUtils()
    {
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

    public static void SavePngFromElementUsingScreenDpi(FrameworkElement element, Stream outStream, Visual rootVisual = null)
    {
        BitmapSource bitmapSource = CreateBitmapFromElementUsingScreenDpi(element, rootVisual, default(Rect?));
        if (bitmapSource == null)
        {
            return;
        }
        EncodePngImage(bitmapSource, outStream);
    }

    public static BitmapSource CreateBitmapFromElementUsingScreenDpi(FrameworkElement element, Visual rootVisual = null, Rect? bounds = null)
    {
        if (element == null)
        {
            return null;
        }
        if (element.ActualWidth <= 0.0 || element.ActualHeight <= 0.0)
        {
            return null;
        }
        AdornerDecorator adornerDecoratorAncestor = GetAdornerDecoratorAncestor(element);
        DebugTools.AssertNotNull(adornerDecoratorAncestor, null);
        Size screenDpiForApplication = GetScreenDpiForApplication();
        return CreateBitmapFromElement(element, screenDpiForApplication.Width, screenDpiForApplication.Height, adornerDecoratorAncestor, rootVisual, bounds);
    }

    [MirrorVerified("Release IL")]
    public static Size GetScreenDpiForApplication()
    {
        Size val = DpiScaleForApplication();
        return new Size(val.Width * 96.0, val.Height * 96.0);
    }

    public static BitmapSource CreatePngFromElement(FrameworkElement element)
    {
        global::Quire.MindTouch.IO.ChunkedMemoryStream chunkedMemoryStream = new global::Quire.MindTouch.IO.ChunkedMemoryStream();
        SavePngFromElementUsingScreenDpi(element, chunkedMemoryStream);
        return BitmapFrame.Create(chunkedMemoryStream);
    }

    // Compile before generated members so DumpBounds retains Mathcad's cached-lambda ordinal 8.
    [global::System.Diagnostics.Conditional("DEBUG")]
    private static void DumpBounds(UIElement element, UIElement ancestor = null, string prefix = "")
    {
        try
        {
            Rect bounds = VisualTreeHelper.GetDescendantBounds(element);
            if (ancestor != null)
            {
                bounds = element.TransformToAncestor(ancestor).TransformBounds(bounds);
            }
            // Mathcad copies Rect to a local before calling Object.ToString.
            string[] parts = new string[]
            {
                prefix,
                element.GetType().Name,
                ": {",
                ((Rect)bounds).ToString(),
                "}, {",
                element.RenderSize.ToString(),
                "}"
            };
            global::System.Diagnostics.Trace.WriteLine(string.Concat(parts));
            VisualTreeHelper.GetChildrenCount(element).Times((int index) => { });
        }
        catch (Exception exception)
        {
            global::System.Diagnostics.Trace.WriteLine(prefix + element.GetType().Name + ": " + exception);
        }
    }

}
