using System.Collections.Generic;

namespace Quire.Controls;

internal partial class SolverAggregate
{
    private readonly List<global::Quire.Calculation.Core.ICalculatable> _calculatableChildren;

    public string DefinedRl { get; private set; }
    public string DefinedParams { get; private set; }
    private string SolverName { get; set; }
    private string SolverTag { get; set; }
    private string[] SolverArgs { get; set; }
    private string LeftOfSolverApply { get; set; }
    private string RightOfSolverApply { get; set; }

    public global::Quire.Calculation.Core.ICalculatable LastChild
    {
        get
        {
            int count = _calculatableChildren.Count;
            if (count == 0)
                return null;
            return ((global::Quire.Controls.SolverAggregateChildWrapper)_calculatableChildren[count - 1])
                .WrappedCalculatable;
        }
    }

    public SolverAggregate(global::Quire.Calculation.Core.ICalculatable calculatable)
        : base(calculatable)
    {
        _calculatableChildren = new List<global::Quire.Calculation.Core.ICalculatable>();
        EquationType = SolveBlockEquationTypes.Solver;
    }

    public bool IsSolverOfSymbolicContainsUndefineSubscripts(string rl)
    {
        return rl.Contains("undefine-subscripts");
    }
}
