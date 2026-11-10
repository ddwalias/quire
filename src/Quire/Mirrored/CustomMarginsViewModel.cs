#nullable disable

using System;
using System.Windows;
using Quire.Controls.Core;
using Quire.Wpf;

namespace Quire;

internal partial class CustomMarginsViewModel
{
    private readonly int _numOfDigitsForRound = 2;

    public string LeftMarginText
    {
        get
        {
            return _leftMarginText;
        }
        set
        {
            // Release IL tests the guard with == and returns early.
            if (_leftMarginText == value || !IsValidMargin(value, out var doubleValue))
            {
                return;
            }

            _leftMarginText = value;
            LeftMargin = doubleValue;
            OnPropertyChanged("LeftMarginText");
        }
    }

    private double LeftMargin
    {
        get
        {
            return _leftMargin;
        }
        set
        {
            if (Math.Round(_leftMargin, _numOfDigitsForRound) == Math.Round(value, _numOfDigitsForRound))
            {
                return;
            }

            _leftMargin = value;
        }
    }

    public string TopMarginText
    {
        get
        {
            return _topMarginText;
        }
        set
        {
            if (_topMarginText == value || !IsValidMargin(value, out var doubleValue))
            {
                return;
            }

            _topMarginText = value;
            TopMargin = doubleValue;
            OnPropertyChanged("TopMarginText");
        }
    }

    private double TopMargin
    {
        get
        {
            return _topMargin;
        }
        set
        {
            if (Math.Round(_topMargin, _numOfDigitsForRound) == Math.Round(value, _numOfDigitsForRound))
            {
                return;
            }

            _topMargin = value;
        }
    }

    public string RightMarginText
    {
        get
        {
            return _rightMarginText;
        }
        set
        {
            if (_rightMarginText == value || !IsValidMargin(value, out var doubleValue))
            {
                return;
            }

            _rightMarginText = value;
            RightMargin = doubleValue;
            OnPropertyChanged("RightMarginText");
        }
    }

    private double RightMargin
    {
        get
        {
            return _rightMargin;
        }
        set
        {
            if (Math.Round(_rightMargin, _numOfDigitsForRound) == Math.Round(value, _numOfDigitsForRound))
            {
                return;
            }

            _rightMargin = value;
        }
    }

    public string BottomMarginText
    {
        get
        {
            return _bottomMarginText;
        }
        set
        {
            if (_bottomMarginText == value || !IsValidMargin(value, out var doubleValue))
            {
                return;
            }

            _bottomMarginText = value;
            BottomMargin = doubleValue;
            OnPropertyChanged("BottomMarginText");
        }
    }

    private double BottomMargin
    {
        get
        {
            return _bottomMargin;
        }
        set
        {
            if (Math.Round(_bottomMargin, _numOfDigitsForRound) == Math.Round(value, _numOfDigitsForRound))
            {
                return;
            }

            _bottomMargin = value;
        }
    }

    public PageSizeUnits PageSizeUnits => _currentPageLayout.PageSizeUnits;

    public Thickness NewPageMargin
    {
        get
        {
            // Release IL builds the margin into one local on both paths before returning it.
            Thickness newPageMargin;
            if (_currentPageLayout.PageSizeUnits == PageSizeUnits.Centimeters)
            {
                newPageMargin = new Thickness(LeftMargin / DisplayUnitsUtils.CmToMmMult, TopMargin / DisplayUnitsUtils.CmToMmMult, RightMargin / DisplayUnitsUtils.CmToMmMult, BottomMargin / DisplayUnitsUtils.CmToMmMult);
            }
            else
            {
                newPageMargin = new Thickness(LeftMargin, TopMargin, RightMargin, BottomMargin);
            }

            return newPageMargin;
        }
    }

    public CustomMarginsViewModel(WorksheetPageLayout currentPageLayout)
        : base(ResourceProvider<CustomMarginsDialogResourceDictionary>.Instance)
    {
        _currentPageLayout = currentPageLayout;
        SetMarginsInDisplayUnits(DipsToDisplayUnitsMargins(_currentPageLayout.PageMargin));
        Size maxPageMarginsRestriction = WorksheetPageLayout.GetMaxPageMarginsRestriction(_currentPageLayout);
        _maxPageMarginsRestriction = new Size(DisplayUnitsUtils.DipsToDisplayUnits(maxPageMarginsRestriction.Width, _currentPageLayout.PageSizeUnits), DisplayUnitsUtils.DipsToDisplayUnits(maxPageMarginsRestriction.Height, _currentPageLayout.PageSizeUnits));
        Thickness minPageMarginsRestriction = WorksheetPageLayout.GetMinPageMarginsRestriction(_currentPageLayout);
        _minPageMarginsRestriction = new Thickness(DisplayUnitsUtils.DipsToDisplayUnits(minPageMarginsRestriction.Left, _currentPageLayout.PageSizeUnits), DisplayUnitsUtils.DipsToDisplayUnits(minPageMarginsRestriction.Top, _currentPageLayout.PageSizeUnits), DisplayUnitsUtils.DipsToDisplayUnits(minPageMarginsRestriction.Right, _currentPageLayout.PageSizeUnits), DisplayUnitsUtils.DipsToDisplayUnits(minPageMarginsRestriction.Bottom, _currentPageLayout.PageSizeUnits));
    }

    private bool IsValidMargin(string strVal, out double doubleValue)
    {
        bool result = false;
        if (strVal.IsNullOrEmpty())
        {
            doubleValue = 0.0;
            result = true;
        }
        else if (double.TryParse(strVal, out doubleValue) && doubleValue >= 0.0)
        {
            result = true;
        }

        return result;
    }

    public void FixPageMargins()
    {
        if (DebugTools.DevAssert(_fixMargins.HasValue, null).Failed)
        {
            return;
        }

        SetMarginsInDisplayUnits(_fixMargins.Value);
        RemoveAllErrors();
    }

    public void ValidateInput()
    {
        RemoveAllErrors();
        _fixMargins = null;
        Thickness thickness = _currentPageLayout.PageSizeUnitsToDipsMargins(NewPageMargin);
        if (WorksheetPageLayout.GetMarginTypeOfMarginByPageSize(_currentPageLayout.PageSize, _currentPageLayout.PageOrientation, thickness, out var actualMargin) == PageMarginType.Custom && !thickness.IsEqualTo(actualMargin, WorksheetPageLayout.MarginEpsilon))
        {
            _fixMargins = DipsToDisplayUnitsMargins(actualMargin);
            if (_minPageMarginsRestriction.Left.IsGreaterThan(LeftMargin))
            {
                SetError("LeftMarginText", CustomMarginsDialogErrors.SmallerThanMinimumErrorKey, _minPageMarginsRestriction.Left);
            }

            if (_minPageMarginsRestriction.Right.IsGreaterThan(RightMargin))
            {
                SetError("RightMarginText", CustomMarginsDialogErrors.SmallerThanMinimumErrorKey, _minPageMarginsRestriction.Right);
            }

            if (_minPageMarginsRestriction.Top.IsGreaterThan(TopMargin))
            {
                SetError("TopMarginText", CustomMarginsDialogErrors.SmallerThanMinimumErrorKey, _minPageMarginsRestriction.Top);
            }

            if (_minPageMarginsRestriction.Bottom.IsGreaterThan(BottomMargin))
            {
                SetError("BottomMarginText", CustomMarginsDialogErrors.SmallerThanMinimumErrorKey, _minPageMarginsRestriction.Bottom);
            }

            if (_maxPageMarginsRestriction.Width.IsLessThan(LeftMargin + RightMargin))
            {
                SetError("LeftMarginText", CustomMarginsDialogErrors.LeftRightSumBiggerThanMaxErrorKey, _maxPageMarginsRestriction.Width);
                SetError("RightMarginText", CustomMarginsDialogErrors.LeftRightSumBiggerThanMaxErrorKey, _maxPageMarginsRestriction.Width);
            }

            if (_maxPageMarginsRestriction.Height.IsLessThan(TopMargin + BottomMargin))
            {
                SetError("TopMarginText", CustomMarginsDialogErrors.TopBottomSumBiggerThanMaxErrorKey, _maxPageMarginsRestriction.Height);
                SetError("BottomMarginText", CustomMarginsDialogErrors.TopBottomSumBiggerThanMaxErrorKey, _maxPageMarginsRestriction.Height);
            }
        }
    }

    private void RemoveAllErrors()
    {
        RemoveAllErrorsForProperty("LeftMarginText");
        RemoveAllErrorsForProperty("RightMarginText");
        RemoveAllErrorsForProperty("TopMarginText");
        RemoveAllErrorsForProperty("BottomMarginText");
    }

    private void SetMarginsInDisplayUnits(Thickness margins)
    {
        LeftMarginText = margins.Left.ToString("N2");
        TopMarginText = margins.Top.ToString("N2");
        RightMarginText = margins.Right.ToString("N2");
        BottomMarginText = margins.Bottom.ToString("N2");
    }

    private Thickness DipsToDisplayUnitsMargins(Thickness margins)
    {
        return new Thickness(DisplayUnitsUtils.DipsToDisplayUnits(margins.Left, _currentPageLayout.PageSizeUnits), DisplayUnitsUtils.DipsToDisplayUnits(margins.Top, _currentPageLayout.PageSizeUnits), DisplayUnitsUtils.DipsToDisplayUnits(margins.Right, _currentPageLayout.PageSizeUnits), DisplayUnitsUtils.DipsToDisplayUnits(margins.Bottom, _currentPageLayout.PageSizeUnits));
    }
}
