#nullable disable
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Markup;
using Quire.Controls.Core;
using Quire.Wpf;

namespace Quire.Controls.Text;

public partial class HyperlinkDialog
{
    private HyperlinkDialogViewModel ViewModel { get; }

    public static bool ShowHyperlinkDialog(HyperlinkDialogViewModel hyperlinkViewModel, Window owner)
    {
        if (DebugTools.AssertNotNull(hyperlinkViewModel, null).Failed)
        {
            return false;
        }
        HyperlinkDialog hyperlinkDialog = new HyperlinkDialog(hyperlinkViewModel);
        if (owner != null)
        {
            hyperlinkDialog.Owner = owner;
        }
        return hyperlinkDialog.ShowDialog() == true;
    }

    private HyperlinkDialog(HyperlinkDialogViewModel hyperlinkViewModel)
    {
        ViewModel = hyperlinkViewModel;
        DataContext = hyperlinkViewModel;
        InitializeComponent();
        FocusManager.SetFocusedElement(this, (ViewModel.UserCanModifyText && ViewModel.UserInputText.IsNullOrEmpty()) ? InsertDisplayTextBox : InsertAddressTextBox);
        Loaded += (object sender, RoutedEventArgs e) =>
        {
            HyperlinkDialogLoaded();
        };
    }

    private void HyperlinkDialogLoaded()
    {
        DialogLoaded();
        InsertDisplayTextBox.ClearValue(FrameworkElement.MaxWidthProperty);
        InsertAddressTextBox.ClearValue(FrameworkElement.MaxWidthProperty);
    }

    protected new void OnOkButtonClick(object sender, RoutedEventArgs e)
    {
        if (DebugTools.DevAssert(!ViewModel.HasErrors, null).Failed)
        {
            return;
        }
        base.OnOkButtonClick(sender, e);
    }

    // XAML fields declared after .ctor to preserve closure ordinal <.ctor>b__4_0.
    internal TextBox InsertDisplayTextBox;

    internal TextBox InsertAddressTextBox;

    internal Button OkButton;

    private bool _contentLoaded;

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.Text;component/hyperlinkdialog.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
        case 1:
            InsertDisplayTextBox = (TextBox)target;
            break;
        case 2:
            InsertAddressTextBox = (TextBox)target;
            break;
        case 3:
            OkButton = (Button)target;
            OkButton.Click += OnOkButtonClick;
            break;
        default:
            _contentLoaded = true;
            break;
        }
    }
}
