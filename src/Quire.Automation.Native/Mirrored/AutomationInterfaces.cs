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

    public partial interface IAutomationValue : global::Mathcad.IValue, global::Quire.AutomationPrivate.IValuePrivate
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

namespace Quire.Engine
{
    public partial interface IMathcadComponent
    {
        bool LockComponent();
        bool LockComponent(bool wait);
        void UnlockComponent();
        void SetValue(string place, global::Mathcad.IValue value);
        global::Mathcad.IValue GetValue(string place);
        bool ShouldSkipUnchangedValue(string place);
        void Commit();
    }

    public partial interface IMathcadComponent2
    {
        bool LockComponent();
        bool LockComponent(bool wait);
        void UnlockComponent();
        void SetValue(string place, global::Mathcad.IValue value);
        global::Mathcad.IValue GetValue(string place);
    }
}


