using System;

namespace Quire.Controls.ExcelComponent.Model;

internal abstract partial class OleBaseComponentCalculatableWrapper
{
    protected Action EquationChangedHandler { get; set; }

    public string LhsExpression { get; protected set; }

    public string RhsExpression { get; protected set; }
}
