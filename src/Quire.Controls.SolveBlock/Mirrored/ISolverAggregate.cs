namespace Quire.Controls;

internal partial interface ISolverAggregate
{
    void AddChild(global::Quire.Calculation.Core.ICalculatable calculatable);
    void AddChildAggregate(
        global::Quire.Calculation.Core.ICalculatable calculatable,
        global::Quire.Calculation.Core.ICalculatableContainer container);
    bool IsSolverOfSymbolicContainsFind(string rl);
    void InsertChildBefore(
        global::Quire.Calculation.Core.ICalculatable insertBefore,
        global::Quire.Calculation.Core.ICalculatable calcToInsert);
    global::System.Collections.Generic.IEnumerable<global::Quire.Calculation.Core.ICalculatable> Children { get; }
    global::Quire.Calculation.Core.ICalculatable LastChild { get; }
    string DefinedRl { get; }
    void RemoveChild(global::Quire.Calculation.Core.ICalculatable calcToRemove);
    bool IsChild(global::Quire.Calculation.Core.ICalculatable calculatable);
    void MoveChildBefore(
        global::Quire.Calculation.Core.ICalculatable moveBefore,
        global::Quire.Calculation.Core.ICalculatable calcToMove);
    int GetIndexOfChild(global::Quire.Calculation.Core.ICalculatable child);
    void CommitAllChildren();
}
