using System;
using System.Globalization;

namespace Quire;

internal partial class CMathcadPrimeWorksheetReadonlyOptions
{
    private bool _fileLocationHistoryDisabled;
    private global::Quire.Automation.WorksheetOperations _operationsWithEnabledStateGeneration;
    private bool _requestToUpdateInputsEnabled;
    private bool _caseInsensitiveAliasComparisonEnabled;
    private static bool TryConvertToBool(object value, ref bool returnValue)
    {
        if (value != null)
        {
            if (value is bool)
            {
                returnValue = (bool)value;
                return true;
            }

            if (value is short || value is int || value is long)
            {
                returnValue = Convert.ToInt32(value) != 0;
                return true;
            }
        }

        return false;
    }
    public global::Quire.Controls.Core.DocumentReadonlyOptions ConvertToDocumentReadonlyOptions()
    {
        return new global::Quire.Controls.Core.DocumentReadonlyOptions(
            _operationsWithEnabledStateGeneration.HasFlag(global::Quire.Automation.WorksheetOperations.Save),
            !_fileLocationHistoryDisabled,
            _requestToUpdateInputsEnabled,
            _caseInsensitiveAliasComparisonEnabled);
    }
    public CMathcadPrimeWorksheetReadonlyOptions() { }
}
