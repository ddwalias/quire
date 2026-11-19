using System;
using System.Runtime.InteropServices;

namespace Quire;

internal partial class ComNativeExternalApi
{
    [DllImport("ole32.dll")]
    public static extern int CoRegisterClassObject([MarshalAs(UnmanagedType.LPStruct)] Guid rclsid, [MarshalAs(UnmanagedType.Interface)] object pUnkn, ComInteropEnums.CLSCTX dwClsContext, ComInteropEnums.REGCLS flags, [Out] out uint lpdwRegister);
    [DllImport("ole32.dll")]
    public static extern int CoRevokeClassObject(uint dwRegister);
    [DllImport("ole32.dll")]
    public static extern int CoResumeClassObjects();
    public ComNativeExternalApi() { }
}
