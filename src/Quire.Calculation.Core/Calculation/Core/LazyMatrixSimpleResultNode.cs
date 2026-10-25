#nullable disable
using System;
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

    [MirrorVerified("PtcCalculationCoreLazyMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public SimpleResultNode this[int r, int c]
    {
        get
        {
            SimpleResultNode simpleResultNode = (SimpleResultNode)TransformValueToResult(MatrixProxy.Get((uint)r, (uint)c));
            if (simpleResultNode is UnitedValueSimpleResultNode unitedValueSimpleResultNode && MatrixProxy.IsUniform)
            {
                simpleResultNode = unitedValueSimpleResultNode.Value;
            }
            return simpleResultNode;
        }
        set
        {
            throw new InvalidOperationException("Logic error: Setting an element of a LazyMatrixSimpleResultNode directly is not allowed.");
        }
    }

    IResultNode IMatrixResultNode.this[int r, int c] => this[r, c];

    [MirrorVerified("PtcCalculationCoreLazyMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public TableSize Size => new TableSize
    {
        Rows = Rows,
        Columns = Columns
    };

    [MirrorVerified("PtcCalculationCoreLazyMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public LazyMatrixSimpleResultNode(IMatrixValueProxyWrapper matrix, Func<IValueProxyWrapper, IResultNode> transformValueToResult)
    {
        MatrixProxy = matrix;
        TransformValueToResult = transformValueToResult;
        Rows = (int)MatrixProxy.Rows;
        Columns = (int)MatrixProxy.Cols;
    }

    [MirrorVerified("PtcCalculationCoreLazyMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public EagerMatrixSimpleResultNode GetSubMatrix(int topRow, int leftCol, int height, int width)
    {
        if (topRow < 0 || topRow + height > Rows)
        {
            throw new ArgumentOutOfRangeException("topRow", "The requested sub-matrix must be a valid subset of this matrix, the top or bottom is out of bounds.");
        }
        if (leftCol < 0 || leftCol + width > Columns)
        {
            throw new ArgumentOutOfRangeException("leftCol", "The requested sub-matrix must be a valid subset of this matrix, the left or right is out of bounds.");
        }
        EagerMatrixSimpleResultNode eagerMatrixSimpleResultNode = new EagerMatrixSimpleResultNode(height, width);
        for (int i = 0; i < height; i++)
        {
            for (int j = 0; j < width; j++)
            {
                eagerMatrixSimpleResultNode[i, j] = this[topRow + i, leftCol + j];
            }
        }
        return eagerMatrixSimpleResultNode;
    }
}
