#nullable disable
#pragma warning disable

namespace Quire.ChartAppLib;

public partial interface IChartAppOLE
{
    void setTraceData(ref TraceData data);

    void deleteTraceByTraceId(int traceId);

    void setChartMinMax(ref ChartMinMax data);

    void setRect(double _width, double _height);

    void setFreezeImage();

    void setUnFreezeImage();

    int isImageReady();

    void setHelpPath(string data);
}
