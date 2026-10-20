#nullable disable

namespace Quire.EngNotebook.Automation;

public partial interface IMathcadPrimeWorksheets
{
    int Count { get; }
    IMathcadPrimeWorksheet2 Item(int indexArg);
}
