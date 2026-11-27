using System.Windows.Threading;

namespace Quire.AutomationServiceEngNote;

public partial class CMathcadPrimeWorksheets
{
    private readonly bool _isUiInteractiveModeDisabled;

    public int Count
    {
        get
        {
            try
            {
                DelegateFunction0<int> count = DoGetCount;
                return (int)MathcadPrimeComServerManager.Instance.InvokeDelegate(count, DispatcherPriority.Normal);
            }
            catch
            {
            }

            return 0;
        }
    }

    public CMathcadPrimeWorksheets(bool isUiInteractiveModeDisabled) { _isUiInteractiveModeDisabled = isUiInteractiveModeDisabled; }

    private static int DoGetCount()
    {
        try
        {
            if (MathcadPrimeComServerManager.Instance.PrimeWindow.OpenDocumentsList != null)
            {
                return MathcadPrimeComServerManager.Instance.PrimeWindow.OpenDocumentsList.Count;
            }
        }
        catch
        {
        }

        return 0;
    }
}
