using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Transform;
using Quire.PersistentDataObjects;

namespace Quire.Controls;

public enum SolveBlockEquationTypes
{
    Definition,
    Constraint,
    Solver,
}

/// <summary>
/// Solve block: guess values, constraints and a solver line (<c>s := find(x, y)</c>). The block is a single
/// engine node; only the solver line's definitions are visible to the rest of the worksheet.
/// </summary>
public partial class SolveBlockControl : CalculatableRegionControl
{
    private static readonly Brush HeaderBrush = Frozen(Color.FromRgb(0x5A, 0x6B, 0x80));
    private static readonly Brush FrameBrush = Frozen(Color.FromRgb(0xA8, 0xB8, 0xCC));

    private readonly Dictionary<SolveBlockEquationTypes, StackPanel> _sections = [];
    private readonly List<(SolveBlockEquationTypes Type, EquationControl Line)> _lines = [];
    private readonly TextBlock _message = new() { Foreground = ErrorBrush, FontSize = 11, TextWrapping = TextWrapping.Wrap, Visibility = Visibility.Collapsed, MaxWidth = 380 };

    public SolveBlockControl(SolveBlockRegionPersistentData? data = null)
    {
        data ??= new SolveBlockRegionPersistentData();
        RegionId = data.Id;

        var root = new StackPanel { MinWidth = 220 };
        foreach (var (type, title) in new[]
                 {
                     (SolveBlockEquationTypes.Definition, "Guess Values"),
                     (SolveBlockEquationTypes.Constraint, "Constraints"),
                     (SolveBlockEquationTypes.Solver, "Solver"),
                 })
        {
            var header = new DockPanel { Margin = new Thickness(0, 2, 0, 0) };
            var add = new TextBlock
            {
                Text = "+", FontWeight = FontWeights.Bold, Foreground = HeaderBrush, Cursor = Cursors.Hand, Padding = new Thickness(6, 0, 4, 0),
                ToolTip = $"Add a line to {title}",
            };
            add.MouseLeftButtonDown += (_, e) => { AddLine(type, EmptyStatement.Instance).BeginEdit(); e.Handled = true; };
            DockPanel.SetDock(add, Dock.Right);
            if (type != SolveBlockEquationTypes.Solver) header.Children.Add(add);
            header.Children.Add(new TextBlock { Text = title, FontSize = 11, FontWeight = FontWeights.SemiBold, Foreground = HeaderBrush });
            var panel = new StackPanel { Margin = new Thickness(10, 0, 0, 4), MinHeight = 18 };
            _sections[type] = panel;
            root.Children.Add(header);
            root.Children.Add(panel);
            if (type != SolveBlockEquationTypes.Solver)
                root.Children.Add(new Border { Height = 1, Background = FrameBrush, Margin = new Thickness(0, 2, 0, 2) });
        }
        root.Children.Add(_message);

        Body = new Border
        {
            BorderBrush = FrameBrush, BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(3),
            Padding = new Thickness(6, 2, 6, 4), Background = Brushes.White, Child = root,
        };

        foreach (var g in data.Guesses) AddLine(SolveBlockEquationTypes.Definition, g);
        foreach (var c in data.Constraints) AddLine(SolveBlockEquationTypes.Constraint, c);
        AddLine(SolveBlockEquationTypes.Solver, data.Solver);
        UpdateRl();
    }

    public IReadOnlyList<(SolveBlockEquationTypes Type, EquationControl Line)> Lines => _lines;

    /// <summary>Adds an empty line to a section (guess values or constraints).</summary>
    public EquationControl AddLine(SolveBlockEquationTypes type) => AddLine(type, EmptyStatement.Instance);

    private EquationControl AddLine(SolveBlockEquationTypes type, Statement statement)
    {
        var line = EquationControl.FromStatement(statement);
        int insertAt = _lines.FindLastIndex(l => l.Type <= type) + 1;
        _lines.Insert(insertAt, (type, line));
        _sections[type].Children.Add(line);
        line.StatementChanged += (_, _) => UpdateRl();
        line.Changed += (_, _) => RaiseChanged();
        line.EditingChanged += (_, _) =>
        {
            IsEditing = _lines.Any(l => l.Line.IsEditing);
            // remove an abandoned empty line once the current input has been processed
            if (!line.IsEditing && line.IsEmpty)
                Dispatcher.BeginInvoke(() =>
                {
                    if (!line.IsEditing && line.IsEmpty && _sections[type].Children.Count > 1) RemoveLine(line);
                });
        };
        line.RequestLeave += (_, e) =>
        {
            // Enter on a guess or constraint starts a new line in the same section, like typing down a list
            // Enter on an empty line leaves the block
            if (e.Direction == LeaveDirection.Down && type != SolveBlockEquationTypes.Solver && !line.IsEmpty)
                AddLine(type, EmptyStatement.Instance).BeginEdit();
            else
                RaiseLeave(e.Direction);
        };
        return line;
    }

    private void RemoveLine(EquationControl line)
    {
        int i = _lines.FindIndex(l => l.Line == line);
        if (i < 0) return;
        _sections[_lines[i].Type].Children.Remove(line);
        _lines.RemoveAt(i);
        UpdateRl();
        RaiseChanged();
    }

    private void UpdateRl()
    {
        string Section(SolveBlockEquationTypes t, Func<Statement, string?> f) =>
            string.Join(" ", _lines.Where(l => l.Type == t).Select(l => f(l.Line.Statement)).Where(s => s != null));

        // every non-empty line yields exactly one engine child, so results map back line by line
        var guesses = Section(SolveBlockEquationTypes.Definition, s => s is EmptyStatement ? null : MathNodeToRlTransform.Transform(s));
        var constraints = Section(SolveBlockEquationTypes.Constraint, s => s switch
        {
            EmptyStatement => null,
            ExprStatement x => MathNodeToRlTransform.Expr(x.Expr),
            UnparsedStatement => "(unparsed)",
            _ => "(invalid)",
        });
        var solver = Section(SolveBlockEquationTypes.Solver, s => s is EmptyStatement ? null : MathNodeToRlTransform.Transform(s));
        Rl = $"(solveblock (guesses {guesses}) (constraints {constraints}) (solver {solver}))";
    }

    public override void BeginEdit(string? initialText = null)
    {
        var target = _lines.FirstOrDefault(l => l.Line.IsEmpty).Line ?? _lines.Last().Line;
        target.BeginEdit(initialText);
    }

    public override void EndEdit(bool commit)
    {
        foreach (var (_, line) in _lines.ToArray()) line.EndEdit(commit);
    }

    public override void InsertText(string text) =>
        (_lines.FirstOrDefault(l => l.Line.IsEditing).Line ?? _lines.Last().Line).InsertText(text);

    public override bool IsEmpty => _lines.All(l => l.Line.IsEmpty);

    protected override void OnResult(CalculationResult result)
    {
        var children = result.Children ?? [];
        int nonEmpty = 0;
        foreach (var (_, line) in _lines)
        {
            if (line.Statement is EmptyStatement) continue;
            if (nonEmpty < children.Count) line.SetResult(children[nonEmpty]);
            nonEmpty++;
        }
        HasError = result.HasError;
        _message.Text = result.Error?.Message ?? "";
        _message.Visibility = result.Error == null ? Visibility.Collapsed : Visibility.Visible;
    }

    protected override void OnResultFormatChanged()
    {
        foreach (var (_, line) in _lines) line.ApplyResultFormat(ResultFormat);
        base.OnResultFormatChanged();
    }

    public override RegionPersistentData Save()
    {
        var data = new SolveBlockRegionPersistentData { Id = RegionId };
        foreach (var (type, line) in _lines)
        {
            if (line.Statement is EmptyStatement) continue;
            switch (type)
            {
                case SolveBlockEquationTypes.Definition: data.Guesses.Add(line.Statement); break;
                case SolveBlockEquationTypes.Constraint: data.Constraints.Add(line.Statement); break;
                case SolveBlockEquationTypes.Solver: data.Solver = line.Statement; break;
            }
        }
        return data;
    }
}
