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

public partial class ProtectDialog
{
    private DisplayStateEnum _displayState;

    private AreaBorderStateEnum _areaBorderState;

    private PasswordBox _primaryPasswordBox;

    private PasswordBox _reenterPasswordBox;

    internal CheckBox showTimeStampCheckBox;

    internal CheckBox showAreaBorderCheckBox;

    internal CheckBox lockDisplayStateCheckBox;

    internal ComboBox displayStateComboBox;

    private bool _contentLoaded;

    public DisplayStateEnum DisplayState
    {
        get
        {
            return _displayState;
        }
        set
        {
            if (value == _displayState)
            {
                return;
            }
            _displayState = value;
            FirePropertyChanged("DisplayState");
        }
    }

    public AreaBorderStateEnum AreaBorderState
    {
        get
        {
            return _areaBorderState;
        }
        set
        {
            if (value == _areaBorderState)
            {
                return;
            }
            _areaBorderState = value;
            FirePropertyChanged("AreaBorder");
        }
    }

    protected override bool DoPasswordsMatch => _primaryPasswordBox.Password == _reenterPasswordBox.Password;

    public static bool ShowProtectionDialog(ProtectionDialogState protectionState, ResourceDictionary customControlStringsDictionary, Window owner)
    {
        ProtectDialog protectDialog = new ProtectDialog(protectionState, customControlStringsDictionary);
        if (owner != null)
        {
            protectDialog.Owner = owner;
            protectDialog.WindowStartupLocation = WindowStartupLocation.CenterOwner;
        }
        bool accepted = protectDialog.ShowDialog() == true;
        if (accepted)
        {
            protectionState.Password = protectDialog._primaryPasswordBox.Password;
            protectionState.IsDisplayStateLocked = protectDialog.lockDisplayStateCheckBox.IsChecked == true;
            protectionState.ShowTimeStamp = protectDialog.showTimeStampCheckBox.IsChecked == true;
            protectionState.IsShowAreaBorder = protectDialog.AreaBorderState == AreaBorderStateEnum.Shown;
            protectionState.IsCollapsed = protectDialog.DisplayState == DisplayStateEnum.Collapsed;
        }
        return accepted;
    }

    private ProtectDialog(ProtectionDialogState protectionState, ResourceDictionary customControlStringsDictionary)
    {
        DataContext = this;
        InitializeComponent();
        ResourceProvider<ResourceDictionary>.VerifyResources(customControlStringsDictionary, typeof(ProtectDialogKeys));
        Resources.MergedDictionaries.Add(customControlStringsDictionary);
        DisplayState = (DebugTools.AssertNotNull(protectionState, null).Passed && protectionState.IsCollapsed) ? DisplayStateEnum.Collapsed : DisplayStateEnum.Expanded;
        displayStateComboBox.SelectedItem = DisplayState;
        showAreaBorderCheckBox.IsChecked = protectionState.IsShowAreaBorder;
        AreaBorderState = protectionState.IsShowAreaBorder ? AreaBorderStateEnum.Shown : AreaBorderStateEnum.Hidden;
        lockDisplayStateCheckBox.IsChecked = protectionState.IsDisplayStateLocked;
        _primaryPasswordBox.Focus();
    }

    private void OnControlDisplayStateChanged(object sender, SelectionChangedEventArgs e)
    {
        ComboBox comboBox = sender as ComboBox;
        if (DebugTools.DevAssert(comboBox != null, null).Failed)
        {
            return;
        }
        DisplayState = (DisplayStateEnum)comboBox.SelectedItem;
    }

    private void OnChangeBorderVisibilityState(object sender, RoutedEventArgs e)
    {
        CheckBox checkBox = sender as CheckBox;
        if (DebugTools.DevAssert(checkBox != null, null).Failed)
        {
            return;
        }
        AreaBorderState = checkBox.IsChecked == true ? AreaBorderStateEnum.Shown : AreaBorderStateEnum.Hidden;
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
        Uri resourceLocator = new Uri("/Quire.Controls.Core;component/protection/protectdialog.xaml", UriKind.Relative);
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
                _primaryPasswordBox = (PasswordBox)target;
                _primaryPasswordBox.PasswordChanged += OnPasswordChanged;
                break;
            case 2:
                _reenterPasswordBox = (PasswordBox)target;
                _reenterPasswordBox.PasswordChanged += OnPasswordChanged;
                break;
            case 3:
                showTimeStampCheckBox = (CheckBox)target;
                break;
            case 4:
                showAreaBorderCheckBox = (CheckBox)target;
                showAreaBorderCheckBox.Click += OnChangeBorderVisibilityState;
                break;
            case 5:
                lockDisplayStateCheckBox = (CheckBox)target;
                break;
            case 6:
                displayStateComboBox = (ComboBox)target;
                displayStateComboBox.SelectionChanged += OnControlDisplayStateChanged;
                break;
            case 7:
                ((Button)target).Click += OnAcceptPasswordButtonClick;
                break;
            case 8:
                ((Button)target).Click += OnCancelButtonClick;
                break;
            default:
                _contentLoaded = true;
                break;
        }
    }

    [MirrorVerified("PtcControlsCoreProtectionProtectDialogMechanicalDifferentialTests")]
    internal Delegate _CreateDelegate(Type delegateType, string handler) => Delegate.CreateDelegate(delegateType, this, handler);

    partial class ProtectProperties
    {
        public const string DisplayState = "DisplayState";

        [MirrorVerified("PtcControlsCoreProtectionProtectDialogMechanicalDifferentialTests")]
        public ProtectProperties() { }
    }
}
