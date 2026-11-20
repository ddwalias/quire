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
}
