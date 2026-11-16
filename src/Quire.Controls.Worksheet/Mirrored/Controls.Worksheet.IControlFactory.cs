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

public partial interface IControlFactory
{
    IEquationControl CreateEquationControl(string sExpression = default);

    IEquationControl CreateEquationControlWithTextInput(string text);

    IRichTextBase CreateTextboxControl(TextFormat textFormat, XmlLanguage proofingLanguage, string content, string dataFormat, bool caretPositionAtTheEnd = false, ITextStyleChangedHandler textStyleChangedHandler = default, bool isBorder = false);

    IFileNameControl CreateFileNameControl(TextFormat textFormat);

    IFilePathControl CreateFilePathControl(TextFormat textFormat);

    IPageNumberControl CreatePageNumberControl(string template, TextFormat textFormat);

    ISavedDateControl CreateSavedDateControl(string template, TextFormat textFormat);

    IPlotControl CreateXYPlotControl(TextFormat textFormat = default);

    IPlotControl CreatePolarPlotControl(TextFormat textFormat = default);

    ISpecTableControl CreateSpecTableControl(Nullable<TableCoordinate> dimensions);

    ISpecTableControl CreateSpecTableControl(Array data);

    IImageControl CreateImageControl(bool isNewImageControl = true);

    IAggregateMathboardControl CreateSolveBlockControl(Nullable<Size> gridCellSize = default);

    IAggregateMathboardControl CreateExcelComponentControl(Nullable<Size> gridCellSize = default);

    IOleObjectControl CreateOleObjectControl(OleObjectControlCreationOptions creationOptions, string oleContainerPath);

    IAggregateMathboardControl CreateChartComponentControl(Nullable<Size> gridCellSize = default);

    ICollapsibleAreaControl CreateCollapsibleAreaControl(Nullable<Size> gridCellSize = default);

    IParagraphControl CreateTextBlockControl(TextFormat textFormat, XmlLanguage proofingLanguage, string content, string dataFormat, ITextStyleChangedHandler textStyleChangedHandler = default);

    IIncludeWorksheetControl CreateIncludeControl();

    IWorksheetPage CreateWorksheetPageControl();

    IWorksheetPage CreateWorksheetPageControl(Nullable<Size> gridCellSize);

    IPlotControl<XyzEquation> CreateContourPlotControl();

    IPlotControl<XyzEquation> Create3DPlotControl();

    ISelectionDragger CreateSelectionDragger(object draggingOn, Point startLocation);

    IHeaderFooterControl CreateHeaderFooterControl(Nullable<Size> gridCellSize, HeaderFooterRole role);

    IHeaderFooterZombie CreateHeaderFooterZombieControl(IHeaderFooterControl headerFooter);

    IWorksheetPageBreak CreatePageBreakControl();

    IWhiteboardItemZombie CreateWhiteboardItemZombie(IWhiteboardItem item);
}
