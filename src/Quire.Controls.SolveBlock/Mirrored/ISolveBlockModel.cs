namespace Quire.Controls;

internal partial interface ISolveBlockModel
{
    global::Quire.Controls.Core.IWhiteboardItem SolverItem { get; }
    global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> GuessValueItems { get; }
    global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> ConstraintItems { get; }
    int ChildErrorCountTotal { get; }
    event global::System.Action ChildCalcErrorChanged;
}
