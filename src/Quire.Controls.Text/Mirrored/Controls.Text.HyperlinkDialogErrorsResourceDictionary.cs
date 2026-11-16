#nullable disable
#pragma warning disable

using System;
using System.Windows;
using System.Windows.Markup;

namespace Quire.Controls.Text;

public partial class HyperlinkDialogErrorsResourceDictionary
{
    public static readonly Type[] KeysOwnerTypes = { typeof(HyperlinkInputValidationErrorKeys) };
    private bool _contentLoaded;

    public HyperlinkDialogErrorsResourceDictionary()
    {
        InitializeComponent();
    }

    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }

        _contentLoaded = true;
        Uri resourceLocator = new Uri("/Quire.Controls.Text;component/hyperlinkdialogerrorsresourcedictionary.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    void IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }
}
