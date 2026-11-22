#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Quire.Mirror;

namespace Quire.Controls.Core.IORegions;

public partial class IOItemsAliasManager
{
    private static readonly Type _typeOfThis = typeof(IOItemsAliasManager);

    [MirrorVerified("PtcControlsCoreIORegionsIOItemsAliasManagerMechanicalDifferentialTests")]
    private readonly Dictionary<string, IIOItem> _aliasesToItemsMap;

    public const string EvaluationDefaultAlias = "out";

    [MirrorVerified("PtcControlsCoreIORegionsIOItemsAliasManagerMechanicalDifferentialTests")]
    public static event Action<DependencyObject, DependencyPropertyChangedEventArgs> InAliasMapChanged;

    [MirrorVerified("PtcControlsCoreIORegionsIOItemsAliasManagerMechanicalDifferentialTests")]
    public static event Action<DependencyObject, DependencyPropertyChangedEventArgs> OutAliasMapChanged;

    public IOItemsAliasManager()
    {
        _aliasesToItemsMap = new Dictionary<string, IIOItem>();
    }

    internal static IList<string> DistinctIds(IEnumerable<string> ids)
    {
        return (from id in ids
            group id by id into gr
            select gr.First()).ToList();
    }

    public static IList<string> GetSExpressionsList(DependencyObject region, ItemOptions itemOptions)
    {
        IList<string> list = null;
        if (itemOptions.HasFlag(ItemOptions.Inputs) && region is IInputRegion inputRegion)
        {
            list = DistinctIds(inputRegion.GetDefinedIdentifierSExpressions());
        }
        if (itemOptions.HasFlag(ItemOptions.Outputs) && region is IOutputRegion)
        {
            if (list != null)
            {
                list.Add("out");
            }
            else
            {
                list = new List<string> { "out" };
            }
        }
        // Ternary emits the temporary IList<string> local matching Mathcad's stloc.2 / ldloc.2 / ret.
        return DebugTools.AssertNotNull(list, null).Passed ? list : new List<string>();
    }

    public static DependencyProperty InAliasMapProperty = DependencyProperty.RegisterAttached("InAliasMap", typeof(Dictionary<string, string>), _typeOfThis, new PropertyMetadata((object)null, HandleInAliassesPropertyChanged));

    public static DependencyProperty OutAliasMapProperty = DependencyProperty.RegisterAttached("OutAliasMap", typeof(Dictionary<string, string>), _typeOfThis, new PropertyMetadata((object)null, HandleOutAliassesPropertyChanged));

    private static void HandleInAliassesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // Release IL tests the event field against null instead of using ?.Invoke.
        if (InAliasMapChanged != null)
        {
            InAliasMapChanged(d, e);
        }
    }

    private static void HandleOutAliassesPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        // Release IL tests the event field against null instead of using ?.Invoke.
        if (OutAliasMapChanged != null)
        {
            OutAliasMapChanged(d, e);
        }
    }

    public static void SetIOAliasMap(DependencyObject element, Dictionary<string, string> value, bool isIn)
    {
        element.SetValue(isIn ? InAliasMapProperty : OutAliasMapProperty, value);
    }

    public static Dictionary<string, string> GetIOAliasMap(DependencyObject element, bool isIn)
    {
        return (Dictionary<string, string>)element.GetValue(isIn ? InAliasMapProperty : OutAliasMapProperty);
    }

    public static void ClearIOAliasMap(DependencyObject element, bool isIn)
    {
        element.ClearValue(isIn ? InAliasMapProperty : OutAliasMapProperty);
    }

    public static List<string> GetAliases(DependencyObject element, bool isInp)
    {
        // Release IL keeps the map in a local and returns null before reading Values.
        Dictionary<string, string> aliasMap = GetIOAliasMap(element, isInp);
        if (aliasMap != null)
        {
            return aliasMap.Values.ToList();
        }
        return null;
    }

    private static void SetAliases(DependencyObject element, IList<string> aliases, bool isInp)
    {
        IList<string> sExpressionsList = GetSExpressionsList(element, isInp ? ItemOptions.Inputs : ItemOptions.Outputs);
        if (DebugTools.DevAssert(sExpressionsList.Count == aliases.Count, "number of deserialized input/output aliases different from SExpr count").Passed)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            for (int i = 0; i < sExpressionsList.Count; i++)
            {
                dictionary.Add(sExpressionsList[i], aliases[i]);
            }
            SetIOAliasMap(element, dictionary, isInp);
        }
    }

    [MirrorVerified("PtcControlsCoreIORegionsIOItemsAliasManagerMechanicalDifferentialTests")]
    public static void SetInputAliases(DependencyObject element, List<string> aliases) => SetAliases(element, aliases, true);

    [MirrorVerified("PtcControlsCoreIORegionsIOItemsAliasManagerMechanicalDifferentialTests")]
    public static void SetOutputAliases(DependencyObject element, List<string> aliases) => SetAliases(element, aliases, false);

    [MirrorVerified("PtcControlsCoreIORegionsIOItemsAliasManagerMechanicalDifferentialTests")]
    public bool IsExist(string alias) => _aliasesToItemsMap.ContainsKey(alias);

    public bool Add(string alias, IIOItem item)
    {
        if (!IsExist(alias))
        {
            _aliasesToItemsMap.Add(alias, item);
            return true;
        }
        return false;
    }

    public bool Remove(string alias)
    {
        if (IsExist(alias))
        {
            _aliasesToItemsMap.Remove(alias);
            return true;
        }
        return false;
    }

    public IIOItem GetItem(string alias, bool compareAliasCaseInsensitively)
    {
        if (alias == null)
        {
            return null;
        }
        if (compareAliasCaseInsensitively ? FindSingleCaseInsensitiveAliasMatch(ref alias) : IsExist(alias))
        {
            return _aliasesToItemsMap[alias];
        }
        return null;
    }

    private bool FindSingleCaseInsensitiveAliasMatch(ref string alias)
    {
        string aliasToFind = alias;
        IEnumerable<string> source = _aliasesToItemsMap.Keys.Where(itemAlias => itemAlias.Equals(aliasToFind, StringComparison.OrdinalIgnoreCase));
        if (source.Count() != 1)
        {
            return false;
        }
        alias = source.First();
        return true;
    }
}
