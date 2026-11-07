#nullable disable
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using Quire.Core;

namespace Quire.Controls.Core;

public partial interface ITextStylesManager
{
    ObservableCollection<TextStyleData> StylesCollection { get; set; }
    TextStyleChangedHandler TextStyleChangedHandler { get; }
    event Delegate0 StyleCollectionChanged;
    void LoadWorkSheetStyles(IReadOnlyDictionary<string, ITextStyleObject> textStyles);
    string CreateAndAddStyle(string styleName, string styleKey, Style style, string basedOnKey);
    void AddStyle(TextStyleData textStyleData, int location = 0);
    TextStyleData GetTextStyleDataByName(string styleName);
    TextStyleData GetTextStyleDataByKey(string styleKey);
    TextStyleData GetTextStyleDataByStyle(Style style);
    bool IsUniqueTextStyleDataName(string styleName);
    bool IsUniqueStyleObject(Style style);
    string StyleCreateRequest(TextFormat arg);
    void RemoveStyle(TextStyleData textStyleData);
    void UpdateTextStyleData(string name, string key, Style style, string basedOnKey);
    Dictionary<string, ITextStyleObject> ToTextStyleDictionary(bool basedOnValues = true);
}
