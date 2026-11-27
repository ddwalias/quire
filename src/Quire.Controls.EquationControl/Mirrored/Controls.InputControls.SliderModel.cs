using System.Collections;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

internal partial class SliderModel
{
    private double _sliderValue;

    private SliderScriptableControlData Data => (SliderScriptableControlData)_data;

    public SliderModel(IInputControlDataCalculatedValue data)
        : base(new InputSliderEngineFactory(), data)
    {
    }

    public override object SelectedValue => Data.SelectedValueIndex;

    public override bool Apply(string userScript, out object resultObject, out string exception, object[] inputs, IScriptEngineParams engineParams = null)
    {
        return base.Apply(
            userScript,
            out resultObject,
            out exception,
            inputs ?? Data.InputsResultValue ?? scriptableEngine.GetLabelsFromSExpresstion(Data.InputFieldSExpression)?.ToArray<object>(),
            engineParams ?? new ScriptEngineParams(Data)
            {
                IsApply = true
            });
    }

    protected override void HandlePostInputFieldResultChanged(ref object valueIndex, object scriptObject, string errorException, bool isRecalculate = false)
    {
        if (scriptObject is Hashtable hashtable && errorException == null)
        {
            if (hashtable.ContainsKey("orientation") && double.TryParse(hashtable["orientation"].ToString(), out double orientation))
            {
                Orientation = orientation == 0.0 ? Orientation.Horizontal : Orientation.Vertical;
            }

            if (hashtable.ContainsKey("pagesize") && double.TryParse(hashtable["pagesize"].ToString(), out double pageSize))
            {
                LargeChange = pageSize;
            }

            if (hashtable.ContainsKey("linesize") && double.TryParse(hashtable["linesize"].ToString(), out double lineSize))
            {
                SmallChange = lineSize;
            }

            if (hashtable.ContainsKey("minrange") && double.TryParse(hashtable["minrange"].ToString(), out double minRange))
            {
                Minimum = minRange;
            }

            if (hashtable.ContainsKey("maxrange") && double.TryParse(hashtable["maxrange"].ToString(), out double maxRange))
            {
                Maximum = maxRange;
            }

            if (hashtable.ContainsKey("showminmax") && bool.TryParse(hashtable["showminmax"].ToString(), out bool showMinMax))
            {
                ShowMinMax = showMinMax;
            }

            if (hashtable.ContainsKey("invert") && bool.TryParse(hashtable["invert"].ToString(), out bool invert))
            {
                Invert = invert;
            }

            if (hashtable.ContainsKey("border") && bool.TryParse(hashtable["border"].ToString(), out bool border))
            {
                Border = border;
            }

            bool ticks;
            int point;
            if (hashtable.ContainsKey("ticks"))
            {
                TickPlacement = TickPlacement.None;
                if (bool.TryParse(hashtable["ticks"].ToString(), out ticks) & ticks)
                {
                    TickPlacement = TickPlacement.Both;
                    if (hashtable.ContainsKey("point") && int.TryParse(hashtable["point"].ToString(), out point))
                    {
                        if (point == 1)
                        {
                            TickPlacement = TickPlacement.TopLeft;
                        }
                        else if (point == 2)
                        {
                            TickPlacement = TickPlacement.BottomRight;
                        }
                    }
                }
            }

            if (hashtable.ContainsKey("autoticks") && bool.TryParse(hashtable["autoticks"].ToString(), out bool autoTicks))
            {
                IsSnapToTickEnabled = autoTicks;
            }

            if (hashtable.ContainsKey("tickfrequency") && double.TryParse(hashtable["tickfrequency"].ToString(), out double tickFrequency))
            {
                TickFrequency = tickFrequency;
            }

            if (hashtable.ContainsKey("position") && double.TryParse(hashtable["position"].ToString(), out double position))
            {
                Data.SelectedValueIndex = position;
                valueIndex = position;
                IsDirty = !isRecalculate;
                OnPropertyChanged(nameof(Position));
            }
        }
    }

    public override string DefaultScript(InputControlEngineTypes engineType)
    {
        return ((IInputControlScriptingEngine)_engineFactory.CreateEngine(engineType)).Script;
    }

    public Orientation Orientation
    {
        get => Data.Orientation;
        set
        {
            if (Data.Orientation != value)
            {
                Data.Orientation = value;
                OnPropertyChanged(nameof(Orientation));
            }
        }
    }

    public TickPlacement TickPlacement
    {
        get => Data.TickPlacement;
        set
        {
            Data.TickPlacement = value;
            OnPropertyChanged(nameof(TickPlacement));
        }
    }

    public double Position
    {
        get => Data.SelectedValueIndex;
        set
        {
            Data.SelectedValueIndex = value;
            Data.IsNeedsCalculate = false;
            string exception = null;
            object selectedValue = Data.SelectedValueIndex;
            _data.SelectedCalculatedValue = scriptableEngine.GetCalculatedValue(
                Script,
                ref selectedValue,
                out object scriptControlObject,
                out exception,
                Data.InputsResultValue,
                new ScriptEngineParams(Data)
                {
                    IsInteractChanged = true
                });
            HandlePostInputFieldResultChanged(ref selectedValue, scriptControlObject, exception);
            if (exception != null)
            {
                EngineError = exception;
                OnPropertyChanged(nameof(EngineError));
            }
            else
            {
                if (EngineError != null)
                {
                    EngineError = null;
                    OnPropertyChanged(nameof(EngineError));
                }

                HandelValueIndexChangedAndCommit(Data.SelectedValueIndex);
            }

            OnPropertyChanged(nameof(Position));
        }
    }

    public double TickFrequency
    {
        get => Data.TickFrequency;
        set
        {
            Data.TickFrequency = value;
            OnPropertyChanged(nameof(TickFrequency));
        }
    }

    public double SmallChange
    {
        get => Data.SmallChange;
        set
        {
            Data.SmallChange = value;
            OnPropertyChanged(nameof(SmallChange));
        }
    }

    public double LargeChange
    {
        get => Data.LargeChange;
        set
        {
            Data.LargeChange = value;
            OnPropertyChanged(nameof(LargeChange));
        }
    }

    public double Minimum
    {
        get => Data.Minimum;
        set
        {
            Data.Minimum = value;
            OnPropertyChanged(nameof(Minimum));
        }
    }

    public double Maximum
    {
        get => Data.Maximum;
        set
        {
            Data.Maximum = value;
            OnPropertyChanged(nameof(Maximum));
        }
    }

    public bool ShowMinMax
    {
        get => Data.ShowMinMax;
        set
        {
            Data.ShowMinMax = value;
            OnPropertyChanged(nameof(ShowMinMax));
        }
    }

    public bool Invert
    {
        get => Data.Invert;
        set
        {
            Data.Invert = value;
            OnPropertyChanged(nameof(Invert));
        }
    }

    public bool IsSnapToTickEnabled
    {
        get => Data.IsSnapToTickEnabled;
        set
        {
            Data.IsSnapToTickEnabled = value;
            OnPropertyChanged(nameof(IsSnapToTickEnabled));
        }
    }

    public bool Border
    {
        get => Data.Border;
        set
        {
            Data.Border = value;
            OnPropertyChanged(nameof(Border));
        }
    }

    public int Interval
    {
        get => Data.Interval;
        set
        {
            Data.Interval = value;
            OnPropertyChanged(nameof(Interval));
        }
    }
}
