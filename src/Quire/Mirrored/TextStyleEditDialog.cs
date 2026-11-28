#nullable disable

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Quire.Controls.Core;

namespace Quire;

public partial class TextStyleEditDialog
{
    public static readonly DependencyProperty TextStyleNamesProperty = DependencyProperty.Register("TextStyleNames", typeof(string), typeof(TextStyleEditDialogViewModel));

    public const int IndentJumps = 20;

    private readonly Hashtable _controlsInError;

    internal TextStyleEditDialog _this;

    internal TextBox _tbName;

    internal FontFamilyComboBox _cbFontFamily;

    internal FontSizeComboBox fontSizeDropDown;

    internal global::Quire.Controls.ColorPicker FontColorPicker;

    internal global::Quire.Controls.ColorPicker HighlightColorPicker;

    internal global::Fluent.Button Part_TextAndRegionFormatRibbonBar_ToggleNumbering;

    internal global::Fluent.Button Part_TextAndRegionFormatRibbonBar_IncreaseIndentation;

    internal global::Fluent.Button Part_TextAndRegionFormatRibbonBar_DecreaseIndentation;

    internal global::Fluent.Button AlignLeft;

    internal global::Fluent.Button AlignCenter;

    internal global::Fluent.Button AlignRight;

    internal global::Fluent.Button AlignJustify;

    internal RichTextBox MyRichTextBox;

    internal Button OkButton;

    private bool _contentLoaded;

    public string TextStyleNames
    {
        get
        {
            return (string)GetValue(TextStyleNamesProperty);
        }
        set
        {
            SetValue(TextStyleNamesProperty, value);
        }
    }

    private TextStyleEditDialogViewModel ViewModel { get; }

    public static bool ShowTextStyleEditDialog(ITextStyle textStyle, StyleActionTypeEnum action, ITextStylesManager textStylesManager, SpiritMainWindow owner)
    {
        TextStyleEditDialogViewModel viewModel = new TextStyleEditDialogViewModel(textStyle, action, textStylesManager);
        if (DebugTools.AssertNotNull(viewModel, null).Failed)
        {
            return false;
        }

        TextStyleEditDialog dialog = new TextStyleEditDialog(viewModel);
        if (owner != null)
        {
            dialog.CommandBindings.Add(owner.HelpCommandBinding);
            dialog.Owner = owner;
        }

        return dialog.ShowDialog() == true;
    }

    private TextStyleEditDialog(TextStyleEditDialogViewModel viewModel)
    {
        ViewModel = viewModel;
        TextStyleNames = viewModel.TextStyleNames;
        _controlsInError = new Hashtable();
        InitializeComponent();
        InitPreviewWindow();
        DataContext = viewModel;
    }

    protected override void OnClosed(EventArgs e)
    {
        RemoveHandler(Validation.ErrorEvent, new RoutedEventHandler(ValidationErrorEvent));
        base.OnClosed(e);
    }

    private void InitPreviewWindow()
    {
        ViewModel.PreviewBox = MyRichTextBox;
        ViewModel.TextStyleBasedOnKeyNoneLocalized = (string)Resources["BasedOnNoneLocalized"];
        ViewModel.TextStyleNameNewLocalized = (string)Resources["TextStyleNameNewLocalized"];
        MyRichTextBox.SelectAll();
        ViewModel.UpdateMarkerStyle();
        AddHandler(Validation.ErrorEvent, new RoutedEventHandler(ValidationErrorEvent), handledEventsToo: true);
    }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("Mathcad calls ValidationErrorEventArgs.get_Action with call, Roslyn emits callvirt")]
    private void ValidationErrorEvent(object sender, RoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    protected void OnOkButtonClick(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("Mathcad compares the font size against int constant 999, Quire TextStyleEditDialogViewModel.FontSize is double?")]
    private void IncreaseFontSize(object sender, RoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("Mathcad uses int constants for the font size, Quire TextStyleEditDialogViewModel.FontSize is double?")]
    private void DecreaseFontSize(object sender, RoutedEventArgs e) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private void IncreaseIndentation(object sender, RoutedEventArgs e)
    {
        ViewModel.TextIndent = ViewModel.TextIndent.GetValueOrDefault() + 20.0;
    }

    private void DecreaseIndentation(object sender, RoutedEventArgs e)
    {
        double indent = ViewModel.TextIndent.GetValueOrDefault() - 20.0;
        if (indent >= 0.0)
        {
            ViewModel.TextIndent = indent;
        }
    }

    private void HighlightColorOnNoColorClicked()
    {
        if (ViewModel.BackgroundColor.Color != Colors.Transparent)
        {
            ViewModel.BackgroundColor = new SolidColorBrush(Colors.Transparent);
        }
    }

    private void FontColorOnNoColorClicked()
    {
        ViewModel.ForegroundColor = null;
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }

        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire;component/textstyleeditdialog.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
        return Delegate.CreateDelegate(delegateType, this, handler);
    }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("Quire ribbon buttons are Fluent.Button, Mathcad uses DevComponents ButtonEx/ButtonDropDown")]
    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) => throw global::Quire.Mirror.MirrorStub.NotImplemented();
}
