namespace Quire.depman;

public partial interface BooleanMediator
{
    bool Value
    {
        [return: global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.U1)]
        get;
    }

    void AddCallback(global::Quire.Engine.BooleanCallback b);

    void RemoveCallback(global::Quire.Engine.BooleanCallback b);

    void WaitValueTrue(int timeout);
}
