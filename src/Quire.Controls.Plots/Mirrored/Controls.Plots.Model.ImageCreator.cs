using System;
using System.Windows;
using System.Windows.Media;

namespace Quire.Controls.Plots.Model;

internal partial class ImageCreator : IStyleOwner
{
    public static partial class PropertyNames
    {
        public const string UseTraceInvalidStyle = "UseTraceInvalidStyle";
        public const string Alpha = "Alpha";
        public const string Color = "Color";
        public const string StyleType = "StyleType";
    }

    private double[] _hitsArray;
    private double _maxPixelCount;
    private bool _styleChanged;
    private IMapper _densityToRGBAMap = MapperFactory.CreateMapper(StyleType.AlphaConstColor);

    public bool UpdateNeeded => _styleChanged;

    public uint Alpha
    {
        get => _densityToRGBAMap.Alpha;
        set
        {
            if (value == _densityToRGBAMap.Alpha)
            {
                return;
            }
            _densityToRGBAMap.Alpha = value;
            NotifyStyleChanged("Alpha");
        }
    }

    public Color Color
    {
        get => _densityToRGBAMap.Color;
        set
        {
            if (value == _densityToRGBAMap.Color)
            {
                return;
            }
            _densityToRGBAMap.Color = value;
            NotifyStyleChanged("Color");
        }
    }

    public bool UseTraceInvalidStyle
    {
        get => _densityToRGBAMap.UseTraceInvalidStyle;
        set
        {
            if (value == _densityToRGBAMap.UseTraceInvalidStyle)
            {
                return;
            }
            _densityToRGBAMap.UseTraceInvalidStyle = value;
            NotifyStyleChanged("UseTraceInvalidStyle");
        }
    }

    public StyleType StyleType
    {
        get => _densityToRGBAMap.StyleType;
        set
        {
            if (value == _densityToRGBAMap.StyleType)
            {
                return;
            }
            _densityToRGBAMap = MapperFactory.CreateMapper(value, _densityToRGBAMap);
            NotifyStyleChanged("StyleType");
        }
    }

    public event Action<string> StyleChanged;

    internal ImageCreator()
    {
    }

    internal ImageCreator(IStyleOwner sourceCreator)
    {
        Alpha = sourceCreator.Alpha;
        Color = sourceCreator.Color;
        StyleType = sourceCreator.StyleType;
        UseTraceInvalidStyle = sourceCreator.UseTraceInvalidStyle;
    }

    public void ResetUpdateNeeded()
    {
        _styleChanged = false;
    }

    public void Reset()
    {
        _hitsArray = null;
        ResetUpdateNeeded();
    }

    public uint[] CreateRgba(PointCollection dataPoints, Rect dataRange, Int32Rect viewPortRect)
    {
        if (!BuildHitsArray(dataPoints, dataRange, viewPortRect))
        {
            return null;
        }
        return UpdateRgba();
    }

    public uint[] UpdateRgba()
    {
        if (DebugTools.DevAssert(!_hitsArray.IsNullOrEmpty(), null).Failed)
        {
            return null;
        }
        uint[] result = _densityToRGBAMap.MapDensityToRGBA(_hitsArray, _maxPixelCount);
        ResetUpdateNeeded();
        return result;
    }

    private bool BuildHitsArray(PointCollection points, Rect axesBounds, Int32Rect imageBounds)
    {
        if (!imageBounds.IsValidSizeRect() || points.IsNullOrEmpty() || !axesBounds.IsValidSizeRect())
        {
            _hitsArray = null;
            return false;
        }
        int num = imageBounds.Width * imageBounds.Height;
        _hitsArray = new double[num];
        int width = imageBounds.Width;
        int num2 = imageBounds.Width - 1;
        int num3 = imageBounds.Height - 1;
        double num4 = (double)num2 / axesBounds.Width;
        double num5 = (double)num3 / axesBounds.Height;
        double num6 = 0.0;
        foreach (Point point in points)
        {
            if (!axesBounds.Contains(point))
            {
                continue;
            }
            int x = (int)Math.Round((point.X - axesBounds.Left) * num4);
            int num7 = (int)Math.Round((point.Y - axesBounds.Top) * num5);
            num7 = num3 - num7;
            x = SqueezeToBounds(0, num2, x);
            num7 = SqueezeToBounds(0, num3, num7);
            int num8 = num7 * width + x;
            if (0 <= num8 && num8 < _hitsArray.Length)
            {
                _hitsArray[num8]++;
                if (_hitsArray[num8] > num6)
                {
                    num6 = _hitsArray[num8];
                }
            }
        }
        _maxPixelCount = num6;
        return true;
    }

    private static int SqueezeToBounds(int min, int max, int x)
    {
        if (x < min)
        {
            x = min;
        }
        if (x > max)
        {
            x = max;
        }
        return x;
    }

    private void NotifyStyleChanged(string propertyName)
    {
        _styleChanged = true;
        if (StyleChanged != null)
        {
            StyleChanged(propertyName);
        }
    }
}
