using System.Windows.Threading;

namespace Quire;

public partial class CMathcadPrimeWorksheets
{
    private readonly bool _isUiInteractiveModeDisabled;

    public CMathcadPrimeWorksheets(bool isUiInteractiveModeDisabled)
    {
        _isUiInteractiveModeDisabled = isUiInteractiveModeDisabled;
    }

    public int Count
    {
        get
        {
            try
            {
                DelegateFunction0<int> getCount = DoGetCount;
                return (int)MathcadPrimeComServerManager.Instance.InvokeDelegate(getCount, DispatcherPriority.Normal);
            }
            catch
            {
            }

            return 0;
        }
    }

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

    public global::Quire.Automation.IMathcadPrimeWorksheet2 Item(int indexArg)
    {
        try
        {
            DelegateFunction0<global::Quire.Automation.IMathcadPrimeWorksheet2> getItem = () => DoGetOpenItemByIndex(indexArg);
            return MathcadPrimeComServerManager.Instance.InvokeDelegate(getItem, DispatcherPriority.Normal) as global::Quire.Automation.IMathcadPrimeWorksheet2;
        }
        catch
        {
        }

        return null;
    }

    private global::Quire.Automation.IMathcadPrimeWorksheet2 DoGetOpenItemByIndex(int index)
    {
        try
        {
            if (index >= 0 && MathcadPrimeComServerManager.Instance.PrimeWindow.OpenDocumentsList != null
                && MathcadPrimeComServerManager.Instance.PrimeWindow.OpenDocumentsList.Count > index)
            {
                global::Quire.Controls.Core.IEngineeringDocument document = MathcadPrimeComServerManager.Instance.PrimeWindow.OpenDocumentsList[index];
                if (document != null)
                {
                    return new CMathcadPrimeWorksheet(document, _isUiInteractiveModeDisabled);
                }
            }
        }
        catch
        {
        }

        return null;
    }
}
