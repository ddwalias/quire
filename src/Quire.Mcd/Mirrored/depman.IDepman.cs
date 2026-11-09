namespace Quire.depman;

public partial interface IDepman
{
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
}
