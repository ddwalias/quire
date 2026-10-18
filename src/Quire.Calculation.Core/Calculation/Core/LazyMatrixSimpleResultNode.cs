#nullable disable
using Quire.Mirror;
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class LazyMatrixSimpleResultNode
{
    [MirrorVerified("PtcCalculationCoreLazyMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public override bool Equals(object obj) => false;

    [MirrorVerified("PtcCalculationCoreLazyMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public override void Accept(IResultNodeVisitor visitor) => visitor.Visit(this);

    [MirrorVerified("PtcCalculationCoreLazyMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public int Columns { get; private set; }

    [MirrorVerified("PtcCalculationCoreLazyMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public int Rows { get; private set; }

    [MirrorVerified("PtcCalculationCoreLazyMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public IMatrixValueProxyWrapper MatrixProxy { get; private set; }

    [MirrorVerified("PtcCalculationCoreLazyMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public Func<IValueProxyWrapper, IResultNode> TransformValueToResult { get; private set; }

    [MirrorVerified("PtcCalculationCoreLazyMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public override string ToString() => string.Format("LazyMatrix: ({0},{1})", Rows, Columns);

    [MirrorVerified("PtcCalculationCoreLazyMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public string Xml => MatrixProxy.Xml;

    [MirrorVerified("PtcCalculationCoreLazyMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public int Count => Rows * Columns;
}
