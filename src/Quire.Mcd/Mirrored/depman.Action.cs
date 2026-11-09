namespace Quire.depman;

public partial interface Action
{
    ActionResult Invoke(Environment env, object state);

    global::System.Collections.ICollection GetDefs();

    [return: global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.U1)]
    bool IsUndefine();

    [return: global::System.Runtime.InteropServices.MarshalAs(global::System.Runtime.InteropServices.UnmanagedType.U1)]
    bool IsChildOnly();
}
