using Quire.Controls.Core;

namespace Quire.Controls.Worksheet;

public partial class HeaderFooterOptionsViewModel : ViewModelBase
{
    private readonly IWorksheetViewModel _worksheetViewModel;

    private bool _isUsingDefaultTotalPagesValue;

    public bool IsFirstPageHeaderFooterDifferent
    {
        get => _worksheetViewModel.IsFirstPageHeaderFooterDifferent;
        set => _worksheetViewModel.IsFirstPageHeaderFooterDifferent = value;
    }

    public string InvalidTotalPagesMessage { get; set; }

    public int StartingPageNumber
    {
        get => _worksheetViewModel.StartingPageNumber;
        set => _worksheetViewModel.StartingPageNumber = value;
    }

    public int TotalPagesNumber
    {
        get => _worksheetViewModel.TotalPagesOffset + _worksheetViewModel.PageCount + StartingPageNumber - 1;
        set => _worksheetViewModel.TotalPagesOffset = value - _worksheetViewModel.PageCount - StartingPageNumber + 1;
    }

    public HeaderFooterOptionsViewModel(IWorksheetViewModel worksheetViewModel)
    {
        _worksheetViewModel = worksheetViewModel;
    }

    public void HandlePageNumberChanged(object sender, NumericUpDown pageNumberControl, NumericUpDownWatermark totalPagesControl)
    {
        int calculatedDefaultTotalPages = CalculateDefaultTotalPages(pageNumberControl.NumberValue);
        UpdateTotalPagesValidationMessage(totalPagesControl, calculatedDefaultTotalPages);
        UpdateTotalPagesByStartingPage(sender, pageNumberControl, totalPagesControl, calculatedDefaultTotalPages);
        UpdateWatermark(sender, pageNumberControl, totalPagesControl, calculatedDefaultTotalPages);
    }

    public void UpdateTotalPagesValidationMessage(NumericUpDown totalPagesControl, int calculatedDefaultTotalPages)
    {
        int numberValue = totalPagesControl.NumberValue;
        if (numberValue < calculatedDefaultTotalPages)
        {
            totalPagesControl.IsInputValid = false;
            totalPagesControl.ValidationMessage = $"{InvalidTotalPagesMessage} {calculatedDefaultTotalPages}";
        }
        if (!totalPagesControl.IsInputValid && hasInvalidTotalPageAsMessage(totalPagesControl) && numberValue >= calculatedDefaultTotalPages)
        {
            totalPagesControl.IsInputValid = true;
        }
    }

    private bool hasInvalidTotalPageAsMessage(NumericUpDown totalPagesControl)
    {
        return totalPagesControl.ValidationMessage.StartsWith(InvalidTotalPagesMessage);
    }

    public void UpdateTotalPagesByStartingPage(object sender, NumericUpDown pageNumberControl, NumericUpDown totalPagesControl, int calculatedDefaultTotalPages)
    {
        if (sender == pageNumberControl && _isUsingDefaultTotalPagesValue)
        {
            totalPagesControl.NumberValue = calculatedDefaultTotalPages;
        }
        _isUsingDefaultTotalPagesValue = totalPagesControl.NumberValue == calculatedDefaultTotalPages;
    }

    public void UpdateWatermark(object sender, NumericUpDown pageNumberControl, NumericUpDownWatermark totalPagesControl, int calculatedDefaultTotalPages)
    {
        if (sender == pageNumberControl)
        {
            totalPagesControl.WatermarkValue = calculatedDefaultTotalPages.ToString();
        }
    }

    public int CalculateDefaultTotalPages(int startingPage)
    {
        return startingPage + _worksheetViewModel.PageCount - 1;
    }

    public void UpdateTotalPagesValue(NumericUpDown totalPagesControl, int value)
    {
        if (value >= 0 && value <= 1999)
        {
            totalPagesControl.NumberValue = value;
        }
    }
}
