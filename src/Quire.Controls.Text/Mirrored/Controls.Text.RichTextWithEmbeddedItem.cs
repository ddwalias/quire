#nullable disable
#pragma warning disable

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using DocumentTextRange = System.Windows.Documents.TextRange;
using System.Windows.Input;
using System.Windows.Navigation;
using Quire.Controls.Core;
using Quire.Wpf;
using Quire.Undo;
using Quire.Calculation.Core;

namespace Quire.Controls.Text;

public partial class RichTextWithEmbeddedItem
{

    private void OnToggleHideFromCalculationContext(object sender, ExecutedRoutedEventArgs e)
    {
        ToggleHideFromCalculationContext();
    }

    public void Accept(ISpecialCopyVisitor visitor)
    {
        visitor.Visit(this);
    }

    private void ClearSavedState()
    {
        _savedActiveItem = null;
    }

    private void ClearState()
    {
        ActiveItem = null;
    }

    public virtual object Clone()
    {
        return CloneRichText(null);
    }

    protected override void OnStyleChanged(ICollection<TextStyleData> TextStyles)
    {
        base.OnStyleChanged(TextStyles);
    }

    public override void EndInit()
    {
        base.EndInit();
        RepairWrapperHosts();
    }

    protected static TextPointer GetLineStartPosition(TextPointer current, int position)
    {
        return current.GetLineStartPosition(position);
    }

    protected static TextPointer GetFirstLine(FlowDocument doc)
    {
        return GetLineStartPosition(doc.ContentStart, 0);
    }

    protected static TextPointer GetLastLine(FlowDocument doc)
    {
        return GetLineStartPosition(doc.ContentEnd, 0);
    }

    public void RemoveItem(IWhiteboardItem item)
    {
        RemoveEmbeddedItem(item as IWhiteboardItemInText);
    }

    private void SaveState()
    {
        _savedActiveItem = ActiveItem;
    }

    public void UNITTEST_ExecuteAddItemToListCommand(IWhiteboardItemInText item)
    {
        ExecuteAddItemToListCommand(item);
    }

    private void ExecuteDecreaseFontSize(object sender, ExecutedRoutedEventArgs e)
    {
        CurrentMathFormat.DecreaseFontSize();
    }

    private void ExecuteIncreaseFontSize(object sender, ExecutedRoutedEventArgs e)
    {
        CurrentMathFormat.IncreaseFontSize();
    }

    private void CanOpenOrEditOrCopySingleHyperlink(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = CanOpenOrEditOrCopySingleHyperlink();
    }

    private bool CanOpenOrEditOrCopySingleHyperlink()
    {
        return GetContextLinks()?.Count == 1;
    }

    private void CanToggleHideFromCalculationContext(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = HideFromCalculationButtonEnabled;
    }

    public void UNITTEST_ExecuteRemoveItemFromListCommand(IWhiteboardItemInText item)
    {
        ExecuteRemoveItemFromListCommand(item);
    }

    public List<Hyperlink> GetHyperlinksInRange(DocumentTextRange range, bool allowEmpty = false)
    {
        return TextRangeExtension.EnumerateHyperlinksInRange(range, allowEmpty).ToList();
    }

    private void RemoveHyperlinks(object sender, ExecutedRoutedEventArgs e)
    {
        RemoveHyperlinkUndoableCommand(GetContextLinks());
    }

    public void AddItem(IWhiteboardItem item)
    {
        AddEmbeddedItem(item as IWhiteboardItemInText, true);
    }

    private void CanExecuteMathFormatCommands(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = CurrentMathFormat != null;
    }

    private Hyperlink GetHyperlinkFromTextPointer(TextPointer ptr)
    {
        if (ptr == null)
        {
            return null;
        }
        return TextPointerExtensions.GetTextElementFromTextPointer<Hyperlink>(ptr);
    }

    private void HandleChildItemRequestRemove(IWhiteboardItem item, Direction dir)
    {
        RemoveItem(item);
        Activate(true);
    }

    protected override void OnIsInCopyPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
        base.OnIsInCopyPropertyChanged(args);
        UpdateHyperlinkCanHaveToolTip();
    }

    protected override void OnIsInPrintingPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
        base.OnIsInPrintingPropertyChanged(args);
        UpdateHyperlinkCanHaveToolTip();
    }

    protected override void OnIsInSavingCLRPropertyChanged(bool newValue)
    {
        base.OnIsInSavingCLRPropertyChanged(newValue);
        UpdateHyperlinkCanHaveToolTip();
    }

    protected static TextPointer GetNextLine(TextPointer line)
    {
        if (line != null)
        {
            return GetLineStartPosition(line, 1);
        }
        return null;
    }

    protected virtual void FireCalculatableAdded(ICalculatable calc)
    {
        if (CalculatableAdded != null)
        {
            CalculatableAdded(this, calc);
        }
    }

    protected virtual void FireCalculatableRemoved(ICalculatable calc)
    {
        if (CalculatableRemoved != null)
        {
            CalculatableRemoved(calc);
        }
    }

    protected virtual void FireCalculatableMoved(ICalculatable calc)
    {
        if (CalculatableMoved != null)
        {
            CalculatableMoved(calc);
        }
    }

    private IWhiteboardItemInText GetItem(InlineUIContainer container)
    {
        return GetWrapper(container.Child)?.WrappedItem;
    }

    private void ProhibitMouseClickOnItem(DependencyObject obj, bool prohibit)
    {
        if (obj == null)
        {
            return;
        }
        obj.SetValue(AttachedProperties.ProhibitMouseClickProperty, prohibit);
    }

    private void CanDeleteActiveOrSelectedItems(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = ActiveItem != null || SelectionContainsEmbeddedItemsOnly;
    }

    internal List<Hyperlink> GetHyperlinksFromWholeDocument()
    {
        return GetHyperlinksInRange(new DocumentTextRange(Document.ContentStart, Document.ContentEnd), false);
    }

    internal Hyperlink GetHyperlinkAtCaretPosition()
    {
        if (Selection.IsEmpty)
        {
            return GetHyperlinkFromTextPointer(CaretPosition);
        }
        return null;
    }

    private void CanBookmarkTag(object sender, CanExecuteRoutedEventArgs e)
    {
        bool hasActiveItem = ActiveItem != null;
        e.CanExecute = hasActiveItem;
        e.Handled = hasActiveItem;
    }

    protected override void OnDragOver(System.Windows.DragEventArgs e)
    {
        base.OnDragOver(e);
        if (!CanDrop(e))
        {
            e.Effects = DragDropEffects.None;
        }
    }

    public override void SetSelection(int fromOffset, int toOffset)
    {
        ActiveItem = null;
        Activate(true);
        base.SetSelection(fromOffset, toOffset);
    }

    private void OpenHyperlink(object sender, ExecutedRoutedEventArgs e)
    {
        Hyperlink link = CheckAndGetExactlyOneHyperlink();
        if (link != null)
        {
            link.RaiseEvent(new RequestNavigateEventArgs(link.NavigateUri, string.Empty));
        }
    }
}
