#nullable disable
using System.Runtime.InteropServices;

namespace Quire.Automation;

public partial class ApplicationObsolete
{
    private readonly ProxyComClassManager<IMathcadPrimeApplication> _comProxyManagerInstance = ProxyComClassManager<IMathcadPrimeApplication>.Instance;
    private readonly IProxyComClassManagerAgent<IMathcadPrimeApplication> _proxyComClassManagerAgent;
    private IMathcadPrimeApplication2 _realSubjectInstance;
    private IMathcadPrimeWorksheets _worksheets;

    private IMathcadPrimeApplication2 RealSubjectInstance
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

    public ApplicationObsolete()
    {
        Marshal.ThrowExceptionForHR(-2147467263);
    }

    internal ApplicationObsolete(IMathcadPrimeApplication2 mathcadPrimeApplication)
    {
        _realSubjectInstance = mathcadPrimeApplication;
        if (_realSubjectInstance == null)
        {
            Marshal.ThrowExceptionForHR(-2147467259);
        }
        _proxyComClassManagerAgent = new ProxyComClassManagerAgent<IMathcadPrimeApplication>(_comProxyManagerInstance);
        Initialize();
    }

    ~ApplicationObsolete()
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
            CloseAll(saveOptionArg);
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
}
