#nullable disable
using System.Collections.Generic;
using Quire.Calculation.Core;
using Quire.Calculation.Core.Support;

namespace Quire.Controls.IO;

internal partial interface ICalculationOverrideContainer
{
    IEnumerable<ISimpleCalculatable> CalculatableChildren { get; }
    ISimpleCalculatable SetCalculationOverrideForSExpr(string sExprKey, CalculationOverridesTypes type, ISimpleCalculatable newOverride);
    ISimpleCalculatable GetCalculationOverrideForSExpr(string sExprKey, CalculationOverridesTypes type);
    IList<ICalculatable> RemoveCalculationOverrides(CalculationOverridesTypes type);
    ISimpleCalculatable RemoveCalculationOverrideForSExpr(string sExprKey, CalculationOverridesTypes type);
    bool IsEmpty(string sExprKey);
    IEnumerable<ISimpleCalculatable> GetInputCalculationOverrides();
    IEnumerable<ISimpleCalculatable> GetOutputCalculationOverrides();
}
