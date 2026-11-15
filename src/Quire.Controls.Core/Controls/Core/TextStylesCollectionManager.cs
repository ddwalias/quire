#nullable disable
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using Quire.Core;
using Quire.Mirror;

namespace Quire.Controls.Core;

public partial class TextStylesCollectionManager
{
    private ObservableCollection<TextStyleData> _stylesCollection = new ObservableCollection<TextStyleData>();

    [MirrorVerified("PtcControlsCoreTextStylesCollectionManagerMechanicalDifferentialTests")]
    private readonly TextStyleChangedHandler _textStyleChangedHandler;

    private const string StyleResourceKey = "Style.";

    public ObservableCollection<TextStyleData> StylesCollection
    {
        get
        {
            return _stylesCollection;
        }
        set
        {
            _stylesCollection = value;
            FirePropertyChanged("StylesCollection");
        }
    }

    [MirrorVerified("PtcControlsCoreTextStylesCollectionManagerMechanicalDifferentialTests")]
    public TextStyleChangedHandler TextStyleChangedHandler => _textStyleChangedHandler;

    [MirrorVerified("PtcControlsCoreTextStylesCollectionManagerMechanicalDifferentialTests")]
    public event Delegate0 StyleCollectionChanged;

    [MirrorVerified("PtcControlsCoreTextStylesCollectionManagerMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;

    public TextStylesCollectionManager()
    {
        _textStyleChangedHandler = new TextStyleChangedHandler();
    }

    public string StyleCreateRequest(TextFormat arg)
    {
        if (GetTextStyleDataByKey(arg.StyleKey) != null)
        {
            arg.StyleKey = TextStyleBase.TextStyleUniqueKeyGenerator(arg.StyleKey);
        }

        TextStyle textStyle = new TextStyle(arg.StyleKey, arg);
        return CreateAndAddStyle(textStyle.Name, textStyle.StyleKey, textStyle.Style, textStyle.BasedOnKey);
    }

    public Dictionary<string, ITextStyleObject> ToTextStyleDictionary(bool basedOnValues = true)
    {
        return StylesCollection.ToDictionary(textStyleData => textStyleData.StyleKey, textStyleData => (ITextStyleObject)new TextStyle(textStyleData, basedOnValues));
    }

    private void FirePropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public string CreateAndAddStyle(string styleName, string styleKey, Style style, string basedOnKey)
    {
        TextStyleData textStyleData = new TextStyleData(styleName, styleKey, style, basedOnKey);
        AddStyle(textStyleData);
        StyleCollectionChanged?.Invoke();
        return textStyleData.StyleKey;
    }

    private string RenameStyleNameToCounter(string styleName)
    {
        do
        {
            MatchCollection matchCollection = Regex.Matches(styleName, "[(](?<counter>[0-9]+)[)]");
            if (matchCollection.Count > 0 && styleName.EndsWith(matchCollection[0].Value))
            {
                int.TryParse(matchCollection[0].Groups["counter"].Value, out var result);
                result++;
                styleName = styleName.Replace(matchCollection[0].Value, "(" + result + ")");
            }
            else
            {
                styleName += " (1)";
            }
        }
        while (GetTextStyleDataByName(styleName) != null);
        return styleName;
    }

    public void LoadWorkSheetStyles(IReadOnlyDictionary<string, ITextStyleObject> textStyles)
    {
        SortedList<string, TextStyleData> sortedList = new SortedList<string, TextStyleData>();
        LoadWorkSheetStylesFromFile(sortedList, textStyles);
        LoadWorkSheetEmbeddedStyles(sortedList, textStyles);
        List<TextStyleData> list = sortedList.Values.ToList();
        LoadWorkSheetLegacyStyles(list, textStyles);
        LoadWorkSheetBasedOnStyles(list);
        StylesCollection = new ObservableCollection<TextStyleData>(list);
    }

    protected void LoadWorkSheetStylesFromFile(SortedList<string, TextStyleData> stylesCollection, IReadOnlyDictionary<string, ITextStyleObject> textStyles)
    {
        foreach (KeyValuePair<string, ITextStyleObject> item in textStyles)
        {
            if (item.Key.StartsWith("Legacy"))
            {
                break;
            }

            TextStyle textStyle = item.Value as TextStyle;
            if (textStyle != null && textStyle.FontFamily != null)
            {
                if (textStyle.FontFamily.Source.Contains(FontConstants.EmbedFontDelimiter))
                {
                    textStyle.FontFamily = new FontFamily(new Uri("pack://application:,,,"), textStyle.FontFamily.Source);
                }

                if (textStyle.FontFamily.Source.Equals("Mathcad UniMath Prime") && !Fonts.SystemFontFamilies.Contains(new FontFamily("Mathcad UniMath Prime")))
                {
                    textStyle.FontFamily = new FontFamily(new Uri("pack://application:,,,"), string.Concat("Ptc.Controls.Core;component/Fonts/#", "Mathcad UniMath Prime"));
                }

                if (textStyle.FontFamily.Source.Equals("Mathcad UniMath") && !Fonts.SystemFontFamilies.Contains(new FontFamily("Mathcad UniMath")))
                {
                    textStyle.FontFamily = new FontFamily(new Uri("pack://application:,,,"), string.Concat("Ptc.Controls.Core;component/Fonts/#", "Mathcad UniMath"));
                }

                if (textStyle.Margin.HasValue && (textStyle.Margin.Value.Top < 0.0 || textStyle.Margin.Value.Right < 0.0 || textStyle.Margin.Value.Bottom < 0.0 || textStyle.Margin.Value.Left < 0.0))
                {
                    textStyle.Margin = new Thickness(Math.Max(0.0, textStyle.Margin.Value.Left), Math.Max(0.0, textStyle.Margin.Value.Top), Math.Max(0.0, textStyle.Margin.Value.Right), Math.Max(0.0, textStyle.Margin.Value.Bottom));
                }
            }

            string text = item.Value.Name ?? item.Key;
            if (item.Key != "areaLayoutStyle" && item.Key != "headerLayoutStyle" && item.Key != "footerLayoutStyle")
            {
                string baseOnKey = null;
                if (item.Value.BasedOnKey.IsNotEmpty())
                {
                    baseOnKey = item.Value.BasedOnKey;
                }

                if (item.Value.TextDecorations == null && item.Value.BasedOnKey.IsNullOrEmpty())
                {
                    item.Value.TextDecorations = new TextDecorationCollection();
                }

                stylesCollection.Add(text, new TextStyleData(text, item.Key, ((TextStyle)item.Value).Style, baseOnKey));
            }
        }
    }

    protected void LoadWorkSheetLegacyStyles(IList<TextStyleData> stylesCollection, IReadOnlyDictionary<string, ITextStyleObject> textStyles)
    {
        foreach (KeyValuePair<string, ITextStyleObject> textStyle in textStyles)
        {
            if (textStyle.Key.StartsWith("Legacy"))
            {
                string name = textStyle.Value.Name ?? textStyle.Key;
                string baseOnKey = null;
                if (textStyle.Value.BasedOnKey.IsNotEmpty())
                {
                    baseOnKey = textStyle.Value.BasedOnKey;
                }

                if (textStyle.Value.TextDecorations == null && textStyle.Value.BasedOnKey.IsNullOrEmpty())
                {
                    textStyle.Value.TextDecorations = new TextDecorationCollection();
                }

                stylesCollection.Add(new TextStyleData(name, textStyle.Key, ((TextStyle)textStyle.Value).Style, baseOnKey));
            }
        }
    }

    protected void LoadWorkSheetBasedOnStyles(IList<TextStyleData> stylesCollection)
    {
        foreach (TextStyleData textStyleData in stylesCollection)
        {
            if (textStyleData.BaseOnKey.IsNotEmpty())
            {
                TextStyle textStyle = new TextStyle(textStyleData);
                TextStyle basedOn = new TextStyle(stylesCollection.FirstOrDefault(styleData => styleData.StyleKey == textStyleData.BaseOnKey));
                textStyle.UpdateBasedOn(basedOn, stylesCollection);
                textStyleData.Style = textStyle.Style;
            }
        }
    }

    protected void LoadWorkSheetEmbeddedStyles(SortedList<string, TextStyleData> stylesCollection, IReadOnlyDictionary<string, ITextStyleObject> textStyles)
    {
        foreach (DictionaryEntry item in TextStylesDictionary.Instance)
        {
            Style style = item.Value as Style;
            if (style == null || !(item.Key is string text) || textStyles.ContainsKey(text))
            {
                continue;
            }

            string baseOnKey = null;
            string text2 = (string)style.Resources[StyleResourceKey + item.Key];
            if (text == "Normal")
            {
                style = UpdateStyleToLimbo(style, textStyles);
            }

            if (style.BasedOn != null)
            {
                string[] array = new string[style.BasedOn.Resources.Count];
                style.BasedOn.Resources.Keys.CopyTo(array, 0);
                baseOnKey = array.FirstOrDefault(resourceKey => resourceKey.Contains(StyleResourceKey));
                baseOnKey = baseOnKey?.Substring(6);
            }

            if (stylesCollection.ContainsKey(text2))
            {
                text2 = RenameStyleNameToCounter(text2);
            }

            stylesCollection.Add(text2, new TextStyleData(text2, text, style, baseOnKey));
        }
    }

    protected Style UpdateStyleToLimbo(Style style, IReadOnlyDictionary<string, ITextStyleObject> textStyles)
    {
        if (textStyles.ContainsKey("areaLayoutStyle"))
        {
            style = TextStyleBase.CloneStyle(style);
            ITextStyleObject textStyleObject = textStyles["areaLayoutStyle"];
            Setter setter = (Setter)style.Setters.FirstOrDefault(prop => ((Setter)prop).Property.Name == TextElement.FontFamilyProperty.Name);
            if (setter != null)
            {
                setter.Value = ((TextFormat)textStyleObject).FontFamily;
            }

            setter = (Setter)style.Setters.FirstOrDefault(prop => ((Setter)prop).Property.Name == TextElement.FontSizeProperty.Name);
            if (setter != null)
            {
                setter.Value = ((TextFormat)textStyleObject).FontSize;
            }

            setter = (Setter)style.Setters.FirstOrDefault(prop => ((Setter)prop).Property.Name == TextElement.ForegroundProperty.Name);
            if (setter != null)
            {
                setter.Value = ((TextFormat)textStyleObject).Foreground;
            }

            setter = (Setter)style.Setters.FirstOrDefault(prop => ((Setter)prop).Property.Name == TextElement.BackgroundProperty.Name);
            if (setter != null)
            {
                setter.Value = ((TextFormat)textStyleObject).Background;
            }

            setter = (Setter)style.Setters.FirstOrDefault(prop => ((Setter)prop).Property.Name == TextElement.FontWeightProperty.Name);
            if (setter != null)
            {
                setter.Value = ((TextFormat)textStyleObject).FontWeight;
            }

            setter = (Setter)style.Setters.FirstOrDefault(prop => ((Setter)prop).Property.Name == TextElement.FontStyleProperty.Name);
            if (setter != null)
            {
                setter.Value = ((TextFormat)textStyleObject).FontStyle;
            }

            setter = (Setter)style.Setters.FirstOrDefault(prop => ((Setter)prop).Property.Name == TextBlock.TextDecorationsProperty.Name);
            if (setter != null)
            {
                setter.Value = ((TextFormat)textStyleObject).TextDecorations;
            }
        }

        return style;
    }

    public bool IsUniqueTextStyleDataName(string textStyleName)
    {
        return GetTextStyleDataByName(textStyleName) == null;
    }

    public bool IsUniqueStyleObject(Style textStyle)
    {
        return GetTextStyleDataByStyle(textStyle) == null;
    }

    public void AddStyle(TextStyleData textStyleData, int location = 0)
    {
        if (StylesCollection.FirstOrDefault(x => x.StyleName == textStyleData.StyleName) != null)
        {
            textStyleData.StyleName = RenameStyleNameToCounter(textStyleData.StyleName);
        }

        if (textStyleData.StyleKey.StartsWith("Legacy"))
        {
            location = StylesCollection.Count;
        }

        StylesCollection.Insert(location, textStyleData);
    }

    public void RemoveStyle(TextStyleData textStyleData)
    {
        StylesCollection.Remove(textStyleData);
        StyleCollectionChanged?.Invoke();
    }

    public void UpdateTextStyleData(string name, string key, Style style, string basedOnKey)
    {
        ICollection<TextStyleData> collection = new List<TextStyleData>();
        TextStyleData textStyleDataByKey = GetTextStyleDataByKey(key);
        textStyleDataByKey.UpdateStyle(name, style, basedOnKey, TextStyleBase.CloneStyle(textStyleDataByKey.Style.BasedOn));
        collection.Add(textStyleDataByKey);
        UpdateTextStyleDataBasedOn(textStyleDataByKey, collection);
        _textStyleChangedHandler.UpdateStyle(collection);
        StyleCollectionChanged?.Invoke();
    }

    private void UpdateTextStyleDataBasedOn(TextStyleData updatedTextStyleData, ICollection<TextStyleData> collection)
    {
        foreach (TextStyleData item in StylesCollection)
        {
            if (!collection.Contains(item))
            {
                KeyIsBasedOnKey(updatedTextStyleData.StyleKey, item, collection);
            }
        }
    }

    private void KeyIsBasedOnKey(string styleKey, TextStyleData textStyleData, ICollection<TextStyleData> collection)
    {
        if (textStyleData.BaseOnKey != null)
        {
            TextStyleData textStyleDataByKey = GetTextStyleDataByKey(textStyleData.BaseOnKey);
            if (textStyleDataByKey != null && textStyleData.BaseOnKey != styleKey)
            {
                KeyIsBasedOnKey(styleKey, textStyleDataByKey, collection);
            }

            if (textStyleData.BaseOnKey == styleKey || collection.Contains(textStyleDataByKey))
            {
                collection.Add(textStyleData);
                Style style = TextStyleBase.CloneStyle(textStyleData.Style);
                style.BasedOn = textStyleDataByKey?.Style;
                textStyleData.UpdateStyle(textStyleData.StyleName, style, textStyleData.BaseOnKey, textStyleDataByKey.OldStyle);
            }
        }
    }

    public TextStyleData GetTextStyleDataByName(string styleName)
    {
        return StylesCollection.FirstOrDefault(styleData => styleData.StyleName == styleName);
    }

    public TextStyleData GetTextStyleDataByKey(string styleKey)
    {
        return StylesCollection.FirstOrDefault(styleData => styleData.StyleKey == styleKey);
    }

    public TextStyleData GetTextStyleDataByStyle(Style textStyle)
    {
        return StylesCollection.FirstOrDefault(styleData => styleData.Style == textStyle);
    }
}
