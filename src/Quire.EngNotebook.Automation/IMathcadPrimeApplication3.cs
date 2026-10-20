#nullable disable

namespace Quire.EngNotebook.Automation;

public partial interface IMathcadPrimeApplication3
{
    bool Visible { get; set; }
    IMathcadPrimeWorksheet ActiveWorksheet { get; }
    IMathcadPrimeWorksheets Worksheets { get; }
    void Activate();
    void Quit(SaveOption saveOptionArg);
    IMathcadPrimeWorksheet Open(string documentPathArg);
    int InitializeEvents(IMathcadPrimeEvents eventsArg);
    void CloseAll(SaveOption saveOptionArg);
    string GetVersion();
    int InitializeEvents2(IMathcadPrimeEvents2 eventsArg, bool subscribeAllArg);
    int SubscribeEvent(MathcadPrimeEvents primeEventArg);
    int UnsubscribeEvent(MathcadPrimeEvents primeEventArg);
    IMathcadPrimeWorksheetReadonlyOptions CreateWorksheetReadonlyOptions();
    IMathcadPrimeWorksheet3 OpenEx(string documentPathArg, IMathcadPrimeWorksheetReadonlyOptions optionsArg);
    void UpdateInputs();
    void SetContextData(string[] contextData);
}
