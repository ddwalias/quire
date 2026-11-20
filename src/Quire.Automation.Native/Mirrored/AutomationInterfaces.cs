namespace Quire.Automation
{
    public partial interface IScalarValueProxy
    {
        double RealPart { get; }
        double ImagPart { get; }
    }

    public partial interface IStringValueProxy
    {
        string String { get; }
    }

    public partial interface IFunctionValueProxy
    {
        string FunctionInfo { get; }
    }

    public partial interface ISymbolicValueProxy
    {
        object Symbolic { get; }
    }

    public partial interface IAutomationValue : global::Quire.IValue, global::Quire.AutomationPrivate.IValuePrivate
    {
        object Core { get; set; }
        global::Quire.Mcd.Dim Dim { get; }
        string RL { get; }
        global::Quire.Automation.IValueProxy Raw { get; }
        global::Quire.Automation.IValueProxy Simplified { get; }
        global::Quire.Automation.IValueProxy GetProxy(bool bSimplify);
    }

    public partial interface IMatrixValueDataAccessor
    {
        global::System.Collections.Generic.List<global::Quire.Automation.PlainDataTypes> GetRowsPlainDataTypes();
        int GetFirstNonPlainDataRowIndex();
        global::System.Collections.Generic.List<global::System.Collections.IList> GetRowsPlainDataValues(int startIndex, int endIndex);
        global::System.Collections.Generic.List<global::Quire.Automation.PlainDataTypes> GetColumnsPlainDataTypes();
        int GetFirstNonPlainDataColumnIndex();
        global::System.Collections.Generic.List<global::System.Collections.IList> GetColumnsPlainDataValues(int startIndex, int endIndex);
    }

    public partial interface IMatrixValueProxy : global::Quire.Automation.IValueProxy
    {
        uint Rows { get; }
        uint Cols { get; }
        bool IsDimless { get; }
        bool IsRange { get; }
        bool IsRealMatrix { get; }
        bool IsRealMatrixMixed { get; }
        bool IsScalarMatrix { get; }
        bool IsUniform { get; }
        bool IsUniformScalar { get; }
        bool IsVector { get; }
        global::Quire.Automation.IValueProxy Get(uint row, uint col);
        global::Quire.Automation.IMatrixValueProxy SubMatrix(uint r1, uint r2, uint c1, uint c2);
        global::Quire.Automation.IMatrixValueDataAccessor GetPlainData(global::Quire.Automation.PlainDataConversionOptions options);
    }

    public partial interface IValueProxy
    {
        global::Quire.Automation.IScalarValueProxy AsScalar { get; }
        global::Quire.Automation.IStringValueProxy AsString { get; }
        global::Quire.Automation.IMatrixValueProxy AsMatrix { get; }
        global::Quire.Automation.IFunctionValueProxy AsFunction { get; }
        global::Quire.Automation.ISymbolicValueProxy AsSymbolic { get; }
        global::Quire.Automation.UnitPower[] Units { get; }
        bool Simplify { get; }
        object Core { get; }
        string Xml { get; }
        void WriteXml(global::System.IO.TextWriter writer, bool simplify, double scaleFactor);
        void WriteXml(global::System.IO.TextWriter writer, bool simplify);
        void WriteXml(global::System.IO.TextWriter writer);
        void WriteXml(global::System.Xml.XmlWriter xml, bool simplify, double scaleFactor);
        void WriteXml(global::System.Xml.XmlWriter xml, bool simplify);
        void WriteXml(global::System.Xml.XmlWriter xml);
        void WriteUnitsXml(global::System.Xml.XmlWriter xml);
    }
}

namespace Quire.AutomationPrivate
{
    public partial interface IFunctionValue
    {
        int Arity { get; }
    }

    public partial interface INumericValuePrivate
    {
        string UnitsXML(bool bSimplify = false);
    }

    public partial interface IMatrixValuePrivate
    {
        bool IsRange { get; }
    }

    public partial interface IValuePrivate
    {
        int Size { get; }
        string XML(bool bSimplify = false);
        void Save(global::Quire.AutomationPrivate.IStream pstm);
        void Load(global::Quire.AutomationPrivate.IStream pstm);
    }

    public partial interface IValueFactory
    {
        object EngineToAutomation(object engVal);
        object VariantToAutomation(object real, object imag);
        object WrapperToAutomation(object wrapVal);
        object VSDataToAutomation(object vsVal);
        object AutomationToEngine(object o);
        void AutomationToVariant(object o, out object pReal, out object pImag);
        object AutomationToWrapper(object wrapVal);
        object AutomationToVSData(object vsVal);
    }
}

namespace Quire
{
    public partial interface IValue
    {
        string Type { get; }
        string AsString { get; }
    }

    public partial interface IStringValue
    {
        string Type { get; }
        string AsString { get; }
        string Value { get; set; }
    }

    public partial interface INumericValue
    {
        string Type { get; }
        string AsString { get; }
        double Real { get; set; }
        double Imag { get; set; }
        int Integer { get; set; }
    }

    public partial interface IMatrixValue
    {
        string Type { get; }
        string AsString { get; }
        int Rows { get; }
        int Cols { get; }
        object GetElement(int row, int col);
        void SetElement(int row, int col, object val);
    }
}

namespace Quire.Engine
{
    public partial interface IMathcadComponent
    {
        bool LockComponent();
        bool LockComponent(bool wait);
        void UnlockComponent();
        void SetValue(string place, global::Quire.IValue value);
        global::Quire.IValue GetValue(string place);
        bool ShouldSkipUnchangedValue(string place);
        void Commit();
    }

    public partial interface IMathcadComponent2
    {
        bool LockComponent();
        bool LockComponent(bool wait);
        void UnlockComponent();
        void SetValue(string place, global::Quire.IValue value);
        global::Quire.IValue GetValue(string place);
    }
}

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
        global::System.Guid Clsid { get; }
        int IsDirty { get; }
        long MaxSize { get; }
        void Load(global::System.Runtime.InteropServices.ComTypes.IStream stream);
        void Save(global::System.Runtime.InteropServices.ComTypes.IStream stream, bool fClearDirty);
    }
}
