#nullable disable
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.IO;
using Quire.Mirror;
using Quire.Core;

namespace Quire.Wpf;

public partial class ImageUtils
{
    // Preserve Mathcad's bitmap-helper member ordinals.
    [MirrorVerified("Release IL")]
    private static BitmapSource CreateBitmapFromElement(FrameworkElement element, double dpiX, double dpiY, AdornerDecorator decoratorAncestor, Visual rootVisual, Rect? bounds)
    {
        UIElement uIElement;
        Rect val3;
        Rect val;
        UIElement uIElement2;
        Rect? nullableBounds;
        Rect val2;
        AdornerLayer adornerLayer;
        Adorner[] adorners;
        Adorner[] array;
        int i;
        Rect descendantBounds;

        nullableBounds = bounds;
        val = nullableBounds.HasValue ? nullableBounds.GetValueOrDefault() : VisualTreeHelper.GetDescendantBounds(element);
        if (element.Clip != null)
        {
            val.Intersect(element.Clip.Bounds);
        }
        if (decoratorAncestor != null)
        {
            val2 = element.TransformToAncestor(decoratorAncestor).TransformBounds(val);
            adornerLayer = AdornerLayer.GetAdornerLayer(element);
            if (adornerLayer != null && adornerLayer.IsDescendantOf(decoratorAncestor))
            {
                adorners = adornerLayer.GetAdorners(element);
                if (adorners != null)
                {
                    array = adorners;
                    i = 0;
                    while (i < array.Length)
                    {
                        Adorner obj = array[i];
                        descendantBounds = VisualTreeHelper.GetDescendantBounds(obj);
                        descendantBounds = obj.TransformToAncestor(decoratorAncestor).TransformBounds(descendantBounds);
                        val2.Union(descendantBounds);
                        i++;
                    }
                }
            }
            if (decoratorAncestor.Clip != null)
            {
                val2.Intersect(decoratorAncestor.Clip.Bounds);
            }
            uIElement = decoratorAncestor;
            val3 = val2;
        }
        else
        {
            uIElement = element;
            val3 = val;
        }
        uIElement2 = uIElement.TryFindVisualAncestor<UIElement>();
        return CreateBitmapFromVisual(
            dpiX,
            dpiY,
            uIElement2 == null ? val3 : uIElement.TransformToAncestor(uIElement2).TransformBounds(val3),
            uIElement,
            element is IMaybeHasPointThatSnapsToPixel && (element as IMaybeHasPointThatSnapsToPixel).HasPointThatSnapsToPixel(),
            rootVisual == null ? null : uIElement2 == null ? uIElement.TransformToAncestor(rootVisual) : uIElement2.TransformToAncestor(rootVisual));
    }

    [MirrorVerified("Release IL")]
    private static BitmapSource CreateBitmapFromVisual(double dpiX, double dpiY, Rect box, Visual visual, bool coerceBoxPoints, GeneralTransform transformToRoot)
    {
        try
        {
            Size? val = null;
            GeneralTransform generalTransform = transformToRoot?.Inverse;
            if (transformToRoot != null && DebugTools.AssertNotNull(transformToRoot.Inverse, null).Passed)
            {
                box = transformToRoot.TransformBounds(box);
            }
            global::System.Drawing.Size size;
            if (coerceBoxPoints)
            {
                global::System.Drawing.Point point = new global::System.Drawing.Point((box.TopLeft.X * dpiX / 96.0).IntFloor(), (box.TopLeft.Y * dpiY / 96.0).IntFloor());
                global::System.Drawing.Point point2 = new global::System.Drawing.Point((box.BottomRight.X * dpiX / 96.0).IntCeiling(), (box.BottomRight.Y * dpiY / 96.0).IntCeiling());
                size = new global::System.Drawing.Size(point2.X - point.X, point2.Y - point.Y);
                Point val3 = new Point((double)point.X * 96.0 / dpiX, (double)point.Y * 96.0 / dpiY);
                Point val4 = new Point((double)point2.X * 96.0 / dpiX, (double)point2.Y * 96.0 / dpiY);
                box = new Rect(val3, val4);
                if (DebugTools.IsDebug && transformToRoot == null)
                {
                    GeneralTransform transformToRootVisual = global::Quire.Native.PtcCore.SnapToRootVisualUtils.GetTransformToRootVisual(visual);
                    if (DebugTools.AssertNotNull(transformToRootVisual, null).Passed)
                    {
                        Rect val5 = transformToRootVisual.TransformBounds(box);
                        Point val6 = new Point(val5.TopLeft.X * dpiX / 96.0, val5.TopLeft.Y * dpiY / 96.0);
                        DebugTools.DevAssert(val6.X.Equals(val6.X.Round()), null);
                        DebugTools.DevAssert(val6.Y.Equals(val6.Y.Round()), null);
                        DebugTools.DevAssert(box.Size == val5.Size, null);
                    }
                }
            }
            else
            {
                size = new global::System.Drawing.Size((box.Width * dpiX / 96.0).IntCeiling(1E-05), (box.Height * dpiX / 96.0).IntCeiling(1E-05));
                box = new Rect(box.TopLeft, new Size((double)size.Width * 96.0 / dpiX, (double)size.Height * 96.0 / dpiY));
            }
            if (generalTransform != null)
            {
                val = box.Size;
                box = generalTransform.TransformBounds(box);
            }
            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(size.Width, size.Height, dpiX, dpiY, PixelFormats.Pbgra32);
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                VisualBrush brush = CreateVisualBrush(visual, box, val.HasValue);
                drawingContext.DrawRectangle(brush, null, new Rect(val ?? box.Size));
            }
            renderTargetBitmap.Render(drawingVisual);
            ((Freezable)renderTargetBitmap).Freeze();
            return renderTargetBitmap;
        }
        catch (Exception ex)
        {
            Type type = ((visual is AdornerDecorator) ? ((object)(visual as AdornerDecorator).Child).GetType() : ((object)visual).GetType());
            DebugTools.FailAssertionDoNotThrow("{0}(0x{1:x8}){2}{3}", type.Name, ((object)visual).GetHashCode(), Environment.NewLine, ex);
            throw;
        }
    }

    public static void EncodeImage(BitmapSource source, string imageType, Stream target) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
}
