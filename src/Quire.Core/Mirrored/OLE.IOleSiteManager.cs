#nullable disable
#pragma warning disable

namespace Quire.OLE;

partial interface IOleSiteManager
{
    bool IsOutOfPlaceActive { get; }
    bool IsRunning { get; }
    string ProgId { get; }
    bool SiteCreated { get; }
    event global::System.Action Saved ;
    event global::System.Action Shown ;
    event global::System.Action Hidden ;
    event global::System.Action ViewChanged ;
    event global::System.Action Closed ;
    event global::System.Action DataChanged ;
    event global::System.Action EditingStarted ;
    event global::System.Action EditingEnded ;
    bool CreateSiteFromProgID(string progID, string oleContainerPath) ;
    bool CreateSiteFromFile(string filePath, bool link, string oleContainerPath) ;
    bool SetExtent(int width, int height) ;
    void GetExtent(out int width, out int height) ;
    bool Save(string filename, string[] excludeElementsNames) ;
    bool Load(string filename, global::System.Collections.Generic.Dictionary<global::System.Guid, global::System.Guid> guidsConversionDict) ;
    bool UpdateStorage() ;
    bool ActivateOutOfPlace() ;
    bool ActivateInPlace() ;
    bool ActivatePrimary() ;
    void Deactivate() ;
    nint CreateHBitmap(global::System.Windows.Int32Rect newRect, nint hwndPtr) ;
    void SetHostNames(string appName, string objName) ;
    global::Quire.OLE.OleResult Run() ;
    object GetOleObject(bool forceRun) ;
    bool CreateSiteWithDialog(nint winHandle, string title, string oleContainerPath, out bool isNew, out bool isLink, out bool isCanceled) ;
    global::Quire.OLE.OleResult UpdateLink() ;
    void SetOleContainerPath(string containerPath) ;
}
