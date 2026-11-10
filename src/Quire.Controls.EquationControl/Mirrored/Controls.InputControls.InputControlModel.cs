using System.ComponentModel;
using System.Windows;
using Quire.Calculation.Core;
using Quire.EquationEditor.Tree;
using Quire.FunctionalitiesLimitation;
using Quire.Services;

namespace Quire.Controls.InputControls;

public abstract partial class InputControlModel
{
    public IInputControlEngine InputEngine { get; protected set; }

    public string EngineError
    {
        get => Data.EngineError;
        protected set => Data.EngineError = value;
    }

    public virtual double FontSize
    {
        get => DefaultFontSize;
        set
        {
        }
    }

    public bool IsFunctionalityEnabled => FunctionalityService.Instance.IsFunctionalityEnabled(EnumFunctionalityKey.InsertInputControlCommand);

    public CalculatableState State { get; protected set; }

    public IInputControlDataCalculatedValue Data => _data;

    public virtual InputControlEngineTypes EngineType
    {
        get => Data.EngineType;
        set
        {
            Data.EngineType = value;
            CreateEngine(value);
        }
    }

    public Size OutputFieldSize
    {
        get => Data.OutputFieldSize;
        set => Data.OutputFieldSize = value;
    }

    public bool IsInitializeComponent { get; set; }

    public bool IsDirty
    {
        get => _isDirty;
        set
        {
            if (_isDirty != value)
            {
                _isDirty = value;
                if (_isDirty)
                {
                    OnPropertyChanged(nameof(IsDirty));
                }
            }
        }
    }

    public bool IsEnabled
    {
        get => _isEnabled;
        set => _isEnabled = value;
    }

    protected virtual void HandlePostInputFieldResultChanged(ref object valueIndex, object scriptObject, string errorException, bool isRecalculate = false)
    {
    }

    protected virtual void OnPropertyChanged(string propertyName = default)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public virtual void HandlePropertyChanged(string propertyName, object value)
    {
    }

    protected void CreateEngine(InputControlEngineTypes engineType)
    {
        if (InputEngine != null)
        {
            InputEngine.Dispose();
        }

        InputEngine = _engineFactory.CreateEngine(engineType);
    }
}
