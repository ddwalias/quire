using System;
using Quire.Controls.Core;
using Quire.EngNotebookPlus.Automation;

namespace Quire.AutomationServiceEngNotePlus;

internal partial class CMathcadPrimeWorksheetReadonlyOptions
{
    private bool _fileLocationHistoryDisabled;

    private WorksheetOperations _operationsWithEnabledStateGeneration;

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

    public DocumentReadonlyOptions ConvertToDocumentReadonlyOptions()
    {
        return new DocumentReadonlyOptions(_operationsWithEnabledStateGeneration.HasFlag(WorksheetOperations.Save), !_fileLocationHistoryDisabled, _requestToUpdateInputsEnabled, _caseInsensitiveAliasComparisonEnabled);
    }
}
