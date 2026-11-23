using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Quire.Controls.Core;
using Quire.Controls.Core.Serialization;
using Quire.Controls.Plots;
using Quire.Controls.Plots.Contour.View;
using Quire.Controls.Plots.Plot3D.View;
using Quire.PersistentData;
using Quire.PersistentDataObjects;
using Quire.Serialization;
using Quire.Controls;
using Quire.Controls.CollapsibleArea;
using Quire.Controls.Include;
using Quire.Controls.ExcelComponent;
using Quire.Controls.ExcelComponent.View;
using Quire.Controls.Plots.View;
using Quire.Controls.Text;

namespace Quire.Controls.Worksheet;

public partial class RegionFactory
{
    public static readonly IRegionFactory Instance = new RegionFactory();

    public IRegionPersistentData CreateFailedRegionPersistentData()
    {
        BitmapImage bitmapImage = new BitmapImage(new Uri("pack://application:,,,/Ptc.Controls.Core;component/images/deserialization_failed.png"));
        ((Freezable)bitmapImage).Freeze();
        ImageRegionPersistentData imageRegionPersistentData = new ImageRegionPersistentData(new ImageControl
        {
            Source = bitmapImage
        });
        MC14ConversionAnnotation.AddConversionAnnotation(((IRegionPersistentData)imageRegionPersistentData).Control, PersistentDataObjects.MC14ConversionAnnotationType.CorruptedRegion);
        return imageRegionPersistentData;
    }

    private static IRegionPersistentData CreateTemplatedTextBoxTypedPersistentData(fieldType fieldType)
    {
        switch (fieldType.ItemElementName)
        {
            case ItemChoiceType1.fileName:
                return new TemplatedTextBoxPersistentData(new FileNameControl());
            case ItemChoiceType1.filePath:
                return new TemplatedTextBoxPersistentData(new FilePathControl());
            case ItemChoiceType1.pageNumber:
                return new TemplatedTextBoxPersistentData(new PageNumberControl());
            case ItemChoiceType1.dateSaved:
                return new TemplatedTextBoxPersistentData(new SavedDateControl());
            default:
                return null;
        }
    }

    private static IRegionPersistentData CreatePlotTypedPersistentData(plotType plotData)
    {
        object item = plotData.Item;
        if (item is xyPlotType)
        {
            return new PlotRegionPersistentData(new XYPlotControl());
        }

        if (item is polarPlotType)
        {
            return new PlotRegionPersistentData(new PolarPlotControl());
        }

        if (item is contourPlotType)
        {
            return new ContourPlotRegionPersistentData(new ContourPlotControl());
        }

        if (item is Plot3DType)
        {
            return new Plot3DRegionPersistentData(new PlotControl3D());
        }

        return null;
    }

    public IRegionPersistentData ConvertFromIRegionType(IDeS11NProvider deS11NProvider, Action<UIElement> deserializationPrologue, IRegionType regionData, Func<IRegionType, bool> skip)
    {
        if (regionData.Item is ProtectedItemType protectedItemType)
        {
            protectedItemType.DeS11NProvider = deS11NProvider;
            regionData.Item = protectedItemType.DataToProtect;
        }

        if (skip != null && skip(regionData))
        {
            return null;
        }

        object item = regionData.Item;
        IRegionPersistentData regionPersistentData = null;
        if (item is mathType)
        {
            regionPersistentData = new MathRegionPersistentData(new EquationControl());
        }
        else if (item is plotType)
        {
            regionPersistentData = CreatePlotTypedPersistentData(item as plotType);
        }
        else if (item is worksheetRegionTypeSpectable)
        {
            regionPersistentData = new SpecTableRegionPersistentData(new SpecTableControl(0, 0));
        }
        else if (item is solveBlockType)
        {
            regionPersistentData = new ContainerRegionPersistentData(new SolveBlockControl
            {
                RegionFactory = Instance,
                TextStyleChangedHandler = deS11NProvider.TextStyleChangedHandler
            });
        }
        else if (item is includeWorksheetType)
        {
            regionPersistentData = new IncludeRegionPersistentData(new IncludeWorksheetControl
            {
                RegionFactory = Instance
            });
        }
        else if (item is pictureType)
        {
            regionPersistentData = new ImageRegionPersistentData(new ImageControl());
        }
        else if (item is excelContainerType)
        {
            regionPersistentData = new ExcelContainerRegionPersistentData(new ExcelContainer());
        }
        else if (item is chartContainerType)
        {
            regionPersistentData = new ChartContainerRegionPersistentData(new ChartContainer());
        }
        else if (item is textType)
        {
            regionPersistentData = new RichTextBaseRegionPersistentData((item as textType).istextblock ? ((RichTextWithEmbeddedItem)new Paragraph
            {
                RegionFactory = Instance,
                TextStyleChangedHandler = deS11NProvider.TextStyleChangedHandler
            }) : ((RichTextWithEmbeddedItem)new TextRegion
            {
                RegionFactory = Instance,
                TextStyleChangedHandler = deS11NProvider.TextStyleChangedHandler
            }));
        }
        else if (item is fieldType)
        {
            regionPersistentData = CreateTemplatedTextBoxTypedPersistentData(item as fieldType);
        }
        else if (item is excelComponentType)
        {
            regionPersistentData = new ExcelComponentPersistentData(new ExcelComponentControl
            {
                RegionFactory = Instance
            });
        }
        else if (item is chartComponentType)
        {
            regionPersistentData = new ChartComponentPersistentData(new ChartComponentControl
            {
                RegionFactory = Instance
            });
        }
        else if (item is worksheetRegionTypePageBreak)
        {
            regionPersistentData = new WorksheetPageBreakPersistentData(new WorksheetPageBreak());
        }
        else if (item is AreaType)
        {
            regionPersistentData = new ContainerRegionPersistentData(new CollapsibleAreaControl
            {
                RegionFactory = Instance,
                TextStyleChangedHandler = deS11NProvider.TextStyleChangedHandler
            });
        }
        else if (item is oleObjectRegionType)
        {
            regionPersistentData = new OleObjectPersistentData(new OleObjectControl());
        }

        if (regionPersistentData != null)
        {
            ISupportCustomInitialization supportCustomInitialization = regionPersistentData.Control as ISupportCustomInitialization;
            if (supportCustomInitialization != null)
            {
                supportCustomInitialization.InCustomInitialization = true;
            }

            if (regionPersistentData.Control is ISolveBlockControl solveBlockControl)
            {
                solveBlockControl.TextStyleChangedHandler = deS11NProvider.TextStyleChangedHandler;
            }

            if (regionPersistentData.Control is PlotControlBase plotControlBase)
            {
                plotControlBase.TextStyleChangedHandler = deS11NProvider.TextStyleChangedHandler;
            }

            deserializationPrologue?.Invoke(regionPersistentData.Control);
            regionPersistentData.DeS11NProvider = deS11NProvider;
            regionPersistentData.Deserialize(regionData);
            if (supportCustomInitialization != null)
            {
                supportCustomInitialization.InCustomInitialization = false;
            }

            return regionPersistentData;
        }

        throw new NotImplementedException();
    }
}
