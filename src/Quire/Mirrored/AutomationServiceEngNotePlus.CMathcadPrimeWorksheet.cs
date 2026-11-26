using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Quire.Controls.Core.IORegions;
using System.Collections.Generic;
using System.Windows.Threading;
using Quire.Controls.Core;
using Quire.EngNotebookPlus.Automation;

namespace Quire.AutomationServiceEngNotePlus;

public partial class CMathcadPrimeWorksheet
{
    private readonly bool _isUiInteractiveModeDisabled;

    public CMathcadPrimeWorksheet(IEngineeringDocument engineeringDocument, bool isUiInteractiveModeDisabled, bool isComClientControlled = false)
    {
        try
        {
            if (engineeringDocument == null)
            {
                throw new Exception("The IEngineeringDocument document cannot be null.");
            }

            worksheetData = new WorksheetData(engineeringDocument.FilePath, engineeringDocument.DocumentName, isComClientControlled);
            _isUiInteractiveModeDisabled = isUiInteractiveModeDisabled;
        }
        catch
        {
            throw new Exception("An unexpected error occurred when creating the CMathcadPrimeWorksheet object.");
        }
    }

    public string Name
    {
        get
        {
            try
            {
                return worksheetData.DocumentName;
            }
            catch
            {
            }

            return string.Empty;
        }
    }

    public string FullName
    {
        get
        {
            try
            {
                return worksheetData.DocumentFullName;
            }
            catch
            {
            }

            return string.Empty;
        }
    }

    public bool IsReadOnly
    {
        get
        {
            try
            {
                DelegateFunction0<bool> isReadOnly = DoIsReadOnly;
                return (bool)MathcadPrimeComServerManager.Instance.InvokeDelegate(isReadOnly, DispatcherPriority.Normal);
            }
            catch
            {
            }

            return true;
        }
    }

    private bool DoIsReadOnly()
    {
        try
        {
            if (string.IsNullOrEmpty(worksheetData.DocumentFullName))
            {
                return false;
            }

            return DoIsDocumentOpen() ? (File.GetAttributes(worksheetData.DocumentFullName) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly : false;
        }
        catch
        {
        }

        return true;
    }

    private bool DoIsDocumentOpen()
    {
        bool isOpen = false;
        try
        {
            isOpen = GetAssociatedEngineeringDocument() != null;
        }
        catch
        {
        }

        return isOpen;
    }

    public bool IsOpen()
    {
        try
        {
            DelegateFunction0<bool> isOpen = DoIsDocumentOpen;
            return (bool)MathcadPrimeComServerManager.Instance.InvokeDelegate(isOpen, DispatcherPriority.Normal);
        }
        catch
        {
        }

        return false;
    }

    private IEngineeringDocument GetAssociatedEngineeringDocument()
    {
        IEngineeringDocument engineeringDocument = null;
        if (MathcadPrimeComServerManager.Instance.PrimeWindow.OpenDocumentsList != null)
        {
            engineeringDocument = MathcadPrimeComServerManager.Instance.PrimeWindow.OpenDocumentsList.FirstOrDefault(IsAssociatedEngineeringDocument);
        }

        return engineeringDocument;
    }

    private void DoSetModified(bool isModified)
    {
        try
        {
            IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
            if (engineeringDocument != null)
            {
                engineeringDocument.IsDirty = isModified;
            }
        }
        catch
        {
        }
    }

    private void DoSetTitle(string title)
    {
        try
        {
            IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
            if (engineeringDocument != null)
            {
                engineeringDocument.CustomizedWindowTitle = title;
            }
        }
        catch
        {
        }
    }

    public string GetTitle()
    {
        try
        {
            DelegateFunction0<string> getTitle = DoGetTitle;
            return MathcadPrimeComServerManager.Instance.InvokeDelegate(getTitle, DispatcherPriority.Normal) as string;
        }
        catch
        {
        }

        return null;
    }

    private void DoSetWorksheetTabIcon(string iconFullPath)
    {
        try
        {
            if (!string.IsNullOrEmpty(iconFullPath))
            {
                IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
                if (engineeringDocument != null)
                {
                    engineeringDocument.DocumentTabIcon = iconFullPath;
                }
            }
        }
        catch
        {
        }
    }

    private void DoSetWorksheetTabName(string tabName)
    {
        try
        {
            IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
            if (engineeringDocument != null)
            {
                engineeringDocument.CustomizedDocumentName = tabName;
            }
        }
        catch
        {
        }
    }

    private void DoSetClosingPrompt(string text)
    {
        try
        {
            if (!string.IsNullOrEmpty(text))
            {
                IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
                if (engineeringDocument != null)
                {
                    engineeringDocument.CustomizedClosePrompt = text;
                }
            }
        }
        catch
        {
        }
    }

    private void DoSetWorksheetDisplayedFilePath(string filePath)
    {
        try
        {
            IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
            if (engineeringDocument != null)
            {
                engineeringDocument.CustomizedFilePath = filePath;
            }
        }
        catch
        {
        }
    }

    private void DoSynchronize()
    {
        try
        {
            IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
            if (engineeringDocument != null)
            {
                engineeringDocument.Recalculate();
            }
        }
        catch
        {
        }
    }

    private void DoPauseCalculation()
    {
        UpdateCalculationStatus(false);
    }

    private void DoResumeCalculation()
    {
        UpdateCalculationStatus(true);
    }

    private void DoActivate()
    {
        try
        {
            ActivateWorksheet(GetAssociatedEngineeringDocument());
        }
        catch
        {
        }
    }

    private void ActivateWorksheet(IEngineeringDocument engineeringDocument)
    {
        if (engineeringDocument != null)
        {
            MathcadPrimeComServerManager.Instance.PrimeWindow.ActiveDocument = engineeringDocument;
        }
    }

    private partial class WorksheetData
    {
        private string _documentFullName;
        private string _documentName;

        public WorksheetData(string documentFullName, string documentName, bool isComClientControlled)
        {
            UpdateWorksheetData(documentFullName, documentName);
            if (isComClientControlled)
            {
                SetIsComClientControlled();
            }
        }

        public string DocumentFullName
        {
            get => _documentFullName;
            set => _documentFullName = string.IsNullOrEmpty(value) ? string.Empty : value;
        }

        public string DocumentName => _documentName;

        public void UpdateWorksheetData(string documentFullName, string documentName)
        {
            _documentFullName = documentFullName ?? string.Empty;
            _documentName = documentName ?? string.Empty;
        }

        public void SetIsComClientControlled()
        {
            MathcadPrimeComServerManager.Instance.AddWorksheet(_documentName, _documentFullName);
        }

        public void UnsetIsComClientControlled()
        {
            MathcadPrimeComServerManager.Instance.RemoveWorksheet(_documentName, _documentFullName);
        }

        public bool IsComClientControlled()
        {
            return MathcadPrimeComServerManager.Instance.ListOfControlledWorksheets.Contains(new KeyValuePair<string, string>(DocumentName, DocumentFullName));
        }
    }

    public void Synchronize()
    {
        try
        {
            Delegate0 synchronize = DoSynchronize;
            MathcadPrimeComServerManager.Instance.InvokeDelegate(synchronize, DispatcherPriority.Normal);
        }
        catch
        {
        }
    }

    public void PauseCalculation()
    {
        try
        {
            Delegate0 pauseCalculation = DoPauseCalculation;
            MathcadPrimeComServerManager.Instance.InvokeDelegate(pauseCalculation, DispatcherPriority.Normal);
        }
        catch
        {
        }
    }

    public void ResumeCalculation()
    {
        try
        {
            Delegate0 resumeCalculation = DoResumeCalculation;
            MathcadPrimeComServerManager.Instance.InvokeDelegate(resumeCalculation, DispatcherPriority.Normal);
        }
        catch
        {
        }
    }

    public void Activate()
    {
        try
        {
            Delegate0 activate = DoActivate;
            MathcadPrimeComServerManager.Instance.InvokeDelegate(activate, DispatcherPriority.Normal);
        }
        catch
        {
        }
    }

    private void UpdateCalculationStatus(bool status)
    {
        try
        {
            IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
            if (engineeringDocument != null)
            {
                engineeringDocument.IsCalculationsEnabled = status;
                if (status && !engineeringDocument.IsAutoCalcOn)
                {
                    engineeringDocument.IsAutoCalcOn = true;
                }
            }
        }
        catch
        {
        }
    }

    public void Save()
    {
        bool isSaved = false;
        try
        {
            DelegateFunction0<bool> save = DoSave;
            isSaved = (bool)MathcadPrimeComServerManager.Instance.InvokeDelegate(save, DispatcherPriority.Normal);
        }
        catch
        {
            isSaved = false;
        }

        if (!isSaved && _isUiInteractiveModeDisabled)
        {
            Marshal.ThrowExceptionForHR(unchecked((int)0x80004004));
        }
    }

    private bool DoSave()
    {
        try
        {
            return DoSave(GetAssociatedEngineeringDocument());
        }
        catch
        {
        }

        return false;
    }

    private AutomationSpiritInputs DoGetInputs()
    {
        try
        {
            IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
            if (engineeringDocument != null)
            {
                return new AutomationSpiritInputs(engineeringDocument.InputOutputRegionManager.Inputs);
            }
        }
        catch
        {
        }

        return null;
    }

    private AutomationSpiritOutputs DoGetOutputs()
    {
        try
        {
            IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
            if (engineeringDocument != null)
            {
                return new AutomationSpiritOutputs(engineeringDocument.InputOutputRegionManager.Outputs);
            }
        }
        catch
        {
        }

        return null;
    }

    private void DoSetWorksheetWorkingDirectory(string workingDir)
    {
        try
        {
            IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
            if (engineeringDocument != null)
            {
                engineeringDocument.CWD = workingDir;
            }
        }
        catch
        {
        }
    }

    public IMathcadPrimeValuesSetter CreateValuesSetter()
    {
        try
        {
            IValuesSetter valuesSetter = DoCreateScaledValueSettersCollection();
            return valuesSetter != null ? new AutomationSpiritValuesSetter(valuesSetter) : null;
        }
        catch
        {
        }

        return null;
    }

    private IOManagerErrorCode DoSetStringValue(string alias, string stringValue)
    {
        try
        {
            IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
            return engineeringDocument != null ? engineeringDocument.InputOutputRegionManager.SetStringValue(alias, stringValue) : (IOManagerErrorCode)(-1);
        }
        catch
        {
        }

        return (IOManagerErrorCode)(-1);
    }

    private IOManagerErrorCode DoSetSExprValue(string alias, string sexpression)
    {
        try
        {
            IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
            return engineeringDocument != null ? engineeringDocument.InputOutputRegionManager.SetSExprValue(alias, sexpression) : (IOManagerErrorCode)(-1);
        }
        catch
        {
        }

        return (IOManagerErrorCode)(-1);
    }
}
