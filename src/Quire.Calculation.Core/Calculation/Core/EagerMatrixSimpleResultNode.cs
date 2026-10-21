#nullable disable
using System.Collections.Generic;
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class EagerMatrixSimpleResultNode
{
    [MirrorVerified("PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public override void Accept(IResultNodeVisitor visitor) => visitor.Visit(this);

    [MirrorVerified("PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests")]
    private readonly IList<SimpleResultNode> _children;

    [MirrorVerified("PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public int Count => _children.Count;

    [MirrorVerified("PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public TableSize Size => new TableSize
    {
        Rows = Rows,
        Columns = Columns
    };

    [MirrorVerified("PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public int Columns { get; private set; }

    [MirrorVerified("PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public int Rows => Count / Columns;

    [MirrorVerified("PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public SimpleResultNode this[int r, int c] { get => _children[r * Columns + c]; set => _children[r * Columns + c] = value; }

    [MirrorVerified("PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests")]
    IResultNode IMatrixResultNode.this[int r, int c] => this[r, c];

    [MirrorVerified("PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public EagerMatrixSimpleResultNode(int rowCount, int colCount)
    {
        Columns = colCount;
        _children = new List<SimpleResultNode>(rowCount * colCount);
        for (int i = 0; i < rowCount * colCount; i++)
        {
            _children.Add(null);
        }
    }

    [MirrorVerified("PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests")]
    private EagerMatrixSimpleResultNode(IMatrixResultNode node)
        : this(node.Rows, node.Columns)
    {
        for (int i = 0; i < node.Rows; i++)
        {
            for (int j = 0; j < node.Columns; j++)
            {
                IResultNode resultNode = node[i, j];
                IMatrixResultNode matrixNode = resultNode as IMatrixResultNode;
                if (matrixNode != null)
                {
                    resultNode = ConvertToEagerMatrix(matrixNode);
                }
                UnitedValueSimpleResultNode unitedValueSimpleResultNode = resultNode as UnitedValueSimpleResultNode;
                if (unitedValueSimpleResultNode != null)
                {
                    matrixNode = unitedValueSimpleResultNode.Value as IMatrixResultNode;
                    if (matrixNode != null)
                    {
                        unitedValueSimpleResultNode.Value = (SimpleResultNode)ConvertToEagerMatrix(matrixNode);
                    }
                }
                SetElementAt(i, j, resultNode);
            }
        }
    }

    [MirrorVerified("PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public void SetElementAt(int r, int c, IResultNode element) => this[r, c] = (SimpleResultNode)element;

    [MirrorVerified("PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public override string ToString() => string.Format("Matrix: ({0},{1})", Rows, Columns);

    [MirrorVerified("PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public static IEagerMatrixResultNode ConvertToEagerMatrix(IMatrixResultNode matrixNode)
    {
        if (matrixNode is IEagerMatrixResultNode result)
        {
            return result;
        }
        return new EagerMatrixSimpleResultNode(matrixNode);
    }

    [MirrorVerified("PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests")]
    public override bool Equals(object obj)
    {
        if (obj is EagerMatrixSimpleResultNode obj2)
        {
            return IsChildrenEquals(obj2);
        }
        return false;
    }

    [MirrorVerified("PtcCalculationCoreEagerMatrixSimpleResultNodeMechanicalDifferentialTests")]
    private bool IsChildrenEquals(EagerMatrixSimpleResultNode obj)
    {
        bool result = false;
        if (Rows == obj.Rows && Columns == obj.Columns)
        {
            for (int i = 0; i < _children.Count; i++)
            {
                if (_children[i] == null && _children[i] != null)
                {
                    break;
                }
                if (_children[i] != null && !_children[i].Equals(obj._children[i]))
                {
                    break;
                }
            }
            result = true;
        }
        return result;
    }
}
