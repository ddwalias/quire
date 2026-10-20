#nullable disable

namespace Quire.EngNotebook.Automation;

public partial interface IMathcadPrimeEvents
{
    void OnSelect(IMathcadPrimeInputs inputsOnSelectArg, IMathcadPrimeOutputs outputsOnSelectArg);
    void OnSave(string documentNameArg);
    void OnExit();
}
