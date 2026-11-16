using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Threading;
using Quire.Calculation;
using Quire.Calculation.Core;
using Quire.Controls;
using Quire.Controls.Core;
using Quire.Controls.Core.BookmarkTags;
using Quire.Controls.Core.IORegions;
using Quire.Controls.Core.Serialization;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.ExcelComponent;
using Quire.Controls.Include;
using Quire.Controls.Mathboard;
using Quire.Controls.Plots;
using Quire.Controls.Text;
using Quire.Controls.Worksheet.Printing;
using Quire.Core;
using Quire.PersistentData;
using Quire.PersistentDataObjects;
using Quire.Services;
using Quire.Wpf;
using Quire.Wpf.Printing;

namespace Quire.Controls.Worksheet;

public partial interface IWorksheetModel
{
    WhiteboardItemsCollection WhiteboardItems { get; }

    ObservableRangeCollection<IWorksheetPageBreak> PageBreaks { get; }

    ITextStylesManager TextStylesManager { get; set; }

    string EngineVersion { get; }

    CalculationOptions DocCalculationOptions { get; set; }

    CalculationVariables DocCalculationVariables { get; set; }

    UnitSystems DefaultUnitSystem { get; set; }

    UnitSystems CustomBaseUnitSystem { get; set; }

    List<UnitDescriptor> CustomBaseUnitDescriptors { get; set; }

    List<UnitDescriptor> CustomDerivedUnitDescriptors { get; set; }

    bool SimplifyUnitSystem { get; set; }

    bool IsCalculationsEnabled { get; set; }

    ICalculatable LastVisibleCalculatable { get; set; }

    bool IsAutoCalcOn { get; set; }

    IIORegionManager InputOutputRegionManager { get; set; }

    IBookmarksManager BookmarksManager { get; set; }

    SymbolicEngine SymbolicEngine { get; }

    bool ContainsSymbolicEvaluation { get; }

    event Action<IWhiteboardItem, Point> ItemMoved ;

    void AddItemAtLocation(object item, Point location);

    void RemoveItem(IWhiteboardItem item);

    void SetItemWorksheetLocation(object item, Point newLocation);

    Point GetItemWorksheetLocation(object item);

    void CommitItemIfNeeded(IWhiteboardItem item);

    void CommitItem(IWhiteboardItem item);

    void FireCommitAndNotifyDependentCalculatables(IEnumerable<ICalculatable> toBeCommitted);

    void FireWaitForCalculationIdle();

    void FireWaitForCalculationIdle(int seconds, DispatcherPriority priority);

    void FireRecalculateAll();

    void FireCalculateNow();

    void FireCalculateRegionNow(IWhiteboardItem item);

    IWhiteboardItem FirstItemInDirection(IEnumerable<IWhiteboardItem> items, Direction direction, Point startPoint);

    void HandleScriptingRunWorksheetRecalculate(bool flag);
}
