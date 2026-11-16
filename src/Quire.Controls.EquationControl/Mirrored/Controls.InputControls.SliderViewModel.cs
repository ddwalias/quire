using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

internal partial class SliderViewModel
{
    public event EventHandler InvertSliderChanged;

    public SliderViewModel(IInputControlDataCalculatedValue data)
        : base(new SliderModel(data))
    {
    }

    private SliderModel Model => (SliderModel)_model;

    protected override void HandleModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        switch (e.PropertyName)
        {
            case "FlowDirection":
            case "TickFrequency":
            case "TickPlacement":
            case "VerticalAlignment":
            case "HorizontalAlignment":
            case "Maximum":
            case "Minimum":
            case "ShowMinMax":
            case "SmallChange":
            case "LargeChange":
            case "Position":
                OnPropertyChanged(e.PropertyName);
                break;
            case "Border":
                OnPropertyChanged("BorderThickness");
                break;
            case "Orientation":
                OnPropertyChanged(e.PropertyName);
                OnPropertyChanged("VerticalAlignment");
                OnPropertyChanged("HorizontalAlignment");
                SwapDimensions();
                break;
            case "Invert":
                OnPropertyChanged(e.PropertyName);
                InvertSliderChanged?.Invoke(this, EventArgs.Empty);
                break;
        }
    }

    public double Position
    {
        get => Model.Position;
        set => Model.Position = value;
    }

    public VerticalAlignment VerticalAlignment
    {
        get
        {
            if (Orientation == Orientation.Horizontal)
            {
                return VerticalAlignment.Top;
            }

            return VerticalAlignment.Stretch;
        }
    }

    public HorizontalAlignment HorizontalAlignment
    {
        get
        {
            if (Orientation == Orientation.Horizontal)
            {
                return HorizontalAlignment.Stretch;
            }

            return HorizontalAlignment.Left;
        }
    }

    public Orientation Orientation
    {
        get => Model.Orientation;
        set => Model.Orientation = value;
    }

    public TickPlacement TickPlacement
    {
        get => Model.TickPlacement;
        set => Model.TickPlacement = value;
    }

    public double TickFrequency => Model.TickFrequency;

    public double SmallChange => Model.SmallChange;

    public double LargeChange => Model.LargeChange;

    public double Minimum
    {
        get => Model.Minimum;
        set => Model.Minimum = value;
    }

    public Thickness BorderThickness => Model.Border ? new Thickness(1) : new Thickness(0);

    public double Maximum
    {
        get => Model.Maximum;
        set => Model.Maximum = value;
    }

    public bool ShowMinMax
    {
        get => Model.ShowMinMax;
        set => Model.ShowMinMax = value;
    }

    public bool Invert
    {
        get => Model.Invert;
        set => Model.Invert = value;
    }

    public bool IsSnapToTickEnabled => Model.IsSnapToTickEnabled;

    public int Interval => Model.Interval;

    protected void SwapDimensions()
    {
        SliderModel model = Model;
        SliderModel model2 = Model;
        double width = Model.Width;
        double height = Model.Height;
        model.Height = width;
        model2.Width = height;
        OnPropertyChanged("Height");
        OnPropertyChanged("Width");
    }
}
