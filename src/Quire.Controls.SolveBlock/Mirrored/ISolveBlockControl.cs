namespace Quire.Controls;

public partial interface ISolveBlockControl
{
    global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> GuessValues { get; }
    global::System.Collections.Generic.IEnumerable<global::Quire.Controls.Core.IWhiteboardItem> Constraints { get; }
    global::Quire.ITextStyleChangedHandler TextStyleChangedHandler { get; set; }
    global::Quire.Controls.Core.IWhiteboardItem Solver { get; }
    SolveBlockEquationTypes GetSolveBlockEquationType(global::Quire.Controls.Core.IWhiteboardItem item);
}
