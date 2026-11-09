using System.Globalization;
using System.Linq;

namespace Quire.Controls.Plots.Plot3D.Model;

internal static partial class VectorFormatHelper
{
    internal static string VectorRl(params double[] vector)
    {
        string[] values = vector
            .Select(element => string.Format(CultureInfo.InvariantCulture, "(number {0})", element))
            .ToArray();

        return string.Format("(op_matrix (unboxed {0}) (unboxed 1) (list {1}))", vector.Length, string.Join(" ", values));
    }
}
