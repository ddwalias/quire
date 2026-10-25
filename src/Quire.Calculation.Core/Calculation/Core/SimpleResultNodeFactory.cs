#nullable disable
using System;
using Quire;
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class SimpleResultNodeFactory
{
    public static IResultNodeFactory Default;

    [MirrorVerified("PtcCalculationCoreSimpleResultNodeFactoryCctorMechanicalDifferentialTests")]
    static SimpleResultNodeFactory() => Default = new SimpleResultNodeFactory();

    [MirrorVerified("PtcCalculationCoreSimpleResultNodeFactoryMechanicalDifferentialTests")]
    public IEmptyResultNode CreateEmptyResultNode() => new EmptySimpleResultNode();

    [MirrorVerified("PtcCalculationCoreSimpleResultNodeFactoryMechanicalDifferentialTests")]
    public IRealResultNode CreateRealResultNode() => new RealSimpleResultNode();

    public IRealResultNode CreateRealResultNode(double rvalue) => new RealSimpleResultNode { Value = rvalue };

    [MirrorVerified("PtcCalculationCoreSimpleResultNodeFactoryMechanicalDifferentialTests")]
    public IImaginaryResultNode CreateImaginaryResultNode() => new ImaginarySimpleResultNode();

    public IImaginaryResultNode CreateImaginaryResultNode(double ivalue) => new ImaginarySimpleResultNode { Value = ivalue };

    [MirrorVerified("PtcCalculationCoreSimpleResultNodeFactoryMechanicalDifferentialTests")]
    public IComplexResultNode CreateComplexResultNode() => new ComplexSimpleResultNode();

    public IComplexResultNode CreateComplexResultNode(double rvalue, double ivalue) => new ComplexSimpleResultNode { RValue = rvalue, IValue = ivalue };

    [MirrorVerified("PtcCalculationCoreSimpleResultNodeFactoryMechanicalDifferentialTests")]
    public IStringResultNode CreateStringResultNode() => new StringSimpleResultNode();

    public IStringResultNode CreateStringResultNode(string value) => new StringSimpleResultNode { Value = value };

    public IFunctionResultNode CreateFunctionResultNode(string descriptionOfFunction) => new FunctionSimpleResultNode { Description = descriptionOfFunction };

    [MirrorVerified("PtcCalculationCoreSimpleResultNodeFactoryMechanicalDifferentialTests")]
    public IDisplayMatrixResultNode CreateDisplayMatrixResultNode(int rows, int cols, TableCoordinate viewportOffset, TableSize viewportSize) => new DisplayMatrixSimpleResultNode(rows, cols, viewportOffset, viewportSize);

    [MirrorVerified("PtcCalculationCoreSimpleResultNodeFactoryMechanicalDifferentialTests")]
    public IEagerMatrixResultNode CreateEagerMatrixResultNode(int rows, int cols) => new EagerMatrixSimpleResultNode(rows, cols);

    [MirrorVerified("PtcCalculationCoreSimpleResultNodeFactoryMechanicalDifferentialTests")]
    public ILazyMatrixResultNode CreateLazyMatrixResultNode(IMatrixValueProxyWrapper matrix, Func<IValueProxyWrapper, IResultNode> transformValueToResult) => new LazyMatrixSimpleResultNode(matrix, transformValueToResult);

    [MirrorVerified("PtcCalculationCoreSimpleResultNodeFactoryMechanicalDifferentialTests")]
    public IUnitedValueResultNode CreateUnitedValueResultNode() => new UnitedValueSimpleResultNode();

    public ISymbolicResultNode CreateSymbolicResultNode(string xmcd) => new SymbolicSimpleResultNode { Xml = xmcd };

    [MirrorVerified("PtcCalculationCoreSimpleResultNodeFactoryMechanicalDifferentialTests")]
    public SimpleResultNodeFactory() { }
}
