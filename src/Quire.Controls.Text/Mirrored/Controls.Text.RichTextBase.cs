#nullable disable
#pragma warning disable

using System;
using System.ComponentModel;
using System.Windows.Input;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DocumentTextRange = System.Windows.Documents.TextRange;
using System.Windows.Documents;
using Quire.Controls.Core;
using Quire.Core;
using Quire.Undo;
using Quire.Controls.Core.Whiteboard;
using Quire.Wpf;

namespace Quire.Controls.Text;

public partial class RichTextBase
{
    public virtual string GetHelpContext()
    {
        return RichTextBaseConstants.RichTextHelpContext;
    }

    public bool HighlightAll(string text, FindOptions options)
    {
        throw new NotImplementedException();
    }

    internal virtual void CopyImageToTempBitmap(Image image)
    {
        ImageUtils.CopyImageToTempBitmap(image);
    }

    public static string GetTextInRange(DocumentTextRange range)
    {
        return range.Text;
    }

    private void IsSelectedChangedHandler()
    {
        UpdateCurrentFormats();
    }

    protected void ClearInsertionPointsCache()
    {
        _cachedInsertionPoints = null;
    }

    private void ClearIsInSerialization()
    {
        ClearValue(IsInSerializationProperty);
    }

    public void ClearRevertCoerceAfterSerializationProcess()
    {
        ClearValue(RevertCoerceAfterSerializationProcessProperty);
    }

    private void ClearTextFormatCache()
    {
        _textFormat = null;
    }

    public void DisableSpellCheck()
    {
        ApplySpellCheckValue(false);
    }

    protected virtual void OnIsInPrintingPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
        HandleIsInPrintingChanged(args);
    }

    protected virtual void OnIsInSavingCLRPropertyChanged(bool newValue)
    {
        WrapUndoAtSerialization(newValue);
    }

    protected static void RemoveBaselineOffsetFromImage(Image imageToClean)
    {
        imageToClean.ClearValue(TextBlock.BaselineOffsetProperty);
    }

    public virtual void SimulateMouseClick(Point location, int numberOfClicks)
    {
        Activate(true);
    }

    private void CacheScriptStateInRange(DocumentTextRange range)
    {
        _cacheSelectedScriptElementsOffsets = TextRangeExtension.GetRangeItemsOffsets<CustomScriptRunBase>(range);
    }

    protected void FireGotFocusEvent()
    {
        OnGotFocusEvent.Invoke(this);
    }

    public bool GetIsInSerialization()
    {
        return (bool)GetValue(IsInSerializationProperty);
    }

    internal TextPointer GetPointerToInsertionPoint(int insertionPoint)
    {
        return InsertionPoints.ElementAtOrDefault(insertionPoint);
    }

    protected virtual void HandleIsOnVisiblePageChanged()
    {
        StartOrStopSpellCheckActivationTimer(CanEnableSpellCheck);
    }

    protected virtual void HandleIsVisibleChanged()
    {
        StartOrStopSpellCheckActivationTimer(CanEnableSpellCheck);
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        HandleSelectionChanged(null, null);
    }

    private void SetIsInSerialization()
    {
        SetValue(IsInSerializationProperty, true);
    }

    public void SetRevertCoerceAfterSerializationProcess()
    {
        SetValue(RevertCoerceAfterSerializationProcessProperty, true);
    }

    protected virtual void OnIsInCopyPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
        WrapUndoAtSerialization((bool)args.NewValue);
    }

    private void WrapUndoAtSerialization(bool bStartCommandScope)
    {
        if (bStartCommandScope)
        {
            StartCommandScope();
            return;
        }
        EndCommandScope();
    }

    protected override bool ShouldSerializeProperty(DependencyProperty dp)
    {
        if (dp == SpellCheck.IsEnabledProperty)
        {
            return false;
        }
        return base.ShouldSerializeProperty(dp);
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
        base.OnGotFocus(e);
        OnGotFocusEvent.Invoke(this);
    }

    private void CacheTextFormat()
    {
        _textFormat = TextFormat.GetTextFormatForWholeDocument(Document);
    }

    public virtual void SetSelection(int fromOffset, int toOffset)
    {
        Selection.Select(GetPointerToInsertionPoint(fromOffset), GetPointerToInsertionPoint(toOffset));
    }

    protected override void OnDrop(System.Windows.DragEventArgs dragEventArgs)
    {
        IsOnDrop = true;
        base.OnDrop(dragEventArgs);
        IsOnDrop = false;
    }

    protected virtual void ExecuteToggleScriptUndoCommand(DocumentTextRange toggleRange, ScriptState toggleToState)
    {
        CommandBuffer.Execute(ToggleScriptUndoCommand.CreateToggleScriptUndoCommand(this, toggleRange, toggleToState, true), null);
    }

    protected virtual void PastingHandler(object sender, DataObjectPastingEventArgs e)
    {
        _pasteDataObject = e.DataObject;
        PrePasting();
    }

    private void AttemptActivatingSpellCheck()
    {
        StartOrStopSpellCheckActivationTimer(false);
        if (CanEnableSpellCheck)
        {
            ApplySpellCheckValue(true);
        }
    }

    protected override void OnPreviewTextInput(TextCompositionEventArgs e)
    {
        if (EnumerableExtensions.IsNotEmpty(e.Text))
        {
            CacheSubscriptStateOnTypeOver(true);
        }
        base.OnPreviewTextInput(e);
    }

    public void Copy()
    {
        DocumentAttachedProperties.SetIsInCopy(this, true);
        try
        {
            base.Copy();
        }
        finally
        {
            DocumentAttachedProperties.ClearIsInCopy(this);
        }
    }

    protected void FirePropertyChanged(PropertyChangedEventArgs e)
    {
        PropertyChangedEventHandler handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, e);
        }
    }
}
