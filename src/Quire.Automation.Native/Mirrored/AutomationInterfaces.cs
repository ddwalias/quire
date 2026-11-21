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


