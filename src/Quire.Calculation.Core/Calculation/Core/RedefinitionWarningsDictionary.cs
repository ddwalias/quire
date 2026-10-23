#nullable disable
using System;
using System.Windows;
using System.Windows.Markup;
using Quire.Mirror;

namespace Quire.Calculation.Core;

public partial class RedefinitionWarningsDictionary
{
    public static readonly Type[] KeysOwnerTypes;

    private bool _contentLoaded;

    [MirrorVerified("PtcCalculationCoreRedefinitionWarningsDictionaryCctorMechanicalDifferentialTests")]
    static RedefinitionWarningsDictionary() => KeysOwnerTypes = [typeof(RedefinitionWarningMsgCodes)];

    public RedefinitionWarningsDictionary()
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
        Uri resourceLocator = new Uri("/Quire.Calculation.Core;component/redefinitionwarningsdictionary.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) => _contentLoaded = true;
}
