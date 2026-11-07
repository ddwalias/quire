#nullable disable
using System;
using Quire.Calculation.Core;

namespace Quire.Controls.Core.IORegions;

public partial interface IOutputRegion
{
    event Action<IResultNode> ResultChanged;
    double GetRealResult();
    double GetRealUnitedResult(string unitsOfResult);
    void ActivateAndScopeResultPrivate();
    string GetEvaluationSExp(bool getDefineForDefineEval);
    bool IsSuitableForOutputTaggingPrivate();
}
