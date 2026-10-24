#nullable disable
using System.Windows;
using System.Windows.Media;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using Quire.Mirror;
using Quire.Mirror;

namespace Quire;

public partial class TextStyleBase
{
    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public double Weight { get; }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public string Name { get; set; }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public string StyleKey { get; set; }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public Style Style { get; private set; }

    [MirrorVerified("Release IL")]
    public Nullable<Quire.Core.TextAlignmentExt> TextAlignmentExt => default;

    [MirrorVerified("Release IL")]
    public static string TextStyleUniqueKeyGenerator(string key)
    {
        return string.Concat(TextStyleStripKeyGenerator(key), "~", DateTime.Now.Ticks.ToString());
    }

    [MirrorVerified("Release IL")]
    public static string TextStyleStripKeyGenerator(string key)
    {
        if (key.Split('~', StringSplitOptions.None).Length != 1)
        {
            return key.Substring(0, key.LastIndexOf('~'));
        }
        return key;
    }

    [MirrorVerified("Release IL")]
    public object GetSetterValue(string propertyName, bool isFixInHeritage = false)
    {
        if (_htPropertyToIndex[propertyName] != null)
        {
            return ((System.Windows.Setter)Style.Setters[(int)_htPropertyToIndex[propertyName]]).Value;
        }
        if (isFixInHeritage)
        {
            return GetBasedOnSetterValue(Style, propertyName);
        }
        return null;
    }

    public bool IsDeletedProperty(string propertyName, object value)
    {
        if (_htDeletedPropertyToIndex.ContainsKey(propertyName) && value != null)
        {
            return _htOldPropertyToIndex[propertyName].ToString().Equals(value.ToString());
        }
        return false;
    }

    public bool IsModifyProperty(string propertyName, object value)
    {
        if (value != null && _htOldPropertyToIndex.ContainsKey(propertyName) && !_htOldPropertyToIndex[propertyName].Equals(value))
        {
            return true;
        }
        return false;
    }

    public bool IsModifyPropertyCollection(string propertyName, global::System.Collections.ICollection value)
    {
        if (value != null && _htOldPropertyToIndex.ContainsKey(propertyName) && ((global::System.Collections.ICollection)_htOldPropertyToIndex[propertyName]).Count != value.Count)
        {
            return true;
        }
        return false;
    }

    public bool IsModifyPropertyString(string propertyName, object value)
    {
        if (value != null && _htOldPropertyToIndex.ContainsKey(propertyName) && !_htOldPropertyToIndex[propertyName].ToString().Equals(value.ToString()))
        {
            return true;
        }
        return false;
    }

    public bool IsModifyProperty(string propertyName, double value, double tolerance)
    {
        if (_htOldPropertyToIndex.ContainsKey(propertyName))
        {
            return Math.Abs(double.Parse(_htOldPropertyToIndex[propertyName].ToString()) - value) > tolerance;
        }
        return false;
    }

    public static object GetBasedOnSetterValue(Style style, string propertyName)
    {
        if (style?.BasedOn != null)
        {
            Style basedOn = style.BasedOn;
            do
            {
                object setterValue = GetSetterValue(basedOn, propertyName);
                if (setterValue != null)
                {
                    return setterValue;
                }
                basedOn = basedOn.BasedOn;
            }
            while (basedOn != null);
        }
        return null;
    }

    public static Style CloneStyle(Style style)
    {
        if (style == null)
        {
            return null;
        }
        Style clone = new Style(style.TargetType, style.BasedOn);
        foreach (Setter setter in style.Setters.OfType<Setter>())
        {
            clone.Setters.Add(new Setter(setter.Property, setter.Value));
        }
        return clone;
    }

    public Dictionary<string, TextStylePropertyInfoExt> GetSetters(Style textStyle, Dictionary<string, TextStylePropertyInfoExt> setterCollection, bool isInherited = false)
    {
        if (textStyle.BasedOn != null)
        {
            GetSetters(textStyle.BasedOn, setterCollection, isInherited: true);
        }
        foreach (Setter setter in textStyle.Setters.OfType<Setter>())
        {
            object value = setter.Value;
            if (setter.Property.Name == "FontSize")
            {
                value = Math.Round(Quire.Wpf.WpfUtils.DipsToPoints((double)value), 1);
            }
            setterCollection[setter.Property.Name] = new TextStylePropertyInfoExt(isInherited, value);
        }
        return setterCollection;
    }

    protected Style CloneStyleAndSetters(Style style)
    {
        Style clone = new Style(style.TargetType, style.BasedOn);
        CloneSetters(style.Setters, clone);
        return clone;
    }

    public void SetSetterValue(string propertyName, object value)
    {
        if (value == null)
        {
            if (!_htPropertyToIndex.ContainsKey(propertyName)) goto Return;
            Style.Setters.RemoveAt((int)_htPropertyToIndex[propertyName]);
            _htPropertyToIndex.Remove(propertyName);
            return;
        }
        if (_htPropertyToIndex.ContainsKey(propertyName))
        {
            ((Setter)Style.Setters[(int)_htPropertyToIndex[propertyName]]).Value = value;
            return;
        }
        global::System.Reflection.FieldInfo dependencyProperties = GetDependencyProperties(typeof(System.Windows.Documents.TextElement), propertyName);
        if (dependencyProperties == null)
        {
            dependencyProperties = GetDependencyProperties(typeof(System.Windows.Documents.Inline), propertyName);
        }
        if (dependencyProperties == null)
        {
            dependencyProperties = GetDependencyProperties(typeof(System.Windows.Documents.Block), propertyName);
        }
        if (dependencyProperties == null)
        {
            dependencyProperties = GetDependencyProperties(typeof(global::Quire.Core.TextFormat), propertyName);
        }
        if (dependencyProperties == null)
        {
            dependencyProperties = GetDependencyProperties(typeof(System.Windows.Documents.Paragraph), propertyName);
        }
        if (dependencyProperties == null)
        {
            dependencyProperties = GetDependencyProperties(typeof(System.Windows.Documents.Typography), propertyName);
        }
        if (dependencyProperties != null)
        {
            Style.Setters.Add(new Setter((DependencyProperty)dependencyProperties.GetValue(null), value));
            _htPropertyToIndex[propertyName] = Style.Setters.Count - 1;
        }

    Return:
        return;
    }

    protected void ClearHeritageProperties()
    {
        if (Style.BasedOn != null)
        {
            foreach (Setter setter in Style.BasedOn.Setters.OfType<Setter>())
            {
                if (!_htPropertyToIndex.ContainsKey(setter.Property.Name))
                {
                    UpdateMemberValue(setter.Property.Name, null);
                }
            }
        }
    }

    protected void UpdateBasedOnMembersValue(Style style)
    {
        if (style != null)
        {
            UpdateBasedOnMembersValue(style.BasedOn);
            foreach (Setter setter in style.Setters.OfType<Setter>())
            {
                if (!_htPropertyToIndex.ContainsKey(setter.Property.Name))
                {
                    UpdateMemberValue(setter.Property.Name, setter.Value);
                }
            }
        }
    }

    protected void CloneSetters(SetterBaseCollection setters, Style style)
    {
        foreach (Setter setter in setters.OfType<Setter>())
        {
            style.Setters.Add(new Setter(setter.Property, setter.Value));
            _htPropertyToIndex[setter.Property.Name] = style.Setters.Count - 1;
            _htDeletedPropertyToIndex.Remove(setter.Property.Name);
            UpdateMemberValue(setter.Property.Name, setter.Value);
        }
    }

    public void UpdateDeletedProperties(Style oldStyle)
    {
        if (oldStyle != null)
        {
            int index = 0;
            foreach (Setter setter in oldStyle.Setters.OfType<Setter>())
            {
                _htDeletedPropertyToIndex[setter.Property.Name] = index;
                _htOldPropertyToIndex[setter.Property.Name] = setter.Value;
                index++;
            }
            if (!_htOldPropertyToIndex.ContainsKey(global::System.Windows.Documents.TextElement.ForegroundProperty.Name))
            {
                _htOldPropertyToIndex[global::System.Windows.Documents.TextElement.ForegroundProperty.Name] = GetBasedOnSetterValue(oldStyle, global::System.Windows.Documents.TextElement.ForegroundProperty.Name) ?? new SolidColorBrush(Colors.Black);
            }
            if (!_htOldPropertyToIndex.ContainsKey(global::System.Windows.Documents.TextElement.BackgroundProperty.Name))
            {
                _htOldPropertyToIndex[global::System.Windows.Documents.TextElement.BackgroundProperty.Name] = GetBasedOnSetterValue(oldStyle, global::System.Windows.Documents.TextElement.BackgroundProperty.Name) ?? new SolidColorBrush(Colors.Transparent);
            }
            if (!_htOldPropertyToIndex.ContainsKey(global::System.Windows.Documents.TextElement.FontWeightProperty.Name))
            {
                _htOldPropertyToIndex[global::System.Windows.Documents.TextElement.FontWeightProperty.Name] = GetBasedOnSetterValue(oldStyle, global::System.Windows.Documents.TextElement.FontWeightProperty.Name) ?? (object)FontWeights.Normal;
            }
            if (!_htOldPropertyToIndex.ContainsKey(global::System.Windows.Documents.TextElement.FontStyleProperty.Name))
            {
                _htOldPropertyToIndex[global::System.Windows.Documents.TextElement.FontStyleProperty.Name] = GetBasedOnSetterValue(oldStyle, global::System.Windows.Documents.TextElement.FontStyleProperty.Name) ?? (object)FontStyles.Normal;
            }
            if (!_htOldPropertyToIndex.ContainsKey(global::System.Windows.Documents.TextElement.FontSizeProperty.Name))
            {
                _htOldPropertyToIndex[global::System.Windows.Documents.TextElement.FontSizeProperty.Name] = GetBasedOnSetterValue(oldStyle, global::System.Windows.Documents.TextElement.FontSizeProperty.Name) ?? (object)global::Quire.Wpf.WpfUtils.PointsToDips(11.0);
            }
        }
        oldStyle = null;
    }

    [MirrorVerified("Release IL")]
    public void MergeBasedOn()
    {
        _baseOnKey = null;
        ClearHeritageProperties();
        _htPropertyToIndex.Clear();
        if (Style.BasedOn != null)
        {
            Style style = CloneStyleAndSetters(Style);
            MergeBasedOnSetters(style, style.BasedOn);
            style.BasedOn = null;
            Style = style;
            int index = 0;
            foreach (System.Windows.Setter setter in System.Linq.Enumerable.OfType<System.Windows.Setter>(Style.Setters))
            {
                _htPropertyToIndex[setter.Property.Name] = index;
                index++;
                UpdateMemberValue(setter.Property.Name, setter.Value);
            }
        }
    }

    [MirrorVerified("Release IL")]
    public static void MergeBasedOnSetters(Style style, Style basedOn)
    {
        if (basedOn.BasedOn != null)
        {
            MergeBasedOnSetters(style, basedOn.BasedOn);
        }
        foreach (System.Windows.Setter setter in System.Linq.Enumerable.OfType<System.Windows.Setter>(basedOn.Setters))
        {
            if (Quire.Wpf.StylesExtensions.GetSetterValue(style, setter.Property) == null)
            {
                style.Setters.Add(new System.Windows.Setter(setter.Property, setter.Value));
            }
        }
    }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    protected string _baseOnKey;

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public string BasedOnKey { get => _baseOnKey; set => _baseOnKey = value; }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public double Stretch { get; }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    protected FontFamily _fontFamily;

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public FontFamily FontFamily { get => _fontFamily; set => Update("FontFamily", value); }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public bool Bold { get; }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public bool Italic { get; }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public bool Normal { get; }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public bool Oblique { get; }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    protected Brush _background;

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public Brush Background { get => _background; set => Update("Background", value); }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    protected Brush _foreground;

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public Brush Foreground { get => _foreground; set => Update("Foreground", value); }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    protected TextDecorationCollection _textDecorationCollection;

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public TextDecorationCollection TextDecorations { get => _textDecorationCollection; set => Update("TextDecorations", value); }

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    protected TextEffectCollection _textEffectCollection;

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public TextEffectCollection TextEffects => _textEffectCollection;

    [MirrorVerified("PtcTextStyleBaseMechanicalDifferentialTests")]
    public bool Condenst { get; }

    [MirrorVerified("Release IL")]
    protected TextStyleBase(string name) : this(new TextStyleData(name, TextStyleUniqueKeyGenerator(name), new Style(), null), true) { }

    [MirrorVerified("Release IL")]
    protected TextStyleBase(string name, string key) : this(new TextStyleData(name, key, new Style(), null), true) { }

    [MirrorVerified("Release IL")]
    protected void Update(string propertyName, object value)
    {
        Style = CloneStyleAndSetters(Style);
        SetSetterValue(propertyName, value);
        UpdateMemberValue(propertyName, GetSetterValue(propertyName, true));
    }

    [MirrorVerified("Release IL")]
    protected Nullable<double> _textIndent;

    [MirrorVerified("Release IL")]
    public Nullable<double> TextIndent { get => _textIndent; set => Update("TextIndent", value); }

    [MirrorVerified("Release IL")]
    protected Nullable<FontStyle> _fontStyle;

    [MirrorVerified("Release IL")]
    public Nullable<FontStyle> FontStyle { get => _fontStyle; set => Update("FontStyle", value); }

    [MirrorVerified("Release IL")]
    protected Nullable<FontWeight> _fontWeight;

    [MirrorVerified("Release IL")]
    public Nullable<FontWeight> FontWeight { get => _fontWeight; set => Update("FontWeight", value); }

    [MirrorVerified("Release IL")]
    protected Nullable<double> _fontSize;

    [MirrorVerified("Release IL")]
    public Nullable<double> FontSize { get => _fontSize; set => Update("FontSize", value); }

    [MirrorVerified("Release IL")]
    protected Nullable<Thickness> _margin;

    [MirrorVerified("Release IL")]
    public Nullable<Thickness> Margin { get => _margin; set => Update("Margin", value); }

    [MirrorVerified("Release IL")]
    protected Nullable<TextMarkerStyle> _markerStyle;

    [MirrorVerified("Release IL")]
    public Nullable<TextMarkerStyle> MarkerStyle { get => _markerStyle; set => Update("MarkerStyle", value); }

    [MirrorVerified("Release IL")]
    protected Nullable<FontVariants> _fontVariants;

    [MirrorVerified("Release IL")]
    public Nullable<FontVariants> FontVariants { get => _fontVariants; set => Update("Variants", value); }

    [MirrorVerified("Release IL")]
    protected BaselineAlignment _baselineAlignment;

    [MirrorVerified("Release IL")]
    public Nullable<BaselineAlignment> BaselineAlignment { get => new BaselineAlignment?(_baselineAlignment); set => Update("BaselineAlignment", value); }

    [MirrorVerified("Release IL")]
    public Nullable<TextAlignment> TextAlignment { get => _textAlignment; set => Update("TextAlignment", value); }

    [MirrorVerified("Release IL")]
    public static object GetSetterValue(Style style, string propertyName)
    {
        if (style != null)
        {
            System.Windows.Setter setter = (System.Windows.Setter)style.Setters.FirstOrDefault((System.Windows.SetterBase prop) => ((System.Windows.Setter)prop).Property.Name == propertyName);
            if (setter != null)
            {
                return setter.Value;
            }
        }
        return null;
    }
}
