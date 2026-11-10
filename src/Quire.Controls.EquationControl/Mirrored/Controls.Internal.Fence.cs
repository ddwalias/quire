using System;
using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Internal;

internal abstract partial class Fence
{
    public Size ContentSize
    {
        get => (Size)GetValue(ContentSizeProperty);
        set => SetValue(ContentSizeProperty, value);
    }

    public override double EffectiveBaselineY => throw new NotImplementedException();

    public override double EffectiveScriptBaselineY => throw new NotImplementedException();

    public virtual Geometry Geometry => throw new NotImplementedException();

    public bool IsComposable => _components[1] != null;

    public FormattedText First => _components[0];

    public FormattedText Last => _components[2];

    public FormattedText Middle => _components[1];

    public FormattedText Single => _components[3];

    internal override HitTestResult GetNodeAtPoint(Point hitPoint) => throw new NotImplementedException();

    public virtual object Clone() => throw new NotImplementedException();
}
