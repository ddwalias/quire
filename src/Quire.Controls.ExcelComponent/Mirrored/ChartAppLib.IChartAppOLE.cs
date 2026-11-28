#nullable disable
#pragma warning disable

namespace Quire.ChartAppLib;

[global::System.Runtime.InteropServices.ComImport]
[global::System.Runtime.InteropServices.Guid("74A3AE50-4504-4A14-9BC6-443B83A83C30")]
public partial interface IChartAppOLE
{
    [global::Quire.Mirror.MirrorStub, global::Quire.Mirror.MirrorBlocked("vtable gap: Roslyn emits it when it embeds an interop interface")]
    extern void _VtblGap1_1();

    void setTraceData(ref TraceData data);

    void deleteTraceByTraceId(int traceId);

    void setChartMinMax(ref ChartMinMax data);

    [global::Quire.Mirror.MirrorStub, global::Quire.Mirror.MirrorBlocked("vtable gap: Roslyn emits it when it embeds an interop interface")]
    extern void _VtblGap2_1();

    void setRect(double _width, double _height);

    void setFreezeImage();

    void setUnFreezeImage();

    int isImageReady();

    [global::Quire.Mirror.MirrorStub, global::Quire.Mirror.MirrorBlocked("vtable gap: Roslyn emits it when it embeds an interop interface")]
    extern void _VtblGap3_1();

    void setHelpPath(string data);
}
