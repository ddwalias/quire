#nullable disable
#pragma warning disable

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Interop;
using Quire.FunctionalitiesLimitation;
using Quire.License;
using Quire.Win32;
using Quire.Wpf;

namespace Quire.Freemium;

public partial class FreemiumAboutBox
{
    public static readonly DependencyProperty AboutCategoryTitleProperty = DependencyProperty.Register("AboutCategoryTitle", typeof(string), typeof(FreemiumAboutBox), new UIPropertyMetadata((object)null));
    public static readonly DependencyProperty BenefitTextProperty = DependencyProperty.Register("BenefitText", typeof(string), typeof(FreemiumAboutBox), new UIPropertyMetadata(null, OnBenefitTextChanged));

    public static FreemiumAboutBox Instance => _instance ??= new FreemiumAboutBox();

    public string AboutCategoryTitle
    {
        get => (string)GetValue(AboutCategoryTitleProperty);
        set => SetValue(AboutCategoryTitleProperty, value);
    }

    public string BenefitText
    {
        get => (string)GetValue(BenefitTextProperty);
        set => SetValue(BenefitTextProperty, value);
    }

    private string MoreHelpHyperLink { get; set; }

    private FreemiumAboutBox()
    {
        _aboutCategoryTitleResourceDictionary = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Quire;component/Freemium/FreemiumAboutBoxAboutCategoryTitleResource.xaml")
        };
        _premiumBenefitsResourceDictionary = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Quire;component/Freemium/PremiumBenefits.xaml")
        };
        _helpLinkResourceDictionary = new ResourceDictionary
        {
            Source = new Uri("pack://application:,,,/Quire;component/Freemium/FreemiumAboutBoxHelpLinkResource.xaml")
        };
        Loaded += OnFreemiumAboutBoxLoaded;
        LicenseManager.Instance.LicenseStatusChanged += OnLicenseStatusChanged;
        InitializeComponent();
    }

    public void Show(object commandParameter, Window owner)
    {
        Owner = owner;
        if (owner is SpiritMainWindow spiritMainWindow)
        {
            spiritMainWindow.OptionsAboutDialogOpening += OnOptionsWindowOpened;
        }
        if (commandParameter is DisplayFreemiumAboutBoxCommandParameter displayFreemiumAboutBoxCommandParameter)
        {
            AboutCategoryTitle = GetSuitableResourceFromResourceDictionary(displayFreemiumAboutBoxCommandParameter.FunctionalityKey, _aboutCategoryTitleResourceDictionary);
            BenefitText = GetSuitableResourceFromResourceDictionary(displayFreemiumAboutBoxCommandParameter.FunctionalityKey, _premiumBenefitsResourceDictionary);
            MoreHelpHyperLink = GetSuitableResourceFromResourceDictionary(displayFreemiumAboutBoxCommandParameter.FunctionalityKey, _helpLinkResourceDictionary);
            PositionProvider = displayFreemiumAboutBoxCommandParameter.LockedItem;
        }
        else
        {
            DebugTools.DevAssert(false, "Parameter for FreemiumAboutBox is not of DisplayFreemiumAboutBoxCommandParameter type.");
        }
        Show();
    }

    internal static string GetSuitableResourceFromResourceDictionary(EnumFunctionalityKey functionalityKey, ResourceDictionary resourceDictionary)
    {
        EnumFunctionalityKey enumFunctionalityKey = functionalityKey;
        string text = resourceDictionary[enumFunctionalityKey] as string;
        while (text == null && enumFunctionalityKey != EnumFunctionalityKey.GlobalCategory)
        {
            enumFunctionalityKey = FunctionalitiesLimitationDictionary.GetParentFunctionalityKey(enumFunctionalityKey);
            text = resourceDictionary[enumFunctionalityKey] as string;
        }
        // Release IL requires string.Format here; an interpolated string compiles to DefaultInterpolatedStringHandler.
        DebugTools.AssertNotNull(text, string.Format("Could not find the Suitable Resource for {0} functionality in the {1}.", functionalityKey, resourceDictionary.Source));
        return text;
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);
        Loaded -= OnFreemiumAboutBoxLoaded;
        LicenseManager.Instance.LicenseStatusChanged -= OnLicenseStatusChanged;
        if (Owner is SpiritMainWindow spiritMainWindow)
        {
            spiritMainWindow.OptionsAboutDialogOpening -= OnOptionsWindowOpened;
        }
        if (!Owner.IsActive)
        {
            Owner.Activate();
        }
        _instance = null;
    }

    private void OnOptionsWindowOpened()
    {
        Close();
    }

    private void OnLicenseStatusChanged()
    {
        if (LicenseManager.Instance.HasValidLicense)
        {
            Close();
        }
    }

    private void OnCloseButtonClick(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void BenefitHyperlinkClick(object sender, RoutedEventArgs e)
    {
        AppCommands.DisplayHelp.Execute(MoreHelpHyperLink, Owner);
    }

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        RemoveMinimizeMaximizeRestoreOptions();
    }

    private void RemoveMinimizeMaximizeRestoreOptions()
    {
        nint handle = new WindowInteropHelper(this).Handle;
        if (handle != nint.Zero)
        {
            nint systemMenu = User32.GetSystemMenu(handle, bRevert: false);
            if (systemMenu != nint.Zero)
            {
                User32.DeleteMenu(systemMenu, 61472u, 0u);
                User32.DeleteMenu(systemMenu, 61488u, 0u);
                User32.DeleteMenu(systemMenu, 61728u, 0u);
            }
        }
    }

    private void OnFreemiumAboutBoxLoaded(object sender, RoutedEventArgs e)
    {
        if (sender is Window window)
        {
            if (benefitTextScrollViewer != null && benefitTextBlock != null && vCardDisplay != null)
            {
                double benefitTextFontSize = WpfUtils.PointsToDips(benefitTextBlock.FontSize);
                double vCardWidth = vCardDisplay.ActualWidth;
                vCardDisplay.SetValue(Grid.ColumnSpanProperty, 2);
                vCardDisplay.UpdateLayout();
                window.MinWidth = vCardWidth + (window.ActualWidth - vCardDisplay.ActualWidth);
                window.MinHeight = benefitTextFontSize + (window.ActualHeight - benefitTextScrollViewer.ViewportHeight);
            }
            else
            {
                DebugTools.DevAssert(false, "One of the window elements was not found.");
            }
        }
    }

    private static void OnBenefitTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is FreemiumAboutBox freemiumAboutBox && freemiumAboutBox.FindName("BenefitTextScrollViewer") is ScrollViewer scrollViewer)
        {
            scrollViewer.ScrollToHome();
        }
    }

    [global::System.Diagnostics.DebuggerNonUserCode]
    public void InitializeComponent()
    {
        if (_contentLoaded)
        {
            return;
        }
        _contentLoaded = true;
        global::System.Uri resourceLocator = new global::System.Uri("/Quire;component/freemium/freemiumaboutbox.xaml", global::System.UriKind.Relative);
        global::System.Windows.Application.LoadComponent(this, resourceLocator);
    }

    [global::System.Diagnostics.DebuggerNonUserCode]
    [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
    void global::System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target)
    {
        switch (connectionId)
        {
            case 1:
                benefitTextScrollViewer = (global::System.Windows.Controls.ScrollViewer)target;
                return;
            case 2:
                benefitTextBlock = (global::System.Windows.Controls.TextBlock)target;
                return;
            case 3:
                ((global::System.Windows.Documents.Hyperlink)target).Click += BenefitHyperlinkClick;
                return;
            case 4:
                vCardDisplay = (global::System.Windows.Controls.StackPanel)target;
                return;
            case 5:
                ((global::System.Windows.Controls.Button)target).Click += OnCloseButtonClick;
                return;
        }
        _contentLoaded = true;
    }
}
