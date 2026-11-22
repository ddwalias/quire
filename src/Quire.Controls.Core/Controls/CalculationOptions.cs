#nullable disable
using System;
using System.ComponentModel;
using System.Windows;
using Quire.Calculation.Core;
using Quire.Mirror;
using Quire.PersistentDataObjects;

namespace Quire.Controls;

public partial class CalculationOptions
{
    private static readonly Type TypeOfThis = typeof(CalculationOptions);

    public static readonly DependencyProperty StrictComparisionsProperty = DependencyProperty.Register("StrictComparisions", typeof(bool), TypeOfThis, new PropertyMetadata(true, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
    {
        HandleOptionChanged(d, e, "strict_comparisons");
    }));

    public static readonly DependencyProperty StringsIndexedFromOriginProperty = DependencyProperty.Register("StringsIndexedFromOrigin", typeof(bool), TypeOfThis, new PropertyMetadata(false, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
    {
        HandleOptionChanged(d, e, "strings_indexed_from_origin");
    }));

    public static readonly DependencyProperty StrictSingularityCheckingProperty = DependencyProperty.Register("StrictSingularityChecking", typeof(bool), TypeOfThis, new PropertyMetadata(false, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
    {
        HandleOptionChanged(d, e, "strict_singularity_checking");
    }));

    public static readonly DependencyProperty UnitsAndConstantsInSymbolicsProperty = DependencyProperty.Register("UnitsAndConstantsInSymbolics", typeof(bool), TypeOfThis, new PropertyMetadata(false, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
    {
        HandleOptionChanged(d, e, "units_and_constants_in_symbolics");
    }));

    public static readonly DependencyProperty IsAutoCalcOnProperty = DependencyProperty.Register("IsAutoCalcOn", typeof(bool), TypeOfThis, new PropertyMetadata(true));

    public static readonly DependencyProperty NumberOfWorkerThreadsProperty = DependencyProperty.Register("NumberOfWorkerThreads", typeof(int), TypeOfThis, new PropertyMetadata(1, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
    {
        HandleOptionChanged(d, e, "number_of_threads");
    }));

    public static readonly DependencyProperty SymbolicResultMaximumSizeProperty = DependencyProperty.Register("SymbolicResultMaximumSize", typeof(int), TypeOfThis, new PropertyMetadata(CalculationOptionDefaults.SymbolicResultMaximumSize, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
    {
        HandleOptionChanged(d, e, "symbolic_result_maximum_size");
    }));

    public static readonly DependencyProperty BinOctHexModeProperty = DependencyProperty.Register("BinOctHexMode", typeof(bool), TypeOfThis, new PropertyMetadata(false, (DependencyObject d, DependencyPropertyChangedEventArgs e) =>
    {
        HandleBinOctHexModeOptionChanged(d, e, "bin_oct_hex_mode");
    }));

    private bool _restrictedMode;

    public bool StrictComparisions
    {
        get => (bool)GetValue(StrictComparisionsProperty);
        set => SetValue(StrictComparisionsProperty, value);
    }

    public bool StringsIndexedFromOrigin
    {
        get => (bool)GetValue(StringsIndexedFromOriginProperty);
        set => SetValue(StringsIndexedFromOriginProperty, value);
    }

    public bool StrictSingularityChecking
    {
        get => (bool)GetValue(StrictSingularityCheckingProperty);
        set => SetValue(StrictSingularityCheckingProperty, value);
    }

    public bool UnitsAndConstantsInSymbolics
    {
        get => (bool)GetValue(UnitsAndConstantsInSymbolicsProperty);
        set => SetValue(UnitsAndConstantsInSymbolicsProperty, value);
    }

    public bool IsAutoCalcOn
    {
        get => (bool)GetValue(IsAutoCalcOnProperty);
        set => SetValue(IsAutoCalcOnProperty, value);
    }

    public int NumberOfWorkerThreads
    {
        get => (int)GetValue(NumberOfWorkerThreadsProperty);
        set => SetValue(NumberOfWorkerThreadsProperty, value);
    }

    public int SymbolicResultMaximumSize
    {
        get => (int)GetValue(SymbolicResultMaximumSizeProperty);
        set => SetValue(SymbolicResultMaximumSizeProperty, value);
    }

    public bool BinOctHexMode
    {
        get => (bool)GetValue(BinOctHexModeProperty);
        set => SetValue(BinOctHexModeProperty, value);
    }

    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public SymbolicEngineType SymbolicEngineType
    {
        get => SymbolicEngineType.axiom;
        set
        {
            if (value != SymbolicEngineType.axiom)
            {
                EngineTypeAutoCorrected = true;
            }
        }
    }

    [MirrorVerified("PtcControlsCalculationOptionsMechanicalDifferentialTests")]
    public bool EngineTypeAutoCorrected { get; private set; }

    public bool RestrictedMode
    {
        get => _restrictedMode;
        set
        {
            if (value != _restrictedMode)
            {
                _restrictedMode = value;
                CalculationOptionChanged?.Invoke("restricted_mode", RestrictedMode);
            }
        }
    }

    [MirrorVerified("PtcControlsCalculationOptionsMechanicalDifferentialTests")]
    public event Delegate2<string, object> CalculationOptionChanged;

    [MirrorVerified("PtcControlsCalculationOptionsMechanicalDifferentialTests")]
    public event Delegate2<string, object> BinOctHexModeOptionChanged;

    private static void HandleOptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e, string optionName)
    {
        (d as CalculationOptions)?.CalculationOptionChanged?.Invoke(optionName, e.NewValue);
    }

    private static void HandleBinOctHexModeOptionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e, string optionName)
    {
        (d as CalculationOptions)?.BinOctHexModeOptionChanged?.Invoke(optionName, e.NewValue);
    }

    [MirrorVerified("PtcControlsCalculationOptionsMechanicalDifferentialTests")]
    public CalculationOptions() { }
}
