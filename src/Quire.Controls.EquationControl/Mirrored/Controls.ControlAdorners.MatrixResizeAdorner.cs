#nullable disable

namespace Quire.Controls.ControlAdorners;

[global::Quire.Mirror.MirrorOf("Ptc.Controls.ControlAdorners.MatrixResizeAdorner, Ptc.Controls.EquationControl")]
public partial class MatrixResizeAdorner : global::Quire.Controls.Core.VisualAdorner, global::System.Windows.Markup.IComponentConnector
{
    private const double DefaultThickness = 10d;
    private readonly global::Quire.Controls.EquationControl _eqControl;
    private global::Quire.Controls.Core.VisualDragger _activeDragger;
    private readonly global::Quire.Controls.Internal.VerticalFence _rightBracket;
    private bool _matrixSizeChanged;
    private global::System.Windows.Point _positionBeforeSizeChanged;
    private bool _rowAdded;
    private bool _columnAdded;
    private double _rowAddThreshold;
    private double _rowRemoveThreshold;
    private double _columnAddThreshold;
    private double _columnRemoveThreshold;
    private global::Quire.Undo.CommandScope _commandScope;
    private global::Quire.TableSize _startDragTableSize;
    public static readonly global::System.Windows.DependencyProperty ThicknessProperty;
    public static readonly global::System.Windows.DependencyProperty VisualMatrixProperty;
    internal global::Quire.Controls.Core.VisualDragger RightDragger;
    internal global::System.Windows.Media.Animation.BeginStoryboard BeginFadeinRightDragger;
    internal global::System.Windows.Media.Animation.BeginStoryboard BeginFadeoutRightDragger;
    internal global::Quire.Controls.Core.VisualDragger DownDragger;
    internal global::System.Windows.Media.Animation.BeginStoryboard BeginFadeinDownDragger;
    internal global::System.Windows.Media.Animation.BeginStoryboard BeginFadeoutDownDragger;
    internal global::Quire.Controls.Core.VisualDragger DownRightDragger;
    internal global::System.Windows.Media.Animation.BeginStoryboard BeginFadeinDraggers;
    internal global::System.Windows.Media.Animation.BeginStoryboard BeginFadeoutDraggers;
    private bool _contentLoaded;
    private global::Quire.Controls.Core.MatrixFormat MatrixFormat { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public global::System.Collections.Generic.IList<int> MatrixPathFromRoot { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public double Thickness { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    internal global::Quire.Controls.Internal.VisualMatrixNode VisualMatrix { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public bool CanActivate { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    internal MatrixResizeAdorner(global::Quire.Controls.EquationControl equationControl, global::Quire.Controls.Internal.VisualMatrixNode visualMatrix) : base(default(global::System.Windows.UIElement)) { throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    private static void VisualMatrixChanged(global::System.Windows.DependencyObject obj, global::System.Windows.DependencyPropertyChangedEventArgs args) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void Dragger_DragStart(object sender, global::System.Windows.Input.MouseEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void Dragger_DragEnd(object sender, global::System.Windows.RoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void Dragger_Drag(object sender, global::Quire.Controls.Core.DragEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void DragUpDown(global::System.Windows.Vector vector, ref global::Quire.TableSize newSize) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void AddRow(ref global::Quire.TableSize newSize) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void RemoveRow(ref global::Quire.TableSize newSize) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void DragLeftRight(global::System.Windows.Vector vector, ref global::Quire.TableSize newSize) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void AddColumn(ref global::Quire.TableSize newSize) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void RemoveColumn(ref global::Quire.TableSize newSize) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void HandleVisualMatrixChanged(global::Quire.Controls.Internal.VisualMatrixNode oldMatrix, global::Quire.Controls.Internal.VisualMatrixNode newMatrix) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public override void Remove() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void SetRightBracketSize() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void CalculateThresholds() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void ResetDraggingStartPoint() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected override global::System.Windows.Size ArrangeOverride(global::System.Windows.Size finalSize) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void InitializeComponent() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    static MatrixResizeAdorner() { throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
}
