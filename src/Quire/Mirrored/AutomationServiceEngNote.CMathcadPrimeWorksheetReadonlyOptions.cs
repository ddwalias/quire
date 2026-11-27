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
            int status;
            switch (optionNameArg)
            {
                case WorksheetReadonlyOptionNames.FileLocationHistoryDisabled:
                {
                    bool disabled = false;
                    if (TryConvertToBool(optionValueArg, ref disabled))
                    {
                        _fileLocationHistoryDisabled = disabled;
                        status = 0;
                    }
                    else
                    {
                        status = 1;
                    }
                    break;
                }
                case WorksheetReadonlyOptionNames.OperationsWithEnabledStateGeneration:
                {
                    if (Enum.TryParse<WorksheetOperations>(optionValueArg.ToString(), out var operations))
                    {
                        _operationsWithEnabledStateGeneration = operations;
                        status = 0;
                    }
                    else
                    {
                        status = 1;
                    }
                    break;
                }
                case WorksheetReadonlyOptionNames.RequestToUpdateInputsEnabled:
                {
                    bool enabled = false;
                    if (TryConvertToBool(optionValueArg, ref enabled))
                    {
                        _requestToUpdateInputsEnabled = enabled;
                        status = 0;
                    }
                    else
                    {
                        status = 1;
                    }
                    break;
                }
                case WorksheetReadonlyOptionNames.CaseInsensitiveAliasComparisonEnabled:
                {
                    bool enabled = false;
                    if (TryConvertToBool(optionValueArg, ref enabled))
                    {
                        _caseInsensitiveAliasComparisonEnabled = enabled;
                        status = 0;
                    }
                    else
                    {
                        status = 1;
                    }
                    break;
                }
                default:
                    status = 1;
                    break;
            }
            return status;
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
