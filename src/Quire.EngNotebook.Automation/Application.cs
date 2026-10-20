#nullable disable
using System.Runtime.InteropServices;

namespace Quire.EngNotebook.Automation;

public partial class Application
{
    private readonly ProxyComClassManager<IMathcadPrimeApplication> _comProxyManagerInstance = ProxyComClassManager<IMathcadPrimeApplication>.Instance;
    private readonly IProxyComClassManagerAgent<IMathcadPrimeApplication> _proxyComClassManagerAgent;
    private IMathcadPrimeApplication3 _realSubjectInstance;
    private IMathcadPrimeWorksheets _worksheets;

    private IMathcadPrimeApplication3 RealSubjectInstance
    {
        get
        {
            if (_realSubjectInstance == null)
            {
                Marshal.ThrowExceptionForHR(-2147023174);
            }
            return _realSubjectInstance;
        }
    }

    public bool Visible
    {
        get => RealSubjectInstance.Visible;
        set => RealSubjectInstance.Visible = value;
    }

    public IMathcadPrimeWorksheet ActiveWorksheet
    {
        get
        {
            IMathcadPrimeWorksheet activeWorksheet = RealSubjectInstance.ActiveWorksheet;
            try
            {
                return new Worksheet(activeWorksheet);
            }
            catch
            {
            }
            return null;
        }
    }

    public IMathcadPrimeWorksheets Worksheets
    {
        get
        {
            IMathcadPrimeWorksheets worksheets = RealSubjectInstance.Worksheets;
            try
            {
                return _worksheets ?? (_worksheets = new Worksheets(worksheets));
            }
            catch
            {
            }
            return null;
        }
    }

    public Application()
    {
        Marshal.ThrowExceptionForHR(-2147467263);
    }

    internal Application(IMathcadPrimeApplication3 mathcadPrimeApplication)
    {
        _realSubjectInstance = mathcadPrimeApplication;
        if (_realSubjectInstance == null)
        {
            Marshal.ThrowExceptionForHR(-2147467259);
        }
        _proxyComClassManagerAgent = new ProxyComClassManagerAgent<IMathcadPrimeApplication>(_comProxyManagerInstance);
        Initialize();
    }

    ~Application()
    {
        try
        {
            ReleaseAll();
        }
        catch
        {
        }
    }

    private void Initialize()
    {
        if (_proxyComClassManagerAgent.IsRegistered)
        {
            _comProxyManagerInstance.RefineConnections += OnRefineConnections;
        }
    }

    private void Release()
    {
        if (_proxyComClassManagerAgent.IsRegistered)
        {
            _comProxyManagerInstance.RefineConnections -= OnRefineConnections;
        }
        try
        {
            if (RealSubjectInstance is IReleasableRealSubject)
            {
                ((IReleasableRealSubject)RealSubjectInstance).ReleaseComEvents();
            }
        }
        catch
        {
        }
        _proxyComClassManagerAgent.Release();
    }

    private void ReleaseRealSubject()
    {
        try
        {
            if (RealSubjectInstance != null)
            {
                if (Marshal.IsComObject(RealSubjectInstance))
                {
                    Marshal.FinalReleaseComObject(RealSubjectInstance);
                }
                _realSubjectInstance = null;
            }
        }
        catch
        {
        }
    }

    private void OnRefineConnections()
    {
        if (!_comProxyManagerInstance.IsConnectionAlive(this))
        {
            ReleaseAll();
        }
    }

    private void ReleaseAll()
    {
        if (_proxyComClassManagerAgent.IsReleased)
        {
            return;
        }
        Release();
        ReleaseRealSubject();
    }

    public void Activate()
    {
        RealSubjectInstance.Activate();
    }

    public void Quit(SaveOption saveOptionArg)
    {
        Release();
        if (!_comProxyManagerInstance.HasConnections)
        {
            RealSubjectInstance.Quit(saveOptionArg);
        }
        ReleaseRealSubject();
    }

    public IMathcadPrimeWorksheet Open(string documentPathArg)
    {
        IMathcadPrimeWorksheet mathcadPrimeWorksheet = RealSubjectInstance.Open(documentPathArg);
        try
        {
            return new Worksheet(mathcadPrimeWorksheet);
        }
        catch
        {
        }
        return null;
    }

    public int InitializeEvents(IMathcadPrimeEvents eventsArg)
    {
        return RealSubjectInstance.InitializeEvents(eventsArg);
    }

    public void CloseAll(SaveOption saveOptionArg)
    {
        RealSubjectInstance.CloseAll(saveOptionArg);
    }

    public string GetVersion()
    {
        return RealSubjectInstance.GetVersion();
    }

    public int InitializeEvents2(IMathcadPrimeEvents2 eventsArg, bool subscribeAllArg)
    {
        return RealSubjectInstance.InitializeEvents2(eventsArg, subscribeAllArg);
    }

    public int SubscribeEvent(MathcadPrimeEvents primeEventArg)
    {
        return RealSubjectInstance.SubscribeEvent(primeEventArg);
    }

    public int UnsubscribeEvent(MathcadPrimeEvents primeEventArg)
    {
        return RealSubjectInstance.UnsubscribeEvent(primeEventArg);
    }

    public IMathcadPrimeWorksheetReadonlyOptions CreateWorksheetReadonlyOptions()
    {
        IMathcadPrimeWorksheetReadonlyOptions mathcadPrimeWorksheetReadonlyOptions = RealSubjectInstance.CreateWorksheetReadonlyOptions();
        try
        {
            return new WorksheetReadonlyOptions(mathcadPrimeWorksheetReadonlyOptions);
        }
        catch
        {
        }
        return null;
    }

    public IMathcadPrimeWorksheet3 OpenEx(string documentPathArg, IMathcadPrimeWorksheetReadonlyOptions optionsArg)
    {
        IMathcadPrimeWorksheet3 mathcadPrimeWorksheet = RealSubjectInstance.OpenEx(documentPathArg, ((IProxyComRealSubject<IMathcadPrimeWorksheetReadonlyOptions>)optionsArg).RealSubjectInstance);
        try
        {
            return new Worksheet(mathcadPrimeWorksheet);
        }
        catch
        {
        }
        return null;
    }

    public void UpdateInputs()
    {
        RealSubjectInstance.UpdateInputs();
    }

    public void SetContextData(string[] contextData)
    {
        if (contextData == null || contextData.Length == 0)
        {
            return;
        }
        RealSubjectInstance.SetContextData(contextData);
    }
}
