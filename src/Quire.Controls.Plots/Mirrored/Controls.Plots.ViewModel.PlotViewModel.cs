#nullable disable
#pragma warning disable

using System.Collections.Generic;
using System.Windows.Media;
using Quire.Controls.Core;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.ViewModel;

internal partial class PlotViewModel
{
    public bool ShowAxisExpressions
    {
        get
        {
            return _model.ShowAxisExpressions;
        }
        set
        {
            _model.ShowAxisExpressions = value;
        }
    }

    public bool IsActive
    {
        get
        {
            return _isActive;
        }
        set
        {
            if (value != _isActive)
            {
                _isActive = value;
                OnPropertyChanged("IsActive");
            }
        }
    }

    public List<IPlotTrace> SelectedTraces
    {
        get
        {
            return _selectedTraces;
        }
        set
        {
            if (!_selectedTraces.IsNullOrEmpty())
            {
                _selectedTraces.Each(DeselectTrace);
            }
            _selectedTraces = value;
            if (!_selectedTraces.IsNullOrEmpty())
            {
                _selectedTraces.Each(SelectTrace);
            }
            SetValuesForStyleProperties();
            OnPropertyChanged("SelectedTraces");
        }
    }

    public Color? SelectedColor
    {
        get
        {
            return _selectedColor;
        }
        set
        {
            if (_selectedColor != value)
            {
                _selectedColor = value;
                OnPropertyChanged("SelectedColor");
            }
        }
    }
}
