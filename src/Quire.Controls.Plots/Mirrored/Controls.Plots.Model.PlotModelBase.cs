using System;
using System.Collections.Generic;
using System.ComponentModel;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.PersistentDataObjects;
using Quire.Serialization;

namespace Quire.Controls.Plots.Model;

public abstract partial class PlotModelBase : IPlotModelBase, INotifyPropertyChanged
{
    internal partial class PropertyNames
    {
        internal const string CalcIsDisabled = "CalcIsDisabled";
        internal const string HasError = "HasError";
        internal const string ShowAxisExpressions = "ShowAxisExpressions";
        internal const string PlotBackgroundType = "PlotBackgroundType";
        internal const string IsCalculating = "IsCalculating";
        internal const string GraphWidth = "GraphWidth";
        internal const string GraphHeight = "GraphHeight";

        protected PropertyNames()
        {
        }
    }

    private readonly List<string> _propertyChanging = new List<string>();
    private bool _hasError;
    private bool _showAxisExpressions = true;
    private BackgroundTypeE _plotBackgroundType;
    private bool _isCalculating;
    private double _graphHeight;
    private double _graphWidth;
    private bool _isDeserializing;

    public event PropertyChangedEventHandler PropertyChanged;

    public event Action Changed;

    public event Action DeserializationEnded;

    internal bool HasError
    {
        get => _hasError;
        set
        {
            if (_hasError != value)
            {
                _hasError = value;
                FirePropertyChanged("HasError");
            }
        }
    }

    public bool IsDeserializing
    {
        get => _isDeserializing;
        protected set
        {
            if (value != _isDeserializing)
            {
                _isDeserializing = value;
                if (!_isDeserializing)
                {
                    FireDeserializationEnded();
                }
            }
        }
    }

    private void FireDeserializationEnded()
    {
        Action deserializationEnded = DeserializationEnded;
        if (deserializationEnded != null)
        {
            deserializationEnded();
        }
    }

    public virtual IEnumerable<ICalculatable> GetHiddenAndVisibleCalculatables()
    {
        yield break;
    }

    public abstract bool SetDataObject(IDeS11NProvider de11NProvider, plotType dataObject, IResultManager resultManager);

    public abstract plotType GetDataObject(IS11NProvider s11NProvider, IResultManager resultManager, bool hideFromCalculationContext);

    protected void FirePropertyChanged(string propertyName)
    {
        if (AssertionE.Failed == (AssertionE)DebugTools.DevAssert(!_propertyChanging.Contains(propertyName), null))
        {
            return;
        }

        _propertyChanging.Add(propertyName);
        if (PropertyChanged != null)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
        _propertyChanging.Remove(propertyName);
    }

    protected void OnChanged()
    {
        if (Changed != null && !IsDeserializing)
        {
            Changed();
        }
    }

    protected void OnPropertyChanged(string propertyName)
    {
        FirePropertyChanged(propertyName);
        OnChanged();
    }

    public bool ShowAxisExpressions
    {
        get => _showAxisExpressions;
        set
        {
            if (_showAxisExpressions != value)
            {
                _showAxisExpressions = value;
                OnPropertyChanged("ShowAxisExpressions");
            }
        }
    }

    public BackgroundTypeE PlotBackgroundType
    {
        get => _plotBackgroundType;
        set
        {
            if (value != _plotBackgroundType)
            {
                _plotBackgroundType = value;
                OnPropertyChanged("PlotBackgroundType");
            }
        }
    }

    public double GraphHeight
    {
        get => _graphHeight;
        set
        {
            if (value != _graphHeight)
            {
                _graphHeight = value;
                OnPropertyChanged("GraphHeight");
            }
        }
    }

    public double GraphWidth
    {
        get => _graphWidth;
        set
        {
            if (value != _graphWidth)
            {
                _graphWidth = value;
                OnPropertyChanged("GraphWidth");
            }
        }
    }

    internal SizeType GraphSizeDataObject
    {
        get => new SizeType
        {
            height = GraphHeight,
            width = GraphWidth
        };
        set
        {
            bool isDeserializing = IsDeserializing;
            IsDeserializing = true;
            if (value != null)
            {
                GraphHeight = value.height;
                GraphWidth = value.width;
            }
            IsDeserializing = isDeserializing;
        }
    }

    public bool IsCalculating
    {
        get => _isCalculating;
        protected set
        {
            if (_isCalculating != value)
            {
                _isCalculating = value;
                FirePropertyChanged("IsCalculating");
            }
        }
    }

    public event EventHandler<EngineErrorEventArgs> PlotCalculatableError;

    protected void OnPlotCalculatableError(EngineErrorEventArgs args)
    {
        if (PlotCalculatableError != null)
        {
            PlotCalculatableError(this, args);
        }
    }

    protected PlotModelBase()
    {
    }
}
