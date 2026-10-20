#nullable disable

namespace Quire.EngNotebookPlus.Automation;

public partial interface IMathcadPrimeApplication
{
    bool Visible { get; set; }
    IMathcadPrimeWorksheet ActiveWorksheet { get; }
    void Activate();
    void Quit(SaveOption saveOptionArg);
    IMathcadPrimeWorksheet Open(string documentPathArg);
    int InitializeEvents(IMathcadPrimeEvents eventsArg);
}
