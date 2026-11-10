#nullable disable
using System.Drawing;
using Quire.ComponentModel;
using Quire.Controls.EquationProperties;
using Quire.EquationEditor;
using Quire.Mirror;

namespace Quire.Controls;

public partial class MathcadSettings
{
    [MirrorVerified("Release IL")]
    public static readonly MathcadSettings Instance;

    [MirrorVerified("Release IL")]
    private static MathcadSettings _instance;

    [MirrorVerified("Release IL")]
    private static readonly PropertyBag _properties;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec CaretColorSpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec ReadOnlyCaretColorSpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec CaretBlinkSpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec CaretBlinkPeriodSpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec CaretHighlightOpacitySpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec ScopeOpacitySpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec TypeoverColorSpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec ScopeColorSpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec NearestNeighborColorSpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec NearestNeighborOpacitySpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec DefaultZoomSpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec HighlightOperandsSpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec ParenthesizeOperandsSpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec AutoLabelIdsSpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec ShowAlignmentPointSpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec FractionBarVerticalMarginSpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec FourCornersSpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec SpaceBarScopingSpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec ShowStandardToolBarSpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec EvalPlaceholderSpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec LineWrappingSignSpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec LineWrappingSignOffsetSpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec ShowResultMatrixFormatIndicesSpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec FractionBarOverhangSpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec TextRegionDefaultWidthSpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec TextRegionDefaultFontSpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec HandleFocusForImeSpec;

    [MirrorVerified("Release IL")]
    public static readonly global::Quire.ComponentModel.PropertySpec ComboBoxColorSpec;

    [MirrorVerified("Release IL")]
    public MathcadSettings()
    {
        _instance = this;
    }

    [MirrorVerified("Release IL")]
    static MathcadSettings()
    {
        Instance = new MathcadSettings();
        CaretColorSpec = new PropertySpec("Color", typeof(Color));
        ReadOnlyCaretColorSpec = new PropertySpec("ReadOnly Color", typeof(Color));
        CaretBlinkSpec = new PropertySpec("Blink", typeof(bool));
        CaretBlinkPeriodSpec = new PropertySpec("Blink Period (ms)", typeof(int));
        CaretHighlightOpacitySpec = new PropertySpec("Highlight Opacity", typeof(double));
        ScopeOpacitySpec = new PropertySpec("Scope Highlight Opacity", typeof(double));
        TypeoverColorSpec = new PropertySpec("Typeover Color", typeof(Color));
        ScopeColorSpec = new PropertySpec("Scope Highlight Color", typeof(Color));
        NearestNeighborColorSpec = new PropertySpec("Nearest Neighbor Color", typeof(Color));
        NearestNeighborOpacitySpec = new PropertySpec("Nearest Neighbor Opacity", typeof(double));
        DefaultZoomSpec = new PropertySpec("Default Zoom", typeof(double));
        HighlightOperandsSpec = new PropertySpec("Highlight operands", typeof(bool));
        ParenthesizeOperandsSpec = new PropertySpec("Parenthesize operands", typeof(bool));
        AutoLabelIdsSpec = new PropertySpec("Auto labeling identifiers", typeof(bool));
        ShowAlignmentPointSpec = new PropertySpec("Show alignment points", typeof(bool));
        FractionBarVerticalMarginSpec = new PropertySpec("Fraction bar vertical margin", typeof(double));
        FourCornersSpec = new PropertySpec("Four Corners region border style", typeof(bool));
        SpaceBarScopingSpec = new PropertySpec("Spacebar scoping", typeof(bool));
        ShowStandardToolBarSpec = new PropertySpec("Show Standard Toolbar", typeof(bool));
        EvalPlaceholderSpec = new PropertySpec("Evaluation Placeholder", typeof(string));
        LineWrappingSignSpec = new PropertySpec("Line Wrapping Sign", typeof(string));
        LineWrappingSignOffsetSpec = new PropertySpec("Line Wrapping Sign Offset", typeof(double));
        ShowResultMatrixFormatIndicesSpec = new PropertySpec("Show result matrix indices", typeof(bool));
        FractionBarOverhangSpec = new PropertySpec("Overhang", typeof(double));
        TextRegionDefaultWidthSpec = new PropertySpec("Text Region Default Width", typeof(int));
        TextRegionDefaultFontSpec = new PropertySpec("Text Region Default Font", typeof(Font));
        HandleFocusForImeSpec = new PropertySpec("Handle Focus For IME", typeof(bool));
        ComboBoxColorSpec = new PropertySpec("ComboBox Color", typeof(Color));
        _properties = new PropertyBag();
        string category = "Application Properties";
        ShowStandardToolBarSpec.Category = category;
        _properties.Properties.Add(ShowStandardToolBarSpec);
        string category2 = "Insertion Marker Properties";
        CaretColorSpec.Category = category2;
        _properties.Properties.Add(CaretColorSpec);
        ReadOnlyCaretColorSpec.Category = category2;
        _properties.Properties.Add(ReadOnlyCaretColorSpec);
        CaretBlinkSpec.Category = category2;
        _properties.Properties.Add(CaretBlinkSpec);
        CaretBlinkPeriodSpec.Category = category2;
        _properties.Properties.Add(CaretBlinkPeriodSpec);
        CaretHighlightOpacitySpec.Category = category2;
        _properties.Properties.Add(CaretHighlightOpacitySpec);
        string category3 = "Operator Selection Properties";
        ParenthesizeOperandsSpec.Category = category3;
        _properties.Properties.Add(ParenthesizeOperandsSpec);
        HighlightOperandsSpec.Category = category3;
        _properties.Properties.Add(HighlightOperandsSpec);
        string category4 = "Worksheet Properties";
        DefaultZoomSpec.Category = category4;
        _properties.Properties.Add(DefaultZoomSpec);
        string category5 = "Math Normal Properties";
        TypeoverColorSpec.Category = category5;
        _properties.Properties.Add(TypeoverColorSpec);
        ScopeColorSpec.Category = category5;
        _properties.Properties.Add(ScopeColorSpec);
        ScopeOpacitySpec.Category = category5;
        _properties.Properties.Add(ScopeOpacitySpec);
        NearestNeighborColorSpec.Category = category5;
        _properties.Properties.Add(NearestNeighborColorSpec);
        NearestNeighborOpacitySpec.Category = category5;
        _properties.Properties.Add(NearestNeighborOpacitySpec);
        ShowAlignmentPointSpec.Category = category5;
        _properties.Properties.Add(ShowAlignmentPointSpec);
        FractionBarVerticalMarginSpec.Category = category5;
        _properties.Properties.Add(FractionBarVerticalMarginSpec);
        FourCornersSpec.Category = category5;
        _properties.Properties.Add(FourCornersSpec);
        EvalPlaceholderSpec.Category = category5;
        _properties.Properties.Add(EvalPlaceholderSpec);
        LineWrappingSignSpec.Category = category5;
        _properties.Properties.Add(LineWrappingSignSpec);
        LineWrappingSignOffsetSpec.Category = category5;
        _properties.Properties.Add(LineWrappingSignOffsetSpec);
        ShowResultMatrixFormatIndicesSpec.Category = category5;
        _properties.Properties.Add(ShowResultMatrixFormatIndicesSpec);
        ComboBoxColorSpec.Category = category5;
        _properties.Properties.Add(ComboBoxColorSpec);
        string category6 = "Edting Behavior";
        SpaceBarScopingSpec.Category = category6;
        _properties.Properties.Add(SpaceBarScopingSpec);
        AutoLabelIdsSpec.Category = category6;
        _properties.Properties.Add(AutoLabelIdsSpec);
        HandleFocusForImeSpec.Category = category6;
        _properties.Properties.Add(HandleFocusForImeSpec);
        string category7 = "Text Region";
        TextRegionDefaultWidthSpec.Category = category7;
        _properties.Properties.Add(TextRegionDefaultWidthSpec);
        TextRegionDefaultFontSpec.Category = category7;
        _properties.Properties.Add(TextRegionDefaultFontSpec);
        _properties.GetValue += OnPropertiesGetValue;
        _properties.SetValue += OnPropertiesSetValue;
    }

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

    [MirrorVerified("Release IL")]
    public Color TypeoverColor
    {
        get => Settings.Default.TypeoverColor;
        set
        {
            Settings.Default.TypeoverColor = value;
            Settings.Default.Save();
            FirePropertyChanged(nameof(TypeoverColor));
        }
    }

    [MirrorVerified("Release IL")]
    public Color CaretColor
    {
        get => Settings.Default.CaretColor;
        set
        {
            Settings.Default.CaretColor = value;
            Settings.Default.Save();
            FirePropertyChanged(nameof(CaretColor));
        }
    }

    [MirrorVerified("Release IL")]
    public Color ReadOnlyCaretColor
    {
        get => Settings.Default.ReadOnlyCaretColor;
        set
        {
            Settings.Default.ReadOnlyCaretColor = value;
            Settings.Default.Save();
            FirePropertyChanged(nameof(ReadOnlyCaretColor));
        }
    }

    [MirrorVerified("Release IL")]
    public bool CaretBlink
    {
        get => Settings.Default.CaretBlink;
        set
        {
            Settings.Default.CaretBlink = value;
            Settings.Default.Save();
            FirePropertyChanged(nameof(CaretBlink));
        }
    }

    [MirrorVerified("Release IL")]
    public int CaretBlinkPeriod
    {
        get => Settings.Default.CaretBlinkPeriod;
        set
        {
            Settings.Default.CaretBlinkPeriod = value;
            Settings.Default.Save();
            FirePropertyChanged(nameof(CaretBlinkPeriod));
        }
    }

    [MirrorVerified("Release IL")]
    public double CaretHighlightOpacity
    {
        get => Settings.Default.CaretHighlightOpacity;
        set
        {
            Settings.Default.CaretHighlightOpacity = value;
            Settings.Default.Save();
            FirePropertyChanged(nameof(CaretHighlightOpacity));
        }
    }

    [MirrorVerified("Release IL")]
    public Color ScopeColor
    {
        get => Settings.Default.ScopeColor;
        set
        {
            Settings.Default.ScopeColor = value;
            Settings.Default.Save();
            FirePropertyChanged(nameof(ScopeColor));
        }
    }

    [MirrorVerified("Release IL")]
    public double ScopeOpacity
    {
        get => Settings.Default.ScopeOpacity;
        set
        {
            Settings.Default.ScopeOpacity = value;
            Settings.Default.Save();
            FirePropertyChanged(nameof(ScopeOpacity));
        }
    }

    [MirrorVerified("Release IL")]
    public Color NearestNeighborColor
    {
        get => Settings.Default.NearestNeighborColor;
        set
        {
            Settings.Default.NearestNeighborColor = value;
            Settings.Default.Save();
            FirePropertyChanged(nameof(NearestNeighborColor));
        }
    }

    [MirrorVerified("Release IL")]
    public double NearestNeighborOpacity
    {
        get => Settings.Default.NearestNeighborOpacity;
        set
        {
            Settings.Default.NearestNeighborOpacity = value;
            Settings.Default.Save();
            FirePropertyChanged(nameof(NearestNeighborOpacity));
        }
    }

    [MirrorVerified("Release IL")]
    public bool HighlightOperands
    {
        get => Settings.Default.HighlightOperands;
        set
        {
            Settings.Default.HighlightOperands = value;
            Settings.Default.Save();
            FirePropertyChanged(nameof(HighlightOperands));
        }
    }

    [MirrorVerified("Release IL")]
    public bool ParenthesizeOperands
    {
        get => Settings.Default.ParenthesizeOperands;
        set
        {
            Settings.Default.ParenthesizeOperands = value;
            Settings.Default.Save();
            FirePropertyChanged(nameof(ParenthesizeOperands));
        }
    }

    [MirrorVerified("Release IL")]
    public TableSize ResultMatrixMaxDisplaySize
    {
        get => Settings.Default.ResultMatrixMaxDisplaySize;
        set
        {
            Settings.Default.ResultMatrixMaxDisplaySize = value;
            Settings.Default.Save();
            FirePropertyChanged(nameof(ResultMatrixMaxDisplaySize));
        }
    }

    [MirrorVerified("Release IL")]
    public double FractionBarVerticalMargin
    {
        get => Settings.Default.FractionBarVerticalMargin;
        set
        {
            Settings.Default.FractionBarVerticalMargin = value;
            Settings.Default.Save();
            FirePropertyChanged(nameof(FractionBarVerticalMargin));
        }
    }

    [MirrorVerified("Release IL")]
    public string EvalPlaceholder
    {
        get => Settings.Default.EvalPlaceholder;
        set
        {
            Settings.Default.EvalPlaceholder = value;
            Settings.Default.Save();
            FirePropertyChanged(nameof(EvalPlaceholder));
        }
    }

    [MirrorVerified("Release IL")]
    public string LineWrappingSign
    {
        get => Settings.Default.LineWrappingSign;
        set
        {
            Settings.Default.LineWrappingSign = value;
            Settings.Default.Save();
            FirePropertyChanged(nameof(LineWrappingSign));
        }
    }

    [MirrorVerified("Release IL")]
    public double LineWrappingSignOffset
    {
        get => Settings.Default.LineWrappingSignOffset;
        set
        {
            Settings.Default.LineWrappingSignOffset = value;
            Settings.Default.Save();
            FirePropertyChanged(nameof(LineWrappingSignOffset));
        }
    }

    [MirrorVerified("Release IL")]
    public bool ShowResultMatrixFormatIndices
    {
        get => Settings.Default.ShowResultMatrixFormatIndices;
        set
        {
            Settings.Default.ShowResultMatrixFormatIndices = value;
            Settings.Default.Save();
            FirePropertyChanged(nameof(ShowResultMatrixFormatIndices));
        }
    }

    [MirrorVerified("Release IL")]
    public Color ComboBoxColor
    {
        get => Settings.Default.ComboBoxColor;
        set
        {
            Settings.Default.ComboBoxColor = value;
            Settings.Default.Save();
            FirePropertyChanged(nameof(ComboBoxColor));
        }
    }

    [MirrorVerified("Release IL")]
    public bool AutoLabelIds
    {
        get => MathNodeSettings.Instance.AutoLabelIds;
        set
        {
            MathNodeSettings.Instance.AutoLabelIds = value;
            FirePropertyChanged(nameof(AutoLabelIds));
        }
    }

    [MirrorVerified("Release IL")]
    private void FirePropertyChanged(string name = default)
    {
        PropertyChanged?.Invoke(this, new global::System.ComponentModel.PropertyChangedEventArgs(name));
    }
}
