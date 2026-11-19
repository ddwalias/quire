using System.Windows.Input;
using Quire.Controls.Core;

namespace Quire.Controls.Internal;

public partial class DebugStyle : ViewModelBase
{
    public static partial class PropertyNames
    {
        public static string HighlightColorOffset = "HighlightColorOffset";
        public static string HighlightOperator = "HighlightOperator";
        public static string HighlightLeftOperand = "HighlightLeftOperand";
        public static string HighlightRightOperand = "HighlightRightOperand";
        public static string HighlightStyle = "HighlightStyle";
        public const string PermanentHighlight = "PermanentHighlight";
    }

    public static readonly RoutedCommand SetHighlightMethodCommand = new RoutedCommand("SetHighlightMethod", typeof(DebugStyle));
    public static readonly RoutedCommand SetHighlightColorOffsetCommand = new RoutedCommand("SetHighlightColorOffset", typeof(DebugStyle));

    private bool _permanentHighlight;
    private int _highlightColorOffset;
    private bool _highlightOperator;
    private bool _highlightLeftOperand;
    private bool _highlightRightOperand;
    private ColoringMethod _highlightStyle;

    public DebugStyle()
    {
        CreateCommandBinding(SetHighlightMethodCommand, OnChangeHighlightMethod);
        CreateCommandBinding(SetHighlightColorOffsetCommand, OnChangeHighlightColorOffsetMethod);
    }

    public void OnChangeHighlightColorOffsetMethod(object param)
    {
        int num = (int)param;
        HighlightColorOffset += num;
    }

    public void OnChangeHighlightMethod(object param)
    {
        HighlightStyle = (ColoringMethod)param;
    }

    public bool PermanentHighlight
    {
        get
        {
            return _permanentHighlight;
        }
        set
        {
            if (_permanentHighlight == value)
            {
                return;
            }
            _permanentHighlight = value;
            NotifyPropertyChanged("PermanentHighlight");
        }
    }

    public int HighlightColorOffset
    {
        get
        {
            return _highlightColorOffset;
        }
        set
        {
            if (_highlightColorOffset == value)
            {
                return;
            }
            _highlightColorOffset = value;
            NotifyPropertyChanged(PropertyNames.HighlightColorOffset);
        }
    }

    public bool HighlightOperator
    {
        get
        {
            return _highlightOperator;
        }
        set
        {
            if (_highlightOperator == value)
            {
                return;
            }
            _highlightOperator = value;
            NotifyPropertyChanged(PropertyNames.HighlightOperator);
        }
    }

    public bool HighlightLeftOperand
    {
        get
        {
            return _highlightLeftOperand;
        }
        set
        {
            if (_highlightLeftOperand == value)
            {
                return;
            }
            _highlightLeftOperand = value;
            NotifyPropertyChanged(PropertyNames.HighlightLeftOperand);
        }
    }

    public bool HighlightRightOperand
    {
        get
        {
            return _highlightRightOperand;
        }
        set
        {
            if (_highlightRightOperand == value)
            {
                return;
            }
            _highlightRightOperand = value;
            NotifyPropertyChanged(PropertyNames.HighlightRightOperand);
        }
    }

    public ColoringMethod HighlightStyle
    {
        get
        {
            return _highlightStyle;
        }
        set
        {
            if (_highlightStyle == value)
            {
                return;
            }
            _highlightStyle = value;
            NotifyPropertyChanged(PropertyNames.HighlightStyle);
        }
    }

    private void NotifyPropertyChanged(string info)
    {
        OnPropertyChanged(info);
    }
}
