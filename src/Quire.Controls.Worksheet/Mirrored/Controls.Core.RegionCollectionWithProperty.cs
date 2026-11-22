using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using Quire.Core;
using Quire.Undo;
using Quire.Controls.Core.Whiteboard;
using Quire.Wpf;

namespace Quire.Controls.Core;

public partial class RegionCollectionWithProperty
{
    private static readonly Type _typeOfThis;

    public static readonly DependencyProperty TextFormatProperty;

    public static readonly DependencyProperty CalcIsDisabledProperty;

    public static readonly DependencyProperty ShowAxisExpressionsProperty;

    public static readonly DependencyProperty ShowTicksProperty;

    public static readonly DependencyProperty ShowNumbersProperty;

    public static readonly DependencyProperty PlotBackgroundTypeProperty;

    public static readonly DependencyProperty PlotCameraTypeProperty;

    static RegionCollectionWithProperty()
    {
        _typeOfThis = typeof(RegionCollectionWithProperty);
        TextFormatProperty = TextFormat.TextFormatProperty.AddOwner(_typeOfThis, new FrameworkPropertyMetadata(null, HandleTextFormatChanged));
        CalcIsDisabledProperty = AttachedProperties.CalcIsDisabledProperty.AddOwner(_typeOfThis);
        ShowAxisExpressionsProperty = PlotCommonProperties.ShowAxisExpressionsProperty.AddOwner(_typeOfThis);
        ShowTicksProperty = PlotCommonProperties.ShowTicksProperty.AddOwner(_typeOfThis);
        ShowNumbersProperty = PlotCommonProperties.ShowNumbersProperty.AddOwner(_typeOfThis);
        PlotBackgroundTypeProperty = AttachedProperties.PlotBackgroundTypeProperty.AddOwner(_typeOfThis);
        PlotCameraTypeProperty = AttachedProperties.PlotCameraTypeProperty.AddOwner(_typeOfThis);
    }

    public RegionCollectionWithProperty(IEnumerable<IWhiteboardItem> regions)
    {
        _regions = DebugTools.AssertNotNull(regions, null).Passed ? new ObservableRangeCollection<IWhiteboardItem>(regions) : new ObservableRangeCollection<IWhiteboardItem>();
        _regions.RangeChanged += HandleCollectionChanged;
        HandleCollectionChanged(_regions, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, _regions));
    }
    public bool ShowAxisExpressions
    {
        get => (bool)GetValue(ShowAxisExpressionsProperty);
        set => SetValue(ShowAxisExpressionsProperty, value);
    }

    public bool ShowTicks
    {
        get => (bool)GetValue(ShowTicksProperty);
        set => SetValue(ShowTicksProperty, value);
    }

    public bool ShowNumbers
    {
        get => (bool)GetValue(ShowNumbersProperty);
        set => SetValue(ShowNumbersProperty, value);
    }

    public TextFormat TextFormat
    {
        get => (TextFormat)GetValue(TextFormatProperty);
        set => SetValue(TextFormatProperty, value);
    }

    public bool CalcIsDisabled
    {
        get => (bool)GetValue(CalcIsDisabledProperty);
        set => SetValue(CalcIsDisabledProperty, value);
    }

    public IEnumerable<IWhiteboardItem> Regions => _regions;

    public IEnumerable<IRichTextBase> TextBlocks => Regions.OfType<IRichTextBase>();

    public IEnumerable<ICurrentMathFormatProperty> MathFormatElements => Regions.OfType<ICurrentMathFormatProperty>();

    public MathFormat CurrentMathFormat
    {
        get => MathFormat;
        set => throw new NotImplementedException();
    }

    private NumberFormat _currentNumberFormat;

    private MatrixFormat _currentMatrixFormat;

    private TextFormat _currentTextFormat;

    private XmlLanguage _currentProofingLanguage = XmlLanguage.Empty;

    private readonly ObservableRangeCollection<IWhiteboardItem> _regions;

    private bool? _oleObjectAutoResize;

    public event PropertyChangedEventHandler PropertyChanged;

    public MathFormat MathFormat { get; private set; }

    public NumberFormat CurrentNumberFormat
    {
        get => _currentNumberFormat;
        set
        {
            _currentNumberFormat = value;
            FirePropertyChanged(new PropertyChangedEventArgs("CurrentNumberFormat"));
        }
    }

    public MatrixFormat CurrentMatrixFormat
    {
        get => _currentMatrixFormat;
        set
        {
            _currentMatrixFormat = value;
            FirePropertyChanged(new PropertyChangedEventArgs("CurrentMatrixFormat"));
        }
    }

    public TextFormat CurrentTextFormat
    {
        get => _currentTextFormat;
        set
        {
            _currentTextFormat = value;
            FirePropertyChanged(new PropertyChangedEventArgs("CurrentTextFormat"));
        }
    }

    public XmlLanguage CurrentProofingLanguage
    {
        get => _currentProofingLanguage;
        private set
        {
            _currentProofingLanguage = value;
            FirePropertyChanged(new PropertyChangedEventArgs("CurrentProofingLanguage"));
        }
    }

    public void HandleMathFormatChanged(DependencyPropertyChangedEventArgs e)
    {
    }

    private void FirePropertyChanged(PropertyChangedEventArgs e)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, e);
        }
    }

    private static void HandleTextFormatChanged(DependencyObject d, DependencyPropertyChangedEventArgs args)
    {
        TextFormat oldFormat = args.OldValue as TextFormat;
        TextFormat newFormat = args.NewValue as TextFormat;
        if (d is RegionCollectionWithProperty regionCollectionWithProperty)
        {
            regionCollectionWithProperty.HandleTextFormatChanged(oldFormat, newFormat);
        }
    }

    private void HandleTextFormatChanged(TextFormat oldFormat, TextFormat newFormat)
    {
        if (oldFormat != null)
        {
            oldFormat.FormatChanged -= OnTextFormattingChanged;
        }

        if (newFormat != null)
        {
            newFormat.FormatChanged += OnTextFormattingChanged;
        }

        CurrentTextFormat = TextFormat;
    }

    private void OnTextFormattingChanged(DependencyPropertyChangedEventArgs eventArgs)
    {
        if (EnvironmentScope.IsCurrentlyInsideUndoOrRedo)
        {
            return;
        }

        using (new CommandScope("SetTextFormatOnRegionCollectionCmd"))
        {
            foreach (IWhiteboardItem region in _regions)
            {
                if (region is ITextFormatOwner owner)
                {
                    SetTextFormatPropertyValueWithCommand(eventArgs, owner, this);
                }
                else
                {
                    if (!(region is IWhiteboardControl whiteboardControl))
                    {
                        continue;
                    }

                    foreach (ITextFormatOwner item in whiteboardControl.WhiteboardItems.OfType<ITextFormatOwner>())
                    {
                        SetTextFormatPropertyValueWithCommand(eventArgs, item, this);
                    }
                }
            }
        }
    }
}
