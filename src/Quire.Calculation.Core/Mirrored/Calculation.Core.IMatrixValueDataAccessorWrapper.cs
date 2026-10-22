#nullable disable
#pragma warning disable

namespace Quire.Calculation.Core;

partial interface IMatrixValueDataAccessorWrapper
{
    global::System.Collections.Generic.IList<global::Quire.Calculation.Core.PlainDataTypes> GetColumnsPlainDataTypes();
    global::System.Collections.Generic.IList<global::System.Collections.IList> GetColumnsPlainDataValues(int startIndex, int endIndex);
    int GetFirstNonPlainDataColumnIndex();
    int GetFirstNonPlainDataRowIndex();
    global::System.Collections.Generic.IList<global::Quire.Calculation.Core.PlainDataTypes> GetRowsPlainDataTypes();
    global::System.Collections.Generic.IList<global::System.Collections.IList> GetRowsPlainDataValues(int startIndex, int endIndex);
    global::Quire.Calculation.Core.PlainDataTypes GetEntireMatrixDataTypes();
}
