#nullable disable

namespace Quire.Automation;

public partial interface IMathcadPrimeWorksheets
{
    int Count { get; }
    IMathcadPrimeWorksheet2 Item(int indexArg);
}
