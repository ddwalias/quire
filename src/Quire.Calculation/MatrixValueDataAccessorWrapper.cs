#nullable disable
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Quire;
using Quire.Automation;
using Quire.Calculation.Core;

namespace Quire.Calculation;

internal partial class MatrixValueDataAccessorWrapper : IMatrixValueDataAccessorWrapper
{
    private IMatrixValueDataAccessor AccessorProxy { get; set; }

    public MatrixValueDataAccessorWrapper(IMatrixValueDataAccessor accessor)
    {
        AccessorProxy = accessor;
    }

    public Quire.Calculation.Core.PlainDataTypes GetEntireMatrixDataTypes()
    {
        EnumHelper.TryParse<Quire.Calculation.Core.PlainDataTypes>(AccessorProxy.GetColumnsPlainDataTypes().Aggregate(Quire.Automation.PlainDataTypes.Uninitialized, (aggregate, current) => aggregate | current), out var enumValue);
        return enumValue;
    }

    public IList<Quire.Calculation.Core.PlainDataTypes> GetColumnsPlainDataTypes()
    {
        return ConvertPlainDataTypeList(AccessorProxy.GetColumnsPlainDataTypes());
    }

    private static List<Quire.Calculation.Core.PlainDataTypes> ConvertPlainDataTypeList(IList<Quire.Automation.PlainDataTypes> list)
    {
        List<Quire.Calculation.Core.PlainDataTypes> list2 = new List<Quire.Calculation.Core.PlainDataTypes>(list.Count);
        foreach (Quire.Automation.PlainDataTypes item in list)
        {
            if (EnumHelper.TryParse<Quire.Calculation.Core.PlainDataTypes>(item, out var enumValue, EnumHelper.SettingsWithCacheAndAssert))
            {
                list2.Add(enumValue);
                continue;
            }
            list2 = null;
            break;
        }
        return list2;
    }

    public IList<IList> GetColumnsPlainDataValues(int startIndex, int endIndex)
    {
        return AccessorProxy.GetColumnsPlainDataValues(startIndex, endIndex);
    }

    public int GetFirstNonPlainDataColumnIndex()
    {
        return AccessorProxy.GetFirstNonPlainDataColumnIndex();
    }

    public int GetFirstNonPlainDataRowIndex()
    {
        return AccessorProxy.GetFirstNonPlainDataRowIndex();
    }

    public IList<Quire.Calculation.Core.PlainDataTypes> GetRowsPlainDataTypes()
    {
        return ConvertPlainDataTypeList(AccessorProxy.GetRowsPlainDataTypes());
    }

    public IList<IList> GetRowsPlainDataValues(int startIndex, int endIndex)
    {
        return AccessorProxy.GetRowsPlainDataValues(startIndex, endIndex);
    }
}
