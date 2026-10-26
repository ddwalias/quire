#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Quire;
using Quire.Mirror;

namespace Quire.Controls.Core;

public static partial class WorksheetPropertyNames
{
    private static readonly Type _typeOfWorksheetProperties;

    private static readonly Type _typeOfThis;

    [MirrorVerified("PtcControlsCoreWorksheetPropertyNamesMechanicalDifferentialTests")]
    private static readonly string[] _names;

    public const string PageSizeValue = "PageSizeValue";

    public const string PageMargin = "PageMargin";

    public const string WorksheetPageLayoutMode = "WorksheetPageLayoutMode";

    public const string GridCellSize = "GridCellSize";

    public const string FilePath = "FilePath";

    public const string CacheWorksheet = "CacheWorksheet";

    public const string DocCalculationOptions = "DocCalculationOptions";

    public const string DocCalculationVariables = "DocCalculationVariables";

    public const string WorksheetName = "WorksheetName";

    public const string DisplayFilePath = "DisplayFilePath";

    public const string SpellCheckEnabled = "SpellCheckEnabled";

    public const string DefaultUnitSystem = "DefaultUnitSystem";

    public const string SimplifyUnitSystem = "SimplifyUnitSystem";

    public const string HelpServerPath = "HelpServerPath";

    static WorksheetPropertyNames()
    {
        _typeOfWorksheetProperties = typeof(IWorksheetProperties);
        _typeOfThis = typeof(WorksheetPropertyNames);
        List<string> list = new List<string>();
        FieldInfo[] fields = _typeOfThis.GetFields(BindingFlags.Static | BindingFlags.Public);
        for (int i = 0; i < fields.Length; i++)
        {
            FieldInfo fieldInfo = fields[i];
            string text = fieldInfo.GetValue(null) as string;
            if (text != null)
            {
                string name = fieldInfo.Name;
                if (name == text)
                {
                    list.Add(name);
                    DebugTools.AssertNotNull(_typeOfWorksheetProperties.GetProperty(name), "Property is missing in " + _typeOfWorksheetProperties.Name + ": " + name);
                }
                else
                {
                    DebugTools.FailAssertion("Property name: " + name + " doesn't match the value: " + text);
                }
            }
        }
        _names = list.ToArray();
    }

    public static bool Contains(string name)
    {
        return _names.Contains(name);
    }

    [MirrorVerified("PtcControlsCoreWorksheetPropertyNamesMechanicalDifferentialTests")]
    public static string[] GetNames() => _names;

    [MirrorVerified("PtcControlsCoreWorksheetPropertyNamesMechanicalDifferentialTests")]
    public static Type GetTypeOfWorksheetProperties() => _typeOfWorksheetProperties;
}
