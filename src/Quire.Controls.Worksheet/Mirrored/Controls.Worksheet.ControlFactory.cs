using System;
using System.Windows;
using System.Windows.Markup;
using Quire.Controls.CollapsibleArea;
using Quire.Controls.Core;
using Quire.Controls.Core.Whiteboard;
using Quire.Controls.ExcelComponent;
using Quire.Controls.ExcelComponent.View;
using Quire.Controls.Include;
using Quire.Controls.Mathboard;
using Quire.Controls.Plots;
using Quire.Controls.Plots.Contour.View;
using Quire.Controls.Plots.Plot3D.View;
using Quire.Controls.Text;
using Quire.Core;
using Quire.Services;
using Quire.Wpf;

namespace Quire.Controls.Worksheet;

internal partial class ControlFactory
{
    private const string DataFormatRichText = "Rich Text Format";

    public bool IsConnected { get; set; }

    public IEquationControl CreateEquationControl(string sExpression = null)
    {
        return new EquationControl(sExpression);
    }

    public IEquationControl CreateEquationControlWithTextInput(string text)
    {
        return EquationControl.CreateWithTextInput(text);
    }

    public IImageControl CreateImageControl(bool isNewImageControl = true)
    {
        return ImageControl.CreateImageControl(isNewImageControl);
    }

    public IParagraphControl CreateTextBlockControl(TextFormat textFormat, XmlLanguage proofingLanguage, string content, string dataFormat, ITextStyleChangedHandler textStyleChangedHandler = null)
    {
        Paragraph paragraph = new Paragraph
        {
            RegionFactory = RegionFactory.Instance,
            TextStyleChangedHandler = textStyleChangedHandler
        };
        InitializeTextRegion(paragraph, textFormat, proofingLanguage, content, dataFormat);
        return paragraph;
    }

    public IRichTextBase CreateTextboxControl(TextFormat textFormat, XmlLanguage proofingLanguage, string content, string dataFormat, bool caretPositionAtTheEnd = false, ITextStyleChangedHandler textStyleChangedHandler = null, bool isBorder = false)
    {
        TextRegion textRegion = new TextRegion
        {
            RegionFactory = RegionFactory.Instance,
            TextStyleChangedHandler = textStyleChangedHandler
        };
        InitializeTextRegion(textRegion, textFormat, proofingLanguage, content, dataFormat, caretPositionAtTheEnd, isBorder);
        return textRegion;
    }

    public IFileNameControl CreateFileNameControl(TextFormat textFormat)
    {
        FileNameControl fileNameControl = new FileNameControl();
        if (textFormat != null)
        {
            textFormat.ApplyAllTextFormatValuesToControl(fileNameControl);
        }
        return fileNameControl;
    }

    public IFilePathControl CreateFilePathControl(TextFormat textFormat)
    {
        FilePathControl filePathControl = new FilePathControl();
        if (textFormat != null)
        {
            textFormat.ApplyAllTextFormatValuesToControl(filePathControl);
        }
        return filePathControl;
    }

    public IPageNumberControl CreatePageNumberControl(string template, TextFormat textFormat)
    {
        PageNumberControl pageNumberControl = new PageNumberControl
        {
            TemplateFormat = template
        };
        if (textFormat != null)
        {
            textFormat.ApplyAllTextFormatValuesToControl(pageNumberControl);
        }
        return pageNumberControl;
    }

    public ISavedDateControl CreateSavedDateControl(string template, TextFormat textFormat)
    {
        SavedDateControl savedDateControl = new SavedDateControl
        {
            TemplateFormat = template
        };
        if (textFormat != null)
        {
            textFormat.ApplyAllTextFormatValuesToControl(savedDateControl);
        }
        return savedDateControl;
    }

    public IPlotControl CreateXYPlotControl(TextFormat textFormat = null)
    {
        double num = WpfUtils.InchesToDips(2.4);
        XYPlotControl xYPlotControl = new XYPlotControl(new Size(num, num));
        if (textFormat != null)
        {
            xYPlotControl.InitialTextFormat = textFormat;
        }
        return xYPlotControl;
    }

    public IPlotControl CreatePolarPlotControl(TextFormat textFormat = null)
    {
        PolarPlotControl polarPlotControl = new PolarPlotControl();
        if (textFormat != null)
        {
            polarPlotControl.InitialTextFormat = textFormat;
        }
        return polarPlotControl;
    }

    public IPlotControl<XyzEquation> CreateContourPlotControl()
    {
        return new ContourPlotControl();
    }

    public IPlotControl<XyzEquation> Create3DPlotControl()
    {
        return new PlotControl3D();
    }

    public ISpecTableControl CreateSpecTableControl(TableCoordinate? dimensions)
    {
        if (!dimensions.HasValue)
        {
            return new SpecTableControl();
        }
        return new SpecTableControl(dimensions.Value.Row, dimensions.Value.Column);
    }

    public ISpecTableControl CreateSpecTableControl(Array data)
    {
        return new SpecTableControl((string[,])data);
    }

    public IAggregateMathboardControl CreateSolveBlockControl(Size? gridCellSize)
    {
        return new SolveBlockControl(gridCellSize)
        {
            RegionFactory = RegionFactory.Instance
        };
    }

    public IAggregateMathboardControl CreateExcelComponentControl(Size? gridCellSize)
    {
        return new ExcelComponentControl(gridCellSize)
        {
            RegionFactory = RegionFactory.Instance
        };
    }

    public IOleObjectControl CreateOleObjectControl(OleObjectControlCreationOptions creationOptions, string oleContainerPath)
    {
        return OleObjectControl.Create(creationOptions, oleContainerPath);
    }

    public IAggregateMathboardControl CreateChartComponentControl(Size? gridCellSize)
    {
        return new ChartComponentControl(gridCellSize)
        {
            RegionFactory = RegionFactory.Instance
        };
    }

    public ICollapsibleAreaControl CreateCollapsibleAreaControl(Size? gridCellSize)
    {
        return new CollapsibleAreaControl(gridCellSize)
        {
            RegionFactory = RegionFactory.Instance
        };
    }

    public IIncludeWorksheetControl CreateIncludeControl()
    {
        IncludeWorksheetControl includeWorksheetControl = new IncludeWorksheetControl();
        ((IRegionFactoryOwner)includeWorksheetControl).RegionFactory = RegionFactory.Instance;
        return includeWorksheetControl;
    }

    public IWorksheetPage CreateWorksheetPageControl()
    {
        return new WorksheetPageControl();
    }

    public IWorksheetPage CreateWorksheetPageControl(Size? gridCellSize)
    {
        return new WorksheetPageControl(gridCellSize);
    }

    public ISelectionDragger CreateSelectionDragger(object draggingOn, Point startLocation)
    {
        return new SelectionDragger(draggingOn as UIElement, startLocation);
    }

    public IHeaderFooterControl CreateHeaderFooterControl(Size? gridCellSize, HeaderFooterRole role)
    {
        return new HeaderFooterControl(gridCellSize, role);
    }

    public IHeaderFooterZombie CreateHeaderFooterZombieControl(IHeaderFooterControl headerFooter)
    {
        return new HeaderFooterZombie(headerFooter);
    }

    public IWorksheetPageBreak CreatePageBreakControl()
    {
        return new WorksheetPageBreak();
    }

    public IWhiteboardItemZombie CreateWhiteboardItemZombie(IWhiteboardItem item)
    {
        return new WhiteboardItemZombie(item);
    }

    private static void InitializeTextRegion(IRichTextBase control, TextFormat textFormat, XmlLanguage proofingLanguage, string content, string dataFormat, bool caretPositionAtTheEnd = false, bool isBorder = false)
    {
        control.SetFormattedContent(content, dataFormat, proofingLanguage, textFormat);
        textFormat?.ApplyAllTextFormatValuesToControl(control, (dataFormat == DataFormatRichText) ? control.Document.Blocks.FirstBlock : null);
        if (caretPositionAtTheEnd)
        {
            control.SetCaretOffsetAtContentEnd();
        }
        if (isBorder)
        {
            WhiteboardDependencyProperties.SetIsRegionBorderVisible((DependencyObject)control, value: true);
        }
    }
}
