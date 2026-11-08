#nullable disable
#pragma warning disable

using System;
using System.Globalization;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using DocumentTextRange = System.Windows.Documents.TextRange;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Input;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.FunctionalitiesLimitation;
using Quire.Wpf;
using Quire.Core;
using Quire.Serialization;
using Quire.PersistentDataObjects;
using Quire.Services;
using Quire.Undo;

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

    private void SetParagraphMarginToZero()
    {
        Style style = new Style(typeof(System.Windows.Documents.Paragraph));
        style.Setters.Add(new Setter(Block.MarginProperty, new Thickness(0.0)));
        Resources.Add(typeof(System.Windows.Documents.Paragraph), style);
    }

    public void SetText(string text)
    {
        System.Windows.Documents.Paragraph paragraph = new System.Windows.Documents.Paragraph(new Run(text)) { TextAlignment = TextAlignment.Center };
        Document.Blocks.Clear();
        Document.Blocks.Add(paragraph);
    }

    public void ResetText()
    {
        SetText(InitialTitleText);
        IsDirty = false;
    }

    public void ApplyInitialTextFormat(TextFormat textFormat)
    {
        if (textFormat == null)
        {
            return;
        }
        textFormat.ApplyAllTextFormatValuesToControl(this, Document.Blocks.FirstBlock);
        ChangeTextRegionAccordingToRules();
    }

    private void ChangeTextRegionAccordingToRules()
    {
        SanitizeUnsupportedFormatting();
        Width = MeasureContentWidth() + 30.0;
        Document.TextAlignment = TextAlignment.Center;
    }

    public override IList<IRegionPersistentData> SetRegionPersistentData(IDeS11NProvider d11NProvider, object data)
    {
        if (data == null)
        {
            return null;
        }
        IsDirty = true;
        return base.SetRegionPersistentData(d11NProvider, data);
    }

    protected override void PrePasting()
    {
        base.PrePasting();
        MathFormatForPaste.SetDefault();
        _pastingCommandScope = new CommandScope("Paste RichTextTitle.");
    }

    protected override void PostPasting()
    {
        try
        {
            base.PostPasting();
        }
        finally
        {
            _pastingCommandScope.Dispose();
            _pastingCommandScope = null;
            MathFormatForPaste.Instance?.Dispose();
        }
    }

    private double MeasureContentWidth()
    {
        double width = 0.0;
        foreach (Block block in Document.Blocks)
        {
            System.Windows.Documents.Paragraph paragraph = block as System.Windows.Documents.Paragraph;
            if (paragraph != null)
            {
                double paragraphWidth = MeasureParagraphWidth(paragraph);
                width = Math.Max(width, paragraphWidth);
            }
        }
        // Release IL branches on width > 0 to the return of width, so the zero case is tested first.
        if (!(width > 0.0))
        {
            return 0.0;
        }
        return width;
    }

    private double MeasureParagraphWidth(System.Windows.Documents.Paragraph paragraph)
    {
        double width = 0.0;
        foreach (Inline inline in paragraph.Inlines)
        {
            width += MeasureInlineWidth(inline);
        }
        return width;
    }

    private double MeasureRunWidth(Run run)
    {
        if (string.IsNullOrEmpty(run.Text))
        {
            return 0.0;
        }
        Typeface typeface = new Typeface(run.FontFamily, run.FontStyle, run.FontWeight, run.FontStretch);
        FormattedText text = new FormattedText(run.Text, CultureInfo.CurrentCulture, run.FlowDirection, typeface, run.FontSize, Brushes.Black, new NumberSubstitution(), 1.0);
        return text.WidthIncludingTrailingWhitespace;
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
