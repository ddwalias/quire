#nullable disable
#pragma warning disable
using System;
using System.Collections.Generic;

namespace Quire.Controls.Include;

public partial interface IMultiLevelInclusionItem
{
    bool CircularInclusion { get; set; }
    bool CacheCorrupted { get; }
    bool CacheValid { get; set; }
    bool OriginalMissingCacheUsed { get; set; }
    List<string> ParentFileNames { set; }
    string PathIndexes { set; }
    bool IsParentDataUpdated { set; }
    event Action<string, ErrorsList> ErrorsUpdated;
    event Action<string, WarningsList> WarningsUpdated;
    bool CheckIfNeedReloadAndUpdateIncluded(out bool canReload);
    void ReloadIfNeeded();
    void UpdateEngineeringDocCalculationOptions(global::Quire.Controls.CalculationOptions newOptions);
    void UpdateEngineeringDocCalculationVariables(global::Quire.Controls.CalculationVariables newVariables);
    bool CacheCorruptedForAnyChildren();
    void PropagateParentData();
    void RemoveParentData();
    void UpdateChildren(Action<IMultiLevelInclusionItem> action, bool recursely, bool fromTop);
    void ClearWarnings();
    void ClearErrors();
    void ResetData();
}
