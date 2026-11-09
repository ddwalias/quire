#nullable disable
#pragma warning disable

using System.Collections.Generic;
using System.Linq;
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
using Quire.Controls.Core.BookmarkTags;
using Quire.Controls.Core.Whiteboard;
using Quire.IO;
using Quire.PersistentDataObjects;
using Quire.Serialization;
using Quire.Services;
using Quire.FunctionalitiesLimitation;

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

    private void UpdateHyperlinkCanHaveToolTip()
    {
        HyperlinkCanHaveToolTip = !(IsInPrinting || IsInCopy) && !IsInSaving;
    }

    private void CanRemoveHyperlinks(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = IsActive && ActiveItem == null && EnumerableExtensions.IsNotEmpty(GetContextLinks());
    }

    private bool IsItemRemovedFromDocument(IWhiteboardItemInText item)
    {
        InlineUIContainer container = GetContainer(item);
        if (container != null)
        {
            return !Document.ContentStart.IsInSameDocument(container.ElementStart);
        }
        return true;
    }

    private void RestoreSavedState()
    {
        if (_savedActiveItem != null && WhiteboardItems.Contains(_savedActiveItem))
        {
            ActiveItem = _savedActiveItem;
        }
    }

    public void PasteItem(IWhiteboardItemInText item)
    {
        if (DebugTools.DevAssert(CanAddItems).Failed)
        {
            return;
        }
        AddEmbeddedItem(item, false);
        CommitItem(item);
    }

    public void ActivateActiveItem()
    {
        if (ActiveItem == null)
        {
            return;
        }
        ActiveItem.Activate(true);
        ContainerItemManager.ActivateItem(ActiveItem);
        SetItemContextMenuHandling(ActiveItem);
    }

    private void CanRemoveBookmarkTag(object sender, CanExecuteRoutedEventArgs e)
    {
        bool hasActiveItem = ActiveItem != null;
        e.CanExecute = hasActiveItem && BookmarkTag.HasBookmarkTag(ActiveItem as DependencyObject);
        e.Handled = hasActiveItem;
    }

    private void CanSetNumberFormat(object sender, CanExecuteRoutedEventArgs e)
    {
        e.CanExecute = CurrentNumberFormat != null || CurrentMatrixFormat != null;
    }

    public IRichTextWithEmbeddedItem CloneRichText(IEngineeringDocument engineeringDocument)
    {
        RichTextWithEmbeddedItem clone = new RichTextWithEmbeddedItem();
        clone.CloneContent(this, engineeringDocument, false);
        return clone;
    }

    private void ExecuteClearMathFormat(object sender, ExecutedRoutedEventArgs e)
    {
        ExecuteCommandKeepingSelection(CurrentMathFormat.ClearFormat, "ClearMathFormat");
    }

    private void ExecuteCommandKeepingSelection(Func<bool> internalCommandDo, string name)
    {
        CommandBuffer.Execute(new EmbededItemsCommandWithKeepSelection(this, name, internalCommandDo), null);
    }

    public int GetEmbeddedItemSymbolOffset(IWhiteboardItemInText item)
    {
        return textType.GetOffsetToPosition(Document, GetContainer(item).ElementStart);
    }

    private void RemoveHyperlinkUndoableCommand(Hyperlink link)
    {
        RemoveHyperlinkUndoableCommand(link != null ? EnumerableExtensions.Yield(link).ToList() : null);
    }

    private void RemoveHyperlinkUndoableCommand(List<Hyperlink> links)
    {
        if (DebugTools.DevAssert(links.IsNotEmpty()).Failed)
        {
            return;
        }
        bool undoAll = false;
        using (CommandScope commandScope = new CommandScope("Remove hyperlink from text"))
        {
            using (DeclareChangeBlock())
            {
                foreach (Hyperlink link in links)
                {
                    InlineCollection parentInlines = link.GetParentInlines();
                    if (DebugTools.DevAssert(parentInlines != null && parentInlines.Contains(link)).Failed)
                    {
                        undoAll = true;
                        break;
                    }
                    link.SetTextFormatForLinkRemoval(out List<Inline> inlines);
                    link.Inlines.RemoveAll();
                    foreach (Inline inline in inlines)
                    {
                        parentInlines.InsertBefore(link, inline);
                    }
                    ReproduceRemovedHyperlinkColors(inlines, link);
                    parentInlines.Remove(link);
                }
            }
            if (undoAll)
            {
                while (CommandBuffer.CanUndo)
                {
                    CommandBuffer.Undo();
                }
            }
        }
    }

    internal bool ContainsSimpleText(DocumentTextRange textRange)
    {
        if (EnumerateEmbeddedObjectsInRange(textRange).IsNullOrEmpty())
        {
            return textRange.EnumerateInlinesInRange<LineBreak>().IsNullOrEmpty();
        }
        return false;
    }

    protected override void OnPreviewKeyDown(KeyEventArgs e)
    {
        if (HandleLinkOpsOnPreviewKeyDown(e.Key))
        {
            e.Handled = true;
            return;
        }
        base.OnPreviewKeyDown(e);
    }

    internal DocumentTextRange GetWordRange()
    {
        return GetWordRangeAtCaretPosition(out string _, TextFromRunsStops.FindWordStrict, false);
    }

    private void SetItemContextMenuHandling(IWhiteboardItem item)
    {
        if (ContainerItemManager.IsItemActivated(item))
        {
            ContainerItemManager.ClearItemHandlesContextMenus(item);
            return;
        }
        ContainerItemManager.SetItemHandlesContextMenus(item, ContainerItemManager.GetItemHandlesContextMenus(this));
    }

    protected override void OnContextMenuOpening(ContextMenuEventArgs e)
    {
        if (!IsFocused)
        {
            return;
        }
        base.OnContextMenuOpening(e);
    }

    public void CommitItem(IWhiteboardItem item)
    {
        ICalculatable calculatable = item as ICalculatable;
        if (calculatable != null)
        {
            calculatable.Commit();
        }
    }

    private void HandleChildItemGotFocus(IWhiteboardItem item)
    {
        if (item != ActiveItem && WhiteboardItems.Contains(item))
        {
            ActiveItem = item as IWhiteboardItemInText;
        }
    }

    public bool ContainsError(EngineError error)
    {
        foreach (ICalculatable child in CalculatableChildren)
        {
            ICalculationErrorOwner owner = child as ICalculationErrorOwner;
            if (owner != null && owner.ContainsError(error))
            {
                return true;
            }
        }
        return false;
    }

    public void ShowError(EngineError error)
    {
        foreach (ICalculatable child in CalculatableChildren)
        {
            ICalculationErrorOwner owner = child as ICalculationErrorOwner;
            if (owner != null && owner.ContainsError(error))
            {
                owner.ShowError(error);
                return;
            }
        }
    }

    internal Hyperlink GetHyperlinkAtCaretPositionIncludingEdges()
    {
        return GetHyperlinkAtCaretPosition()
            ?? TextPointerExtensions.GetAdjacentHyperlink(CaretPosition, CaretPosition.LogicalDirection)
            ?? TextPointerExtensions.GetAdjacentHyperlink(CaretPosition, CaretPosition.LogicalDirection == LogicalDirection.Forward ? LogicalDirection.Backward : LogicalDirection.Forward);
    }

    private void HandleRemoveBookmarkTag(object sender, ExecutedRoutedEventArgs e)
    {
        if (DebugTools.AssertNotNull(ActiveItem).Failed || DebugTools.AssertNotNull(BookmarksManager).Failed)
        {
            return;
        }
        string bookmarkTag = BookmarkTag.GetBookmarkTag(ActiveItem as DependencyObject);
        BookmarksManager.RemoveBookmarkTagCommand(ActiveItem, bookmarkTag);
    }

    public void RemoveEmbeddedItem(IWhiteboardItemInText item)
    {
        InlineUIContainer container = GetContainer(item);
        if (DebugTools.AssertNotNull(container, "Removing an embedded item that is not hosted in the document.").Failed)
        {
            return;
        }
        SetCaretPositionAndClearSuggestedX(container.ElementEnd.GetInsertionPosition(LogicalDirection.Forward));
        if (item.BookmarksManager != null)
        {
            item.BookmarksManager = null;
        }
        container.ElementStart.Paragraph.Inlines.Remove(container);
    }

    private void RemoveItemFromList(IWhiteboardItemInText item)
    {
        if (!WhiteboardItems.Contains(item))
        {
            return;
        }
        WhiteboardItems.Remove(item);
        if (item == ActiveItem)
        {
            ActiveItem = null;
        }
        item.OnGotFocusEvent -= HandleChildItemGotFocus;
        item.RequestRemove -= HandleChildItemRequestRemove;
    }

    private InlineUIContainer GetContainer(IWhiteboardItemInText item)
    {
        IWhiteboardItemInTextWrapper wrapper = ContainerItemManager.GetParent(item) as IWhiteboardItemInTextWrapper;
        if (wrapper == null)
        {
            return null;
        }
        object parent = ContainerItemManager.GetParent(wrapper);
        Grid grid = parent as Grid;
        if (grid != null)
        {
            parent = ContainerItemManager.GetParent(grid);
        }
        return parent as InlineUIContainer;
    }

    private List<Hyperlink> GetContextLinks()
    {
        if (!Selection.IsEmpty)
        {
            return GetHyperlinksInRange(Selection);
        }
        return GetHyperlinkAtCaretPositionIncludingEdges()?.Yield().ToList();
    }

    private bool CanDrop(System.Windows.DragEventArgs args)
    {
        TextPointer position = GetPositionFromPoint(args.GetPosition(this), false);
        if (DataObjectContainsAnyEmbeddedItems(args.Data, RichTextEmbeddedItemsInfo.HasEmbeddedImagesAndMath))
        {
            return GetHyperlinkFromTextPointer(position) == null;
        }
        return true;
    }

    private void OnSetMatrixFormat(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source != this)
        {
            return;
        }
        using (CommandScope commandScope = new CommandScope(e.Command.ToString()))
        {
            DispatchCommandToItems(e.Command as RoutedUICommand, e.Parameter, PropertiesForUpdate.CurrentMatrixFormat);
        }
    }

    private void OnSetNumberFormat(object sender, ExecutedRoutedEventArgs e)
    {
        if (e.Source != this)
        {
            return;
        }
        using (CommandScope commandScope = new CommandScope(e.Command.ToString()))
        {
            DispatchCommandToItems(e.Command as RoutedUICommand, e.Parameter, e.Command == EquationCommands.ClearResultFormat ? PropertiesForUpdate.CurrentNumberFormatClear : PropertiesForUpdate.CurrentNumberFormat);
        }
    }

    private void ShowWarningDialog(NavigateUrlErrorWarningKeys messageKey, string param)
    {
        MessageBoxManager.ShowWarning(Application.Current.MainWindow, string.Format((TryFindResource(messageKey) as string) ?? "{0}", param), WpfUtils.ApplicationFullName);
    }

    private void ShowWarningDialog(NavigateUrlErrorWarningKeys messageKey)
    {
        MessageBoxManager.ShowWarning(Application.Current.MainWindow, (TryFindResource(messageKey) as string) ?? string.Empty, WpfUtils.ApplicationFullName);
    }

    private IWhiteboardItem GetDescendantItem(DependencyObject descendantObject)
    {
        IWhiteboardItem item = null;
        if (ActiveItem == null)
        {
            return item;
        }
        item = WpfUtils.TryFindAncestorIncludingSelf<IWhiteboardItem>(descendantObject, false);
        if (item == null)
        {
            return null;
        }
        if (item != this)
        {
            return null;
        }
        if (WpfUtils.TryFindVisualAncestor<IWhiteboardItemContainer>(item as DependencyObject, false) == null)
        {
            return null;
        }
        return item;
    }

    internal Uri ResolveUrl(Uri path)
    {
        if (path == null)
        {
            return null;
        }
        if (path.IsAbsoluteUri)
        {
            return path;
        }
        if (!UrlUtils.TryCreateAbsoluteUri(BaseRelativeDirectory, path, out Uri absoluteUri))
        {
            return null;
        }
        return absoluteUri;
    }

    private Hyperlink CheckAndGetExactlyOneHyperlink()
    {
        List<Hyperlink> contextLinks = GetContextLinks();
        return DebugTools.DevAssert(contextLinks != null && contextLinks.Count == 1).Failed ? null : contextLinks.FirstOrDefault();
    }

    private bool ShowQuestionWarningDialog(NavigateUrlErrorWarningKeys messageKey, string path)
    {
        return MessageBoxManager.ShowQuestion(Application.Current.MainWindow, string.Format((TryFindResource(messageKey) as string) ?? "{0}", path), WpfUtils.ApplicationFullName, MessageBoxButton.YesNo) == MessageBoxResult.Yes;
    }

    public bool HasHyperlinkInMouseContext(object context)
    {
        TextElement element = context as TextElement;
        Hyperlink link = element != null
            ? element.GetTextElementParentOrSelf<Hyperlink>()
            : GetHyperlinkFromTextPointer(GetPositionFromPoint(Mouse.GetPosition(this), false));
        return link != null;
    }

    public void AddEmbeddedItem(IWhiteboardItemInText item, bool activate)
    {
        using (CommandScope commandScope = new CommandScope("Add embedded item"))
        {
            if (activate)
            {
                ContainerItemManager.ActivateItem(item);
            }
            using (DeclareChangeBlock())
            {
                if (!Selection.IsEmpty)
                {
                    Selection.Text = "";
                }
                AddEmbeddedItem(item, null);
            }
        }
    }

    private static bool DataObjectContainsAnyEmbeddedItems(IDataObject dataObject, RichTextEmbeddedItemsInfo filter = RichTextEmbeddedItemsInfo.HasEmbeddedImagesAndMath)
    {
        if (dataObject.GetTextWithEmbeddedItemsInfo() is RichTextEmbeddedItemsInfo info)
        {
            return (info & filter) != 0;
        }
        return false;
    }

    private static bool FindInEmbeddedItem(IWhiteboardItemInText item, string text, bool startFromCaret, LogicalDirection direction, FindOptions options, List<Pair<int, int>> subscriptPositions)
    {
        WhiteboardItemSearchRestrictor restrictor = WhiteboardItemSearchRestrictor.TryCreateFindRestrictor(item);
        if (restrictor == null)
        {
            return false;
        }
        return restrictor.FindOnce(text, startFromCaret, direction, options, subscriptPositions);
    }

    private void CanModifyHyperlink(object sender, CanExecuteRoutedEventArgs e)
    {
        if (!CanModifyHyperlink())
        {
            return;
        }
        e.CanExecute = true;
        ModifyHyperlinkCommandParameter parameter = e.Parameter as ModifyHyperlinkCommandParameter;
        if (parameter != null)
        {
            parameter.ResultIsSingleLinkEditOperation = CanOpenOrEditOrCopySingleHyperlink();
        }
    }

    private void ModifyHyperlink(object sender, ExecutedRoutedEventArgs e)
    {
        if (DebugTools.DevAssert(CanModifyHyperlink()).Failed)
        {
            return;
        }
        ModifyHyperlink(e.Parameter as ModifyHyperlinkCommandParameter ?? new ModifyHyperlinkCommandParameter(true, null, null));
    }

    public override IList<IRegionPersistentData> SetRegionPersistentData(IDeS11NProvider d11NProvider, object data)
    {
        IList<IRegionPersistentData> regions = null;
        base.SetRegionPersistentData(d11NProvider, data);
        textType text = (textType)data;
        if (text.Regions != null && text.Regions.Any())
        {
            regions = _deserializationStrategy.SetRegions(text.Regions, d11NProvider);
            CaretPosition = Document.ContentStart;
        }
        return regions;
    }

    protected virtual void DeactivateActiveItem()
    {
        if (ActiveItem == null)
        {
            return;
        }
        ActiveItem.Activate(false);
        ContainerItemManager.DeactivateItem(ActiveItem);
        ICalculatable calculatable = ActiveItem as ICalculatable;
        if (calculatable != null)
        {
            ICalculatableInfo calculatableInfo = calculatable as ICalculatableInfo;
            if (calculatableInfo == null || !calculatableInfo.IsCommitted)
            {
                calculatable.Commit();
            }
        }
        SetItemContextMenuHandling(ActiveItem);
    }

    private void HandleBookmarkTag(object sender, ExecutedRoutedEventArgs e)
    {
        if (DebugTools.AssertNotNull(ActiveItem).Failed)
        {
            return;
        }
        string tag = BookmarkTag.GetBookmarkTag(ActiveItem as DependencyObject);
        BookmarkTagViewModel viewModel = new BookmarkTagViewModel(tag);
        if (DebugTools.AssertNotNull(BookmarksManager).Passed && BookmarkTagDialog.ShowBookmarkTagsDialog(viewModel, Application.Current.MainWindow))
        {
            BookmarksManager.SetBookmarkTagCommand(ActiveItem, tag, viewModel.Tag);
        }
    }

    private void DisplayHyperlink(Hyperlink link, bool showAboutBox = true)
    {
        HyperlinkProperties.SetLinkVisited(link, true);
        if (FunctionalityService.Instance.IsFunctionalityEnabled((EnumFunctionalityKey)14))
        {
            OpenHyperlinkUrl(link.NavigateUri);
            return;
        }
        if (DebugTools.DevAssert(showAboutBox).Passed)
        {
            DisplayFreemiumAboutBoxCommandParameter parameter = new DisplayFreemiumAboutBoxCommandParameter(new HyperlinkToScreenProvider(this, CaretPosition), (EnumFunctionalityKey)14);
            FunctionalitiesLimitationCommands.DisplayFreemiumAboutBox.Execute(parameter, null);
        }
    }

    private void HandleItemsListChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                IEnumerable<IWhiteboardItemInText> addedItems = e.NewItems.Cast<IWhiteboardItemInText>();
                AddItems(addedItems);
                return;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                IEnumerable<IWhiteboardItemInText> removedItems = e.OldItems.Cast<IWhiteboardItemInText>();
                RemoveItems(removedItems);
                return;
            case System.Collections.Specialized.NotifyCollectionChangedAction.Move:
                IEnumerable<IWhiteboardItemInText> movedItems = e.NewItems.Cast<IWhiteboardItemInText>();
                MoveItems(movedItems);
                return;
            default:
                DebugTools.FailAssertion("Unsupported action: {0}", e.Action);
                return;
        }
    }

    private void UpdateItemOffsetMap()
    {
        foreach (IWhiteboardItem item in WhiteboardItems)
        {
            IWhiteboardItemInText embeddedItem = item as IWhiteboardItemInText;
            if (IsItemRemovedFromDocument(embeddedItem))
            {
                _itemOffsetMap.Remove(embeddedItem);
            }
            else
            {
                _itemOffsetMap[embeddedItem] = textType.GetOffsetToPosition(Document, GetContainer(embeddedItem).ElementStart);
            }
        }
    }

    public void CopyWholeRangeXamlToDataObject(IDataObject dataObject)
    {
        string xaml = TextRange.SerializeRangeContentToXamlString();
        List<IWhiteboardItem> items = GetWhiteboardItemsInRange(TextRange).ToList();
        string xamlWithItems = InlayXamlWithEmbeddedItems(TextRange, xaml, out RichTextEmbeddedItemsInfo info, items);
        dataObject.SetData(DataFormats.Xaml, xamlWithItems ?? xaml);
        DataObjectExtensions.SetTextWithEmbeddedItemsInfo(dataObject, info);
    }

    private void OnPreviewEnter(object sender, ExecutedRoutedEventArgs e)
    {
        if (ActiveItem == null)
        {
            return;
        }
        IWhiteboardItemInText activeItem = (IWhiteboardItemInText)ActiveItem;
        if (activeItem.CanReturn())
        {
            return;
        }
        ActiveItem = null;
        Activate(true);
        InlineUIContainer container = GetContainer(activeItem);
        if (DebugTools.AssertNotNull(container, "The active embedded item is not hosted in the document.").Passed)
        {
            SetCaretPositionAndClearSuggestedX(container.ElementEnd.GetInsertionPosition(LogicalDirection.Forward));
        }
        e.Handled = true;
    }

    public void ReplacePlaceholderWithEmbeddedItem(IWhiteboardItemInText item, int symbolOffset)
    {
        TextPointer position = textType.GetPositionAtOffset(Document, symbolOffset);
        if (position != null)
        {
            Run run = position.GetAdjacentElement(LogicalDirection.Forward) as Run;
            if (run != null && run.Text == " ")
            {
                var paragraph = position.Paragraph;
                if (DebugTools.AssertNotNull(paragraph).Passed)
                {
                    DebugTools.DevAssert(RemoveInlineRecursively(paragraph.Inlines, run), "Failed to remove object placeholder, see if RemoveInlineRecursively needs to be updated");
                }
            }
            else
            {
                DebugTools.FailAssertion("Expected object placeholder (empty run) was not found, and therefore the object was inserted w/o replacing anything.");
            }
            AddEmbeddedItem(item, position);
        }
    }

    private void SetItem(InlineUIContainer container, IWhiteboardItemInText item)
    {
        IWhiteboardItemInTextWrapper oldWrapper = ContainerItemManager.GetParent(item) as IWhiteboardItemInTextWrapper;
        if (oldWrapper != null)
        {
            oldWrapper.WrappedItem = null;
        }
        IWhiteboardItemInTextWrapper wrapper = GetWrapper(container.Child);
        if (wrapper != null)
        {
            wrapper.WrappedItem = item;
            return;
        }
        Grid grid = container.Child as Grid;
        if (grid != null)
        {
            grid.Children.Clear();
            grid.Children.Add(new WhiteboardItemInTextWrapper(item));
            return;
        }
        DebugTools.FailAssertion("Unexpected InlineUIContainer.Child: " + container.Child?.GetType().FullName);
    }

    private void DeleteActiveOrSelectedItems(object sender, ExecutedRoutedEventArgs e)
    {
        if (ActiveItem != null)
        {
            RemoveItem(ActiveItem);
        }
        else
        {
            EditingCommands.Delete.Execute(null, this);
        }
        Activate(true);
    }

    private static IWhiteboardItemInTextWrapper GetWrapper(object embeddedElement)
    {
        IWhiteboardItemInTextWrapper wrapper = embeddedElement as IWhiteboardItemInTextWrapper;
        if (wrapper != null)
        {
            return wrapper;
        }
        Grid grid = embeddedElement as Grid;
        if (grid == null)
        {
            return null;
        }
        IWhiteboardItemInTextWrapper result = null;
        foreach (UIElement element in grid.Children)
        {
            IWhiteboardItemInTextWrapper candidate = element as IWhiteboardItemInTextWrapper;
            if (candidate != null)
            {
                if (DebugTools.DevAssert(result == null, "More than one wrapper hosted in the Grid of an embedded item.").Failed)
                {
                    break;
                }
                result = candidate;
            }
        }
        return result;
    }

    private void NavigateToHyperLink(Hyperlink link)
    {
        if (DebugTools.AssertNotNull(link).Failed)
        {
            return;
        }
        TextPointer position = null;
        try
        {
            position = GetPositionFromPoint(Mouse.GetPosition(this), true);
        }
        catch (Exception)
        {
        }
        if (DebugTools.DevAssert(position != null && link.ContentStart.CompareTo(position) <= 0 && link.ContentEnd.CompareTo(position) >= 0).Failed)
        {
            position = link.ElementStart;
        }
        if (!IsActive)
        {
            Activate(true);
        }
        else
        {
            Focus();
        }
        CaretPosition = position;
        DisplayHyperlink(link);
    }

    internal void CollectKeyDataForPostChangeHyperlinkInsertion(Key key, ModifierKeys keyModifiers)
    {
        if (key == Key.Tab || key == Key.Return || key == Key.Space)
        {
            int caretOffset = CaretOffset;
            _keyDownData = DebugTools.DevAssert(caretOffset >= 0).Passed ? new PreviewKeyDownData(key, caretOffset) : (PreviewKeyDownData?)null;
        }
    }

    private bool RangeContainsOnlyInlineElements(DocumentTextRange range, bool prohibitSelectionInBlockUiContainer = false)
    {
        bool result = true;
        TextPointer start = range.Start.GetInsertionPosition(LogicalDirection.Forward);
        TextPointer end = range.End.GetInsertionPosition(LogicalDirection.Backward);
        while ((start != null && start.CompareTo(end) < 0) & result)
        {
            TextPointerContext context = start.GetPointerContext(LogicalDirection.Forward);
            if (context == TextPointerContext.ElementStart)
            {
                result = start.GetAdjacentElement(LogicalDirection.Forward) is Inline;
            }
            else if (prohibitSelectionInBlockUiContainer && context == TextPointerContext.EmbeddedElement)
            {
                result = start.Parent is Inline;
            }
            start = start.GetNextContextPosition(LogicalDirection.Forward);
        }
        return result;
    }

    public bool HideFromCalculationButtonEnabled
    {
        get => _hideFromCalculationButtonEnabled;
        set
        {
            _hideFromCalculationButtonEnabled = value;
            FirePropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(HideFromCalculationButtonEnabled)));
        }
    }

    public bool HideFromCalculationButtonChecked
    {
        get => _hideFromCalculationButtonChecked;
        set
        {
            _hideFromCalculationButtonChecked = value;
            FirePropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(HideFromCalculationButtonChecked)));
        }
    }

    public NumberFormat CurrentNumberFormat
    {
        get => _currentNumberFormat;
        set
        {
            _currentNumberFormat = value;
            FirePropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(CurrentNumberFormat)));
        }
    }

    public MatrixFormat CurrentMatrixFormat
    {
        get => _currentMatrixFormat;
        set
        {
            _currentMatrixFormat = value;
            FirePropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(CurrentMatrixFormat)));
        }
    }

    public MathFormat CurrentMathFormat
    {
        get => _currentMathFormat;
        set
        {
            _currentMathFormat = value;
            FirePropertyChanged(new System.ComponentModel.PropertyChangedEventArgs(nameof(CurrentMathFormat)));
        }
    }

    public bool HyperlinkCanHaveToolTip
    {
        get => (bool)GetValue(HyperlinkCanHaveToolTipProperty);
        set => SetValue(HyperlinkCanHaveToolTipProperty, value);
    }

    public bool CanAddItems
    {
        get
        {
            if (ActiveItem == null)
            {
                return !SelectionFullyInsideHyperlink;
            }

            return false;
        }
    }

    public EngineError CurrentError
    {
        get
        {
            ICalculationErrorOwner errorOwner = ActiveItem as ICalculationErrorOwner;
            if (errorOwner != null)
            {
                return errorOwner.CurrentError;
            }

            return null;
        }
    }

    public bool CanHideFromCalculationContext => !WhiteboardItems.OfType<ICalculationHideableChild>().IsNullOrEmpty();

    public bool HasRegionsWithConverterAnnotations => this.HasRegionsWithConverterAnnotations();

    public StaticMathFormat TargetMathFormat => new StaticMathFormat(new MathFormatDobjController(this));

    public IBookmarksManager BookmarksManager { get; set; }

    public WhiteboardItemsCollection WhiteboardItems => _whiteboardItems;

    public IRegionFactory RegionFactory { get; set; }

    public IEnumerable<ICalculatable> CalculatableChildren => _childManager.CalculatableChildren;
}
