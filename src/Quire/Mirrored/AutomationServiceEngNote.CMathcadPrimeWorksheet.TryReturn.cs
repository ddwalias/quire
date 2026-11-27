using System;
using Quire.Controls.Core;

namespace Quire.AutomationServiceEngNote;

public partial class CMathcadPrimeWorksheet
{
    private bool DoGetModified()
    {
        try
        {
            IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
            return engineeringDocument == null ? false : engineeringDocument.IsDirty;
        }
        catch
        {
        }
        return false;
    }

    private string DoGetTitle()
    {
        try
        {
            IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
            return engineeringDocument == null ? null : engineeringDocument.CustomizedWindowTitle;
        }
        catch
        {
        }
        return null;
    }

    private string DoGetWorksheetTabIcon()
    {
        try
        {
            IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
            return engineeringDocument == null ? null : engineeringDocument.DocumentTabIcon;
        }
        catch
        {
        }
        return null;
    }

    private string DoGetWorksheetTabName()
    {
        try
        {
            IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
            return engineeringDocument == null ? null : engineeringDocument.CustomizedDocumentName;
        }
        catch
        {
        }
        return null;
    }

    private string DoGetClosingPrompt()
    {
        try
        {
            IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
            return engineeringDocument == null ? null : engineeringDocument.CustomizedClosePrompt;
        }
        catch
        {
        }
        return null;
    }

    private string DoGetWorksheetDisplayedFilePath()
    {
        try
        {
            IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
            return engineeringDocument == null ? null : engineeringDocument.CustomizedFilePath;
        }
        catch
        {
        }
        return null;
    }

    private string DoGetWorksheetWorkingDirectory()
    {
        try
        {
            IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
            return engineeringDocument == null ? null : engineeringDocument.CWD;
        }
        catch
        {
        }
        return null;
    }

    private int DoGetDefaultCalculationTimeout()
    {
        try
        {
            IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
            return engineeringDocument == null ? 0 : engineeringDocument.InputOutputRegionManager.DefaultCalculationTimeout;
        }
        catch
        {
        }
        return 0;
    }

    public string InputGetStringValue(string aliasArg)
    {
        try
        {
            return DoGetStringValue(aliasArg, isInput: true);
        }
        catch
        {
        }
        return null;
    }

    public string OutputGetStringValue(string aliasArg)
    {
        try
        {
            return DoGetStringValue(aliasArg, isInput: false);
        }
        catch
        {
        }
        return null;
    }

    public string InputGetSExprValue(string aliasArg)
    {
        try
        {
            return DoInputGetSExprValue(aliasArg);
        }
        catch
        {
        }
        return null;
    }
}
