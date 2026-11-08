using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using System.Windows;
using Quire.Calculation.Core;
using Quire.Controls.Core;
using Quire.PersistentDataObjects;
using Quire.Serialization;
using Quire.Core.Serialization;
using Quire;

namespace Quire.Controls.Include;

internal partial class IncludeWorksheetModel : IDisposable, IMultiLevelInclusionItem
{
    private string _includedFilePath;
    private CalculationVariables _parentDocCalculationVariables;
    private CalculationOptions _parentDocCalculationOptions;
    private IList<UIElement> _regions;
    private readonly ManualResetEvent _waitForLoad = new ManualResetEvent(initialState: true);
    private List<string> _parentFileNameList;
    private bool _isParentDataUpdated;
    private string _pathIndexes;
    private bool _isTopLevel;
    private readonly IncludeWorksheetFileManager _fileManager;
    private readonly IncludeWorksheetCalculatableHandler _calculatableHandler;
    private readonly ErrorWarningHandlerModel _errorwarningHandlerModel;
    private const string FakeFreemiumEqText = "freemium = ";
    public Func<bool> IsInFreemiumMode;

    public event Action ContentChanged;

    public IncludeWorksheetModel(ResourceDictionary resources)
    {
        _fileManager = new IncludeWorksheetFileManager();
        _calculatableHandler = new IncludeWorksheetCalculatableHandler(errorCode =>
        {
            FireErrorOnChild(errorCode, BuildFileInfoArgs());
        }, HandleSystemVariableValueChange);
        _calculatableHandler.CalculatableAdded += FireCalculatableAdded;
        _calculatableHandler.CalculatableRemoved += FireCalculatableRemoved;
        _calculatableHandler.SynchronizationStatusChanged += FireSynchronizationStatusChanged;
        _calculatableHandler.CalculatingStatusChanged += FireCalculatingStatusChanged;
        _errorwarningHandlerModel = new ErrorWarningHandlerModel(resources, GetParentDirectoryPath);
        _parentDocCalculationVariables = null;
        _parentDocCalculationOptions = null;
        _regions = new List<UIElement>();
        RegionsLoaded = false;
        LastSavedDate = DateTime.MinValue;
        LastSavedFilePath = null;
        PathIndexes = null;
        IncludedFileCalculationVariables = null;
        IncludedFileCalculationOptions = null;
    }

    private void FireCalculatingStatusChanged()
    {
        if (CalculatingStatusChanged != null)
        {
            CalculatingStatusChanged();
        }
    }

    private void FireSynchronizationStatusChanged()
    {
        if (SynchronizationStatusChanged != null)
        {
            SynchronizationStatusChanged();
        }
    }

    private void FireCalculatableRemoved(ICalculatable calculatable)
    {
        if (CalculatableRemoved != null)
        {
            CalculatableRemoved(calculatable);
        }
    }

    private void FireCalculatableAdded(ICalculatable calculatable)
    {
        if (CalculatableAdded != null)
        {
            CalculatableAdded(calculatable);
        }
    }

    public string IncludedFilePath
    {
        get => _includedFilePath;
        set
        {
            if (!string.IsNullOrEmpty(_includedFilePath) && _includedFilePath.Equals(value))
            {
                return;
            }
            _includedFilePath = value;
            FireContentChanged();
            CacheValid &= IsLastLoadedPath(_includedFilePath);
            RegionsLoaded &= IsLastLoadedPath(_includedFilePath);
        }
    }

    internal DateTime LastSavedDate { get; set; }

    internal string LastSavedFilePath { get; set; }

    public string PathIndexes
    {
        get => _pathIndexes;
        set
        {
            _pathIndexes = value;
            _errorwarningHandlerModel.PathIndexes = _pathIndexes;
            if (_pathIndexes != null)
            {
                FireErrorsUpdated(_pathIndexes, ErrorsList);
                FireWarningsUpdated(_pathIndexes, WarningsList);
            }
        }
    }

    internal bool RegionsLoaded { get; set; }

    private bool _reloadedInFremium;
    private bool _forceAddingIncludedCalculatableWrappers;

    internal bool ReloadedInFreemium
    {
        get => _reloadedInFremium;
        set
        {
            if (_reloadedInFremium == value)
            {
                return;
            }
            _reloadedInFremium = value;
            if (_reloadedInFremium)
            {
                _calculatableHandler.RemoveAllCalculatables();
                if (_isParentDataUpdated)
                {
                    ClearErrors();
                    ClearWarnings();
                    UpdateChildren(elem =>
                    {
                        elem.ClearErrors();
                        elem.ClearWarnings();
                    }, recursely: true, fromTop: true);
                    FireErrorOnChild("premium%_only", BuildFileInfoArgs());
                }
                _calculatableHandler.ReloadedInFreemium = true;
                return;
            }
            _calculatableHandler.ReloadedInFreemium = false;
            if (_isParentDataUpdated)
            {
                ClearErrors();
                ClearWarnings();
                UpdateChildren(elem =>
                {
                    elem.ClearErrors();
                    elem.ClearWarnings();
                }, recursely: true, fromTop: true);
            }
            _calculatableHandler.AddSystemVariableValuesHandlers();
            _forceAddingIncludedCalculatableWrappers = true;
        }
    }

    public bool IsParentDataUpdated
    {
        private get => _isParentDataUpdated;
        set
        {
            if (!value)
            {
                return;
            }
            if (!_isParentDataUpdated)
            {
                _isParentDataUpdated = true;
                PropagateParentData();
                if (CacheCorrupted && IsTopLevel)
                {
                    FireWarningOnChild(new List<string> { "IncludeControlWarningsCorruptedCachedPart" }, BuildFileInfoArgs());
                }
            }
        }
    }

    private event Action OnParentDataSet;

    internal bool IsTopLevel
    {
        get => _isTopLevel;
        set
        {
            if (!_isTopLevel & value)
            {
                _isTopLevel = true;
                if (WarningsList.Count == 1 && WarningsList[0].Code.Equals("IncludeControlWarningsCache"))
                {
                    OriginalMissingCacheUsed = true;
                    UpdateChildren(elem =>
                    {
                        elem.OriginalMissingCacheUsed = true;
                    }, recursely: true, fromTop: true);
                }
            }
        }
    }

    private static bool InDeserialization { get; set; }

    public bool CircularInclusion { get; set; }

    private string GetParentDirectoryPath()
    {
        string text = (ParentFileNames != null) ? ParentFileNames[ParentFileNames.Count - 1] : null;
        if (!string.IsNullOrEmpty(text))
        {
            if (!Path.HasExtension(text))
            {
                return text;
            }
            return Path.GetDirectoryName(text);
        }
        return null;
    }

    private Pair<string, bool> BuildFileInfoArgs()
    {
        string first = IncludedFullFilePath ?? IncludedFilePath;
        bool second = false;
        try
        {
            second = Path.IsPathRooted(IncludedFilePath);
        }
        catch
        {
        }
        return new Pair<string, bool>(first, second);
    }

    private string CreateIncludedFullFilePath(string path)
    {
        string result = null;
        if (Uri.TryCreate(path, UriKind.RelativeOrAbsolute, out Uri result2))
        {
            if (result2.IsAbsoluteUri && result2.IsFile)
            {
                return path;
            }
            try
            {
                bool flag = !string.IsNullOrEmpty(path) && !Path.IsPathRooted(path);
                string parentDirectoryPath = GetParentDirectoryPath();
                if (flag && !string.IsNullOrEmpty(parentDirectoryPath))
                {
                    result = Path.GetFullPath(Path.Combine(parentDirectoryPath, path));
                }
                else
                {
                    result = path;
                }
            }
            catch (Exception)
            {
            }
        }
        return result;
    }

    internal string IncludedFullFilePath => CreateIncludedFullFilePath(IncludedFilePath);

    public void ResetData()
    {
        ClearErrors();
        ClearWarnings();
        UpdateLastSavedDateAndFileName();
        IncludedFileCalculationOptions = null;
        IncludedFileCalculationVariables = null;
        Regions = null;
        _fileManager.Reset();
    }

    internal void ResetDataRecursivly()
    {
        UpdateChildren(elem =>
        {
            elem.ResetData();
        }, recursely: true, fromTop: false);
        ResetData();
    }

    private bool IsEmptyFilePath()
    {
        if (string.IsNullOrEmpty(IncludedFilePath))
        {
            if (!IsLastLoadedPath())
            {
                ResetDataRecursivly();
            }
            RegionsLoaded = true;
            return true;
        }
        return false;
    }

    private bool CausesCircularReference()
    {
        if (ParentFileNames.Count != 0 && !string.IsNullOrEmpty(IncludedFullFilePath) && ParentFileNames.Contains(IncludedFullFilePath))
        {
            CircularInclusion = true;
            RegionsLoaded = true;
            return true;
        }
        return false;
    }

    private bool FileNotExist()
    {
        if (!File.Exists(IncludedFullFilePath))
        {
            OriginalMissingCacheUsed = CacheValid;
            if (!CacheValid)
            {
                LastSavedDate = DateTime.MinValue;
            }
            return true;
        }
        return false;
    }

    private bool ChildIncludeFilesNeedReload(out bool canReloadAllChildren)
    {
        bool flag = false;
        canReloadAllChildren = true;
        foreach (UIElement region in _regions)
        {
            IMultiLevelInclusionProvider multiLevelInclusionProvider = region as IMultiLevelInclusionProvider;
            if (multiLevelInclusionProvider != null)
            {
                flag |= multiLevelInclusionProvider.MultiLevelInclusionItem.CheckIfNeedReloadAndUpdateIncluded(out bool canReload);
                canReloadAllChildren &= canReload;
                OriginalMissingCacheUsed |= multiLevelInclusionProvider.MultiLevelInclusionItem.OriginalMissingCacheUsed;
                CircularInclusion |= multiLevelInclusionProvider.MultiLevelInclusionItem.CircularInclusion;
                CacheValid &= multiLevelInclusionProvider.MultiLevelInclusionItem.CacheValid;
            }
        }
        return flag;
    }

    public event Action<string, ErrorsList> ErrorsUpdated;

    public event Action<string, WarningsList> WarningsUpdated;

    public bool CheckIfNeedReloadAndUpdateIncluded(out bool canReload)
    {
        string text = "IncludeWorksheetModel.CheckIfNeedReloadAndUpdateIncluded:";
        if (HideFromCalculationContext || IsEmptyFilePath())
        {
            if (HideFromCalculationContext)
            {
                text += " HideFromCalculationContext";
            }
            else
            {
                text += " EmptyFilePath";
            }
            DebugTools.LogData(text);
            canReload = true;
            return false;
        }
        if (IsInFreemiumMode != null)
        {
            ReloadedInFreemium = IsInFreemiumMode();
        }
        if (ReloadedInFreemium)
        {
            text += " ReloadedInFreemium";
            DebugTools.LogData(text);
            canReload = true;
            return false;
        }
        if (CausesCircularReference())
        {
            text += " CausesCircularReference";
            DebugTools.LogData(text);
            canReload = false;
            return false;
        }
        if (FileNotExist())
        {
            text = text + " FileNotExist CacheValid:" + CacheValid;
            DebugTools.LogData(text);
            canReload = !CacheValid;
            return !CacheValid;
        }
        try
        {
            if (CacheCorrupted || !IsLastLoadedPath() || !LastSavedDateIsUpToDate())
            {
                CacheValid = false;
                canReload = true;
                return true;
            }
            bool result = ChildIncludeFilesNeedReload(out canReload);
            text = text + " childIncludeFilesNeedReload:" + result;
            DebugTools.LogData(text);
            return result;
        }
        catch (Exception)
        {
            FireErrorOnChild("IncludeControlErrorsFileCannotBeAccessed", BuildFileInfoArgs());
            canReload = false;
            return false;
        }
    }

    internal void Reload()
    {
        DebugTools.DevAssert(_fileManager != null, null);
        try
        {
            _waitForLoad.Reset();
            ResetDataRecursivly();
            OriginalMissingCacheUsed = false;
            CacheCorrupted = false;
            DebugTools.DevAssert(!string.IsNullOrEmpty(IncludedFilePath), null);
            Path.GetDirectoryName(IncludedFullFilePath);
            _fileManager.OpenFile(IncludedFullFilePath, CacheValid);
            IncludedFileCalculationOptions = _fileManager.DocCalculationOptions;
            IncludedFileCalculationVariables = _fileManager.DocCalculationVariables;
            FixIncludedFileCalculationVariableCWD();
            Regions = _fileManager.Regions;
            RegionsLoaded = true;
            FireContentChanged();
        }
        catch (FileNotFoundException)
        {
            FireErrorOnChild("IncludeControlErrorsFileDoesNotExist", BuildFileInfoArgs());
        }
        catch (FormatException)
        {
            FireErrorOnChild("IncludeControlErrorsIncorrectFileFormat", BuildFileInfoArgs());
        }
        catch (IncompatibleFormatWorksheetException)
        {
            FireErrorOnChild("IncludeControlErrorsFileNotCompatible", BuildFileInfoArgs());
        }
        catch (Exception)
        {
            FireErrorOnChild("IncludeControlErrorsFileCannotBeAccessed", BuildFileInfoArgs());
        }
        finally
        {
            _waitForLoad.Set();
        }
        HandleEngineeringDocCalculationSettingChanged(null, null);
    }

    private void FireContentChanged()
    {
        if (ContentChanged != null)
        {
            ContentChanged();
        }
    }

    private void FixIncludedFileCalculationVariableCWD()
    {
        if (IncludedFileCalculationVariables != null)
        {
            string text = null;
            if (ParentFileNames != null && ParentFileNames.Count > 1)
            {
                text = ParentFileNames[ParentFileNames.Count - 1];
            }
            try
            {
                IncludedFileCalculationVariables.CurrentWorkingDirectory = Path.GetDirectoryName(text ?? IncludedFullFilePath);
            }
            catch (Exception ex)
            {
                DebugTools.FailAssertion(ex.Message);
                if (_parentDocCalculationVariables != null)
                {
                    IncludedFileCalculationVariables.CurrentWorkingDirectory = _parentDocCalculationVariables.CurrentWorkingDirectory;
                }
            }
        }
    }

    private void UpdateLastSavedDateAndFileName()
    {
        if (!string.IsNullOrEmpty(IncludedFilePath) && File.Exists(IncludedFullFilePath) && !LastSavedDateIsUpToDate())
        {
            LastSavedDate = GetLastSavedFileDate();
        }
        LastSavedFilePath = IncludedFullFilePath;
    }

    internal void CleanCalculatables()
    {
        _calculatableHandler.CleanIncludedCalculatableWrappers(!_forceAddingIncludedCalculatableWrappers);
    }

    private void UpdateChildren(Action<IMultiLevelInclusionItem> setter)
    {
        UpdateChildren(setter, recursely: false, fromTop: true);
    }

    public void UpdateChildren(Action<IMultiLevelInclusionItem> setter, bool recursely, bool fromTop)
    {
        if (_regions != null)
        {
            foreach (UIElement region in _regions)
            {
                IMultiLevelInclusionProvider multiLevelInclusionProvider = region as IMultiLevelInclusionProvider;
                if (multiLevelInclusionProvider != null)
                {
                    if (recursely && !fromTop)
                    {
                        multiLevelInclusionProvider.MultiLevelInclusionItem.UpdateChildren(setter, recursely: true, fromTop: false);
                    }
                    setter(multiLevelInclusionProvider.MultiLevelInclusionItem);
                    if (recursely & fromTop)
                    {
                        multiLevelInclusionProvider.MultiLevelInclusionItem.UpdateChildren(setter, recursely: true, fromTop: true);
                    }
                }
            }
        }
    }

    public void PropagateParentData()
    {
        if (_regions != null)
        {
            int num = 0;
            foreach (UIElement region in _regions)
            {
                IMultiLevelInclusionProvider multiLevelInclusionProvider = region as IMultiLevelInclusionProvider;
                if (multiLevelInclusionProvider != null)
                {
                    IMultiLevelInclusionItem multiLevelInclusionItem = multiLevelInclusionProvider.MultiLevelInclusionItem;
                    multiLevelInclusionItem.ErrorsUpdated += FireErrorsUpdated;
                    multiLevelInclusionItem.WarningsUpdated += FireWarningsUpdated;
                    multiLevelInclusionItem.CacheValid = CacheValid;
                    if (ParentFileNames != null)
                    {
                        List<string> parentFileNames = new List<string>(ParentFileNames) { IncludedFullFilePath };
                        multiLevelInclusionItem.ParentFileNames = parentFileNames;
                    }
                    if (PathIndexes != null)
                    {
                        multiLevelInclusionItem.PathIndexes = string.Format("{0}_{1}", PathIndexes, num);
                    }
                    multiLevelInclusionItem.UpdateEngineeringDocCalculationOptions(_parentDocCalculationOptions);
                    multiLevelInclusionItem.UpdateEngineeringDocCalculationVariables(_parentDocCalculationVariables);
                    multiLevelInclusionItem.IsParentDataUpdated = true;
                    num++;
                }
            }
        }
        if (OnParentDataSet != null)
        {
            OnParentDataSet();
        }
    }

    public void RemoveParentData()
    {
        if (_regions != null)
        {
            foreach (UIElement region in _regions)
            {
                IMultiLevelInclusionProvider multiLevelInclusionProvider = region as IMultiLevelInclusionProvider;
                if (multiLevelInclusionProvider != null)
                {
                    IMultiLevelInclusionItem multiLevelInclusionItem = multiLevelInclusionProvider.MultiLevelInclusionItem;
                    multiLevelInclusionItem.UpdateEngineeringDocCalculationOptions(null);
                    multiLevelInclusionItem.UpdateEngineeringDocCalculationVariables(null);
                    multiLevelInclusionItem.ErrorsUpdated -= FireErrorsUpdated;
                    multiLevelInclusionItem.WarningsUpdated -= FireWarningsUpdated;
                }
            }
        }
    }

    internal IList<UIElement> Regions
    {
        get => _regions;
        set
        {
            DebugTools.LogData("IncludeWorksheetModel.Regions: " + ((_regions == null) ? (-1) : _regions.Count) + "=>" + ((value == null) ? (-1) : value.Count));
            if (_regions != null)
            {
                CleanCalculatables();
                RemoveParentData();
                _regions.Clear();
            }
            if (value != null)
            {
                _regions = value;
                if (IsParentDataUpdated)
                {
                    PropagateParentData();
                    ReloadChildren();
                }
                _calculatableHandler.CreateAndAddIncludedCalculatableWrappers(_regions);
            }
        }
    }

    private void ReloadChildren()
    {
        if (_regions != null)
        {
            foreach (UIElement region in _regions)
            {
                IMultiLevelInclusionProvider multiLevelInclusionProvider = region as IMultiLevelInclusionProvider;
                if (multiLevelInclusionProvider != null)
                {
                    IMultiLevelInclusionItem multiLevelInclusionItem = multiLevelInclusionProvider.MultiLevelInclusionItem;
                    if (!InDeserialization)
                    {
                        multiLevelInclusionItem.ReloadIfNeeded();
                        CacheCorrupted |= multiLevelInclusionItem.CacheCorrupted;
                        if (multiLevelInclusionItem.CircularInclusion)
                        {
                            CircularInclusion |= multiLevelInclusionItem.CircularInclusion;
                            break;
                        }
                    }
                }
            }
        }
    }

    internal void Commit()
    {
        ReloadIfNeeded();
        _calculatableHandler.Commit();
    }

    public void ReloadIfNeeded()
    {
        DebugTools.LogData("IncludeWorksheetModel.ReloadIfNeeded: IsParentDataUpdated:" + IsParentDataUpdated);
        if (!IsParentDataUpdated)
        {
            OnParentDataSet += ReloadIfNeeded;
            return;
        }
        OnParentDataSet -= ReloadIfNeeded;
        if (CheckIfNeedReloadAndUpdateIncluded(out bool canReload) & canReload)
        {
            Reload();
        }
        else
        {
            if (_forceAddingIncludedCalculatableWrappers)
            {
                _calculatableHandler.AddIncludedCalculatableWrappers();
            }
            if (!canReload && CircularInclusion)
            {
                FireErrorOnChild("IncludeControlErrorsRecursive", BuildFileInfoArgs());
            }
            else if (!canReload && CacheValid)
            {
                UpdateChildren(elem =>
                {
                    elem.OriginalMissingCacheUsed = true;
                    elem.ClearWarnings();
                }, recursely: true, fromTop: true);
                FireWarningOnChild(new List<string> { "IncludeControlWarningsCache" }, BuildFileInfoArgs());
            }
        }
        _forceAddingIncludedCalculatableWrappers = false;
    }

    private bool IsLastLoadedPath() => LastSavedFilePath == IncludedFullFilePath;

    internal bool IsLastLoadedPath(string path) => LastSavedFilePath == CreateIncludedFullFilePath(path);

    private bool LastSavedDateIsUpToDate()
    {
        DateTime lastSavedFileDate = GetLastSavedFileDate();
        DebugTools.LogData("IncludeWorksheetModel.LastSavedDateIsUpToDate: fileDate: " + lastSavedFileDate.ToString("dd MMM yyyy, HH:mm:ss.ff") + " LastSavedDate: " + LastSavedDate.ToString("dd MMM yyyy, HH:mm:ss.ff") + " now: " + DateTime.Now.ToString("dd MMM yyyy, HH:mm:ss.ff"));
        return LastSavedDate == lastSavedFileDate;
    }

    private DateTime GetLastSavedFileDate()
    {
        DateTime lastWriteTime = File.GetLastWriteTime(IncludedFullFilePath);
        return new DateTime(lastWriteTime.Year, lastWriteTime.Month, lastWriteTime.Day, lastWriteTime.Hour, lastWriteTime.Minute, lastWriteTime.Second);
    }

    internal CalculationVariables IncludedFileCalculationVariables { get; set; }

    internal CalculationOptions IncludedFileCalculationOptions { get; set; }

    public void UpdateEngineeringDocCalculationVariables(CalculationVariables newVariables)
    {
        if (ParentCalculationVariables != null)
        {
            ParentCalculationVariables.CalculationVariableChanged -= HandleEngineeringDocCalculationSettingChanged;
        }
        ParentCalculationVariables = newVariables;
        if (ParentCalculationVariables != null)
        {
            ParentCalculationVariables.CalculationVariableChanged += HandleEngineeringDocCalculationSettingChanged;
        }
        else
        {
            FireResetWarnings();
        }
    }

    public void UpdateEngineeringDocCalculationOptions(CalculationOptions newOptions)
    {
        if (ParentCalculationOptions != null)
        {
            ParentCalculationOptions.CalculationOptionChanged -= HandleEngineeringDocCalculationSettingChanged;
        }
        ParentCalculationOptions = newOptions;
        if (ParentCalculationOptions != null)
        {
            ParentCalculationOptions.CalculationOptionChanged += HandleEngineeringDocCalculationSettingChanged;
        }
        else
        {
            FireResetWarnings();
        }
    }

    private bool ShouldShowCalculationSettingsWarnings()
    {
        if (_calculatableHandler.CanManipulateCalculatable && !OriginalMissingCacheUsed && !CacheCorrupted && _parentDocCalculationOptions != null)
        {
            return _parentDocCalculationOptions.IsAutoCalcOn;
        }
        return false;
    }

    internal void HandleEngineeringDocCalculationSettingChanged(string name, object value)
    {
        if (ShouldShowCalculationSettingsWarnings())
        {
            FireResetWarnings();
            FireWarningOnChild(GetCalculationSettingsDiffsCodes(), BuildFileInfoArgs());
        }
    }

    private List<string> GetCalculationSettingsDiffsCodes()
    {
        List<string> list = new List<string>();
        CalculationVariables includedFileCalculationVariables = IncludedFileCalculationVariables;
        CalculationOptions includedFileCalculationOptions = IncludedFileCalculationOptions;
        if (includedFileCalculationVariables != null && _parentDocCalculationVariables != null)
        {
            if (_parentDocCalculationVariables.IndexOrigin != includedFileCalculationVariables.IndexOrigin)
            {
                list.Add("IncludeControlWarningsConflictPropertyNameORIGIN");
            }
            if (_parentDocCalculationVariables.Tol != includedFileCalculationVariables.Tol)
            {
                list.Add("IncludeControlWarningsConflictPropertyNameTOL");
            }
            if (_parentDocCalculationVariables.CTol != includedFileCalculationVariables.CTol)
            {
                list.Add("IncludeControlWarningsConflictPropertyNameCTOL");
            }
            if (CalculationVariables.EscapeDirectory(_parentDocCalculationVariables.CurrentWorkingDirectory) != CalculationVariables.EscapeDirectory(includedFileCalculationVariables.CurrentWorkingDirectory))
            {
                list.Add("IncludeControlWarningsConflictPropertyNameCWD");
            }
        }
        if (includedFileCalculationOptions != null && _parentDocCalculationOptions != null)
        {
            if (_parentDocCalculationOptions.StrictComparisions != includedFileCalculationOptions.StrictComparisions)
            {
                list.Add("IncludeControlWarningsConflictPropertyNameApproximateEquality");
            }
            if (_parentDocCalculationOptions.StringsIndexedFromOrigin != includedFileCalculationOptions.StringsIndexedFromOrigin)
            {
                list.Add("IncludeControlWarningsConflictPropertyNameOriginInStrings");
            }
            if (_parentDocCalculationOptions.StrictSingularityChecking != includedFileCalculationOptions.StrictSingularityChecking)
            {
                list.Add("IncludeControlWarningsConflictPropertyNameStrictSingularityCheck");
            }
            if (_parentDocCalculationOptions.UnitsAndConstantsInSymbolics != includedFileCalculationOptions.UnitsAndConstantsInSymbolics)
            {
                list.Add("IncludeControlWarningsConflictPropertyNameUnitsAndConstantsInSymbolics");
            }
        }
        return list;
    }

    private bool _cacheValid;

    public bool CacheValid
    {
        get => _cacheValid;
        set
        {
            if (_cacheValid != value)
            {
                _cacheValid = value;
                UpdateChildren(elem =>
                {
                    elem.CacheValid = _cacheValid;
                });
            }
        }
    }

    public bool OriginalMissingCacheUsed { get; set; }

    public bool CacheCorrupted { get; set; }

    private EquationControl CreateFakeEquationControl()
    {
        return EquationControl.CreateWithTextInput("freemium = ");
    }

    private IList<UIElement> RegionsToSerialize
    {
        get
        {
            IList<UIElement> list = Regions;
            if (ReloadedInFreemium)
            {
                list = ((Regions != null) ? new List<UIElement>(Regions) : new List<UIElement>());
                if (Regions == null)
                {
                    list.Add(CreateFakeEquationControl());
                }
                else
                {
                    list.Insert(0, CreateFakeEquationControl());
                }
            }
            return list;
        }
    }

    private IList<UIElement> FindAndExcludeFakeRegion(IList<UIElement> regions)
    {
        if (regions != null && regions.Count > 0)
        {
            EquationControl equationControl = regions[0] as EquationControl;
            if (equationControl != null && CreateFakeEquationControl().EquationSExpression == equationControl.EquationSExpression)
            {
                regions.Remove(equationControl);
                ReloadedInFreemium = true;
            }
        }
        return regions;
    }

    internal void ReadCachedPart(PackagePart part)
    {
        DebugTools.DevAssert(_fileManager != null, null);
        _errorwarningHandlerModel.ClearErrors();
        try
        {
            _waitForLoad.Reset();
            if (!string.IsNullOrEmpty(IncludedFilePath))
            {
                DebugTools.DevAssert(Regions == null || Regions.Count == 0, null);
                UpdateLastSavedDateAndFileName();
                CacheCorrupted = part == null;
                if (CacheCorrupted)
                {
                    return;
                }
                _fileManager.ReadIncludedWorksheetFromPackage(part);
                CacheValid = true;
                IncludedFileCalculationOptions = _fileManager.DocCalculationOptions;
                IncludedFileCalculationVariables = _fileManager.DocCalculationVariables;
                FixIncludedFileCalculationVariableCWD();
                RegionsLoaded = true;
                Regions = FindAndExcludeFakeRegion(_fileManager.Regions);
            }
        }
        catch
        {
            CacheCorrupted = true;
        }
        finally
        {
            _waitForLoad.Set();
        }
    }

    internal PackagePart WriteToPackage(Package package, string uniqueId)
    {
        _fileManager.Regions = RegionsToSerialize;
        _fileManager.DocCalculationOptions = IncludedFileCalculationOptions;
        _fileManager.DocCalculationVariables = IncludedFileCalculationVariables;
        _fileManager.PathIndexes = uniqueId;
        return _fileManager.WriteIncludedWorksheetToPackage(package);
    }

    public bool CacheCorruptedForAnyChildren()
    {
        return _regions.OfType<IMultiLevelInclusionProvider>().Aggregate(CacheCorrupted, (current, provider) => current | provider.MultiLevelInclusionItem.CacheCorruptedForAnyChildren());
    }

    internal void WaitForLoad(int seconds)
    {
        FileManager.WaitFor(seconds, _waitForLoad);
    }

    public List<string> ParentFileNames
    {
        get => _parentFileNameList;
        set
        {
            bool flag = _parentFileNameList != null && _parentFileNameList.Count > 0;
            _parentFileNameList = value;
            if (_parentFileNameList != null && _parentFileNameList.Exists(elem => !string.IsNullOrEmpty(elem)))
            {
                if (!flag || IsLastLoadedPath())
                {
                    LastSavedFilePath = IncludedFullFilePath;
                }
                List<string> parentFileNamesWithMe = new List<string>(_parentFileNameList) { IncludedFullFilePath };
                UpdateChildren(elem =>
                {
                    elem.ParentFileNames = parentFileNamesWithMe;
                });
                FixIncludedFileCalculationVariableCWD();
            }
        }
    }

    internal CalculationOptions ParentCalculationOptions
    {
        get => _parentDocCalculationOptions;
        set
        {
            UpdateChildren(elem =>
            {
                elem.UpdateEngineeringDocCalculationOptions(value);
            });
            _parentDocCalculationOptions = value;
        }
    }

    internal CalculationVariables ParentCalculationVariables
    {
        get => _parentDocCalculationVariables;
        set
        {
            UpdateChildren(elem =>
            {
                elem.UpdateEngineeringDocCalculationVariables(value);
            });
            _parentDocCalculationVariables = value;
        }
    }

    private void UpdateParentFileNameByCWDIfNeed(string CWD)
    {
        if (ParentFileNames.Count == 1 && !Path.HasExtension(ParentFileNames[0]))
        {
            ParentFileNames = new List<string> { CWD };
        }
    }

    internal IRegionFactory RegionFactory
    {
        get => _fileManager.RegionFactory;
        set => _fileManager.RegionFactory = value;
    }

    public void DeserializeEpilog(bool isConvertedFromMathcad14)
    {
        if (CacheValid && CacheCorruptedForAnyChildren())
        {
            CacheValid = false;
            CacheCorrupted = true;
        }
    }

    public IList<IRegionPersistentData> SetRegionPersistentData(object data, Func<string, PackagePart> onGetPackagePartByRefId)
    {
        includeWorksheetType includeWorksheetType = data as includeWorksheetType;
        if (includeWorksheetType == null)
        {
            throw new ArgumentException("data should be of includeWorksheetType");
        }
        try
        {
            string href = includeWorksheetType.href;
            DateTime lastSavedDate = DateTime.Parse(includeWorksheetType.lastsaveddate, CultureInfo.InvariantCulture);
            bool cacheValid = CacheValid;
            IncludedFilePath = href;
            if (!string.IsNullOrEmpty(href))
            {
                CacheValid = cacheValid;
                if (CacheValid)
                {
                    InDeserialization = true;
                    bool usedcache = includeWorksheetType.usedcache;
                    if (!string.IsNullOrEmpty(includeWorksheetType.itemidref))
                    {
                        PackagePart part = null;
                        if (onGetPackagePartByRefId != null)
                        {
                            part = onGetPackagePartByRefId(includeWorksheetType.itemidref);
                        }
                        ReadCachedPart(part);
                    }
                    else
                    {
                        CacheCorrupted = usedcache;
                    }
                    RegionsLoaded = true;
                    LastSavedDate = lastSavedDate;
                    if (includeWorksheetType.error != null)
                    {
                        ErrorsList errorsList = new ErrorsList();
                        errorsList.AddRange(_errorwarningHandlerModel.SetIncludeWorksheetErrorWarningData(includeWorksheetType.error).Cast<IncludeWorksheetErrorWarningData>().Select(error => new ErrorOnIncludeWorksheet
                        {
                            Code = error.Code,
                            FileName = error.FileName,
                            IsPathRooted = true,
                            PathIndexes = error.PathIndexes
                        }));
                        _errorwarningHandlerModel.ErrorsList = errorsList;
                    }
                    if (includeWorksheetType.warning != null)
                    {
                        WarningsList warningsList = new WarningsList();
                        warningsList.AddRange(_errorwarningHandlerModel.SetIncludeWorksheetErrorWarningData(includeWorksheetType.warning).Cast<IncludeWorksheetErrorWarningData>().Select(warning =>
                        {
                            string text = ErrorWarningHandlerModel.CleanUpLegacyWarnings(warning.Code);
                            if (!text.IsNullOrEmpty())
                            {
                                return new WarningOnIncludeWorksheet
                                {
                                    Code = text,
                                    FileName = warning.FileName,
                                    IsPathRooted = true,
                                    PathIndexes = warning.PathIndexes
                                };
                            }
                            return null;
                        }).Where(warningOnInclude => warningOnInclude != null));
                        _errorwarningHandlerModel.WarningsList = warningsList;
                    }
                    InDeserialization = false;
                }
            }
        }
        catch (Exception innerException)
        {
            throw new SerializationException("include control deserialization failed", innerException);
        }
        return null;
    }

    public object GetRegionPersistentData(bool useCache, IS11NProvider s11NProvider, string uniqueId)
    {
        includeWorksheetType includeWorksheetType = new includeWorksheetType
        {
            href = IncludedFilePath ?? string.Empty,
            lastsaveddate = LastSavedDate.ToString(CultureInfo.InvariantCulture),
            error = _errorwarningHandlerModel.GetIncludeWorksheetErrorWarningData(ErrorsList),
            warning = _errorwarningHandlerModel.GetIncludeWorksheetErrorWarningData(WarningsList),
            disablecalc = HideFromCalculationContext
        };
        bool flag = !string.IsNullOrEmpty(IncludedFilePath) && File.Exists(IncludedFullFilePath) && (Regions == null || Regions.Count == 0);
        if (flag)
        {
            includeWorksheetType.usedcache = false;
        }
        else if (useCache && !string.IsNullOrEmpty(IncludedFilePath) && File.Exists(IncludedFullFilePath))
        {
            CacheValid = true;
        }
        else if (!useCache)
        {
            CacheValid = false;
        }
        string text = null;
        if (RegionsToSerialize != null && RegionsToSerialize.Count > 0)
        {
            text = s11NProvider.PackIncludedWorksheet(IncludedFullFilePath, LastSavedDate, uniqueId, WriteToPackage);
        }
        if (text != null)
        {
            includeWorksheetType.itemidref = text;
        }
        return includeWorksheetType;
    }

    internal void SetData(BackupData value)
    {
        InDeserialization = true;
        RegionsLoaded = value.regionsLoaded;
        LastSavedDate = value.lastSavedDate;
        LastSavedFilePath = value.lastSavedFilePath;
        CacheValid = value.cacheValid;
        OriginalMissingCacheUsed = value.originalMissingCacheUsed;
        CacheCorrupted = value.cacheCorrupted;
        CircularInclusion = value.circularInclusion;
        IncludedFileCalculationOptions = value.includedFileCalculationOptions;
        IncludedFileCalculationVariables = value.includedFileCalculationVariables;
        if (value.regions != null)
        {
            Regions = new RegionsList(value.regions);
        }
        if (value.errorsList != null)
        {
            ErrorsList = new ErrorsList(value.errorsList);
        }
        if (value.warningsList != null)
        {
            WarningsList = new WarningsList(value.warningsList);
        }
        InDeserialization = false;
    }

    internal BackupData GetData(bool isInPrinting)
    {
        return new BackupData
        {
            regions = (isInPrinting ? null : new RegionsList(Regions)),
            regionsLoaded = RegionsLoaded,
            lastSavedDate = LastSavedDate,
            lastSavedFilePath = LastSavedFilePath,
            cacheValid = CacheValid,
            originalMissingCacheUsed = OriginalMissingCacheUsed,
            circularInclusion = CircularInclusion,
            cacheCorrupted = CacheCorrupted,
            errorsList = (isInPrinting ? null : new ErrorsList(ErrorsList)),
            warningsList = (isInPrinting ? null : new WarningsList(WarningsList)),
            includedFileCalculationOptions = IncludedFileCalculationOptions,
            includedFileCalculationVariables = IncludedFileCalculationVariables
        };
    }

    private void FireResetWarnings()
    {
        if (!OriginalMissingCacheUsed)
        {
            FireWarningOnChild(new List<string> { IncludeWorksheetErrorWarningData.ResetCode }, BuildFileInfoArgs());
        }
    }

    private void FireWarningOnChild(List<string> list, Pair<string, bool> fileName)
    {
        _errorwarningHandlerModel.UpdateWarnings(PathIndexes, list, fileName);
        FireWarningsUpdated(PathIndexes, WarningsList);
    }

    internal void FireErrorOnChild(string errorCode, Pair<string, bool> fileName)
    {
        _errorwarningHandlerModel.UpdateErrors(PathIndexes, errorCode, fileName);
        FireErrorsUpdated(PathIndexes, ErrorsList);
    }

    internal ErrorsList ErrorsList
    {
        get => _errorwarningHandlerModel.ErrorsList;
        set
        {
            _errorwarningHandlerModel.ErrorsList = value;
            FireErrorsUpdated(PathIndexes, _errorwarningHandlerModel.ErrorsList);
        }
    }

    internal WarningsList WarningsList
    {
        get => _errorwarningHandlerModel.WarningsList;
        set
        {
            _errorwarningHandlerModel.WarningsList = value;
            FireWarningsUpdated(PathIndexes, _errorwarningHandlerModel.WarningsList);
        }
    }

    private void FireErrorsUpdated(string path, ErrorsList arg)
    {
        if (path == null)
        {
            return;
        }
        if (DebugTools.DevAssert(ErrorsUpdated != null, null).Passed)
        {
            ErrorsUpdated(path, arg);
        }
    }

    private void FireWarningsUpdated(string path, WarningsList arg)
    {
        if (path == null)
        {
            return;
        }
        if (DebugTools.DevAssert(WarningsUpdated != null, null).Passed)
        {
            WarningsUpdated(path, arg);
        }
    }

    public void ClearWarnings()
    {
        _errorwarningHandlerModel.ClearWarnings();
        FireWarningsUpdated(PathIndexes, WarningsList);
    }

    public void ClearErrors()
    {
        _errorwarningHandlerModel.ClearErrors();
        FireErrorsUpdated(PathIndexes, ErrorsList);
    }

    private void HandleSystemVariableValueChange(string name, object value)
    {
        switch (name)
        {
            case "mc_TOL":
                _parentDocCalculationVariables.Tol = (double)value;
                break;
            case "mc_CTOL":
                _parentDocCalculationVariables.CTol = (double)value;
                break;
            case "mc_ORIGIN":
                _parentDocCalculationVariables.IndexOrigin = (int)(double)value;
                break;
            case "mc_CWD":
                _parentDocCalculationVariables.CurrentWorkingDirectory = (string)value;
                UpdateParentFileNameByCWDIfNeed((string)value);
                break;
        }
        HandleEngineeringDocCalculationSettingChanged(name, value);
    }

    public void HookupSystemVariables()
    {
        UpdateEngineeringDocCalculationVariables(new CalculationVariables());
        _calculatableHandler.HookupSystemVariables();
    }

    public void UnHookSystemVariables()
    {
        _calculatableHandler.UnHookSystemVariables();
        UpdateEngineeringDocCalculationVariables(null);
    }

    public event Action<ICalculatable> CalculatableAdded;

    public event Action<ICalculatable> CalculatableRemoved;

    public event Action CalculatingStatusChanged;

    public event Action SynchronizationStatusChanged;

    public IEnumerable<ICalculatable> CalculatableChildren => _calculatableHandler.CalculatableChildren;

    public ICalculatableInfo CurrentErrorInfo => _calculatableHandler.CurrentErrorInfo;

    public bool ContainsError(EngineError error) => _calculatableHandler.ContainsError(error);

    internal int CountEnabledRegions
    {
        get
        {
            if (HideFromCalculationContext || _regions == null)
            {
                return 0;
            }
            return _regions.Count;
        }
    }

    internal bool HideFromCalculationContext
    {
        get => _calculatableHandler.HideFromCalculationContext;
        set => _calculatableHandler.HideFromCalculationContext = value;
    }

    internal SynchronizationStatus SynchronizationStatus => _calculatableHandler.SynchronizationStatus;

    internal CalculatingStatus CalculatingStatus => _calculatableHandler.CalculatingStatus;

    public void Dispose()
    {
        if (Regions != null)
        {
            foreach (UIElement region in Regions)
            {
                IMultiLevelInclusionProvider multiLevelInclusionProvider = region as IMultiLevelInclusionProvider;
                if (multiLevelInclusionProvider != null)
                {
                    IMultiLevelInclusionItem multiLevelInclusionItem = multiLevelInclusionProvider.MultiLevelInclusionItem;
                    multiLevelInclusionItem.ErrorsUpdated -= FireErrorsUpdated;
                    multiLevelInclusionItem.WarningsUpdated -= FireWarningsUpdated;
                }
                IDisposable disposable = region as IDisposable;
                if (disposable != null)
                {
                    disposable.Dispose();
                }
            }
        }
        _calculatableHandler.CalculatableAdded -= CalculatableAdded;
        _calculatableHandler.CalculatableRemoved -= CalculatableRemoved;
        _calculatableHandler.SynchronizationStatusChanged -= SynchronizationStatusChanged;
        _calculatableHandler.CalculatingStatusChanged -= CalculatingStatusChanged;
    }
}
