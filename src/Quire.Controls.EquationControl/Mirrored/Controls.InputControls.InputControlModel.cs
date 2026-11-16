using System.ComponentModel;
using System.Windows;
using Quire.Calculation.Core;
using Quire.EquationEditor.Tree;
using Quire.FunctionalitiesLimitation;
using Quire.Services;

namespace Quire.Controls.InputControls;

public abstract partial class InputControlModel
{
    protected bool _isEnabled = true;
    protected bool _isAutoRecalculate = true;

    public event PropertyChangedEventHandler PropertyChanged;

    protected InputControlModel()
    {
    }

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

    protected Size GetFieldSizeFormSExpression(string sexpression, string sexpressionTag)
    {
        int num = 0;
        int num2 = 0;
        if (sexpression != null && !(sexpression == "PLACEHOLDER") && !sexpression.Contains("1 1 @PLACEHOLDER") && sexpression.IndexOf(sexpressionTag) == 0)
        {
            num = int.Parse(sexpression.Substring(sexpressionTag.Length, 2));
            num2 = int.Parse(sexpression.Substring(sexpressionTag.Length + 2, 2));
        }

        return new Size(num2, num);
    }
}
