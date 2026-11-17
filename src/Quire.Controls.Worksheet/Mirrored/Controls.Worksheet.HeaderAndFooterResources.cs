#nullable disable
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Windows;
using System.Windows.Markup;

namespace Quire.Controls.Worksheet;

public partial class HeaderAndFooterResources : ResourceDictionary, IComponentConnector
{
    public static readonly Type[] KeysOwnerTypes = new Type[1] { typeof(HeaderAndFooterResourcesKeys) };

    private bool _contentLoaded;

    public HeaderAndFooterResources()
    {
        InitializeComponent();
        UpdateElementSeparator();
    }

    public void UpdateElementSeparator()
    {
        IDictionaryEnumerator enumerator = GetEnumerator();
        while (enumerator.MoveNext())
        {
            if (enumerator.Key is string text && text.Contains("dd"))
            {
                this[text] = (enumerator.Value as string).Replace("#", CultureInfo.CurrentUICulture.DateTimeFormat.DateSeparator);
            }
        }
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
        Uri resourceLocator = new Uri("/Quire.Controls.Worksheet;component/headerandfooterresources.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
        _contentLoaded = true;
    }
}
