#nullable disable
using System.Collections.Generic;
using System.Windows;

namespace Quire.Wpf;

public static partial class WpfUtils
{
    // Keep these twelve declarations ahead of the captured lambda to retain Mathcad's DisplayClass12 ordinal.
    public const int NumOfMeaningfulDigitsInDip = 5;
    public const double LayoutEpsilon = 1E-05d;
    public const double DipsPerInch = 96d;
    public const double DipsPerCm = 37.79527559055118d;
    public const double PointsPerInch = 72d;
    public const double HimetricPerCm = 1000d;
    public const double OneScrollLineSize = 16d;
    public static readonly int MouseWheelScrollDelta;
    public static readonly int MouseScrollDeltaForTouchPad;
    public static readonly Size SizeInfinity;
    private static global::System.Nullable<bool> _eastAsianLanguagesInstalled;
    public const string ApplicationAcademicNameResourceKey = "Application.AcademicNameAndVersion";

    // Compile order places the captured lambda after the first twelve WpfUtils declarations, as in Mathcad.
    public static IList<string> DumpLogicalTreeLines(object root)
    {
        if (root == null)
        {
            return new List<string>();
        }
        List<string> tree = new List<string> { root.GetType().Name };
        if (root is not DependencyObject)
        {
            return tree;
        }
        // The repeated `as` preserves the separate Release isinst used for LogicalTreeHelper.GetChildren.
        foreach (object child in LogicalTreeHelper.GetChildren(root as DependencyObject))
        {
            DumpLogicalTree(child).Each((char line) =>
            {
                tree.Add("  " + line);
            });
        }
        return tree;
    }
}
