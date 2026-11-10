using System.Windows;
using Quire.Controls.Core;

namespace Quire.Controls.ExcelComponent.View;

public abstract partial class OleBaseComponentControl
{
    public static double GetItemHeight(DependencyObject whiteboardItem)
    {
        return (double)whiteboardItem.GetValue(FrameworkElement.HeightProperty);
    }

    public static void SetItemHeight(DependencyObject whiteboardItem, double value)
    {
        whiteboardItem.SetValue(FrameworkElement.HeightProperty, (object)value);
    }

    public static double GetOleComponentLocationLeft(DependencyObject whiteboardItem)
    {
        return (double)whiteboardItem.GetValue(OleComponentLocationLeftProperty);
    }

    public static void SetOleComponentLocationLeft(DependencyObject whiteboardItem, double value)
    {
        whiteboardItem.SetValue(OleComponentLocationLeftProperty, (object)value);
    }

    public Size CoerceNewSize(Size newSize, global::Quire.Controls.Core.IResizeData resizeData)
    {
        return newSize;
    }

    public override string GetBlockEquationTypeAttributeName(global::Quire.Controls.Core.IWhiteboardItem item)
    {
        return string.Empty;
    }

    public static double GetItemWidth(DependencyObject whiteboardItem)
    {
        return (double)whiteboardItem.GetValue(FrameworkElement.WidthProperty);
    }

    public static void SetItemWidth(DependencyObject whiteboardItem, double value)
    {
        whiteboardItem.SetValue(FrameworkElement.WidthProperty, (object)value);
    }

    public static double GetOleComponentLocationTop(DependencyObject whiteboardItem)
    {
        return (double)whiteboardItem.GetValue(OleComponentLocationTopProperty);
    }

    public static void SetOleComponentLocationTop(DependencyObject whiteboardItem, double value)
    {
        whiteboardItem.SetValue(OleComponentLocationTopProperty, (object)value);
    }

    private static void HandleOleComponentLocationTopChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        obj.SetValue(AttachedProperties.PreviousLocationTopProperty, args.NewValue);
    }

    private static void HandleOleComponentLocationLeftChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
        obj.SetValue(AttachedProperties.PreviousLocationLeftProperty, args.NewValue);
    }

    public void UpdateAllErrors()
    {
        _oleBaseModel.UpdateAllErrors();
    }

    private void FireCalcErrorChanged()
    {
        FirePropertyChanged(global::Quire.Calculation.Core.ErrorChangedEventArgs.GetArgs());
    }

    public void SetPrintingZoom(bool set, global::System.Nullable<double> newPrintingZoom)
    {
        OleContainerItem.SetPrintingZoom(set, newPrintingZoom);
    }

    public global::Quire.Controls.Core.IResizeData StoreResizeData(global::System.Windows.Size oldSize, global::System.Windows.Size newSize)
    {
        return _oleBaseViewModel.StoreResizeData(oldSize, newSize);
    }

    public void RestoreResizeData(global::Quire.Controls.Core.IResizeData resizeData)
    {
        _oleBaseViewModel.RestoreResizeData(resizeData);
    }
}
