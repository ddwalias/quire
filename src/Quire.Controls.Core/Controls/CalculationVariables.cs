#nullable disable
using System;
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls;

public partial class CalculationVariables
{
    public const string TolVarName = "mc_TOL";
    public const string CTolVarName = "mc_CTOL";
    public const string IndexOriginVarName = "mc_ORIGIN";
    public const string PRNColWidthVarName = "mc_PRNCOLWIDTH";
    public const string PRNPrecisionVarName = "mc_PRNPRECISION";
    public const string CWDVarName = "mc_CWD";
    public const string RandSeedVarName = "rand_seed";

    public static readonly DependencyProperty TolProperty = DependencyProperty.Register("Tol", typeof(double), typeof(CalculationVariables), new PropertyMetadata(0.001, HandleTolChanged));
    public static readonly DependencyProperty CTolProperty = DependencyProperty.Register("CTol", typeof(double), typeof(CalculationVariables), new PropertyMetadata(0.001, HandleCTolChanged));
    public static readonly DependencyProperty IndexOriginProperty = DependencyProperty.Register("IndexOrigin", typeof(int), typeof(CalculationVariables), new PropertyMetadata(0, HandleIndexOriginChanged));
    public static readonly DependencyProperty PRNColWidthProperty = DependencyProperty.Register("PRNColWidth", typeof(int), typeof(CalculationVariables), new PropertyMetadata(8, HandlePRNColWidthChanged));
    public static readonly DependencyProperty PRNPrecisionProperty = DependencyProperty.Register("PRNPrecision", typeof(int), typeof(CalculationVariables), new PropertyMetadata(4, HandlePRNPrecisionChanged));
    public static readonly DependencyProperty CurrentWorkingDirectoryProperty = DependencyProperty.Register("CurrentWorkingDirectory", typeof(string), typeof(CalculationVariables), new PropertyMetadata(HandleCurrentWorkingDirectoryChanged));
    public static readonly DependencyProperty RandSeedProperty = DependencyProperty.Register("RandSeed", typeof(double), typeof(CalculationVariables), new PropertyMetadata(1.0, HandleRandSeedChanged));

    public CalculationVariables()
    {
        CurrentWorkingDirectory = Environment.CurrentDirectory + "\\";
    }

    public double Tol
    {
        get => (double)GetValue(TolProperty);
        set => SetValue(TolProperty, value);
    }

    public double CTol
    {
        get => (double)GetValue(CTolProperty);
        set => SetValue(CTolProperty, value);
    }

    public int IndexOrigin
    {
        get => (int)GetValue(IndexOriginProperty);
        set => SetValue(IndexOriginProperty, value);
    }

    public int PRNColWidth
    {
        get => (int)GetValue(PRNColWidthProperty);
        set => SetValue(PRNColWidthProperty, value);
    }

    public int PRNPrecision
    {
        get => (int)GetValue(PRNPrecisionProperty);
        set => SetValue(PRNPrecisionProperty, value);
    }

    public string CurrentWorkingDirectory
    {
        get => (string)GetValue(CurrentWorkingDirectoryProperty);
        set => SetValue(CurrentWorkingDirectoryProperty, value);
    }

    public double RandSeed
    {
        get => (double)GetValue(RandSeedProperty);
        set => SetValue(RandSeedProperty, value);
    }

    [MirrorVerified("PtcControlsCalculationVariablesMechanicalDifferentialTests")]
    public event Delegate2<string, object> CalculationVariableChanged;

    private static void HandleTolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var variables = d as CalculationVariables;
        if (variables != null && variables.CalculationVariableChanged != null)
        {
            variables.CalculationVariableChanged.Invoke(TolVarName, (double)e.NewValue);
        }
    }

    private static void HandleCTolChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var variables = d as CalculationVariables;
        if (variables != null && variables.CalculationVariableChanged != null)
        {
            variables.CalculationVariableChanged.Invoke(CTolVarName, (double)e.NewValue);
        }
    }

    private static void HandleIndexOriginChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var variables = d as CalculationVariables;
        if (variables != null && variables.CalculationVariableChanged != null)
        {
            variables.CalculationVariableChanged.Invoke(IndexOriginVarName, (int)e.NewValue);
        }
    }

    private static void HandlePRNColWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var variables = d as CalculationVariables;
        if (variables != null && variables.CalculationVariableChanged != null)
        {
            variables.CalculationVariableChanged.Invoke(PRNColWidthVarName, (int)e.NewValue);
        }
    }

    private static void HandlePRNPrecisionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var variables = d as CalculationVariables;
        if (variables != null && variables.CalculationVariableChanged != null)
        {
            variables.CalculationVariableChanged.Invoke(PRNPrecisionVarName, (int)e.NewValue);
        }
    }

    private static void HandleCurrentWorkingDirectoryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var variables = d as CalculationVariables;
        if (variables != null && variables.CalculationVariableChanged != null)
        {
            var directory = EscapeDirectory((string)e.NewValue);
            variables.CalculationVariableChanged.Invoke(CWDVarName, directory);
        }
    }

    private static void HandleRandSeedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var variables = d as CalculationVariables;
        if (variables != null && variables.CalculationVariableChanged != null)
        {
            variables.CalculationVariableChanged.Invoke(RandSeedVarName, (double)e.NewValue);
        }
    }

    public static string EscapeDirectory(string dir)
    {
        if (dir == null)
            return string.Empty;
        if (!dir.EndsWith("\\"))
            dir += "\\";
        dir = dir.Replace("\\", "\\\\");
        dir = dir.Replace("\"", "\\\"");
        return dir;
    }
}
