#nullable disable

namespace Quire.EngNotebookPlus.Automation;

public partial interface IMathcadPrimeWorksheet2
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
    IMathcadPrimeMatrix CreateMatrix(int rowsArg, int columnsArg);
    int SetMatrixValue(string aliasArg, IMathcadPrimeMatrix valueArg, string unitsArg);
    IMathcadPrimeInputMatrixResult InputGetMatrixValue(string aliasArg);
    IMathcadPrimeOutputMatrixResult OutputGetMatrixValue(string aliasArg);
    IMathcadPrimeOutputMatrixResultAs OutputGetMatrixValueAs(string aliasArg, string unitsArg);
    bool IsOpen();
    void Activate();
    void Close(SaveOption saveOptionArg);
}
