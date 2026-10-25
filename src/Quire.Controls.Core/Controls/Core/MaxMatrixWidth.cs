#nullable disable
namespace Quire.Controls.Core;

public partial struct MaxMatrixWidth
{
    public double MaxIntegral;

    public double MaxFractional;

    public double MaxVisualNode;

    public MaxMatrixWidth(double maxIntegral, double maxFractional, double maxVisualNode)
    {
        MaxIntegral = maxIntegral;
        MaxFractional = maxFractional;
        MaxVisualNode = maxVisualNode;
    }
}
