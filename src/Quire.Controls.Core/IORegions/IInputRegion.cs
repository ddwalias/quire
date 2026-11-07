#nullable disable
using System;
using System.Collections.Generic;
using Quire.Undo;

namespace Quire.Controls.Core.IORegions;

public partial interface IInputRegion
{
    IList<string> GetDefinedIdentifierSExpressions();
    bool SubstituteAndCommitInputValue(string sExpr, string valueSExpr, string scalingSExpr);
    UndoableCommand CreateToggleInputCommand(Action<IInputRegion, bool> toggleInputTag, bool setTag);
    bool IsSuitableForInputTaggingPrivate();
    void ActivateAndScopeIdentifierPrivate(string idSExpr);
    string GetDefinitionSExpression(string idSExpr);
}
