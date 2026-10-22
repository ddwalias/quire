using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.EquationEditor.Tree;
using Quire.EquationEditor.Tree.Parsing;
using Quire.EquationEditor.Tree.Transform;
using Quire.EquationEditor.View;
using Quire.PersistentDataObjects;

namespace Quire.Controls;

/// <summary>
/// A math region. It displays its statement in 2D and, while editing, the linear keyboard text with a
/// live 2D preview underneath. Typing <c>:</c> produces <c>:=</c> and Ctrl+= produces the comparison <c>==</c>.
/// </summary>
public partial class EquationControl : CalculatableRegionControl
{
    private static readonly Brush WarningBrush = Frozen(Color.FromRgb(0xB0, 0x70, 0x00));

    private readonly MathView _view = new();
    private readonly MathView _preview = new() { Opacity = 0.85 };
    private readonly TextBox _editor;
    private readonly TextBlock _message = new() { FontSize = 11, TextWrapping = TextWrapping.Wrap, MaxWidth = 420, Visibility = Visibility.Collapsed };
    private readonly StackPanel _editPanel;

    private Statement _statement = EmptyStatement.Instance;
    private string _source = "";
    private ParseException? _parseError;

    public EquationControl() : this(EmptyStatement.Instance) { }

    public EquationControl(Statement statement)
    {
        _editor = new TextBox
        {
            FontFamily = new FontFamily("Cascadia Mono, Consolas"),
            FontSize = 14,
            MinWidth = 120,
            BorderThickness = new Thickness(0, 0, 0, 1),
            BorderBrush = EditBrush,
            Background = Brushes.Transparent,
            AcceptsReturn = false,
            AcceptsTab = false,
        };
        _editor.TextChanged += (_, _) => UpdatePreview();
        _editor.PreviewKeyDown += HandleEditorKeyDown;
        _editor.PreviewTextInput += HandleEditorTextInput;
        _editor.LostKeyboardFocus += (_, e) =>
        {
            if (!IsEditing || e.NewFocus is DependencyObject d && IsAncestorOf(d)) return;
            EndEdit(commit: true);
        };

        _editPanel = new StackPanel { Visibility = Visibility.Collapsed };
        _editPanel.Children.Add(_editor);
        _editPanel.Children.Add(_preview);

        var root = new StackPanel();
        root.Children.Add(_view);
        root.Children.Add(_editPanel);
        root.Children.Add(_message);
        Body = root;

        SetStatement(statement, MathNodeToLinearTransform.Transform(statement));
    }

    public Statement Statement => _statement;

    public string SourceText => _source;

    public override bool IsEmpty => _statement is EmptyStatement && string.IsNullOrWhiteSpace(_source);

    public bool HasSyntaxError => _parseError != null;

    /// <summary>The formatted result currently shown after "=", if any.</summary>
    public MathExpr? DisplayedResult => _view.Result;

    public new IResultNode Result => null;

    /// <summary>Raised whenever the committed statement changes.</summary>
    public event EventHandler? StatementChanged;

    public void SetStatement(Statement statement, string source)
    {
        var rl = MathNodeToRlTransform.Transform(statement);
        // only reuse a result that belongs to the current Rl
        bool sameMeaning = rl == Rl && _parseError == null && base.Result != null && SynchronizationStatus == SynchronizationStatus.Synchronized;
        _statement = statement;
        _source = source;
        _parseError = null;
        _view.Statement = statement;
        if (sameMeaning)
        {
            // e.g. only spacing changed: the engine won't recalculate, so keep showing the last result
            OnResult(base.Result!);
        }
        else
        {
            _view.Result = null;
            _view.ErrorSymbol = null;
            ShowMessage(null, null);
            Rl = rl;
        }
        StatementChanged?.Invoke(this, EventArgs.Empty);
    }

    // ---------------------------------------------------------------- editing

    public override void BeginEdit(string? initialText = null)
    {
        _editor.Text = initialText ?? _source;
        if (initialText == ":") _editor.Text = ":=";
        _editPanel.Visibility = Visibility.Visible;
        _view.Visibility = Visibility.Collapsed;
        IsEditing = true;
        UpdatePreview();
        Dispatcher.BeginInvoke(() =>
        {
            _editor.Focus();
            Keyboard.Focus(_editor);
            _editor.CaretIndex = _editor.Text.Length;
        }, System.Windows.Threading.DispatcherPriority.Input);
    }

    public override void EndEdit(bool commit)
    {
        if (!IsEditing) return;
        _editPanel.Visibility = Visibility.Collapsed;
        _view.Visibility = Visibility.Visible;
        bool changed = commit && !(_editor.Text == _source && _parseError == null);
        // commit before announcing the end of editing, so listeners see the new statement
        if (changed) Commit(_editor.Text.Trim());
        IsEditing = false;
        if (changed) RaiseChanged();
    }

    /// <summary>Parses <paramref name="text"/> and adopts it, or keeps it with a syntax error shown.</summary>
    public void Commit(string text)
    {
        var parsed = LinearParser.TryParseStatement(text, out var error);
        if (parsed != null)
        {
            SetStatement(parsed, text);
        }
        else
        {
            // keep the text so the user can fix it; nothing is sent to the engine
            _source = text;
            _parseError = error;
            _statement = new UnparsedStatement(text);
            _view.Statement = _statement;
            _view.Result = null;
            Rl = "(noop)";
            ShowMessage(error!.Message, ErrorBrush);
            HasError = true;
            StatementChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public override void InsertText(string text)
    {
        if (!IsEditing) BeginEdit();
        int caret = _editor.CaretIndex;
        _editor.SelectedText = text;
        _editor.CaretIndex = caret + text.Length;
        _editor.Focus();
    }

    private void UpdatePreview()
    {
        var parsed = LinearParser.TryParseStatement(_editor.Text, out var error);
        _preview.Statement = parsed;
        _preview.Visibility = parsed is null or EmptyStatement ? Visibility.Collapsed : Visibility.Visible;
        _editor.ToolTip = error?.Message;
        _editor.BorderBrush = error == null ? EditBrush : ErrorBrush;
    }

    private void HandleEditorTextInput(object sender, TextCompositionEventArgs e)
    {
        // ":" means definition, as on a Mathcad keyboard
        if (e.Text == ":" && !(_editor.CaretIndex > 0 && _editor.Text[_editor.CaretIndex - 1] == ':'))
        {
            Insert(":=");
            e.Handled = true;
        }
    }

    private void HandleEditorKeyDown(object sender, KeyEventArgs e)
    {
        switch (e.Key)
        {
            case Key.Enter:
                EndEdit(commit: true);
                RaiseLeave(LeaveDirection.Down);
                e.Handled = true;
                break;
            case Key.Tab:
                EndEdit(commit: true);
                RaiseLeave(LeaveDirection.Right);
                e.Handled = true;
                break;
            case Key.Escape:
                EndEdit(commit: false);
                RaiseLeave(LeaveDirection.Cancel);
                e.Handled = true;
                break;
            case Key.OemPlus when Keyboard.Modifiers == ModifierKeys.Control:
                Insert("==");
                e.Handled = true;
                break;
        }
    }

    private void Insert(string text)
    {
        int caret = _editor.CaretIndex;
        _editor.SelectedText = text;
        _editor.CaretIndex = caret + text.Length;
    }

    // ---------------------------------------------------------------- results

    protected override void OnResult(CalculationResult result)
    {
        if (_parseError != null) return;
        var formatted = ResultFormatter.Instance.Format(result.Value, ResultFormat);
        bool shows = _statement is EvalStatement || _statement is DefineStatement { ShowResult: true };
        _view.Result = shows ? formatted : null;
        _view.ResultIsTable = result.Value is IMatrixResultNode { IsRangeTable: true };
        _view.ErrorSymbol = result.Error?.Symbol;
        HasError = result.Error != null;
        if (result.Error != null) ShowMessage(result.Error.Message, ErrorBrush);
        else if (result.Warning != null) ShowMessage(result.Warning, WarningBrush);
        else ShowMessage(null, null);
    }

    private void ShowMessage(string? text, Brush? brush)
    {
        _message.Text = text ?? "";
        _message.Foreground = brush ?? Brushes.Black;
        _message.Visibility = text == null ? Visibility.Collapsed : Visibility.Visible;
        if (text == null) HasError = false;
    }

    public override RegionPersistentData Save() => _parseError != null
        ? new MathRegionPersistentData { Id = RegionId, Source = _source }
        : new MathRegionPersistentData { Id = RegionId, Statement = _statement };

    public static EquationControl FromData(MathRegionPersistentData data) => FromStatement(data.Statement, data.Source, data.Id);

    /// <summary>Creates a control, re-parsing kept text for statements that didn't parse when saved.</summary>
    public static EquationControl FromStatement(Statement statement, string? source = null, string? id = null)
    {
        if (statement is UnparsedStatement u) source ??= u.Source;
        var control = new EquationControl(source != null ? EmptyStatement.Instance : statement);
        if (id != null) control.RegionId = id;
        if (source != null) control.Commit(source);
        return control;
    }
}
