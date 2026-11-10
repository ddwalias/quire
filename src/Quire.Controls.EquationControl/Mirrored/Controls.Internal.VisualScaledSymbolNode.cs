using System;
using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Internal;

internal partial class VisualScaledSymbolNode
{
    private ScaleTransform _scaleFactor = new ScaleTransform();

    public override double EffectiveBaselineY
    {
        get
        {
            Size desiredSize = DesiredSize;
            return desiredSize.Height;
        }
    }

    public override Geometry Geometry
    {
        get
        {
            Geometry geometry = base.Geometry;
            geometry.Transform = _scaleFactor;
            return geometry;
        }
    }

    public VisualScaledSymbolNode(char symbol)
        : base(symbol)
    {
    }

    public VisualScaledSymbolNode(string symbol)
        : base(symbol)
    {
    }

    public void ScaleToHeight(double height)
    {
        Size size = new Size(_inkBounds.Width, _inkBounds.Height);
        if (size.Height > height)
        {
            _scaleFactor.ScaleX = 1.0;
            _scaleFactor.ScaleY = 1.0;
            return;
        }

        _scaleFactor.ScaleY = height / size.Height;
        if (_scaleFactor.ScaleY > 4.0)
        {
            _scaleFactor.ScaleX = _scaleFactor.ScaleY / 3.0;
        }
    }

    protected override void OnRender(DrawingContext dc)
    {
        dc.DrawGeometry(Font.Foreground, null, Geometry);
    }

    protected override Size MeasureStaticLayout(Size availableSize)
    {
        UpdateText(Font);
        ScaleToHeight(availableSize.Height);
        Rect bounds = Geometry.Bounds;
        double width = bounds.Width;
        bounds = Geometry.Bounds;
        return new Size(width, bounds.Height);
    }

    public VisualScaledSymbolNode Clone()
    {
        VisualScaledSymbolNode clone = new VisualScaledSymbolNode(_symbol);
        clone.MathFormat.SetPropertiesFrom(MathFormat, true);
        clone._scaleFactor = _scaleFactor;
        return clone;
    }

    object global::System.ICloneable.Clone()
    {
        return Clone();
    }
}
