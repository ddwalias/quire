#nullable disable
using Quire.Mirror;

namespace Quire;

public partial struct Interval
{
    [MirrorVerified("Release IL")]
    public double Lower;

    [MirrorVerified("Release IL")]
    public double Upper;

    [MirrorVerified("Release IL")]
    public override string ToString() => string.Format("{0},{1}", Lower, Upper);

    [MirrorVerified("Release IL")]
    public double Normalize(double d) => (d - Lower) / Width;

    [MirrorVerified("Release IL")]
    public double GetValueByNormalizedPosition(double d) => Lower * (1.0 - d) + Upper * d;

    [MirrorVerified("Release IL")]
    public double Width => Upper - Lower;

    [MirrorVerified("Release IL")]
    public double Norm => Math.Abs(Width);

    [MirrorVerified("Release IL")]
    public double Center => (Upper + Lower) / 2.0;
}
