#nullable disable

namespace Quire.EngNotebookPlus.Automation;

public partial interface IMathcadPrimeWorksheet3
{
    string Name { get; }
    string FullName { get; }
    bool IsReadOnly { get; }
    bool Modified { get; set; }
    IMathcadPrimeInputs Inputs { get; }
    IMathcadPrimeOutputs Outputs { get; }
    string WorksheetTabIcon { get; set; }
    string WorksheetTabName { get; set; }
    string WorksheetClosingPrompt { get; set; }
    string WorksheetDisplayedFilePath { get; set; }
    string WorksheetWorkingDirectory { get; set; }
    int DefaultCalculationTimeout { get; set; }
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
    string GetTitle();
    object GetWorksheetReadOnlyOptionValue(WorksheetReadonlyOptionNames optionNameArg);
    IMathcadPrimeValuesSetter CreateValuesSetter();
    int SetStringValue(string aliasArg, string valueArg);
    string InputGetStringValue(string aliasArg);
    string OutputGetStringValue(string aliasArg);
    int SetSExprValue(string aliasArg, string sexpressionArg);
    string InputGetSExprValue(string aliasArg);
    IMathcadPrimeGetValueResult InputGetValue(string aliasArg);
    IMathcadPrimeGetValueResult OutputGetValue(string aliasArg);
    void SaveAndPush(SaveOption saveOptionArg);
}
