using System.Collections.Generic;
using System.Windows.Input;
using Quire.FunctionalitiesLimitation;
using Quire.Wpf;

namespace Quire.Controls;

public partial class SymbolDescriptor
{
    public virtual RoutedCommand Command
    {
        get => _command;
        set => _command = value;
    }

    public virtual object CommandParameter => SymbolicRepresentation;

    public string ToolTipUid { get; set; }

    public virtual string DisplayName
    {
        get => _displayName;
        set => _displayName = value;
    }

    public virtual string Header
    {
        get
        {
            string keybindingToDisplay = KeybindingToDisplay ?? GeneratedKeybindingToDisplay;
            if (!keybindingToDisplay.IsNullOrEmpty())
            {
                return string.Format("{0} ({1})", DisplayName, keybindingToDisplay);
            }

            return DisplayName;
        }
    }

    public string SymbolicRepresentation
    {
        get => _symbolicRepresentation;
        set => _symbolicRepresentation = value;
    }

    public virtual string ScreenRepresentation
    {
        get => _screenRepresentation;
        set => _screenRepresentation = value;
    }

    public EditorGesture Keybinding
    {
        get => _keybinding;
        set => _keybinding = value;
    }

    public virtual string KeybindingToDisplay
    {
        get => _keybindingToDisplay;
        set => _keybindingToDisplay = value;
    }

    public string Description
    {
        get => _description;
        set => _description = value;
    }

    public string Category
    {
        get
        {
            if (_categories.Count > 0)
            {
                return _categories[0];
            }

            return null;
        }
        set => _categories.Insert(0, value);
    }

    public string Category2
    {
        get
        {
            if (_categories.Count > 1)
            {
                return _categories[1];
            }

            return null;
        }
        set => _categories.Insert(1, value);
    }

    public string Category3
    {
        get
        {
            if (_categories.Count > 2)
            {
                return _categories[2];
            }

            return null;
        }
        set => _categories.Insert(2, value);
    }

    public List<string> Categories
    {
        get => _categories;
        set => _categories = value;
    }

    public int CategoryOrdinal
    {
        get => _categoryOrdinal;
        set => _categoryOrdinal = value;
    }

    public EnumFunctionalityKey FunctionalityKey
    {
        get => _functionalityKey;
        set => _functionalityKey = value;
    }

    public EnumFunctionalityKey CategoryFunctionalityKey
    {
        get => _categoryFunctionalityKey;
        set => _categoryFunctionalityKey = value;
    }

    public EnumFunctionalityKey Category2FunctionalityKey
    {
        get => _category2FunctionalityKey;
        set => _category2FunctionalityKey = value;
    }

    public EnumFunctionalityKey Category3FunctionalityKey
    {
        get => _category3FunctionalityKey;
        set => _category3FunctionalityKey = value;
    }

    public virtual string GeneratedKeybindingToDisplay => (string)new EditorGestureConverter().ConvertTo(Keybinding, typeof(string));
}
