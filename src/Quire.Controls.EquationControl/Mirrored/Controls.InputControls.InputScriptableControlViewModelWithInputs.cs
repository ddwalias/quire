using System.Windows;
using Quire.Controls.Core;
using Quire.EquationEditor.Tree;

namespace Quire.Controls.InputControls;

public partial class InputScriptableControlViewModelWithInputs
{
    private bool _isMathRegionActive;

    private new IInputScriptableControlModelWithInputs Model => (IInputScriptableControlModelWithInputs)_model;

    private IInputScriptableControlDataWithInputs Data => (IInputScriptableControlDataWithInputs)_model.Data;

    public InputScriptableControlViewModelWithInputs(IInputScriptableControlModelWithInputs model)
        : base(model)
    {
    }

    public bool IsLhsLabel
    {
        get
        {
            Size outputFieldSize = Model.OutputFieldSize;
            return outputFieldSize.Height == 0.0;
        }
    }

    public Visibility ToggleVisibility
    {
        get
        {
            if (Data.IsInputsExists && IsMathRegionActive)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }
    }

    public bool IsToggleChecked
    {
        get => Data.IsInputsAreaShown;
        set
        {
            Data.IsInputsAreaShown = value;
            OnPropertyChanged("InputFieldVisibility");
        }
    }

    public Visibility InputFieldVisibility
    {
        get
        {
            if (Data.IsInputsAreaShown)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }
    }

    public bool IsInputEnabled
    {
        get => Data.IsInputsExists;
        set
        {
            Data.IsInputsAreaShown = value;
            _isMathRegionActive = true;
            Data.IsInputsExists = value;
            if (value && Data.InputFieldSExpression == null)
            {
                Data.InputFieldSExpression = string.Empty;
            }
            OnPropertyChanged("InputFieldVisibility");
            OnPropertyChanged("ToggleVisibility");
            OnPropertyChanged("IsToggleChecked");
        }
    }

    public bool IsMathRegionActive
    {
        get => _isMathRegionActive;
        set
        {
            _isMathRegionActive = value;
            OnPropertyChanged("ToggleVisibility");
        }
    }

    public void HandleSExpressionChanged(IEquationControl eq)
    {
        Model.HandleSExpressionChanged(eq);
    }
}
