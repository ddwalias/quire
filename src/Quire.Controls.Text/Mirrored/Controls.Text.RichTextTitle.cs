#nullable disable
#pragma warning disable

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.FunctionalitiesLimitation;
using Quire.Wpf;
using Quire.Services;

namespace Quire.Controls.Text;

public partial class RichTextTitle
{
    private const int InnerSpacing = 30;
    private const string XYPlotControl = "XYPlotControl";
    private const string PolarPlotControl = "PolarPlotControl";

    public bool IsDirty;
    private bool _isSanitizing;
    private ITextStyleChangedHandler _textStyleChangedHandler;

    public static readonly RoutedUICommand ToggleOrientation = new RoutedUICommandWithFunctionalityKey(
        "Toggle Orientation", "ToggleOrientation", typeof(RichTextTitle), EnumFunctionalityKey.PlotsCategoryToggleAxisTitleOrientation);

    public string InitialTitleText { get; set; }

    public Action ToggleOrientationRequest { get; set; }

    private static bool IsEditingAllowed => FunctionalityService.Instance.IsFunctionalityEnabled((EnumFunctionalityKey)765);

    protected override bool SupportsLists => false;

    protected override bool SupportsIndentation => false;

    public RichTextTitle()
    {
        SetInitialTitleText();
        SetParagraphMarginToZero();
    }

    protected override void HandleTextChanged(object sender, TextChangedEventArgs e)
    {
        base.HandleTextChanged(sender, e);
        IsDirty = true;
        ChangeTextRegionAccordingToRules();
    }

    protected override void OnStyleChanged(ICollection<TextStyleData> textStyles)
    {
        SetStyleFromTextStyles(textStyles);
    }

    protected override bool ApplyTextStyleCanExecute(object parameter)
    {
        if (IsActive || IsSelected || IsKeyboardFocusWithin)
        {
            return parameter is TextStyleData;
        }
        return false;
    }

    protected override void OnGotFocus(RoutedEventArgs e)
    {
        IsReadOnly = !IsEditingAllowed;
        WhiteboardDependencyProperties.SetIsActive(this, true);
        base.OnGotFocus(e);
    }

    protected override void OnLostFocus(RoutedEventArgs e)
    {
        WhiteboardDependencyProperties.SetIsActive(this, false);
        // Release IL selects from ContentStart to ContentStart (an empty range at the start).
        Selection.Select(Document.ContentStart, Document.ContentStart);
        base.OnLostFocus(e);
    }

    protected override void OnIsInPrintingPropertyChanged(DependencyPropertyChangedEventArgs args)
    {
    }
}
