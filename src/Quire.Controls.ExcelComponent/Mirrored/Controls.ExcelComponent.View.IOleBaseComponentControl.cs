#nullable disable
#pragma warning disable
namespace Quire.Controls.ExcelComponent.View;

public partial interface IOleBaseComponentControl
{
    bool IsPrintingZoomSet { get; }
    bool IsInputSectionVisible { get; }
    bool IsOutputSectionVisible { get; }
    IOleContainerItem OleContainerItem { get; }
    void SetPrintingZoom(bool set, double? newPrintingZoom);
}
