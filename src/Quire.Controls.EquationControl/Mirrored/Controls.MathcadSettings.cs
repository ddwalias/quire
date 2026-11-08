#nullable disable
using Quire.ComponentModel;
using Quire.Mirror;

namespace Quire.Controls;

public partial class MathcadSettings
{
    [MirrorVerified("Release IL")]
    private static MathcadSettings _instance;

    [MirrorVerified("Release IL")]
    private static readonly PropertyBag _properties;

    public static readonly global::Quire.ComponentModel.PropertySpec CaretColorSpec;

    public static readonly global::Quire.ComponentModel.PropertySpec ReadOnlyCaretColorSpec;

    public static readonly global::Quire.ComponentModel.PropertySpec CaretBlinkSpec;

    public static readonly global::Quire.ComponentModel.PropertySpec CaretBlinkPeriodSpec;

    public static readonly global::Quire.ComponentModel.PropertySpec CaretHighlightOpacitySpec;

    public static readonly global::Quire.ComponentModel.PropertySpec TypeoverColorSpec;

    public static readonly global::Quire.ComponentModel.PropertySpec ScopeOpacitySpec;

    public static readonly global::Quire.ComponentModel.PropertySpec ScopeColorSpec;

    public static readonly global::Quire.ComponentModel.PropertySpec HighlightOperandsSpec;

    public static readonly global::Quire.ComponentModel.PropertySpec ParenthesizeOperandsSpec;

    public static readonly global::Quire.ComponentModel.PropertySpec AutoLabelIdsSpec;

    public static readonly global::Quire.ComponentModel.PropertySpec FractionBarVerticalMarginSpec;

    public static readonly global::Quire.ComponentModel.PropertySpec NearestNeighborColorSpec;

    public static readonly global::Quire.ComponentModel.PropertySpec NearestNeighborOpacitySpec;

    public static readonly global::Quire.ComponentModel.PropertySpec EvalPlaceholderSpec;

    public static readonly global::Quire.ComponentModel.PropertySpec LineWrappingSignSpec;

    public static readonly global::Quire.ComponentModel.PropertySpec LineWrappingSignOffsetSpec;

    public static readonly global::Quire.ComponentModel.PropertySpec ShowResultMatrixFormatIndicesSpec;

    public static readonly global::Quire.ComponentModel.PropertySpec HandleFocusForImeSpec;

    public static readonly global::Quire.ComponentModel.PropertySpec ComboBoxColorSpec;

    [MirrorVerified("Release IL")]
    public PropertyBag Properties => _properties;

    [MirrorVerified("Release IL")]
    private static void OnPropertiesSetValue(object sender, PropertySpecEventArgs e)
    {
        if (_instance == null)
        {
            return;
        }
        if (e.Property == CaretColorSpec)
        {
            _instance.CaretColor = (global::System.Drawing.Color)e.Value;
            return;
        }
        if (e.Property == ReadOnlyCaretColorSpec)
        {
            _instance.ReadOnlyCaretColor = (global::System.Drawing.Color)e.Value;
            return;
        }
        if (e.Property == CaretBlinkSpec)
        {
            _instance.CaretBlink = (bool)e.Value;
            return;
        }
        if (e.Property == CaretBlinkPeriodSpec)
        {
            _instance.CaretBlinkPeriod = (int)e.Value;
            return;
        }
        if (e.Property == CaretHighlightOpacitySpec)
        {
            _instance.CaretHighlightOpacity = (double)e.Value;
            return;
        }
        if (e.Property == TypeoverColorSpec)
        {
            _instance.TypeoverColor = (global::System.Drawing.Color)e.Value;
            return;
        }
        if (e.Property == ScopeOpacitySpec)
        {
            _instance.ScopeOpacity = (double)e.Value;
            return;
        }
        if (e.Property == ScopeColorSpec)
        {
            _instance.ScopeColor = (global::System.Drawing.Color)e.Value;
            return;
        }
        if (e.Property == HighlightOperandsSpec)
        {
            _instance.HighlightOperands = (bool)e.Value;
            return;
        }
        if (e.Property == ParenthesizeOperandsSpec)
        {
            _instance.ParenthesizeOperands = (bool)e.Value;
            return;
        }
        if (e.Property == AutoLabelIdsSpec)
        {
            _instance.AutoLabelIds = (bool)e.Value;
            return;
        }
        if (e.Property == FractionBarVerticalMarginSpec)
        {
            _instance.FractionBarVerticalMargin = (double)e.Value;
            return;
        }
        if (e.Property == NearestNeighborColorSpec)
        {
            _instance.NearestNeighborColor = (global::System.Drawing.Color)e.Value;
            return;
        }
        if (e.Property == NearestNeighborOpacitySpec)
        {
            _instance.NearestNeighborOpacity = (double)e.Value;
            return;
        }
        if (e.Property == EvalPlaceholderSpec)
        {
            _instance.EvalPlaceholder = (string)e.Value;
            return;
        }
        if (e.Property == LineWrappingSignSpec)
        {
            _instance.LineWrappingSign = (string)e.Value;
            return;
        }
        if (e.Property == LineWrappingSignOffsetSpec)
        {
            _instance.LineWrappingSignOffset = (double)e.Value;
            return;
        }
        if (e.Property == ShowResultMatrixFormatIndicesSpec)
        {
            _instance.ShowResultMatrixFormatIndices = (bool)e.Value;
            return;
        }
        if (e.Property == HandleFocusForImeSpec)
        {
            _instance.HandleFocusForIme = (bool)e.Value;
            return;
        }
        if (e.Property == ComboBoxColorSpec)
        {
            _instance.ComboBoxColor = (global::System.Drawing.Color)e.Value;
        }
    }

    [MirrorVerified("Release IL")]
    private static void OnPropertiesGetValue(object sender, PropertySpecEventArgs e)
    {
        if (_instance == null)
        {
            return;
        }
        if (e.Property == CaretColorSpec)
        {
            e.Value = _instance.CaretColor;
        }
        if (e.Property == ReadOnlyCaretColorSpec)
        {
            e.Value = _instance.ReadOnlyCaretColor;
        }
        if (e.Property == CaretBlinkSpec)
        {
            e.Value = _instance.CaretBlink;
        }
        if (e.Property == CaretBlinkPeriodSpec)
        {
            e.Value = _instance.CaretBlinkPeriod;
            return;
        }
        if (e.Property == CaretHighlightOpacitySpec)
        {
            e.Value = _instance.CaretHighlightOpacity;
            return;
        }
        if (e.Property == TypeoverColorSpec)
        {
            e.Value = _instance.TypeoverColor;
            return;
        }
        if (e.Property == ScopeOpacitySpec)
        {
            e.Value = _instance.ScopeOpacity;
            return;
        }
        if (e.Property == ScopeColorSpec)
        {
            e.Value = _instance.ScopeColor;
            return;
        }
        if (e.Property == ParenthesizeOperandsSpec)
        {
            e.Value = _instance.ParenthesizeOperands;
            return;
        }
        if (e.Property == AutoLabelIdsSpec)
        {
            e.Value = _instance.AutoLabelIds;
            return;
        }
        if (e.Property == HighlightOperandsSpec)
        {
            e.Value = _instance.HighlightOperands;
            return;
        }
        if (e.Property == FractionBarVerticalMarginSpec)
        {
            e.Value = _instance.FractionBarVerticalMargin;
            return;
        }
        if (e.Property == NearestNeighborColorSpec)
        {
            e.Value = _instance.NearestNeighborColor;
            return;
        }
        if (e.Property == NearestNeighborOpacitySpec)
        {
            e.Value = _instance.NearestNeighborOpacity;
            return;
        }
        if (e.Property == EvalPlaceholderSpec)
        {
            e.Value = _instance.EvalPlaceholder;
            return;
        }
        if (e.Property == LineWrappingSignSpec)
        {
            e.Value = _instance.LineWrappingSign;
            return;
        }
        if (e.Property == LineWrappingSignOffsetSpec)
        {
            e.Value = _instance.LineWrappingSignOffset;
            return;
        }
        if (e.Property == ShowResultMatrixFormatIndicesSpec)
        {
            e.Value = _instance.ShowResultMatrixFormatIndices;
            return;
        }
        if (e.Property == HandleFocusForImeSpec)
        {
            e.Value = _instance.HandleFocusForIme;
            return;
        }
        if (e.Property == ComboBoxColorSpec)
        {
            e.Value = _instance.ComboBoxColor;
        }
    }
}
