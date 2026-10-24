#nullable disable
using Quire.Mirror;

namespace Quire.Core;

public partial class SynchFontSizeOnScriptRunsWrapperCmd
{
    private readonly double _newFntSize;
    private readonly global::System.ValueTuple<int, global::System.Windows.Documents.LogicalDirection> _rangeStart;
    private readonly global::System.ValueTuple<int, global::System.Windows.Documents.LogicalDirection> _rangeEnd;
    private readonly global::System.Windows.Documents.FlowDocument _document;
    private readonly global::Quire.Undo.CompoundCommand _nestedCommand;
    private readonly global::Quire.Delegate0 _internalCmdDelegate;
    private global::System.Collections.Generic.Dictionary<int, double> _dicScriptElementsSzs;

    [MirrorVerified("Release IL")]
    public static SynchFontSizeOnScriptRunsWrapperCmd CreateSynchFontSizeOnScriptRunsWrapperCmd(global::System.Windows.Documents.TextRange range, double newFontSize, global::Quire.Delegate0 internalCmdDelegate, string name)
    {
        SynchFontSizeOnScriptRunsWrapperCmd synchFontSizeOnScriptRunsWrapperCmd = null;
        if (range.Start.DocumentStart.Parent is global::System.Windows.Documents.FlowDocument flowDocument && flowDocument.Parent is global::Quire.Core.ITextFormatOwner)
        {
            synchFontSizeOnScriptRunsWrapperCmd = new SynchFontSizeOnScriptRunsWrapperCmd(range, newFontSize, flowDocument, internalCmdDelegate, name);
        }
        if (synchFontSizeOnScriptRunsWrapperCmd == null)
        {
            DebugTools.FailAssertion(null, global::System.Array.Empty<object>());
        }
        return synchFontSizeOnScriptRunsWrapperCmd;
    }

    private SynchFontSizeOnScriptRunsWrapperCmd(global::System.Windows.Documents.TextRange range, double newFontSize, global::System.Windows.Documents.FlowDocument ownerDocument, global::Quire.Delegate0 internalCmdDelegate, string name)
    {
        _newFntSize = newFontSize;
        _dicScriptElementsSzs = global::System.Linq.Enumerable.ToDictionary(
            global::System.Linq.Enumerable.OfType<global::Quire.Wpf.ICustomFontSizeOwner>(global::Quire.Wpf.TextRangeExtension.EnumerateInlinesInRange<global::System.Windows.Documents.Run>(range)),
            (global::Quire.Wpf.ICustomFontSizeOwner run) => run.GetHashCode(),
            (global::Quire.Wpf.ICustomFontSizeOwner run) => run.FontSizeForReport);
        _document = ownerDocument;
        _rangeStart = (_document.ContentStart.GetOffsetToPosition(range.Start), range.Start.LogicalDirection);
        _rangeEnd = (_document.ContentEnd.GetOffsetToPosition(range.End), range.End.LogicalDirection);
        _nestedCommand = new global::Quire.Undo.CompoundCommand();
        _internalCmdDelegate = internalCmdDelegate;
        Name = name;
    }

    [MirrorVerified("Release IL")]
    private global::System.Windows.Documents.TextRange GetRange()
    {
        return new global::System.Windows.Documents.TextRange(_document.ContentStart.GetPositionAtOffset(_rangeStart.Item1, _rangeStart.Item2), _document.ContentEnd.GetPositionAtOffset(_rangeEnd.Item1, _rangeEnd.Item2));
    }

    [MirrorVerified("Release IL")]
    private global::System.Windows.Documents.TextRange GetExtendedRange()
    {
        global::System.Windows.Documents.TextPointer textPointer = _document.ContentStart.GetPositionAtOffset(_rangeStart.Item1, _rangeStart.Item2);
        global::System.Windows.Documents.TextPointer textPointer2 = _document.ContentEnd.GetPositionAtOffset(_rangeEnd.Item1, _rangeEnd.Item2);
        if (textPointer?.GetAdjacentElement(global::System.Windows.Documents.LogicalDirection.Backward) is global::System.Windows.Documents.Run run)
        {
            textPointer = run.ContentEnd;
        }
        if (textPointer2?.GetAdjacentElement(global::System.Windows.Documents.LogicalDirection.Forward) is global::System.Windows.Documents.Run run2)
        {
            textPointer2 = run2.ContentStart;
        }
        return new global::System.Windows.Documents.TextRange(textPointer, textPointer2);
    }

    [MirrorVerified("Release IL")]
    public bool Do()
    {
        using (new global::Quire.Undo.CommandScope(_nestedCommand))
        {
            _internalCmdDelegate();
        }
        return _nestedCommand.Count >= 1;
    }

    public void Undo()
    {
        global::System.Collections.Generic.List<global::Quire.Wpf.ICustomFontSizeOwner> items = global::Quire.Wpf.TextRangeExtension.EnumerateInlinesInRange<global::System.Windows.Documents.Run>(GetExtendedRange()).OfType<global::Quire.Wpf.ICustomFontSizeOwner>().ToList();
        global::Quire.EnumerableExtensions.Each(items, (global::Quire.Wpf.ICustomFontSizeOwner scriptElement) =>
        {
            if (_dicScriptElementsSzs.TryGetValue(scriptElement.GetHashCode(), out double value))
            {
                scriptElement.FontSizeToRestore = value;
            }
        });
        _nestedCommand.Undo();
        global::Quire.EnumerableExtensions.Each(items, (global::Quire.Wpf.ICustomFontSizeOwner scriptElement) =>
        {
            scriptElement.FontSizeToRestore = null;
        });
    }

    public void Redo()
    {
        global::System.Collections.Generic.List<global::Quire.Wpf.ICustomFontSizeOwner> items = global::Quire.Wpf.TextRangeExtension.EnumerateInlinesInRange<global::System.Windows.Documents.Run>(GetRange()).OfType<global::Quire.Wpf.ICustomFontSizeOwner>().ToList();
        global::Quire.EnumerableExtensions.Each(items, (global::Quire.Wpf.ICustomFontSizeOwner scriptElement) =>
        {
            scriptElement.FontSizeToRestore = _newFntSize;
        });
        _nestedCommand.Redo();
        global::Quire.EnumerableExtensions.Each(items, (global::Quire.Wpf.ICustomFontSizeOwner scriptElement) =>
        {
            scriptElement.FontSizeToRestore = null;
        });
    }

    [MirrorVerified("PtcCoreSynchFontSizeOnScriptRunsWrapperCmdMechanicalDifferentialTests")]
    public string Name { get; }
}
