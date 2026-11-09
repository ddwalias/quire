#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Quire.Controls.Core.IORegions;
using Quire.Core;
using Quire.EquationEditor.Tree;
using Quire.Mirror;

namespace Quire.Controls.IO;

internal partial class IOItemCollection
{
    private readonly bool _isInp;

    [MirrorVerified("PtcControlsIOIOItemCollectionMechanicalDifferentialTests")]
    private readonly IOItemsAliasManager _aliasManager;

    [MirrorVerified("PtcControlsIOIOItemCollectionMechanicalDifferentialTests")]
    public event Action AliasChanged;

    private void FireAliasChanged()
    {
        if (AliasChanged != null)
        {
            AliasChanged.Invoke();
        }
    }

    internal IOItemCollection(bool isInp)
    {
        _isInp = isInp;
        _aliasManager = new IOItemsAliasManager();
    }

    [MirrorVerified("PtcControlsIOIOItemCollectionMechanicalDifferentialTests")]
    public IIOItem GetItem(string alias, bool compareAliasCaseInsensitively) => _aliasManager.GetItem(alias, compareAliasCaseInsensitively);

    public void ResetAlias(IIOItem item, string oldAlias, string newAlias)
    {
        _aliasManager.Remove(oldAlias);
        _aliasManager.Add(newAlias, item);
        FireAliasChanged();
    }

    [MirrorVerified("PtcControlsIOIOItemCollectionMechanicalDifferentialTests")]
    internal void HandleTaggedRegionAdded(DependencyObject region) => CreateIOItems(region);

    internal void HandleTaggedRegionRemoved(DependencyObject region)
    {
        CleanAliasesOfRegion(region, false);
        RemoveIOItems(region);
    }

    internal void HandleTaggedRegionChanged(DependencyObject region)
    {
        if (RegionSExpresionListChanged(region))
        {
            DebugTools.FailAssertion((string)null);
            CleanAliasesOfRegion(region, false);
            RemoveIOItems(region);
            CreateIOItems(region);
        }
    }

    private string CreateDefaultAlias(string name)
    {
        string alias = name;
        int i = 0;
        while (_aliasManager.IsExist(alias))
        {
            alias = string.Format("{0}_{1}", name, i);
            i++;
        }
        return alias;
    }

    private bool IsValidItem(DependencyObject region)
    {
        if (_isInp)
        {
            return IOTag.HasInputTag(region);
        }
        return IOTag.HasOutputTag(region);
    }

    private void CleanAliasesOfRegion(DependencyObject region, bool removeMap) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private IEnumerable<IOItem> CreateIOItemsAndResetAliases(DependencyObject region, IEnumerable<string> idSExpr, bool isInp)
    {
        List<IOItem> items = new List<IOItem>();
        Dictionary<string, string> aliasMap = IOItemsAliasManager.GetIOAliasMap(region, isInp);
        Dictionary<string, string> newAliasMap = new Dictionary<string, string>();
        foreach (string sExpr in idSExpr)
        {
            if (newAliasMap.ContainsKey(sExpr))
            {
                continue;
            }
            IOItem item = new IOItem(region, sExpr, isInp);
            items.Add(item);
            string idName = SExpressionHelperUtilites.ExtractIdOrFunctionDefinitionName(sExpr);
            string alias = aliasMap != null && aliasMap.ContainsKey(sExpr) ? aliasMap[sExpr] : null;
            string newAlias = alias != null && !_aliasManager.IsExist(alias) ? alias : CreateDefaultAlias(idName);
            _aliasManager.Add(newAlias, item);
            newAliasMap.Add(sExpr, newAlias);
        }
        IOItemsAliasManager.SetIOAliasMap(region, newAliasMap, isInp);
        return items;
    }

    private IEnumerable<IIOItem> CreateRegionIOItems(DependencyObject region)
    {
        if (IsValidItem(region))
        {
            return CreateIOItemsAndResetAliases(region, IOItemsAliasManager.GetSExpressionsList(region, _isInp ? ItemOptions.Inputs : ItemOptions.Outputs), _isInp);
        }
        return Enumerable.Empty<IOItem>();
    }

    public IEnumerable<IIOItem> GetRegionIOItems(DependencyObject region)
    {
        return this.Where(item => item.Region.Equals(region));
    }

    private bool RegionSExpresionListChanged(DependencyObject region)
    {
        IList<string> newList = IOItemsAliasManager.GetSExpressionsList(region, _isInp ? ItemOptions.Inputs : ItemOptions.Outputs);
        IList<string> existing = GetSExpresionsListOfExistingIOItems(region);
        if (newList.Count == existing.Count)
        {
            return newList.Except(existing).Any();
        }
        return true;
    }

    private IList<string> GetSExpresionsListOfExistingIOItems(DependencyObject region)
    {
        return GetRegionIOItems(region).Select(item => item.IdSexp).ToList();
    }

    private void CreateIOItems(DependencyObject region) => AddRange(CreateRegionIOItems(region));

    private void RemoveIOItems(DependencyObject region) => RemoveRange(GetRegionIOItems(region));
}
