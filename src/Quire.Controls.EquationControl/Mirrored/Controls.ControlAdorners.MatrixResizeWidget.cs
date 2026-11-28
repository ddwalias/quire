using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace Quire.Controls.ControlAdorners;

[global::Quire.Mirror.MirrorOf("Ptc.Controls.ControlAdorners.MatrixResizeWidget, Ptc.Controls.EquationControl")]
public partial class MatrixResizeWidget : global::System.Windows.Controls.UserControl, global::System.IDisposable, global::System.Windows.Markup.IComponentConnector
{
    public const double OuterGrabAreaSize = 20d;
    private const double MaxWidgetSize = 100d;
    private const double MinimalSize = 10d;
    private bool _isDragging;
    private global::Quire.Undo.CommandScope _commandScope;
    public bool GotKeyPressEvent { get; private set; }
    private readonly global::Quire.Controls.EquationControl _eqControl;
    public static readonly global::System.Windows.DependencyProperty MatrixFormatProperty = global::System.Windows.DependencyProperty.Register("MatrixFormat", typeof(global::Quire.Controls.Core.MatrixFormat), typeof(MatrixResizeWidget), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)MatrixFormatChanged));
    public static readonly global::System.Windows.DependencyProperty TotalSizeProperty = global::System.Windows.DependencyProperty.Register("TotalSize", typeof(global::Quire.TableSize), typeof(MatrixResizeWidget), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)TotalSizeChanged));
    public static readonly global::System.Windows.DependencyProperty InnerMatrixMarginProperty = global::System.Windows.DependencyProperty.Register("InnerMatrixMargin", typeof(global::System.Windows.Thickness), typeof(MatrixResizeWidget), new global::System.Windows.PropertyMetadata((global::System.Windows.PropertyChangedCallback)InnerMatrixMarginChanged));
    public static readonly global::System.Windows.DependencyProperty OuterMatrixWidthProperty = global::System.Windows.DependencyProperty.Register("OuterMatrixWidth", typeof(double), typeof(MatrixResizeWidget), new global::System.Windows.PropertyMetadata((object)10d, (global::System.Windows.PropertyChangedCallback)Ignore, (global::System.Windows.CoerceValueCallback)NotLessThanMinimalSize));
    public static readonly global::System.Windows.DependencyProperty OuterMatrixHeightProperty = global::System.Windows.DependencyProperty.Register("OuterMatrixHeight", typeof(double), typeof(MatrixResizeWidget), new global::System.Windows.PropertyMetadata((object)10d, (global::System.Windows.PropertyChangedCallback)Ignore, (global::System.Windows.CoerceValueCallback)NotLessThanMinimalSize));
    public static readonly global::System.Windows.DependencyProperty DisplayedOffsetProperty = global::System.Windows.DependencyProperty.Register("DisplayedOffset", typeof(global::Quire.TableCoordinate), typeof(MatrixResizeWidget));
    public static readonly global::System.Windows.DependencyProperty ErrorIndicatorVisibleProperty = global::System.Windows.DependencyProperty.Register("ErrorIndicatorVisible", typeof(Visibility), typeof(MatrixResizeWidget), new global::System.Windows.PropertyMetadata((object)Visibility.Hidden));
    public static readonly global::System.Windows.DependencyProperty XErrorIndicatorProperty = global::System.Windows.DependencyProperty.Register("XErrorIndicator", typeof(double), typeof(MatrixResizeWidget), new global::System.Windows.PropertyMetadata((object)0d));
    public static readonly global::System.Windows.DependencyProperty YErrorIndicatorProperty = global::System.Windows.DependencyProperty.Register("YErrorIndicator", typeof(double), typeof(MatrixResizeWidget), new global::System.Windows.PropertyMetadata((object)0d));
    internal MatrixResizeWidget TheControl;
    internal Border OuterBorder;
    internal Border InnerBorder;
    internal Button ExitButton;
    private bool _contentLoaded;

    internal MatrixResizeWidget(global::Quire.Controls.EquationControl eqControl, global::Quire.Controls.Internal.VisualMatrixNode visualMatrix)
    {
        InitializeComponent();
        _eqControl = eqControl;
        AddHandler(global::Quire.Controls.Core.VisualDragger.DragStartEvent, new RoutedEventHandler(HandleDragStart));
        AddHandler(global::Quire.Controls.Core.VisualDragger.DragEndEvent, new RoutedEventHandler(HandleDragEnd));
    }

    internal static global::System.Windows.Controls.Primitives.Popup CreatePopup(global::Quire.Controls.EquationControl eqControl, global::Quire.Controls.Internal.VisualMatrixNode visualMatrix) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static void MatrixWidgetPopupClosed(object sender, global::System.EventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static void MatrixFormatChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleMatrixFormatChanged(string propertyName, object newValue) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.Controls.Core.MatrixFormat MatrixFormat
    {
        get => (global::Quire.Controls.Core.MatrixFormat)GetValue(MatrixFormatProperty);
        set => SetValue(MatrixFormatProperty, value);
    }

    private static void TotalSizeChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public global::Quire.TableSize TotalSize
    {
        get => (global::Quire.TableSize)GetValue(TotalSizeProperty);
        set => SetValue(TotalSizeProperty, value);
    }

    private static void InnerMatrixMarginChanged(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public Thickness InnerMatrixMargin
    {
        get => (Thickness)GetValue(InnerMatrixMarginProperty);
        set => SetValue(InnerMatrixMarginProperty, value);
    }

    private static object NotLessThanMinimalSize(global::System.Windows.DependencyObject d, object basevalue)
    {
        double value = (double)basevalue;
        return value < MinimalSize ? MinimalSize : value;
    }

    private static void Ignore(global::System.Windows.DependencyObject d, global::System.Windows.DependencyPropertyChangedEventArgs e)
    {
    }

    public double OuterMatrixWidth
    {
        get => (double)GetValue(OuterMatrixWidthProperty);
        set => SetValue(OuterMatrixWidthProperty, value);
    }

    public double OuterMatrixHeight
    {
        get => (double)GetValue(OuterMatrixHeightProperty);
        set => SetValue(OuterMatrixHeightProperty, value);
    }

    public global::Quire.TableCoordinate DisplayedOffset
    {
        get => (global::Quire.TableCoordinate)GetValue(DisplayedOffsetProperty);
        set => SetValue(DisplayedOffsetProperty, value);
    }

    public Visibility ErrorIndicatorVisible
    {
        get => (Visibility)GetValue(ErrorIndicatorVisibleProperty);
        set => SetValue(ErrorIndicatorVisibleProperty, value);
    }

    public double XErrorIndicator
    {
        get => (double)GetValue(XErrorIndicatorProperty);
        set => SetValue(XErrorIndicatorProperty, value);
    }

    public double YErrorIndicator
    {
        get => (double)GetValue(YErrorIndicatorProperty);
        set => SetValue(YErrorIndicatorProperty, value);
    }

    private void CalculateInnerMatrixMargin() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void UpdateMatrixFormat() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private double CellStep => 100d / global::System.Math.Max(TotalSize.Columns, TotalSize.Rows);

    private void SetHeightWidth() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleDragStart(object sender, RoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleDragEnd(object sender, RoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CalulateDisplayedOffset() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void Dispose() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void InitializeComponent() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

}
