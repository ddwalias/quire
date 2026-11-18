#nullable disable
#pragma warning disable

using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Controls.Plots.ViewModel;
using Quire.Wpf;

namespace Quire.Controls.Plots;

public abstract partial class PlotEquationBase
{
    public bool IsCommitted { get; protected set; }

    public virtual IPlotEquationViewModelBase ViewModel
    {
        get
        {
            return null;
        }
        internal set
        {
            DebugTools.DevAssert(false, null);
        }
    }

    public MathFormat MathFormat { get; private set; }

    public StaticMathFormat MathFormatForSerialize
    {
        get
        {
            return MathFormat.ToStaticMathFormat();
        }
        set
        {
            MathFormat.FromStaticMathFormat(value);
        }
    }

    public MathFormat CurrentMathFormat
    {
        get
        {
            return MathFormat;
        }
        set
        {
            throw new NotImplementedException();
        }
    }

    public virtual bool InCustomInitialization
    {
        get
        {
            return Expression.InCustomInitialization;
        }
        set
        {
            Expression.InCustomInitialization = value;
            ScalingEquation.InCustomInitialization = value;
        }
    }

    public IMathFormat DeserializeMathFormat
    {
        set
        {
            if (value == null)
            {
                return;
            }
            Expression.DeserializeMathFormat = value;
            ScalingEquation.DeserializeMathFormat = value;
        }
    }

    public IDictionary<IdLabels, ExtendedFont> DeserializeLabeledIdFormat
    {
        set
        {
            if (value == null)
            {
                return;
            }
            Expression.DeserializeLabeledIdFormat = value;
            ScalingEquation.DeserializeLabeledIdFormat = value;
        }
    }

    public event Action SExpressionValueChanged;

    public event PropertyChangedEventHandler PropertyChanged;

    protected PlotEquationBase()
    {
        MathFormat = new MathFormat(this);
        WpfUtils.AddPreemptiveCommandBinding(this, MathFormat.ClearMathFormatCommand, OnClearMathFormatPreview, CanClearMathFormat, bindingMarksEventAsHandled: true);
        WpfUtils.AddPreemptiveCommandBinding(this, MathFormat.IncreaseFontSizeCommand, OnIncreaseFontSizePreview, null, bindingMarksEventAsHandled: true);
        WpfUtils.AddPreemptiveCommandBinding(this, MathFormat.DecreaseFontSizeCommand, OnDecreaseFontSizePreview, null, bindingMarksEventAsHandled: true);
        WpfUtils.AddPreemptiveCommandBinding(this, MathFormat.ChangeFontColorCommand, OnChangeFontColorPreview, null, bindingMarksEventAsHandled: true);
        WpfUtils.AddPreemptiveCommandBinding(this, MathFormat.ChangeHighlightColorCommand, OnChangeHighlightColorPreview, null, bindingMarksEventAsHandled: true);
    }

    public void ResetBeforeAdding()
    {
        ResetIsCommitted();
    }

    protected void ResetIsCommitted()
    {
        IsCommitted = false;
    }

    protected virtual void OnSubEquationExpressionChanged(IEquationControl subEquation)
    {
        ResetIsCommitted();
        FireExpressionValueChanged();
    }

    private void FireExpressionValueChanged()
    {
        if (SExpressionValueChanged != null)
        {
            SExpressionValueChanged();
        }
    }

    protected void AttachSExpressionValueChanged()
    {
        foreach (EquationControl containedEquationControl in ContainedEquationControls)
        {
            containedEquationControl.ExpressionValueChanged += OnSubEquationExpressionChanged;
        }
    }

    protected void DetachSExpressionValueChanged()
    {
        foreach (EquationControl containedEquationControl in ContainedEquationControls)
        {
            containedEquationControl.ExpressionValueChanged -= OnSubEquationExpressionChanged;
        }
    }

    public virtual void HandleMathFormatChanged(DependencyPropertyChangedEventArgs args)
    {
        ((DependencyObject)Expression).SetValue(args.Property, args.NewValue);
        ((DependencyObject)ScalingEquation).SetValue(args.Property, args.NewValue);
        if (args.Property == MathFormatAttachedProperties.FontColorProperty)
        {
            if (MathFormat.IsFontColorSet)
            {
                EquationControl.SetOverrideLabelColor(this, val: true);
            }
            else
            {
                EquationControl.ClearOverrideLabelColor(this);
            }
        }
        if (args.Property == MathFormatAttachedProperties.HighlightColorProperty)
        {
            Background = new SolidColorBrush(MathFormat.HighlightColor.GetValueOrDefault());
            Background.Freeze();
            InvalidateVisual();
        }
        FirePropertyChanged(args.Property.Name);
    }

    protected void FirePropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    private void OnChangeHighlightColorPreview(object sender, ExecutedRoutedEventArgs e)
    {
        MathFormat.ChangeHighlightColor((Color?)e.Parameter);
    }

    private void OnChangeFontColorPreview(object sender, ExecutedRoutedEventArgs e)
    {
        MathFormat.ChangeFontColor((Color)e.Parameter);
    }

    private void OnDecreaseFontSizePreview(object sender, ExecutedRoutedEventArgs e)
    {
        MathFormat.DecreaseFontSize();
    }

    private void OnIncreaseFontSizePreview(object sender, ExecutedRoutedEventArgs e)
    {
        MathFormat.IncreaseFontSize();
    }

    private void OnClearMathFormatPreview(object sender, ExecutedRoutedEventArgs e)
    {
        MathFormat.ClearFormat();
    }

    private void CanClearMathFormat(object sender, CanExecuteRoutedEventArgs args)
    {
        args.CanExecute = CurrentMathFormat.AreAnyPropertiesSet;
        args.Handled = true;
    }
}
