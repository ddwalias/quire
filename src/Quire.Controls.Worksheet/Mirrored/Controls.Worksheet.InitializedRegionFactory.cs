using System;
using Quire.Serialization;

namespace Quire.Controls.Worksheet;

public partial class InitializedRegionFactory : IVisitorFuncOfRegionPersistentData, IVisitor
{
    public static InitializedRegionFactory Instance = new InitializedRegionFactory();

    public object Visit(IRegionPersistentData data)
    {
        return null;
    }

    public object Visit(MathRegionPersistentData equationData)
    {
        return equationData.Control;
    }

    public object Visit(PlotRegionPersistentData plotData)
    {
        return plotData.Control;
    }

    public object Visit(SpecTableRegionPersistentData specTableData)
    {
        return specTableData.Control;
    }

    public object Visit(ImageRegionPersistentData imageData)
    {
        return imageData.Control;
    }

    public object Visit(RichTextBaseRegionPersistentData richTextData)
    {
        return richTextData.Control;
    }

    public object Visit(ContainerRegionPersistentData containerData)
    {
        return containerData.Control;
    }

    public object Visit(IncludeRegionPersistentData includeData)
    {
        return includeData.Control;
    }

    public object Visit(TemplatedTextBoxPersistentData data)
    {
        throw new NotImplementedException();
    }

    public object Visit(WorksheetPageBreakPersistentData pageBreakData)
    {
        return pageBreakData.Control;
    }

    public object Visit(ExcelContainerRegionPersistentData excelContainerData)
    {
        return excelContainerData.Control;
    }

    public object Visit(OleObjectPersistentData oleObjectData)
    {
        return oleObjectData.Control;
    }

    public object Visit(ChartContainerRegionPersistentData chartContainerData)
    {
        return chartContainerData.Control;
    }
}
