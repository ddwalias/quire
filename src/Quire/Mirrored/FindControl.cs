#nullable disable
#pragma warning disable

using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using Quire.Controls.Core.Whiteboard;
using Quire.Wpf;

namespace Quire;

public partial class FindControl
{
    internal FindControl _this;
    internal FindReplaceRichTextBox FindRichBox;
    internal global::DevComponents.WpfRibbon.ButtonDropDown FindPreviousButton;
    internal global::DevComponents.WpfRibbon.ButtonDropDown FindNextButton;
    internal FindReplaceRichTextBox ReplaceWithRichText;
    internal global::DevComponents.WpfRibbon.ButtonDropDown ReplaceButton;
    internal TextBlock ReplaceButtonTooltipTextBlock;
    private bool _contentLoaded;

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("Release IL: Roslyn emits dup for the 'as' result where Mathcad stores it in a local (stloc.0, ldloc.0) before the DevAssert null test")]
    private FindUserProperties GetProperties() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    public FindControl()
    {
        InitializeComponent();
    }

    private void FindRichText_TextChanged(object sender, TextChangedEventArgs e)
    {
        FindReplaceRichTextBox findRichTextBox = sender as FindReplaceRichTextBox;
        string findText = ExtractPlainText(findRichTextBox);
        FindUserProperties properties = GetProperties();
        findRichTextBox?.GetSubscriptPositions(properties.FindTextSubscriptPositions);
        properties.FindText = findText;
        StripFormatting(findRichTextBox);
    }

    private void ReplaceRichText_TextChanged(object sender, TextChangedEventArgs e)
    {
        FindReplaceRichTextBox replaceRichTextBox = sender as FindReplaceRichTextBox;
        string replaceText = ExtractPlainText(replaceRichTextBox);
        FindUserProperties properties = GetProperties();
        replaceRichTextBox?.GetSubscriptPositions(properties.ReplaceTextSubscriptPositions);
        properties.ReplaceText = replaceText;
        StripFormatting(replaceRichTextBox);
    }

    private string ExtractPlainText(FindReplaceRichTextBox rtb)
    {
        return rtb?.Text?.Trim(new char[2] { '\r', '\n' }) ?? string.Empty;
    }

    private void StripFormatting(FindReplaceRichTextBox rtb)
    {
        foreach (Run run in rtb.TextRange.EnumerateRunsInRange())
        {
            run.ClearValue(TextElement.FontSizeProperty);
            run.ClearValue(TextElement.ForegroundProperty);
            run.ClearValue(TextElement.BackgroundProperty);
            run.ClearValue(TextElement.FontWeightProperty);
            run.ClearValue(TextElement.FontStyleProperty);
            run.ClearValue(Inline.TextDecorationsProperty);
            run.ClearValue(TextElement.FontFamilyProperty);
            run.ClearValue(TextElement.FontStretchProperty);
        }
    }

    private void FindReplaceRichText_GotFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        FindReplaceRichTextBox richTextBox = sender as FindReplaceRichTextBox;
        WhiteboardDependencyProperties.SetIsActive(richTextBox, value: true);
        FindUserProperties properties = GetProperties();
        properties.FindReplaceIsActive = true;
        properties.IsSubscriptAndActive = richTextBox.IsSubscript;
    }

    private void FindReplaceRichText_LostFocus(object sender, KeyboardFocusChangedEventArgs e)
    {
        WhiteboardDependencyProperties.SetIsActive(sender as FindReplaceRichTextBox, value: false);
        FindUserProperties properties = GetProperties();
        properties.FindReplaceIsActive = false;
        properties.IsSubscriptAndActive = false;
    }

    private void FindReplaceRichText_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == "IsSubscript" && sender is FindReplaceRichTextBox richTextBox)
        {
            GetProperties().IsSubscriptAndActive = richTextBox.IsSubscript && WhiteboardDependencyProperties.GetIsActive(richTextBox);
        }
    }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("Release IL: Mathcad DevAssert condition keeps one more instruction than the idiomatic || form")]
    private void FindReplaceRichText_PreviewKeyDown(object sender, KeyEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }

        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire;component/statusbarcontrols/findcontrol.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
            case 1:
                _this = (FindControl)target;
                return;
            case 2:
                FindRichBox = (FindReplaceRichTextBox)target;
                FindRichBox.TextChanged += FindRichText_TextChanged;
                FindRichBox.PropertyChanged += FindReplaceRichText_PropertyChanged;
                FindRichBox.GotKeyboardFocus += FindReplaceRichText_GotFocus;
                FindRichBox.LostKeyboardFocus += FindReplaceRichText_LostFocus;
                FindRichBox.PreviewKeyDown += FindReplaceRichText_PreviewKeyDown;
                return;
            case 3:
                FindPreviousButton = (global::DevComponents.WpfRibbon.ButtonDropDown)target;
                return;
            case 4:
                FindNextButton = (global::DevComponents.WpfRibbon.ButtonDropDown)target;
                return;
            case 5:
                ReplaceWithRichText = (FindReplaceRichTextBox)target;
                ReplaceWithRichText.TextChanged += ReplaceRichText_TextChanged;
                ReplaceWithRichText.PropertyChanged += FindReplaceRichText_PropertyChanged;
                ReplaceWithRichText.GotKeyboardFocus += FindReplaceRichText_GotFocus;
                ReplaceWithRichText.LostKeyboardFocus += FindReplaceRichText_LostFocus;
                ReplaceWithRichText.PreviewKeyDown += FindReplaceRichText_PreviewKeyDown;
                return;
            case 6:
                ReplaceButton = (global::DevComponents.WpfRibbon.ButtonDropDown)target;
                return;
            case 7:
                ReplaceButtonTooltipTextBlock = (TextBlock)target;
                return;
        }
        _contentLoaded = true;
    }
}
