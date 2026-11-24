#nullable disable
#pragma warning disable

using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Quire.Calculation.Core;
using Quire.Controls.Plots.Plot3D.Model;
using Quire.Controls.Plots.Presenter;

namespace Quire.Controls.Plots.Plot3D.ViewModel;

internal partial class PlotTraceViewModel3D
{
    private void FirePropertyChanged(string propertyName)
    {
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public bool ChangeSymbol(TraceSymbol value)
    {
        Symbol = value;
        return Symbol == value;
    }

    public void UserChangedStyle()
    {
        _model.UserChangedStyle();
    }

    public bool IsOwnModel(IPlotTraceModel3D model)
    {
        return _model.Equals(model);
    }

    private void ModelPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        string propertyName = e.PropertyName;
        if (propertyName != null)
        {
            if (propertyName == "VisualData")
            {
                CommandManager.InvalidateRequerySuggested();
            }
            if (TypeDescriptor.GetProperties(this)[propertyName] != null)
            {
                FirePropertyChanged(propertyName);
            }
        }
    }

    public bool ChangeLineStyle(LineStyle value)
    {
        LineStyle = value;
        return LineStyle == value;
    }

    public TraceStyleState? DoStyleChangeCommand()
    {
        return null;
    }

    public void UndoStyleChangeCommand(TraceStyleState? doData)
    {
        DebugTools.DevAssert(!doData.HasValue, null);
    }

    public ICalculatableInfo CalculatableInfo => _model;

    public PlotTraceVisualData3D VisualData => _model.VisualData;

    public Rect3D CommonRange => _model.CommonRange;

    public Color Color
    {
        get
        {
            return _model.Color;
        }
        set
        {
            _model.Color = value;
        }
    }

    public TraceSymbol Symbol
    {
        get
        {
            return _model.Symbol;
        }
        set
        {
            _model.Symbol = value;
        }
    }

    public double SymbolWeight
    {
        get
        {
            return _model.SymbolWeight;
        }
        set
        {
            _model.SymbolWeight = value;
        }
    }

    public double LineWeight
    {
        get
        {
            return _model.LineWeight;
        }
        set
        {
            _model.LineWeight = value;
        }
    }

    public Color? SurfaceFill
    {
        get
        {
            return _model.SurfaceFill;
        }
        set
        {
            _model.SurfaceFill = value;
        }
    }

    public Color RenderColor => _model.RenderColor;

    public LineStyle LineStyle
    {
        get
        {
            return _model.LineStyle;
        }
        set
        {
            _model.LineStyle = value;
        }
    }

    public bool IsStyleAutomatic => _model.IsStyleAutomatic;

    public int NumberOfPoints
    {
        get
        {
            return _model.NumberOfPoints;
        }
        set
        {
            int numberOfPoints = _model.NumberOfPoints;
            _model.NumberOfPoints = value;
            if (_model.NumberOfPoints != numberOfPoints)
            {
                _model.Commit();
            }
        }
    }

    public bool IsNumOfPointsMutable => _model.IsNumOfPointsMutable;

    public TraceType3D VisualDataTypeOrDefault => _model.VisualDataTypeOrDefault;

    public bool IsInPrinting
    {
        get
        {
            return _model.IsInPrinting;
        }
        set
        {
            _model.IsInPrinting = value;
        }
    }
}
