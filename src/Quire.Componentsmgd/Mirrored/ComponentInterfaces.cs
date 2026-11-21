namespace Quire.Components
{
    public partial interface IMcxComponent
    {
        void Initialize(object executive);
        void McError(bool bSet);
        object[] Eval(object[] inputs, object reserved);
    }

    public partial interface IPersist
    {
        global::System.Guid Clsid { get; }
    }

    public partial interface IVSPersistStream
    {
        void Load(global::System.Runtime.InteropServices.ComTypes.IStream pstm, bool old);
        void Save(global::System.Runtime.InteropServices.ComTypes.IStream pstm, bool fClearDirty, bool old);
    }

    public partial interface IOleClientSite
    {
        void SaveObject();
        void GetMoniker(uint dwAssign, uint dwWhichMoniker, ref object ppmk);
        void GetContainer(ref object ppContainer);
        void ShowObject();
        void OnShowWindow(bool fShow);
        void RequestNewObjectLayout();
    }

    public partial interface IOleObject
    {
        void SetClientSite(global::Quire.Components.IOleClientSite pClientSite);
        void GetClientSite(ref global::Quire.Components.IOleClientSite ppClientSite);
        void SetHostNames(object szContainerApp, object szContainerObj);
        void Close(uint dwSaveOption);
        void SetMoniker(uint dwWhichMoniker, object pmk);
        void GetMoniker(uint dwAssign, uint dwWhichMoniker, object ppmk);
        void InitFromData(global::Quire.VSINTERFACESLib.IDataObject pDataObject, bool fCreation, uint dwReserved);
        void GetClipboardData(uint dwReserved, ref global::Quire.VSINTERFACESLib.IDataObject ppDataObject);
        void DoVerb(uint iVerb, uint lpmsg, object pActiveSite, uint lindex, uint hwndParent, uint lprcPosRect);
        void EnumVerbs(ref object ppEnumOleVerb);
        void Update();
        void IsUpToDate();
        void GetUserClassID(out global::System.Guid pClsid);
        void GetUserType(uint dwFormOfType, uint pszUserType);
        void SetExtent(uint dwDrawAspect, uint psizel);
        void GetExtent(uint dwDrawAspect, uint psizel);
        void Advise(object pAdvSink, uint pdwConnection);
        void Unadvise(uint dwConnection);
        void EnumAdvise(ref object ppenumAdvise);
        void GetMiscStatus(uint dwAspect, uint pdwStatus);
        void SetColorScheme(object pLogpal);
    }

    public partial interface IPersistStream : global::Quire.Components.IPersist
    {
        new global::System.Guid Clsid { get; }
        int IsDirty { get; }
        long MaxSize { get; }
        void Load(global::System.Runtime.InteropServices.ComTypes.IStream stream);
        void Save(global::System.Runtime.InteropServices.ComTypes.IStream stream, bool fClearDirty);
    }
}
