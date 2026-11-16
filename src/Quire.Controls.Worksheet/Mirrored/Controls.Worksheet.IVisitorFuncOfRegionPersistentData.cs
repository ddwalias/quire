using Quire.Serialization;

namespace Quire.Controls.Worksheet;

public partial interface IVisitorFuncOfRegionPersistentData
{
    object Visit(IRegionPersistentData data);

    object Visit(MathRegionPersistentData equationData);

    object Visit(PlotRegionPersistentData plotData);

    object Visit(SpecTableRegionPersistentData specTableData);

    object Visit(ImageRegionPersistentData imageData);

    object Visit(RichTextBaseRegionPersistentData richTextData);

    object Visit(ContainerRegionPersistentData containerData);

    object Visit(IncludeRegionPersistentData includeData);

    object Visit(TemplatedTextBoxPersistentData data);

    object Visit(WorksheetPageBreakPersistentData pageBreakData);

    object Visit(ExcelContainerRegionPersistentData excelContainerData);

    object Visit(OleObjectPersistentData oleObjectData);

    object Visit(ChartContainerRegionPersistentData chartContainerData);
}
