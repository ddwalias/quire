#nullable disable
using System.Collections.Generic;
using System.Linq;
using Quire.Calculation.Core.Support;
using Quire.Mirror;

namespace Quire.Controls.IO;

internal partial class CalculationOverrides
{
    private ISimpleCalculatable _inputOriginalValueGetter;

    private ISimpleCalculatable _inputOverridenValueSetter;

    private ISimpleCalculatable _inputOverridenValueGetter;

    private ISimpleCalculatable _outputOverridenValueGetter;

    public ISimpleCalculatable GetCalculatableOfType(CalculationOverridesTypes type)
    {
        switch (type)
        {
            case CalculationOverridesTypes.InputOriginalValueGetter:
                return _inputOriginalValueGetter;
            case CalculationOverridesTypes.InputOverriddenValueSetter:
                return _inputOverridenValueSetter;
            case CalculationOverridesTypes.InputOverriddenValueGetter:
                return _inputOverridenValueGetter;
            case CalculationOverridesTypes.OutputOverriddenValueGetter:
                return _outputOverridenValueGetter;
            default:
                DebugTools.FailAssertion("unsupported CalculationOverridesTypes");
                return null;
        }
    }

    public void SetCalculatableOfType(CalculationOverridesTypes type, ISimpleCalculatable calculatable)
    {
        switch (type)
        {
            case CalculationOverridesTypes.InputOriginalValueGetter:
                _inputOriginalValueGetter = calculatable;
                break;
            case CalculationOverridesTypes.InputOverriddenValueSetter:
                _inputOverridenValueSetter = calculatable;
                break;
            case CalculationOverridesTypes.InputOverriddenValueGetter:
                _inputOverridenValueGetter = calculatable;
                break;
            case CalculationOverridesTypes.OutputOverriddenValueGetter:
                _outputOverridenValueGetter = calculatable;
                break;
            default:
                DebugTools.FailAssertion("unsupported CalculationOverridesTypes");
                break;
        }
    }

    internal IEnumerable<ISimpleCalculatable> GetAllCalculatables()
    {
        return GetInputCalculatables().Concat(GetOutputCalculatables());
    }

    internal IEnumerable<ISimpleCalculatable> GetInputCalculatables()
    {
        if (_inputOriginalValueGetter != null)
        {
            yield return _inputOriginalValueGetter;
        }
        if (_inputOverridenValueSetter != null)
        {
            yield return _inputOverridenValueSetter;
        }
        if (_inputOverridenValueGetter != null)
        {
            yield return _inputOverridenValueGetter;
        }
    }

    internal IEnumerable<ISimpleCalculatable> GetOutputCalculatables()
    {
        if (_outputOverridenValueGetter != null)
        {
            yield return _outputOverridenValueGetter;
        }
    }

    [MirrorVerified("PtcControlsIOCalculationOverridesMechanicalDifferentialTests")]
    public CalculationOverrides() { }
}
