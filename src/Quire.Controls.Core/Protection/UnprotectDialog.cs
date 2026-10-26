#nullable disable
using System;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using Quire.Mirror;
using Quire.Wpf;

namespace Quire.Controls.Core.Protection;

public partial class UnprotectDialog : PasswordDialog, IComponentConnector
{
    private readonly Func<string, bool> _doesPasswordsMatch;
    private readonly bool _hasPassword;
    private PasswordBox _PasswordBox;
    private bool _contentLoaded;

    public bool HasPassword => _hasPassword;

    protected override bool DoPasswordsMatch
    {
        get
        {
            if (HasPassword)
            {
                return _doesPasswordsMatch(_PasswordBox.Password);
            }
            return true;
        }
    }

    public static bool ShowUnProtectDialog(Func<string, bool> doesPasswordMatch, ResourceDictionary customControlStringsDictionary, Window owner)
    {
        UnprotectDialog unprotectDialog = new UnprotectDialog(doesPasswordMatch, customControlStringsDictionary);
        if (owner != null)
        {
            unprotectDialog.Owner = owner;
            unprotectDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
        return unprotectDialog.ShowDialog() == true;
    }

    private UnprotectDialog(Func<string, bool> doesPasswordsMatch, ResourceDictionary customControlStringsDictionary)
    {
        DataContext = this;
        _doesPasswordsMatch = doesPasswordsMatch;
        _hasPassword = !_doesPasswordsMatch(string.Empty);
        InitializeComponent();
        ResourceProvider<ResourceDictionary>.VerifyResources(customControlStringsDictionary, new Type[] { typeof(UnprotectDialogKeys) });
        Resources.MergedDictionaries.Add(customControlStringsDictionary);
        _PasswordBox.Focus();
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
        Uri resourceLocator = new Uri("/Quire.Controls.Core;component/protection/unprotectdialog.xaml", UriKind.Relative);
        Application.LoadComponent(this, resourceLocator);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    internal Delegate _CreateDelegate(Type delegateType, string handler)
    {
        return Delegate.CreateDelegate(delegateType, this, handler);
    }

    [DebuggerNonUserCode]
    [GeneratedCode("PresentationBuildTasks", "10.0.11.0")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    void IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
            case 1:
                _PasswordBox = (PasswordBox)target;
                _PasswordBox.PasswordChanged += OnPasswordChanged;
                break;
            case 2:
                ((Button)target).Click += OnAcceptPasswordButtonClick;
                break;
            case 3:
                ((Button)target).Click += OnCancelButtonClick;
                break;
            default:
                _contentLoaded = true;
                break;
        }
    }
}
