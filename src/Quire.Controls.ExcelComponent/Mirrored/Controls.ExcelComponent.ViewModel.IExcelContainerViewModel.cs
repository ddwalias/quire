#nullable disable
#pragma warning disable
namespace Quire.Controls.ExcelComponent.ViewModel;

public partial interface IExcelContainerViewModel
{
    bool? IsAsIcon { get; set; }
    string GetExcelSelectionOrDefault(bool isInput);
}
