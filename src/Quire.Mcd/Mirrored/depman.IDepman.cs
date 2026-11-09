namespace Quire.depman;

public partial interface IDepman
{
    void StartWorkers(int n);

    void StopWorkers();

    global::Quire.Mcd.RuntimeOptions GetRuntimeOptions();

    global::System.Collections.ICollection GetHandles();

    void AddNodeCallback(global::Quire.utl_mgd.Function f);

    void AddNodeCallback(DepNodeCallback c);

    void AddBusyCallback(global::Quire.utl_mgd.Function f);

    void AddBusyCallback(global::Quire.Engine.BooleanCallback b);

    void RemoveBusyCallback(global::Quire.Engine.BooleanCallback b);

    void AddUpToDateCallback(global::Quire.utl_mgd.Function f);

    void AddUpToDateCallback(global::Quire.Engine.BooleanCallback b);

    void RemoveUpToDateCallback(global::Quire.Engine.BooleanCallback b);

    bool IsUpToDate
    {
        [return: global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.U1)]
        get;
    }

    bool IsBusy
    {
        [return: global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.U1)]
        get;
    }

    void BeginMove();

    void EndMove();

    void UnLockAll();

    void SetAutoCalc([global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.U1)] bool b);

    [return: global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.U1)]
    bool GetAutoCalc();

    void SetCalcNow([global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.U1)] bool b);

    [return: global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.U1)]
    bool GetCalcNow();

    void SetCalcNowTrue();

    void SetNumThreads(int n);

    int GetNumThreads();

    void WaitUpToDate(int timeout);

    void StopMessages();

    void DisableDeps();

    void EnableDeps();

    void DumpNodes(string filename);
}
