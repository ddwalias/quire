#nullable disable
#pragma warning disable

using System.Windows;
using System.Windows.Controls;

namespace Quire;

public partial class PrintInProgressDialog
{
    public bool DontBotherMeAgain
    {
        get => _dontBotherMeAgain;
        set => _dontBotherMeAgain = value;
    }

    public PrintInProgressDialog()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void Ok_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }

    private void Cancel_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = false;
    }

    [global::System.Diagnostics.DebuggerNonUserCode]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        global::System.Uri resourceLocator = new global::System.Uri("/Quire;component/printinprogressdialog.xaml", global::System.UriKind.Relative);
        global::System.Windows.Application.LoadComponent(this, resourceLocator);
    }

    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
            case 1:
                textBox1 = (TextBlock)target;
                return;
            case 2:
                DontBotherCheckBox = (CheckBox)target;
                return;
            case 3:
                OkButton = (Button)target;
                OkButton.Click += Ok_Click;
                return;
            case 4:
                CancelButton = (Button)target;
                CancelButton.Click += Cancel_Click;
                return;
        }
        _contentLoaded = true;
    }
}
