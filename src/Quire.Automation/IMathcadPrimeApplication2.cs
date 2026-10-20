#nullable disable

namespace Quire.Automation;

public partial interface IMathcadPrimeApplication2
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
}
