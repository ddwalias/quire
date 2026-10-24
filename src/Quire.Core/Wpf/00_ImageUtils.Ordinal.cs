#nullable disable
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
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

    // Preserve Mathcad's later helper ordinals while their bodies remain unfinished.
    private static BitmapSource CreateBitmapFromElement(FrameworkElement element, double dpiX, double dpiY, AdornerDecorator decoratorAncestor, Visual rootVisual, Rect? bounds) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static BitmapSource CreateBitmapFromVisual(double dpiX, double dpiY, Rect box, Visual visual, bool coerceBoxPoints, GeneralTransform transformToRoot) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public static void EncodeImage(BitmapSource source, string imageType, Stream target) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
}
