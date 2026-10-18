#nullable disable
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class SimpleResultNodeFactory
{
    [MirrorVerified("PtcCalculationCoreSimpleResultNodeFactoryCctorDifferentialTests")]
    static SimpleResultNodeFactory() => Default = new SimpleResultNodeFactory();

    [MirrorVerified("PtcCalculationCoreSimpleResultNodeFactoryMechanicalDifferentialTests")]
    public IEmptyResultNode CreateEmptyResultNode() => new EmptySimpleResultNode();

    [MirrorVerified("PtcCalculationCoreSimpleResultNodeFactoryMechanicalDifferentialTests")]
    public IRealResultNode CreateRealResultNode() => new RealSimpleResultNode();

    [MirrorVerified("PtcCalculationCoreSimpleResultNodeFactoryMechanicalDifferentialTests")]
    public IImaginaryResultNode CreateImaginaryResultNode() => new ImaginarySimpleResultNode();

    [MirrorVerified("PtcCalculationCoreSimpleResultNodeFactoryMechanicalDifferentialTests")]
    public IComplexResultNode CreateComplexResultNode() => new ComplexSimpleResultNode();

    [MirrorVerified("PtcCalculationCoreSimpleResultNodeFactoryMechanicalDifferentialTests")]
    public IStringResultNode CreateStringResultNode() => new StringSimpleResultNode();

    [MirrorVerified("PtcCalculationCoreSimpleResultNodeFactoryMechanicalDifferentialTests")]
    public IDisplayMatrixResultNode CreateDisplayMatrixResultNode(int rows, int cols, TableCoordinate viewportOffset, TableSize viewportSize) => new DisplayMatrixSimpleResultNode(rows, cols, viewportOffset, viewportSize);

    [MirrorVerified("PtcCalculationCoreSimpleResultNodeFactoryMechanicalDifferentialTests")]
    public IEagerMatrixResultNode CreateEagerMatrixResultNode(int rows, int cols) => new EagerMatrixSimpleResultNode(rows, cols);

    [MirrorVerified("PtcCalculationCoreSimpleResultNodeFactoryMechanicalDifferentialTests")]
    public ILazyMatrixResultNode CreateLazyMatrixResultNode(IMatrixValueProxyWrapper matrix, Func<IValueProxyWrapper, IResultNode> transformValueToResult) => new LazyMatrixSimpleResultNode(matrix, transformValueToResult);

    [MirrorVerified("PtcCalculationCoreSimpleResultNodeFactoryMechanicalDifferentialTests")]
    public IUnitedValueResultNode CreateUnitedValueResultNode() => new UnitedValueSimpleResultNode();

    [MirrorVerified("PtcCalculationCoreSimpleResultNodeFactoryMechanicalDifferentialTests")]
    public SimpleResultNodeFactory() { }
}
