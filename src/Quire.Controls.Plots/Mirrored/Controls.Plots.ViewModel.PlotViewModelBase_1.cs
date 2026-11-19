#nullable disable
#pragma warning disable

namespace Quire.Controls.Plots.ViewModel;

public partial class PlotViewModelBase<TModel>
{
    public bool ShowControlLevelErrorFrameForNestedError
    {
        get
        {
            return _showControlLevelErrorFrameForNestedError;
        }
        private set
        {
            if (value != _showControlLevelErrorFrameForNestedError)
            {
                _showControlLevelErrorFrameForNestedError = value;
                OnPropertyChanged("ShowControlLevelErrorFrameForNestedError");
            }
        }
    }

    public bool AreAxisExpressionsVisible
    {
        get
        {
            return _areAxisExpressionsVisible;
        }
        private set
        {
            if (_areAxisExpressionsVisible != value)
            {
                _areAxisExpressionsVisible = value;
                OnAxisExpressionsVisibilityChanged();
                OnPropertyChanged("AreAxisExpressionsVisible");
            }
        }
    }

    public bool CalcIsDisabled
    {
        get
        {
            return _calcIsDisabled;
        }
        set
        {
            if (_calcIsDisabled != value)
            {
                _calcIsDisabled = value;
                OnPropertyChanged("CalcIsDisabled");
            }
        }
    }
}
