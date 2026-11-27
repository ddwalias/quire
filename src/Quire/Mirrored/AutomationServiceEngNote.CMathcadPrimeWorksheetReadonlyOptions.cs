using System;
using Quire.Controls.Core;
using Quire.EngNotebook.Automation;

namespace Quire.AutomationServiceEngNote;

internal partial class CMathcadPrimeWorksheetReadonlyOptions
{
    private bool _fileLocationHistoryDisabled;

    private WorksheetOperations _operationsWithEnabledStateGeneration;

    private bool _requestToUpdateInputsEnabled;

    private bool _caseInsensitiveAliasComparisonEnabled;

    public int SetOptionValue(WorksheetReadonlyOptionNames optionNameArg, object optionValueArg)
    {
        try
        {
            switch (optionNameArg)
            {
                case WorksheetReadonlyOptionNames.FileLocationHistoryDisabled:
                {
                    bool returnValue2 = false;
                    if (TryConvertToBool(optionValueArg, ref returnValue2))
                    {
                        _fileLocationHistoryDisabled = returnValue2;
                        return 0;
                    }
                    return 1;
                }
                case WorksheetReadonlyOptionNames.OperationsWithEnabledStateGeneration:
                {
                    if (Enum.TryParse<WorksheetOperations>(optionValueArg.ToString(), out var result))
                    {
                        _operationsWithEnabledStateGeneration = result;
                        return 0;
                    }
                    return 1;
                }
                case WorksheetReadonlyOptionNames.RequestToUpdateInputsEnabled:
                {
                    bool returnValue3 = false;
                    if (TryConvertToBool(optionValueArg, ref returnValue3))
                    {
                        _requestToUpdateInputsEnabled = returnValue3;
                        return 0;
                    }
                    return 1;
                }
                case WorksheetReadonlyOptionNames.CaseInsensitiveAliasComparisonEnabled:
                {
                    bool returnValue = false;
                    if (TryConvertToBool(optionValueArg, ref returnValue))
                    {
                        _caseInsensitiveAliasComparisonEnabled = returnValue;
                        return 0;
                    }
                    return 1;
                }
                default:
                    return 1;
            }
        }
        catch
        {
        }
        return 1;
    }

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
