#nullable disable

namespace Quire.EngNotebook.Automation;

public partial interface IMathcadPrimeWorksheetReadonlyOptions
{
    int SetOptionValue(WorksheetReadonlyOptionNames optionNameArg, object optionValueArg);
}
