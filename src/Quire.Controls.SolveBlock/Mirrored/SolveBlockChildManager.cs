namespace Quire.Controls;

internal partial class SolveBlockChildManager
{
    public SolveBlockChildManager(
        global::System.Collections.Generic.IEnumerable<object> getChildren,
        global::System.Action<global::Quire.Calculation.Core.ICalculatable> fireCalculatableAdded,
        global::System.Action<global::Quire.Calculation.Core.ICalculatable> fireCalculatableRemoved,
        global::System.Action<global::Quire.Calculation.Core.ICalculatable> fireCalculatableMoved,
        global::System.Func<global::Quire.Calculation.Core.ICalculatable, bool, global::Quire.Calculation.Core.ICalculatable> createCalculatableWrapper,
        global::System.Func<global::Quire.Calculation.Core.ICalculatable, global::Quire.Calculation.Core.ICalculatable> lookupCalculatableWrapper,
        global::System.Action<global::Quire.Calculation.Core.ICalculatable> removeCalculatableWrapper,
        global::System.Action<global::Quire.Calculation.Core.ICalculatable, bool> notifyHiddenStateChanged,
        global::System.Action notifyContainerChildrenChanged)
        : base(
            getChildren,
            fireCalculatableAdded,
            fireCalculatableRemoved,
            fireCalculatableMoved,
            createCalculatableWrapper,
            lookupCalculatableWrapper,
            removeCalculatableWrapper,
            notifyHiddenStateChanged,
            notifyContainerChildrenChanged)
    {
    }

    public new void ChildAddedCore(object child)
    {
        base.ChildAddedCore(child);
    }

    public new void ChildRemovedCore(object child)
    {
        base.ChildRemovedCore(child);
    }
}
