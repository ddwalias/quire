using System;
using System.ComponentModel;
using System.Windows.Media;
using System.Xml;
using Quire.Calculation.Core;

namespace Quire.Controls.Plots.Model;

public partial class BaseMarker : IBaseMarker, INotifyPropertyChanged
{
    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Plots.Model.BaseMarker+PropertyNames, Ptc.Controls.Plots")]
    public static partial class PropertyNames
    {
        public const string Color = "Color";
        public const string ShowCallout = "ShowCallout";
        public const string IsUserSetValue = "IsUserSetValue";
    }

    private Color _color;
    private bool _showCallout = true;
    private IEquationSerializationProxy _equationProxy;
    private IMarkerCalculatable _calculatable;
    private bool _isUserSetValue;

    public Color Color
    {
        get => _color;
        set
        {
            if (_color != value)
            {
                _color = value;
                OnPropertyChanged("Color");
            }
        }
    }

    public IMarkerCalculatable Calculatable
    {
        get => _calculatable;
        protected set
        {
            if (_calculatable != null)
            {
                _calculatable.RequestEvaluation -= CalculatableRequestEvaluation;
            }
            _calculatable = value;
            if (_calculatable != null)
            {
                _calculatable.RequestEvaluation += CalculatableRequestEvaluation;
            }
        }
    }

    private void CalculatableRequestEvaluation(ICalculatable obj)
    {
        if (_equationProxy != null && _equationProxy.IsCleared)
        {
            IsUserSetValue = false;
        }
        Calculatable.OverrideHideFromCalculation = !IsUserSetValue;
    }

    public bool ShowCallout
    {
        get => _showCallout;
        set
        {
            if (_showCallout != value)
            {
                _showCallout = value;
                OnPropertyChanged("ShowCallout");
            }
        }
    }

    public bool IsUserSetValue
    {
        get => _isUserSetValue;
        private set
        {
            SetIsUserSetValue(value, true);
        }
    }

    public void SetIsUserSetValue(bool value, bool updateCalculatableHiddenState)
    {
        if (value != _isUserSetValue)
        {
            _isUserSetValue = value;
            if (updateCalculatableHiddenState)
            {
                Calculatable.OverrideHideFromCalculation = !value;
            }
            OnPropertyChanged("IsUserSetValue");
        }
    }

    [DefaultValue(null)]
    public string EquationSExpression
    {
        get
        {
            if (_equationProxy == null || !IsUserSetValue)
            {
                return null;
            }
            return _equationProxy.EquationSExpression;
        }
        set
        {
            IsUserSetValue = true;
            new DoActionWhenEquationProxyChanged(this, () =>
            {
                _equationProxy.EquationSExpression = value;
                Calculatable.Commit();
            });
        }
    }

    public XmlNode ExpressionXml
    {
        get => _equationProxy.EquationXmlNode;
        protected set
        {
            IsUserSetValue = true;
            new DoActionWhenEquationProxyChanged(this, () =>
            {
                _equationProxy.EquationXmlNode = value;
                // Release IL compares against 3 (SynchronizationStatus.Synchronized in Mathcad)
                if (Calculatable.SynchronizationStatus != (SynchronizationStatus)3)
                {
                    Calculatable.Commit();
                }
            });
        }
    }

    public event PropertyChangedEventHandler PropertyChanged;

    protected void OnPropertyChanged(string propertyName)
    {
        PropertyChangedEventHandler handler = PropertyChanged;
        if (handler != null)
        {
            handler(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    private event Action EquationProxyChanged;

    public void SetEquationProxy(IEquationSerializationProxy value)
    {
        if (_equationProxy != value)
        {
            _equationProxy = value;
            if (EquationProxyChanged != null)
            {
                EquationProxyChanged();
            }
        }
    }

    public BaseMarker()
    {
    }

    [global::Quire.Mirror.MirrorOf("Ptc.Controls.Plots.Model.BaseMarker+DoActionWhenEquationProxyChanged, Ptc.Controls.Plots")]
    private partial class DoActionWhenEquationProxyChanged
    {
        private readonly Action _action;
        private readonly BaseMarker _baseMarker;

        public DoActionWhenEquationProxyChanged(BaseMarker baseMarker, Action action)
        {
            _action = action;
            _baseMarker = baseMarker;
            if (_baseMarker._equationProxy != null)
            {
                _action();
            }
            else
            {
                _baseMarker.EquationProxyChanged += ActionWrapper;
            }
        }

        private void ActionWrapper()
        {
            if (_baseMarker._equationProxy != null)
            {
                _baseMarker.EquationProxyChanged -= ActionWrapper;
                _action();
            }
        }
    }
}
