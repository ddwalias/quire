#nullable disable

namespace Quire.Automation;

public partial interface IMathcadPrimeWorksheet
{
    string Name { get; }
    string FullName { get; }
    bool IsReadOnly { get; }
    bool Modified { get; set; }
    IMathcadPrimeInputs Inputs { get; }
    IMathcadPrimeOutputs Outputs { get; }
    void SetTitle(string titleArg);
    void Save();
    void SaveAs(string fileName);
    void Synchronize();
    void PauseCalculation();
    void ResumeCalculation();
    int SetRealValue(string aliasArg, double valueArg, string unitsArg);
    IMathcadPrimeInputResult InputGetRealValue(string aliasArg);
    IMathcadPrimeOutputResult OutputGetRealValue(string aliasArg);
    IMathcadPrimeOutputResultAs OutputGetRealValueAs(string aliasArg, string unitsArg);
}
