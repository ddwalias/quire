#nullable disable

namespace Quire.Automation;

[global::Quire.Mirror.MirrorOf("Ptc.MathcadPrime.Automation.IMathcadPrimeApplication, Ptc.MathcadPrime.Automation")]
public partial interface IMathcadPrimeApplication
{
    bool Visible { get; set; }
    IMathcadPrimeWorksheet ActiveWorksheet { get; }
    void Activate();
    void Quit(SaveOption saveOptionArg);
    IMathcadPrimeWorksheet Open(string documentPathArg);
    int InitializeEvents(IMathcadPrimeEvents eventsArg);
}
