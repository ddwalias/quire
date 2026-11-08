#nullable disable
#pragma warning disable

using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;
using Quire.Controls.Core;
using Quire.Undo;

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
}
