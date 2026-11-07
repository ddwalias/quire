#nullable disable
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Xml.Linq;
using Quire.Calculation.Core;
using Quire.Controls.Core.IORegions;
using Quire.Controls.Core.Serialization;
using Quire.Core;
using Quire.IO;
using Quire.PersistentData;
using Quire.Serialization;

namespace Quire.Controls.Core;

public partial interface IEngineeringDocument
{
    DateTime? LastSavedDate { get; }
    DateTime? CustomizedSavedDate { get; set; }
    DateTime? DisplaySavedDate { get; }
    IPackageOperationsProvider PackageOpsProvider { get; set; }
    ISerializationHelper DocumentSerializationHelper { get; }
    string FilePath { get; set; }
    string CustomizedFilePath { get; set; }
    string DisplayFilePath { get; }
    string FileName { get; }
    string CustomizedFileName { get; }
    string DisplayFileName { get; }
    string DocumentName { get; set; }
    string CustomizedDocumentName { get; set; }
    string DisplayDocumentName { get; }
    string CWD { get; set; }
    string CustomizedWindowTitle { get; set; }
    string DisplayWindowTitle { get; }
    string DocumentTabIcon { get; set; }
    DocumentReadonlyOptions ReadonlyOptions { get; }
    string CustomizedClosePrompt { get; set; }
    CalculationVariables DocCalculationVariables { get; }
    CalculationOptions DocCalculationOptions { get; }
    NumberFormat MathDisplayNumberFormat { get; set; }
    MatrixFormat MathResultMatrixFormat { get; set; }
    IRegionFactory RegionFactory { get; }
    IDictionary<IdLabels, ExtendedFont> LabeledIdFormat { get; set; }
    MathFormat MathFormat { get; }
    bool CacheWorksheet { get; set; }
    IWorksheetPersistentData WorksheetData { get; }
    bool IsHeaderFocused { get; }
    bool IsFooterFocused { get; }
    bool IsMainActive { get; }
    Point InsertionPoint { get; }
    int PageCount { get; }
    int SelectedRegionCount { get; }
    UnitSystems DefaultUnitSystem { get; set; }
    List<UnitDescriptor> CustomBaseUnitDescriptors { get; set; }
    List<UnitDescriptor> CustomDerivedUnitDescriptors { get; set; }
    bool SimplifyUnitSystem { get; set; }
    bool IsCalculationsEnabled { get; set; }
    bool IsAutoCalcOn { get; set; }
    bool IsDirty { get; set; }
    bool IsNetworkDriveDisconnected { get; }
    IFileOperationsProvider FileOpsProvider { get; set; }
    IMetadataModel MetadataModel { get; set; }
    double Zoom { get; set; }
    bool IsPrinting { get; }
    bool IsNew { get; }
    bool CanBeSavedWithOriginalName { get; }
    IIORegionManager InputOutputRegionManager { get; }
    bool IsDocumentFromPTCPlaces { get; set; }
    IIOItemCollection Inputs { get; }
    IIOItemCollection Outputs { get; }
    bool IsDocumentATemplate { get; set; }
    IWorksheetProperties Worksheet { get; }
    bool NeedsDictionaryRefresh { get; set; }
    string HelpServerPath { get; set; }
    bool ShowEngineChangesBanner { get; set; }
    long MaxUpdateLayoutTimeMsec { get; set; }
    event Action<IEngineeringDocument> IsDirtyChanged;
    event Action<IEngineeringDocument> FilePathChanged;
    event Action<string> CwdUpdated;
    void ResetDocumentCustomization();
    XElementSerializable GetMcdxPageModel(XName name);
    TextFormat GetContentTextFormat();
    TextFormat GetHeaderTextFormat();
    TextFormat GetFooterTextFormat();
    void ActivateHeader();
    void ActivateFooter();
    void ActivateMain();
    void WaitForSave(int seconds);
    void WaitForLoad(int seconds);
    Exception SaveFileAndUpdateCWDState(string fileName, IDocumentSaveOptions options, out bool wasRenamed);
    FileLoadResult OpenFileAndUpdateCWDState(string fileName);
    bool Find(string text, LogicalDirection direction, bool startFromCaret, FindOptions options, List<Pair<int, int>> subscriptPositions);
    void FireWaitForCalculationIdle();
    void FireWaitForCalculationIdle(int seconds, DispatcherPriority priority);
    void Recalculate();
    Exception PreSave(IDocumentSaveOptions saveOptions);
    string GetEngineVersion();
    FileLoadResult NewFromDocument(string fileName);
    void SetAutoCalculationEnabledWithoutTriggeringIsDirty(bool value);
    CopyWidgetBase CreateCopyWidget();
    FrameworkElement PrepareRegionCloneForImage(IWhiteboardItem item, RegionImageSettings imageSettings, CopyWidgetBase popup, ResourceDictionary resourceDictionary = default, Rect? boundsConstraint = default);
    bool SaveAsPdf_ForApiOnly(string filename);
}
