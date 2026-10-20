#nullable disable
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace Quire.EngNotebookPlus.Automation;

internal partial class ProxyComClassManager<T> where T : class
{
    private static readonly object _syncRoot = new object();
    private static ProxyComClassManager<T> _instance = null;
    private static int _lockCount = 0;
    private bool _refineConnectionsInProcess;

    internal static ProxyComClassManager<T> Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_syncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new ProxyComClassManager<T>();
                    }
                }
            }
            return _instance;
        }
    }

    public bool HasConnections => _lockCount > 0;

    internal event RefineConnectionsHandler RefineConnections;

    private ProxyComClassManager()
    {
    }

    internal bool Register(IProxyComClassManagerAgent<T> proxyComClassManagerAgent)
    {
        if (HasConnections)
        {
            InvokeRefineConnections();
        }
        if (proxyComClassManagerAgent == null || proxyComClassManagerAgent.IsRegistered)
        {
            return false;
        }
        Lock();
        return true;
    }

    internal bool Unregister(IProxyComClassManagerAgent<T> proxyComClassManagerAgent)
    {
        if (proxyComClassManagerAgent != null && proxyComClassManagerAgent.IsRegistered)
        {
            Unlock();
        }
        if (HasConnections)
        {
            InvokeRefineConnections();
        }
        return true;
    }

    private static int Lock()
    {
        return Interlocked.Increment(ref _lockCount);
    }

    private static int Unlock()
    {
        return Interlocked.Decrement(ref _lockCount);
    }

    private void InvokeRefineConnections()
    {
        if (_refineConnectionsInProcess)
        {
            return;
        }
        _refineConnectionsInProcess = true;
        RefineConnectionsHandler handler = RefineConnections;
        if (handler != null)
        {
            handler();
        }
        _refineConnectionsInProcess = false;
    }

    internal bool IsConnectionAlive(T proxyComClassInstance)
    {
        if (proxyComClassInstance == null)
        {
            return false;
        }
        bool flag = true;
        try
        {
            flag = Marshal.Release(Marshal.GetComInterfaceForObject(proxyComClassInstance, typeof(T))) != 0;
        }
        catch
        {
            flag = true;
        }
        return flag;
    }

    internal object CreateInstanceByProgId(string progId)
    {
        try
        {
            return Activator.CreateInstance(Type.GetTypeFromProgID(progId));
        }
        catch
        {
        }
        return null;
    }
}
