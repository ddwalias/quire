#nullable disable

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Quire.Calculation.Core;
using Quire.Controls.Core;

namespace Quire;

public partial class SystemUnitEditDialog
{
    public static RoutedCommand BtnEditCommand = new RoutedCommand();

    public static string discardMessageKey = "Ptc.Controls.SystemUnitEditDialog.DialogDiscardMessage";

    internal SystemUnitEditDialog _this;

    internal RadioButton rbCustom;

    internal ListBox lbDerived;

    internal Button OkButton;

    private bool _contentLoaded;

    public SystemUnitEditDialogViewModel ViewModel { get; }

    public void HandleDialogAction(object sender, ExecutedRoutedEventArgs e)
    {
        ViewModel.HandleDialogAction(e.OriginalSource, e.Parameter as DialogAction?);
    }

    public void HandleRbCustomClicked(object sender, ExecutedRoutedEventArgs e)
    {
        ViewModel.HandleRbCustomClicked(rbCustom);
    }

    public static bool ShowDialog(SpiritMainWindow owner, ICollectionView visualUnitDescriptorTable, IDictionary visualCGSUnitDescriptorTable, Dictionary<string, UnitDescriptor> calculatorUnitDescriptors)
    {
        SystemUnitEditDialog dialog = new SystemUnitEditDialog(new SystemUnitEditDialogViewModel(visualUnitDescriptorTable, visualCGSUnitDescriptorTable, calculatorUnitDescriptors, owner));
        if (owner != null)
        {
            dialog.CommandBindings.Add(owner.HelpCommandBinding);
            dialog.Owner = owner;
        }

        return dialog.ShowDialog() == true;
    }

    private SystemUnitEditDialog(SystemUnitEditDialogViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
        ViewModel.DerivedListBox = lbDerived;
        DataContext = viewModel;
        lbDerived.Resources[SystemColors.InactiveSelectionHighlightTextBrushKey] = SystemColors.HighlightTextBrush;
        ViewModel.DiscardMessage = TryFindResource(discardMessageKey) as string;
    }

    protected override void OnOkButtonClick(object sender, RoutedEventArgs e)
    {
        base.OnOkButtonClick(sender, e);
        ViewModel.Commit();
    }

    private void CustomBaseOnSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        e.Handled = true;
        ViewModel.HandlePostCustomBaseOnSelectionChanged(sender as ComboBox);
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
        Uri resourceLocator = new Uri("/Quire;component/systemunit/systemuniteditdialog.xaml", UriKind.Relative);
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
                _this = (SystemUnitEditDialog)target;
                return;
            case 2:
                ((CommandBinding)target).Executed += HandleDialogAction;
                return;
            case 3:
                ((CommandBinding)target).Executed += HandleRbCustomClicked;
                return;
            case 4:
                rbCustom = (RadioButton)target;
                return;
            case 5:
                ((ComboBox)target).SelectionChanged += CustomBaseOnSelectionChanged;
                return;
            case 6:
                lbDerived = (ListBox)target;
                return;
            case 7:
                OkButton = (Button)target;
                OkButton.Click += OnOkButtonClick;
                return;
        }

        _contentLoaded = true;
    }
}
