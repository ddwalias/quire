#nullable disable
using System.Text.RegularExpressions;
using Quire.Automation;
using AutomationUnitPower = Quire.Automation.UnitPower;
using Quire.Calculation.Core;
using Quire.Mirror;

namespace Quire.Calculation;

public partial class ValueToIResultNodeTransform
{
    [MirrorVerified("PtcCalculationValueToIResultNodeTransform_MechanicalDifferentialTests")]
    public ValueToIResultNodeTransform(IResultNodeFactory resultNodeFactory)
    {
        _resultNodeFactory = resultNodeFactory;
    }

    [MirrorVerified("PtcCalculationValueToIResultNodeTransform_MechanicalDifferentialTests")]
    public bool SimplifyUnits { get; set; }

    [MirrorVerified("PtcCalculationValueToIResultNodeTransform_MechanicalDifferentialTests")]
    public IResultNode Transform(IAutomationValue value)
    {
        if (value == null)
            return _resultNodeFactory.CreateEmptyResultNode();
        return MakeResultNode(value);
    }

    [MirrorVerified("PtcCalculationValueToIResultNodeTransform_MechanicalDifferentialTests")]
    private IResultNode MakeResultNode(IAutomationValue value)
    {
        if (value == null)
            return _resultNodeFactory.CreateEmptyResultNode();
        return MakeResultNode(value.GetProxy(SimplifyUnits));
    }

    [MirrorVerified("PtcCalculationValueToIResultNodeTransform_MechanicalDifferentialTests")]
    private IResultNode MakeResultNode(IValueProxy valueProxy)
    {
        IScalarValueProxy scalar = valueProxy.AsScalar;
        if (scalar != null)
            return BuildScalar(scalar);

        IStringValueProxy stringValue = valueProxy.AsString;
        if (stringValue != null)
            return _resultNodeFactory.CreateStringResultNode(CleanLineBreake(stringValue.String));

        IFunctionValueProxy function = valueProxy.AsFunction;
        if (function != null)
        {
            string signature = function.ToString();
            if (signature.StartsWith("{ValueProxy for ") && signature.EndsWith("}"))
                signature = signature.Substring(16, signature.Length - 17);
            return _resultNodeFactory.CreateFunctionResultNode(signature);
        }

        IMatrixValueProxy matrix = valueProxy.AsMatrix;
        if (matrix != null)
            return BuildLazyMatrix(matrix);

        ISymbolicValueProxy symbolic = valueProxy.AsSymbolic;
        if (symbolic != null)
            return _resultNodeFactory.CreateSymbolicResultNode(symbolic.Xml);

        throw new InvalidOperationException("Logic error: Mathcad.Automation.ValueProxy was not a recognized type of value");
    }

    [MirrorVerified("PtcCalculationValueToIResultNodeTransform_MechanicalDifferentialTests")]
    private static string CleanLineBreake(string unclean, string replacement = "")
    {
        if (string.IsNullOrEmpty(unclean))
            return unclean;
        return Regex.Replace(unclean, "\\r\\n|\\r|\\n", replacement);
    }

    [MirrorVerified("PtcCalculationValueToIResultNodeTransform_MechanicalDifferentialTests")]
    private IResultNode BuildScalar(IScalarValueProxy scalar)
    {
        double real = scalar.RealPart;
        bool realIsNonzero = real != 0.0;
        double imaginary = scalar.ImagPart;
        bool imaginaryIsNonzero = imaginary != 0.0;
        IResultNode result;
        if (realIsNonzero & imaginaryIsNonzero)
            result = _resultNodeFactory.CreateComplexResultNode(real, imaginary);
        else if (imaginary != 0.0)
            result = _resultNodeFactory.CreateImaginaryResultNode(imaginary);
        else
            result = _resultNodeFactory.CreateRealResultNode(real);

        if (HasUnits(scalar))
            result = MakeUnitedValue(result, scalar.Units);
        return result;
    }

    [MirrorVerified("PtcCalculationValueToIResultNodeTransform_MechanicalDifferentialTests")]
    private IResultNode MakeUnitedValue(IResultNode result, AutomationUnitPower[] units)
    {
        IUnitedValueResultNode united = _resultNodeFactory.CreateUnitedValueResultNode();
        united.Value = result;
        for (int i = 0; i < units.Length; i++)
        {
            AutomationUnitPower unit = units[i];
            united.AddUnit(unit.Name, unit.Power.Numerator, unit.Power.Denominator);
        }
        return united;
    }

    [MirrorVerified("PtcCalculationValueToIResultNodeTransform_MechanicalDifferentialTests")]
    private static bool HasUnits(IValueProxy valueProxy) => valueProxy.Units.Length > 0;
}
