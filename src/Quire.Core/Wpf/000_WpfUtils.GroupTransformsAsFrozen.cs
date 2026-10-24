#nullable disable
namespace Quire.Wpf;

public static partial class WpfUtils
{
    // Keep this partial first so the generated predicate retains Mathcad's b__17_0 ordinal.
    public static global::System.Windows.Media.GeneralTransform GroupTransformsAsFrozen(params global::System.Windows.Media.GeneralTransform[] children)
    {
        global::System.Windows.Media.GeneralTransform generalTransform;
        if (global::System.Linq.Enumerable.All(children, (global::System.Windows.Media.GeneralTransform child) => child is global::System.Windows.Media.Transform))
        {
            generalTransform = new global::System.Windows.Media.TransformGroup
            {
                Children = new global::System.Windows.Media.TransformCollection(global::System.Linq.Enumerable.Cast<global::System.Windows.Media.Transform>(children))
            };
        }
        else
        {
            generalTransform = new global::System.Windows.Media.GeneralTransformGroup
            {
                Children = new global::System.Windows.Media.GeneralTransformCollection(children)
            };
        }
        ((global::System.Windows.Freezable)generalTransform).Freeze();
        return generalTransform;
    }
}
