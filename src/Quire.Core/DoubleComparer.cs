#nullable disable
using System.Windows;
using System.Windows.Media.Media3D;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire;

public partial class DoubleComparer
{
    [MirrorVerified("PtcDoubleComparerMechanicalDifferentialTests")]
    private DoubleComparer() : this(1E-10) { }

    [MirrorVerified("PtcDoubleComparerMechanicalDifferentialTests")]
    public double Epsilon { get; private set; }

    [MirrorVerified("Release IL")]
    public DoubleComparer(double epsilon)
    {
        Epsilon = epsilon;
    }

    [MirrorVerified("Release IL")]
    public int Compare(Point pt1, Point pt2) => Compare(GetDoubles(pt1), GetDoubles(pt2));

    [MirrorVerified("Release IL")]
    public int Compare(Point3D pt1, Point3D pt2) => Compare(GetDoubles(pt1), GetDoubles(pt2));

    [MirrorVerified("Release IL")]
    public int Compare(Rect rect1, Rect rect2) => Compare(GetDoubles(rect1), GetDoubles(rect2));

    [MirrorVerified("Release IL")]
    public int Compare(double x, double y)
    {
        if (Math.Abs(x - y) < Epsilon)
        {
            return 0;
        }
        if (!(x > y))
        {
            return -1;
        }
        return 1;
    }

    [MirrorVerified("Release IL")]
    public int Compare(IEnumerable<double> seq1, IEnumerable<double> seq2)
    {
        using (IEnumerator<double> enumerator = seq1.GetEnumerator())
        {
            using IEnumerator<double> enumerator2 = seq2.GetEnumerator();
            while (enumerator.MoveNext() && enumerator2.MoveNext())
            {
                int num = Compare(enumerator.Current, enumerator2.Current);
                if (num != 0)
                {
                    return num;
                }
            }
        }
        return 0;
    }

    private static IEnumerable<double> GetDoubles(Rect rect) => GetDoubles(rect.TopLeft).Concat(GetDoubles(rect.BottomRight));
}
