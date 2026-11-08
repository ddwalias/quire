#nullable disable
#pragma warning disable

using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using DocumentTextRange = System.Windows.Documents.TextRange;
using System.Windows.Documents;
using Quire.Controls.Core;
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
}
