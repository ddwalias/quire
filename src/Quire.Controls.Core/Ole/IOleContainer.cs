#nullable disable
using System;

namespace Quire.Controls.Core.Ole;

public partial interface IOleContainer
{
    bool IsOutOfPlaceActive { get; }
    bool SiteCreated { get; }
    IOleContainerViewModel ViewModel { get; }
    bool IsStorageEmpty { get; }
    double Zoom { get; }
    bool IsPrintingZoomSet { get; }
    void CreateWindowlessOle();
    void StartOleCloseOperation();
    void AbortOleCloseOperation();
    void CleanStorage(bool deleteFile);
    void SetPrintingZoom(bool set, double? newPrintingZoom);
}
