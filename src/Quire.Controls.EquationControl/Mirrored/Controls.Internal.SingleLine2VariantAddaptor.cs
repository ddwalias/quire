using Quire.Core;

namespace Quire.Controls.Internal;

internal partial class SingleLine2VariantAddaptor
{
    private readonly VisualMathNode _mathNode;
    private readonly bool _firstNode;

    public bool UseZeroCache { get; set; }

    public double InBottomLineVOffset
    {
        get => 0.0;
        set
        {
            if (value != 0.0)
            {
                DebugTools.DevAssert(false, null);
            }
        }
    }

    public double InTopLineVOffset
    {
        get => 0.0;
        set
        {
            if (value != 0.0)
            {
                DebugTools.DevAssert(false, null);
            }
        }
    }

    public double InHOffset
    {
        get => 0.0;
        set
        {
            if (value != 0.0)
            {
                DebugTools.DevAssert(false, null);
            }
        }
    }

    public double BottomLineBaseLine
    {
        get
        {
            if (_firstNode)
            {
                return _mathNode.TextBaselineY;
            }

            DebugTools.DevAssert(false, null);
            return 0.0;
        }
    }

    public double TopLineBaseLine
    {
        get
        {
            if (!_firstNode)
            {
                return _mathNode.TextBaselineY;
            }

            DebugTools.DevAssert(false, null);
            return 0.0;
        }
    }

    public double BottomLineHeight
    {
        get
        {
            if (_firstNode)
            {
                return _mathNode.DesiredSize.Height;
            }

            DebugTools.DevAssert(false, null);
            return 0.0;
        }
    }

    public double TopLineHeight
    {
        get
        {
            if (!_firstNode)
            {
                return _mathNode.DesiredSize.Height;
            }

            DebugTools.DevAssert(false, null);
            return 0.0;
        }
    }

    public double BottomLineWidth
    {
        get
        {
            if (_firstNode)
            {
                return _mathNode.DesiredSize.Width;
            }

            DebugTools.DevAssert(false, null);
            return 0.0;
        }
    }

    public double TopLineWidth
    {
        get
        {
            if (!_firstNode)
            {
                return _mathNode.DesiredSize.Width;
            }

            DebugTools.DevAssert(false, null);
            return 0.0;
        }
    }

    public double TotalHeight => _mathNode.DesiredSize.Height;

    public double TotalLinesHeightAboveBottom => 0.0;

    public bool IsMultiline => false;

    public int MeasureCallFromRoot => 0;

    public SingleLine2VariantAddaptor(VisualMathNode mathNode, bool firstNode)
    {
        _mathNode = mathNode;
        _firstNode = firstNode;
    }

    public void SetMeasureCallFromRoot(int callNumber)
    {
        DebugTools.DevAssert(false, null);
    }
}
