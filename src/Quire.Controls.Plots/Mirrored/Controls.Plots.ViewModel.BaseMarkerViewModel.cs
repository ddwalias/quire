#nullable disable
#pragma warning disable

using System;
using System.ComponentModel;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.View;
using Quire.Undo;

namespace Quire.Controls.Plots.ViewModel;

public abstract partial class BaseMarkerViewModel
{
    private readonly BaseMarker _baseMarker;

    private bool _isActive;

    private NumberFormat _displayNumberFormat;

    protected BaseMarkerViewModel(BaseMarker baseMarker)
    {
        _baseMarker = baseMarker;
        _baseMarker.PropertyChanged += (object? obj, PropertyChangedEventArgs e2) => OnPropertyChanged(e2.PropertyName);
        CreateCommandBinding(ActivateCommand, (object arg) => Activate());
        CreateCommandBinding(PlotControlBase.ChangeTraceColorCommand, (object color) => ChangeColor(color as SolidColorBrush));
    }

    private void ChangeColor(SolidColorBrush newColor)
    {
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(newColor != null, null))
        {
            return;
        }
        Color = newColor.Color;
    }

    internal BaseMarker Model => _baseMarker;

    public Color Color
    {
        get => _baseMarker.Color;
        set
        {
            if (value != _baseMarker.Color)
            {
                Color oldColor = Color;
                CommandBuffer.Execute(new UndoableCommand("Set marker color", () =>
                {
                    _baseMarker.Color = value;
                    return true;
                }, () => _baseMarker.Color = oldColor));
            }
        }
    }

    public bool ShowCallout
    {
        get => _baseMarker.ShowCallout;
        set => _baseMarker.ShowCallout = value;
    }

    public IMarkerCalculatable Calculatable { get; protected set; }

    public bool IsActive
    {
        get => _isActive;
        set
        {
            if (_isActive != value)
            {
                _isActive = value;
                OnPropertyChanged("IsActive");
            }
        }
    }

    public NumberFormat DisplayNumberFormat
    {
        get => _displayNumberFormat;
        set
        {
            if (_displayNumberFormat != value)
            {
                _displayNumberFormat = value;
                OnPropertyChanged("DisplayNumberFormat");
            }
        }
    }

    public event EventHandler<EventArgs> Activated;

    internal void Activate()
    {
        IsActive = true;
        EventHandler<EventArgs> activated = Activated;
        if (activated != null)
        {
            activated(this, new EventArgs());
        }
    }

    internal void Deactivate() => IsActive = false;
}
