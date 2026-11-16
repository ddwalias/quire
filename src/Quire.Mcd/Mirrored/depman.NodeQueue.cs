namespace Quire.depman;

public partial interface NodeQueue
{
    void StartWorkers(int n);

    void StopWorkers();

    global::Quire.Mcd.RuntimeOptions GetRuntimeOptions();

    void SetNumThreads(int n);

    int GetNumThreads();

    void Interrupt(int threadId);

    void SetAutoCalc([global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.U1)] bool b);

    [return: global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.U1)]
    bool GetAutoCalc();

    void SetCalcNow([global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.U1)] bool b);

    [return: global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.U1)]
    bool GetCalcNow();

    void BeginTransaction();

    void EndTransaction();

    global::Quire.depman.BooleanMediator GetUpToDateMediator();

    global::Quire.depman.BooleanMediator GetBusyMediator();
}
