#nullable disable
using System;
using System.Text.RegularExpressions;
using Quire.Automation;
using Quire.Calculation.Core;

namespace Quire.Calculation;

public partial class ValueToIResultNodeTransform : IValueToIResultNodeTransform
{
    private readonly IResultNodeFactory _resultNodeFactory;

    public bool SimplifyUnits { get; set; }

    public ValueToIResultNodeTransform(IResultNodeFactory resultNodeFactory)
    {
        _resultNodeFactory = resultNodeFactory;
    }

    public IResultNode Transform(IAutomationValue v)
    {
        if (v == null)
        {
            return _resultNodeFactory.CreateEmptyResultNode();
        }
        return MakeResultNode(v);
    }

    private IResultNode MakeResultNode(IAutomationValue v)
    {
        if (v == null)
        {
            return _resultNodeFactory.CreateEmptyResultNode();
        }
        return MakeResultNode(v.GetProxy(SimplifyUnits));
    }

    private IResultNode MakeResultNode(IValueProxyWrapper vproxyWrapper)
    {
        return MakeResultNode((vproxyWrapper as ValueProxyWrapper).Proxy);
    }

    private IResultNode MakeResultNode(IValueProxy vproxy)
    {
        IScalarValueProxy asScalar = vproxy.AsScalar;
        if (asScalar != null)
        {
            return BuildScalar(asScalar);
        }
        IStringValueProxy asString = vproxy.AsString;
        if (asString != null)
        {
            return _resultNodeFactory.CreateStringResultNode(CleanLineBreake(asString.String));
        }
        IFunctionValueProxy asFunction = vproxy.AsFunction;
        if (asFunction != null)
        {
            string text = asFunction.ToString();
            if (text.StartsWith("{ValueProxy for ") && text.EndsWith("}"))
            {
                text = text.Substring(16, text.Length - 17);
            }
            return _resultNodeFactory.CreateFunctionResultNode(text);
        }
        IMatrixValueProxy asMatrix = vproxy.AsMatrix;
        if (asMatrix != null)
        {
            return BuildLazyMatrix(asMatrix);
        }
        ISymbolicValueProxy asSymbolic = vproxy.AsSymbolic;
        if (asSymbolic != null)
        {
            return _resultNodeFactory.CreateSymbolicResultNode(asSymbolic.Xml);
        }
        throw new InvalidOperationException("Logic error: Mathcad.Automation.ValueProxy was not a recognized type of value");
    }

    private static string CleanLineBreake(string unclean, string replacement = "")
    {
        if (string.IsNullOrEmpty(unclean))
        {
            return unclean;
        }
        return Regex.Replace(unclean, "\\r\\n|\\r|\\n", replacement);
    }

    private IResultNode BuildScalar(IScalarValueProxy scalar)
    {
        double realPart = scalar.RealPart;
        bool flag = realPart != 0.0;
        double imagPart = scalar.ImagPart;
        bool flag2 = imagPart != 0.0;
        IResultNode result;
        if (flag & flag2)
        {
            result = _resultNodeFactory.CreateComplexResultNode(realPart, imagPart);
        }
        else if (flag2)
        {
            result = _resultNodeFactory.CreateImaginaryResultNode(imagPart);
        }
        else
        {
            result = _resultNodeFactory.CreateRealResultNode(realPart);
        }
        if (HasUnits(scalar))
        {
            result = MakeUnitedValue(result, scalar.Units);
        }
        return result;
    }

    private IResultNode BuildLazyMatrix(IMatrixValueProxy matrix)
    {
        IResultNode result = _resultNodeFactory.CreateLazyMatrixResultNode(new MatrixValueProxyWrapper(matrix), MakeResultNode);
        if (matrix.IsUniform && HasUnits(matrix))
        {
            result = MakeUnitedValue(result, matrix.Units);
        }
        return result;
    }

    private IResultNode MakeUnitedValue(IResultNode result, Quire.Automation.UnitPower[] units)
    {
        IUnitedValueResultNode unitedValueResultNode = _resultNodeFactory.CreateUnitedValueResultNode();
        unitedValueResultNode.Value = result;
        foreach (Quire.Automation.UnitPower unitPower in units)
        {
            unitedValueResultNode.AddUnit(unitPower.Name, unitPower.Power.Numerator, unitPower.Power.Denominator);
        }
        return unitedValueResultNode;
    }

    private static bool HasUnits(IValueProxy valueProxy)
    {
        return valueProxy.Units.Length != 0;
    }
}
