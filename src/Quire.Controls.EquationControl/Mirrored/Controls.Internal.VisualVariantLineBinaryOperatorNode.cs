#nullable disable

namespace Quire.Controls.Internal;

[global::Quire.Mirror.MirrorOf("Ptc.Controls.Internal.VisualVariantLineBinaryOperatorNode, Ptc.Controls.EquationControl")]
internal abstract partial class VisualVariantLineBinaryOperatorNode : global::Quire.Controls.Internal.VisualLinearBinaryOperatorNode, global::Quire.EquationEditor.View.IVisualVariantLineBinaryOperatorNode, global::Quire.EquationEditor.View.IVisualBinaryOperatorNode, global::Quire.EquationEditor.View.IVisualMathNode, global::Quire.EquationEditor.View.IVisualNode, global::Quire.EquationEditor.View.IVisualVariantLineBinaryOperatorValues
{
    protected double _effectiveBaseline;
    protected double _effectiveScriptBaseline;
    private double _inBottomLineVOffset;
    private double _inHOffset;
    private double _inTopLineVOffset;
    protected double _blblCurrent;
    protected double _blhCurrent;
    protected double _tlhCurrent;
    protected double _blwCurrent;
    protected double _tlwCurrent;
    private double _blbl0 = double.NaN;
    private double _tlbl0 = double.NaN;
    private double _blh0 = double.NaN;
    private double _tlh0 = double.NaN;
    private double _blw0 = double.NaN;
    private double _tlw0 = double.NaN;
    private double _th0 = double.NaN;
    protected double _height;
    protected double _width;
    protected global::System.Windows.Rect _operatorExtendedRectangle;
    public static readonly global::System.Windows.DependencyProperty DoRenderProperty;
    private static global::System.Collections.Generic.IList<global::System.Windows.Media.Color> _colorsTable;
    public int MeasureCallFromRoot { get; private set; }
    protected global::Quire.EquationEditor.View.IVisualVariantLineBinaryOperatorValues FirstVariantLineNode { get; private set; }
    protected global::Quire.EquationEditor.View.IVisualVariantLineBinaryOperatorValues SecondVariantLineNode { get; private set; }
    public virtual bool IsMultiline { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public double TotalLinesHeightAboveBottom { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public override double EffectiveBaselineY { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public override double EffectiveScriptBaselineY { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public double InBottomLineVOffset { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public double InHOffset { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public double InTopLineVOffset { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public double TopLineBaseLine { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public double TopLineHeight { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public double TopLineWidth { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public double BottomLineBaseLine { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public double BottomLineHeight { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public double BottomLineWidth { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public double TotalHeight { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public bool UseZeroCache { get; set; }
    public override global::System.Windows.Rect ScreenInkBounds { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    protected abstract bool HasFirstOperandAtLeft { get; }
    protected abstract bool HasFirstOperandAbove { get; }
    protected bool HasSecondOperandBelow { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public bool DoRender { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); set => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    private static global::System.Collections.Generic.IList<global::System.Windows.Media.Color> ColorsTable { get => throw global::Quire.Mirror.MirrorStub.NotImplemented(); }
    public void ResetOffsets() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    public void SetMeasureCallFromRoot(int callNumber) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected override void OnFirstOperandChanged(global::System.Windows.DependencyPropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected override void OnSecondOperandChanged(global::System.Windows.DependencyPropertyChangedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private static global::Quire.EquationEditor.View.IVisualVariantLineBinaryOperatorValues Convert2VariantLineNode(global::Quire.Controls.Internal.VisualMathNode mathNode, bool firstNode) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected VisualVariantLineBinaryOperatorNode() : base(null) { }
    protected VisualVariantLineBinaryOperatorNode(global::Quire.Controls.Internal.VisualSymbolNode opSymbol) : base(opSymbol) { }
    protected void InsureZeroOffsetsIfNeeded() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected void ClearCurrentValuesCache() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected void ClearZeroCache() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected void CacheZeroValues() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected override void OnRender(global::System.Windows.Media.DrawingContext dc) => base.OnRender(dc);
    private global::System.Windows.Point GetHitPointRelativeToSubNode(global::System.Windows.Point hitPoint, global::System.Windows.Media.Visual subNode) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected global::System.Windows.Media.HitTestResult GetDescendantOfOperand(global::System.Windows.Point hitPoint, global::Quire.Controls.Internal.VisualMathNode operand) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    internal override global::System.Windows.Media.HitTestResult GetNodeAtPoint(global::System.Windows.Point hitPoint) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected virtual global::Quire.Controls.Internal.VisualMathNode GetVisualMathNodeAtPoint(global::System.Windows.Point hitPoint) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected abstract global::System.Windows.Media.Geometry GetFirstChildRectangle();
    protected abstract global::System.Windows.Media.Geometry GetSecondChildRectangle();
    protected static global::System.Windows.Media.Geometry GetOffsetAndExcludeRectangles(global::System.Windows.Point offset, global::System.Windows.Rect mainRect, global::System.Collections.Generic.IEnumerable<global::System.Windows.Rect> rectsToExclude) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected void SetMeasureCallFromRootOnChildren(int callNumber) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private global::System.Windows.Media.Color GetColorForDepth(int idx) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    protected void ComputeDepth_DEBUG() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void Init_EQUATION_WRAPPING_DEBUG_properties() => throw global::Quire.Mirror.MirrorStub.NotImplemented();
    private void OnRender_EQUATION_WRAPPING_DEBUG(global::System.Windows.Media.DrawingContext dc) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
}
