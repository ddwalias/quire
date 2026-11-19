#nullable disable
#pragma warning disable

using System;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Controls.Plots.Model;
using Quire.Controls.Plots.View;

namespace Quire.Controls.Plots.ViewModel;

public abstract partial class BaseMarkerViewModel
{
    internal BaseMarker Model => _baseMarker;

    public bool ShowCallout
    {
        get
        {
            return _baseMarker.ShowCallout;
        }
        set
        {
            _baseMarker.ShowCallout = value;
        }
    }

    public IMarkerCalculatable Calculatable { get; protected set; }

    public bool IsActive
    {
        get
        {
            return _isActive;
        }
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
        get
        {
            return _displayNumberFormat;
        }
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

    internal void Deactivate()
    {
        IsActive = false;
    }
}
