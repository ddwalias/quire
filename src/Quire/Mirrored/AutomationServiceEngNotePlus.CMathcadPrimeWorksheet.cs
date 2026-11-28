using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Threading;
using Quire.Controls.Core;
using Quire.Controls.Core.IORegions;
using Quire.EngNotebookPlus.Automation;

namespace Quire.AutomationServiceEngNotePlus;

public partial class CMathcadPrimeWorksheet
{
    private partial class WorksheetData
    {
        private string _documentFullName;
        private string _documentName;

        public string DocumentFullName
        {
            get => _documentFullName;
            set => _documentFullName = string.IsNullOrEmpty(value) ? string.Empty : value;
        }

        public string DocumentName => _documentName;

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

        public WorksheetData(string documentFullName, string documentName, bool isComClientControlled)
        {
            UpdateWorksheetData(documentFullName, documentName);
            if (isComClientControlled)
            {
                SetIsComClientControlled();
            }
        }

        public void UpdateWorksheetData(string documentFullName, string documentName)
        {
            _documentFullName = documentFullName ?? string.Empty;
            _documentName = documentName ?? string.Empty;
        }
    }

    private WorksheetData worksheetData { get; set; }

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

    public bool Modified
    {
        get
        {
            try
            {
                DelegateFunction0<bool> getModified = DoGetModified;
                return (bool)MathcadPrimeComServerManager.Instance.InvokeDelegate(getModified, DispatcherPriority.Normal);
            }
            catch
            {
            }

            return false;
        }
        set
        {
            try
            {
                Delegate0 setModified = () =>
                {
                    DoSetModified(value);
                };
                MathcadPrimeComServerManager.Instance.InvokeDelegate(setModified, DispatcherPriority.Normal);
            }
            catch
            {
            }
        }
    }

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

    public void SetTitle(string titleArg)
    {
        try
        {
            Delegate0 setTitle = () =>
            {
                DoSetTitle(titleArg);
            };
            MathcadPrimeComServerManager.Instance.InvokeDelegate(setTitle, DispatcherPriority.Normal);
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

    private bool DoSave(IEngineeringDocument engineeringDocument)
    {
        bool isSaved = false;
        try
        {
            MathcadPrimeComServerManager.Instance.DisableMessageBoxManager(_isUiInteractiveModeDisabled);
            if (engineeringDocument != null && !engineeringDocument.IsNew)
            {
                ActivateWorksheet(engineeringDocument);
                int version = engineeringDocument.MetadataModel.Version;
                MathcadPrimeComServerManager.Instance.PrimeWindow.SaveDocumentWithoutInputsOutputsStates(engineeringDocument);
                isSaved = engineeringDocument.MetadataModel.Version > version;
                worksheetData.SetIsComClientControlled();
            }
        }
        finally
        {
            MathcadPrimeComServerManager.Instance.EnableMessageBoxManager(_isUiInteractiveModeDisabled);
        }

        return isSaved;
    }

    public void SaveAs(string fileName)
    {
        bool isSaved = false;
        try
        {
            if (!string.IsNullOrEmpty(fileName))
            {
                DelegateFunction0<bool> saveAs = () => DoSaveAs(fileName);
                isSaved = (bool)MathcadPrimeComServerManager.Instance.InvokeDelegate(saveAs, DispatcherPriority.Normal);
            }
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

    private bool DoSaveAs(string newDocumentPath)
    {
        long? maxUpdateLayoutTimeMsec = null;
        IEngineeringDocument engineeringDocument = null;
        bool isSaved = false;
        try
        {
            MathcadPrimeComServerManager.Instance.DisableMessageBoxManager(_isUiInteractiveModeDisabled);
            engineeringDocument = GetAssociatedEngineeringDocument();
            if (engineeringDocument != null)
            {
                ActivateWorksheet(engineeringDocument);
                string extension = Path.GetExtension(newDocumentPath);
                bool isPrimeFormat = FileManager.IsPrimeExtension(extension);
                bool isPdf = string.Equals(extension, ".pdf", StringComparison.OrdinalIgnoreCase);
                bool isSavedToPrimeFile = false;
                maxUpdateLayoutTimeMsec = engineeringDocument.MaxUpdateLayoutTimeMsec;
                engineeringDocument.MaxUpdateLayoutTimeMsec = 600000L;
                if (isPdf)
                {
                    isSavedToPrimeFile = false;
                    isSaved = TrySaveAsPdf(engineeringDocument, newDocumentPath);
                }
                else if (isPrimeFormat)
                {
                    if (TrySaveAsToPtcServer(newDocumentPath))
                    {
                        isSavedToPrimeFile = true;
                        isSaved = true;
                    }
                    else if (TrySaveAsInMathcadPrimeFormatLocally(engineeringDocument, newDocumentPath))
                    {
                        isSavedToPrimeFile = true;
                        isSaved = true;
                    }
                    else
                    {
                        isSavedToPrimeFile = false;
                        isSaved = false;
                    }
                }
                else
                {
                    isSavedToPrimeFile = false;
                    isSaved = TrySaveAsInNonMathcadPrimeFormatLocally(engineeringDocument, newDocumentPath);
                }

                if (isSavedToPrimeFile)
                {
                    IList<IEngineeringDocument> openDocuments = MathcadPrimeComServerManager.Instance.PrimeWindow.OpenDocumentsList;
                    for (int i = openDocuments.Count - 1; i > -1; i--)
                    {
                        IEngineeringDocument openDocument = openDocuments[i];
                        if (openDocument != null && openDocument.FilePath != null && openDocument.FilePath.Equals(newDocumentPath, StringComparison.OrdinalIgnoreCase))
                        {
                            worksheetData.UnsetIsComClientControlled();
                            worksheetData.UpdateWorksheetData(openDocument.FilePath, openDocument.DocumentName);
                            worksheetData.SetIsComClientControlled();
                            break;
                        }
                    }
                }
            }
        }
        catch
        {
            isSaved = false;
        }
        finally
        {
            MathcadPrimeComServerManager.Instance.EnableMessageBoxManager(_isUiInteractiveModeDisabled);
            if (maxUpdateLayoutTimeMsec.HasValue)
            {
                engineeringDocument.MaxUpdateLayoutTimeMsec = maxUpdateLayoutTimeMsec.Value;
            }
        }

        return isSaved;
    }

    private bool TrySaveAsPdf(IEngineeringDocument engineeringDocument, string newDocumentPath)
    {
        if (SpiritMainWindow.Addons != null && SpiritMainWindow.Addons.CallAddonsToGeneratePdfFile(engineeringDocument, newDocumentPath))
        {
            return true;
        }

        return TrySaveAsInNonMathcadPrimeFormatLocally(engineeringDocument, newDocumentPath);
    }

    private bool TrySaveAsToPtcServer(string newDocumentPath)
    {
        if (SpiritMainWindow.Addons != null)
        {
            return SpiritMainWindow.Addons.CallAddonsSaveToPtcServer(worksheetData.DocumentFullName, newDocumentPath);
        }

        return true;
    }

    private bool TrySaveAsInMathcadPrimeFormatLocally(IEngineeringDocument engineeringDocument, string newDocumentPath)
    {
        bool isSaved = false;
        if (engineeringDocument != null)
        {
            int version = engineeringDocument.MetadataModel.Version;
            isSaved = MathcadPrimeComServerManager.Instance.PrimeWindow.SaveDocumentWithFilename(engineeringDocument, newDocumentPath);
            int newVersion = engineeringDocument.MetadataModel.Version;
            DebugTools.DevAssert(isSaved == newVersion > version, null);
        }

        return isSaved;
    }

    private bool TrySaveAsInNonMathcadPrimeFormatLocally(IEngineeringDocument engineeringDocument, string newDocumentPath)
    {
        if (engineeringDocument != null)
        {
            return MathcadPrimeComServerManager.Instance.PrimeWindow.SaveDocumentAsNonPrimeFormat_ForApiOnly(engineeringDocument, newDocumentPath);
        }

        return false;
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

    private void DoPauseCalculation()
    {
        UpdateCalculationStatus(false);
    }

    private void DoResumeCalculation()
    {
        UpdateCalculationStatus(true);
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

    public int SetRealValue(string aliasArg, double valueArg, string unitsArg)
    {
        try
        {
            DelegateFunction0<IOManagerErrorCode> setRealValue = () => DoSetRealValue(aliasArg, valueArg, unitsArg);
            return (int)MathcadPrimeComServerManager.Instance.InvokeDelegate(setRealValue, DispatcherPriority.Normal);
        }
        catch
        {
        }

        return -1;
    }

    private IOManagerErrorCode DoSetRealValue(string alias, double value, string units)
    {
        try
        {
            IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
            return engineeringDocument != null ? engineeringDocument.InputOutputRegionManager.SetValue(alias, value, units) : IOManagerErrorCode.UnknownError;
        }
        catch
        {
        }

        return IOManagerErrorCode.UnknownError;
    }

    public IMathcadPrimeInputs Inputs
    {
        get
        {
            try
            {
                DelegateFunction0<AutomationSpiritInputs> getInputs = DoGetInputs;
                return MathcadPrimeComServerManager.Instance.InvokeDelegate(getInputs, DispatcherPriority.Normal) as IMathcadPrimeInputs;
            }
            catch
            {
            }

            return null;
        }
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

    public IMathcadPrimeOutputs Outputs
    {
        get
        {
            try
            {
                DelegateFunction0<AutomationSpiritOutputs> getOutputs = DoGetOutputs;
                return MathcadPrimeComServerManager.Instance.InvokeDelegate(getOutputs, DispatcherPriority.Normal) as IMathcadPrimeOutputs;
            }
            catch
            {
            }

            return null;
        }
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

    public IMathcadPrimeInputResult InputGetRealValue(string aliasArg)
    {
        try
        {
            IScalarItem scalarItem = DoGetRealValue(aliasArg, isInput: true);
            return scalarItem != null ? new AutomationSpiritInputResult(scalarItem.ScalarState) : null;
        }
        catch
        {
        }

        return null;
    }

    public IMathcadPrimeOutputResult OutputGetRealValue(string aliasArg)
    {
        try
        {
            IScalarItem scalarItem = DoGetRealValue(aliasArg, isInput: false);
            return scalarItem != null ? new AutomationSpiritOutputResult(scalarItem.ScalarState) : null;
        }
        catch
        {
        }

        return null;
    }

    private IScalarItem DoGetRealValue(string alias, bool isInput)
    {
        DelegateFunction0<IScalarItem> getRealValue = () =>
        {
            try
            {
                IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
                return engineeringDocument == null ? null : engineeringDocument.InputOutputRegionManager.InputOutputGetRealValue(alias, isInput);
            }
            catch
            {
            }

            return (IScalarItem)null;
        };
        return (IScalarItem)MathcadPrimeComServerManager.Instance.InvokeDelegate(getRealValue, DispatcherPriority.Normal);
    }

    public IMathcadPrimeOutputResultAs OutputGetRealValueAs(string aliasArg, string unitsArg)
    {
        try
        {
            IScalarItem scalarItem = DoOutputGetRealValueAs(aliasArg, unitsArg);
            return scalarItem != null ? new AutomationSpiritOutputResultAs(scalarItem.ScalarState) : null;
        }
        catch
        {
        }

        return null;
    }

    private IScalarItem DoOutputGetRealValueAs(string alias, string units)
    {
        DelegateFunction0<IScalarItem> getRealValueAs = () =>
        {
            try
            {
                IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
                return engineeringDocument == null ? null : engineeringDocument.InputOutputRegionManager.InputOutputGetRealValueAs(alias, isInp: false, units);
            }
            catch
            {
            }

            return (IScalarItem)null;
        };
        return (IScalarItem)MathcadPrimeComServerManager.Instance.InvokeDelegate(getRealValueAs, DispatcherPriority.Normal);
    }

    private readonly bool _isUiInteractiveModeDisabled;

    public IMathcadPrimeMatrix CreateMatrix(int rowsArg, int columnsArg)
    {
        try
        {
            if (rowsArg > 0 && columnsArg > 0)
            {
                return new AutomationSpiritMatrix(rowsArg, columnsArg);
            }
        }
        catch
        {
        }

        return null;
    }

    public int SetMatrixValue(string aliasArg, IMathcadPrimeMatrix valueArg, string unitsArg)
    {
        try
        {
            DelegateFunction0<IOManagerErrorCode> setMatrixValue = () => DoSetMatrixValue(aliasArg, valueArg, unitsArg);
            return (int)MathcadPrimeComServerManager.Instance.InvokeDelegate(setMatrixValue, DispatcherPriority.Normal);
        }
        catch
        {
        }

        return -1;
    }

    private IOManagerErrorCode DoSetMatrixValue(string alias, IMathcadPrimeMatrix value, string units)
    {
        try
        {
            IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
            return engineeringDocument != null ? engineeringDocument.InputOutputRegionManager.SetValue(alias, value.Matrix, units) : IOManagerErrorCode.UnknownError;
        }
        catch
        {
        }

        return IOManagerErrorCode.UnknownError;
    }

    public IMathcadPrimeInputMatrixResult InputGetMatrixValue(string aliasArg)
    {
        try
        {
            IMatrixItem matrixItem = DoGetMatrixValue(aliasArg, isInput: true);
            return matrixItem != null ? new AutomationSpiritInputMatrixResult(matrixItem.MatrixState) : null;
        }
        catch
        {
        }

        return null;
    }

    public IMathcadPrimeOutputMatrixResult OutputGetMatrixValue(string aliasArg)
    {
        try
        {
            IMatrixItem matrixItem = DoGetMatrixValue(aliasArg, isInput: false);
            return matrixItem != null ? new AutomationSpiritOutputMatrixResult(matrixItem.MatrixState) : null;
        }
        catch
        {
        }

        return null;
    }

    private IMatrixItem DoGetMatrixValue(string alias, bool isInput)
    {
        DelegateFunction0<IMatrixItem> getMatrixValue = () =>
        {
            try
            {
                IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
                return engineeringDocument == null ? null : engineeringDocument.InputOutputRegionManager.InputOutputGetMatrixValue(alias, isInput);
            }
            catch
            {
            }

            return (IMatrixItem)null;
        };
        return (IMatrixItem)MathcadPrimeComServerManager.Instance.InvokeDelegate(getMatrixValue, DispatcherPriority.Normal);
    }

    public IMathcadPrimeOutputMatrixResultAs OutputGetMatrixValueAs(string aliasArg, string unitsArg)
    {
        try
        {
            IMatrixItem matrixItem = DoOutputGetMatrixValueAs(aliasArg, unitsArg);
            return matrixItem != null ? new AutomationSpiritOutputMatrixResultAs(matrixItem.MatrixState) : null;
        }
        catch
        {
        }

        return null;
    }

    private IMatrixItem DoOutputGetMatrixValueAs(string alias, string units)
    {
        DelegateFunction0<IMatrixItem> getMatrixValueAs = () =>
        {
            try
            {
                IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
                return engineeringDocument == null ? null : engineeringDocument.InputOutputRegionManager.InputOutputGetMatrixValueAs(alias, isInp: false, units);
            }
            catch
            {
            }

            return (IMatrixItem)null;
        };
        return (IMatrixItem)MathcadPrimeComServerManager.Instance.InvokeDelegate(getMatrixValueAs, DispatcherPriority.Normal);
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

    public void Close(SaveOption saveOptionArg)
    {
        try
        {
            Delegate0 close = () =>
            {
                DoClose(saveOptionArg);
            };
            MathcadPrimeComServerManager.Instance.InvokeDelegate(close, DispatcherPriority.Normal);
        }
        catch
        {
        }
    }

    private void DoClose(SaveOption saveOption)
    {
        try
        {
            bool isComClientControlled = worksheetData.IsComClientControlled();
            IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
            if (engineeringDocument != null)
            {
                ActivateWorksheet(engineeringDocument);
                switch (saveOption)
                {
                    case SaveOption.spSaveChanges:
                        DoSave(engineeringDocument);
                        break;
                    case SaveOption.spDiscardChanges:
                        engineeringDocument.IsDirty = false;
                        break;
                    case SaveOption.spPromptToSaveUnrelatedChanges:
                        if (isComClientControlled)
                        {
                            engineeringDocument.IsDirty = false;
                        }

                        break;
                }

                bool isClosed = MathcadPrimeComServerManager.Instance.PrimeWindow.HandleCloseEngineeringDocument(engineeringDocument);
                if (!isClosed && !isComClientControlled && saveOption == SaveOption.spPromptToSaveUnrelatedChanges)
                {
                    while (!isClosed)
                    {
                        isClosed = MathcadPrimeComServerManager.Instance.PrimeWindow.HandleCloseEngineeringDocument(engineeringDocument);
                    }
                }

                if (isClosed & isComClientControlled)
                {
                    worksheetData.UnsetIsComClientControlled();
                }
            }
        }
        catch
        {
        }
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

    private bool IsAssociatedEngineeringDocument(IEngineeringDocument engineeringDocument)
    {
        bool isAssociated = false;
        if (engineeringDocument != null)
        {
            string filePath = engineeringDocument.FilePath;
            string documentName = engineeringDocument.DocumentName;
            if ((filePath == null && string.IsNullOrEmpty(worksheetData.DocumentFullName) && !string.IsNullOrEmpty(documentName) && !string.IsNullOrEmpty(worksheetData.DocumentName) && documentName.Equals(worksheetData.DocumentName, StringComparison.OrdinalIgnoreCase)) || (filePath != null && filePath.Equals(worksheetData.DocumentFullName ?? string.Empty, StringComparison.OrdinalIgnoreCase)))
            {
                isAssociated = true;
            }
        }

        return isAssociated;
    }

    public string WorksheetTabIcon
    {
        get
        {
            try
            {
                DelegateFunction0<string> getTabIcon = DoGetWorksheetTabIcon;
                return MathcadPrimeComServerManager.Instance.InvokeDelegate(getTabIcon, DispatcherPriority.Normal) as string;
            }
            catch
            {
            }

            return null;
        }
        set
        {
            try
            {
                Delegate0 setTabIcon = () =>
                {
                    DoSetWorksheetTabIcon(value);
                };
                MathcadPrimeComServerManager.Instance.InvokeDelegate(setTabIcon, DispatcherPriority.Normal);
            }
            catch
            {
            }
        }
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

    public string WorksheetTabName
    {
        get
        {
            try
            {
                DelegateFunction0<string> getTabName = DoGetWorksheetTabName;
                return MathcadPrimeComServerManager.Instance.InvokeDelegate(getTabName, DispatcherPriority.Normal) as string;
            }
            catch
            {
            }

            return null;
        }
        set
        {
            try
            {
                Delegate0 setTabName = () =>
                {
                    DoSetWorksheetTabName(value);
                };
                MathcadPrimeComServerManager.Instance.InvokeDelegate(setTabName, DispatcherPriority.Normal);
            }
            catch
            {
            }
        }
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

    public string WorksheetClosingPrompt
    {
        get
        {
            try
            {
                DelegateFunction0<string> getClosingPrompt = DoGetClosingPrompt;
                return MathcadPrimeComServerManager.Instance.InvokeDelegate(getClosingPrompt, DispatcherPriority.Normal) as string;
            }
            catch
            {
            }

            return null;
        }
        set
        {
            try
            {
                Delegate0 setClosingPrompt = () =>
                {
                    DoSetClosingPrompt(value);
                };
                MathcadPrimeComServerManager.Instance.InvokeDelegate(setClosingPrompt, DispatcherPriority.Normal);
            }
            catch
            {
            }
        }
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

    public string WorksheetDisplayedFilePath
    {
        get
        {
            try
            {
                DelegateFunction0<string> getDisplayedFilePath = DoGetWorksheetDisplayedFilePath;
                return MathcadPrimeComServerManager.Instance.InvokeDelegate(getDisplayedFilePath, DispatcherPriority.Normal) as string;
            }
            catch
            {
            }

            return null;
        }
        set
        {
            try
            {
                Delegate0 setDisplayedFilePath = () =>
                {
                    DoSetWorksheetDisplayedFilePath(value);
                };
                MathcadPrimeComServerManager.Instance.InvokeDelegate(setDisplayedFilePath, DispatcherPriority.Normal);
            }
            catch
            {
            }
        }
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

    public string WorksheetWorkingDirectory
    {
        get
        {
            try
            {
                DelegateFunction0<string> getWorkingDirectory = DoGetWorksheetWorkingDirectory;
                return MathcadPrimeComServerManager.Instance.InvokeDelegate(getWorkingDirectory, DispatcherPriority.Normal) as string;
            }
            catch
            {
            }

            return null;
        }
        set
        {
            try
            {
                Delegate0 setWorkingDirectory = () =>
                {
                    DoSetWorksheetWorkingDirectory(value);
                };
                MathcadPrimeComServerManager.Instance.InvokeDelegate(setWorkingDirectory, DispatcherPriority.Normal);
            }
            catch
            {
            }
        }
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

    public object GetWorksheetReadOnlyOptionValue(WorksheetReadonlyOptionNames optionNameArg)
    {
        try
        {
            DelegateFunction0<object> getOptionValue = () => DoGetWorksheetReadOnlyOptionValue(optionNameArg);
            return MathcadPrimeComServerManager.Instance.InvokeDelegate(getOptionValue, DispatcherPriority.Normal);
        }
        catch
        {
        }

        return null;
    }

    private object DoGetWorksheetReadOnlyOptionValue(WorksheetReadonlyOptionNames optionName)
    {
        try
        {
            object optionValue = null;
            IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
            if (engineeringDocument != null)
            {
                switch (optionName)
                {
                    case WorksheetReadonlyOptionNames.FileLocationHistoryDisabled:
                        optionValue = !engineeringDocument.ReadonlyOptions.FileLocationHistoryEnabled;
                        break;
                    case WorksheetReadonlyOptionNames.OperationsWithEnabledStateGeneration:
                    {
                        WorksheetOperations operations = WorksheetOperations.None;
                        if (engineeringDocument.ReadonlyOptions.GenerateInputsOutputsStatesOnSave)
                        {
                            operations |= WorksheetOperations.Save;
                        }

                        optionValue = operations;
                        break;
                    }
                    case WorksheetReadonlyOptionNames.RequestToUpdateInputsEnabled:
                        optionValue = engineeringDocument.ReadonlyOptions.RequestToUpdateInputsEnabled;
                        break;
                    case WorksheetReadonlyOptionNames.CaseInsensitiveAliasComparisonEnabled:
                        optionValue = engineeringDocument.ReadonlyOptions.CaseInsensitiveAliasComparisonEnabled;
                        break;
                    default:
                        optionValue = null;
                        break;
                }
            }

            return optionValue;
        }
        catch
        {
        }

        return null;
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

    private IValuesSetter DoCreateScaledValueSettersCollection()
    {
        DelegateFunction0<IValuesSetter> createSetters = () =>
        {
            try
            {
                IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
                return engineeringDocument == null ? null : engineeringDocument.InputOutputRegionManager.CreateScaledValueSettersCollection();
            }
            catch
            {
            }

            return (IValuesSetter)null;
        };
        return (IValuesSetter)MathcadPrimeComServerManager.Instance.InvokeDelegate(createSetters, DispatcherPriority.Normal);
    }

    public int SetStringValue(string aliasArg, string valueArg)
    {
        try
        {
            DelegateFunction0<IOManagerErrorCode> setStringValue = () => DoSetStringValue(aliasArg, valueArg);
            return (int)MathcadPrimeComServerManager.Instance.InvokeDelegate(setStringValue, DispatcherPriority.Normal);
        }
        catch
        {
        }

        return -1;
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

    private string DoGetStringValue(string alias, bool isInput)
    {
        DelegateFunction0<string> getStringValue = () =>
        {
            try
            {
                IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
                IStringItem stringItem = engineeringDocument == null ? null : engineeringDocument.InputOutputRegionManager.InputOutputGetStringValue(alias, isInput);
                return stringItem != null && stringItem.StringState != null ? stringItem.StringState.Value : null;
            }
            catch
            {
            }

            return (string)null;
        };
        return (string)MathcadPrimeComServerManager.Instance.InvokeDelegate(getStringValue, DispatcherPriority.Normal);
    }

    public int SetSExprValue(string aliasArg, string sexpressionArg)
    {
        try
        {
            DelegateFunction0<IOManagerErrorCode> setSExprValue = () => DoSetSExprValue(aliasArg, sexpressionArg);
            return (int)MathcadPrimeComServerManager.Instance.InvokeDelegate(setSExprValue, DispatcherPriority.Normal);
        }
        catch
        {
        }

        return -1;
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

    private string DoInputGetSExprValue(string alias)
    {
        DelegateFunction0<string> getSExprValue = () =>
        {
            try
            {
                IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
                return engineeringDocument == null ? null : engineeringDocument.InputOutputRegionManager.InputGetSExprValue(alias);
            }
            catch
            {
            }

            return (string)null;
        };
        return (string)MathcadPrimeComServerManager.Instance.InvokeDelegate(getSExprValue, DispatcherPriority.Normal);
    }

    public IMathcadPrimeGetValueResult InputGetValue(string aliasArg)
    {
        try
        {
            ICommonValueItem commonValueItem = DoGetCommonValue(aliasArg, isInput: true);
            return commonValueItem != null ? new AutomationGetValueResult(commonValueItem) : null;
        }
        catch
        {
        }

        return null;
    }

    public IMathcadPrimeGetValueResult OutputGetValue(string aliasArg)
    {
        try
        {
            ICommonValueItem commonValueItem = DoGetCommonValue(aliasArg, isInput: false);
            return commonValueItem != null ? new AutomationGetValueResult(commonValueItem) : null;
        }
        catch
        {
        }

        return null;
    }

    private ICommonValueItem DoGetCommonValue(string alias, bool isInput)
    {
        DelegateFunction0<ICommonValueItem> getCommonValue = () =>
        {
            try
            {
                IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
                return engineeringDocument == null ? null : engineeringDocument.InputOutputRegionManager.InputOutputGetCalculatedValue(alias, isInput);
            }
            catch
            {
            }

            return (ICommonValueItem)null;
        };
        return (ICommonValueItem)MathcadPrimeComServerManager.Instance.InvokeDelegate(getCommonValue, DispatcherPriority.Normal);
    }

    public int DefaultCalculationTimeout
    {
        get
        {
            try
            {
                DelegateFunction0<int> getTimeout = DoGetDefaultCalculationTimeout;
                return (int)MathcadPrimeComServerManager.Instance.InvokeDelegate(getTimeout, DispatcherPriority.Normal);
            }
            catch
            {
            }

            return 0;
        }
        set
        {
            try
            {
                Delegate0 setTimeout = () =>
                {
                    DoSetDefaultCalculationTimeout(value);
                };
                MathcadPrimeComServerManager.Instance.InvokeDelegate(setTimeout, DispatcherPriority.Normal);
            }
            catch
            {
            }
        }
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

    private void DoSetDefaultCalculationTimeout(int defaultCalculationTimeout)
    {
        try
        {
            IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
            if (engineeringDocument != null)
            {
                engineeringDocument.InputOutputRegionManager.DefaultCalculationTimeout = defaultCalculationTimeout;
            }
        }
        catch
        {
        }
    }

    public void SaveAndPush(SaveOption saveOptionArg)
    {
        try
        {
            if (saveOptionArg == SaveOption.spDiscardChanges)
            {
                return;
            }

            DelegateFunction0<bool> saveAndPush = () => DoSaveAndPush(saveOptionArg);
            MathcadPrimeComServerManager.Instance.InvokeDelegate(saveAndPush, DispatcherPriority.Normal);
        }
        catch
        {
        }
    }

    private bool DoSaveAndPush(SaveOption saveOption)
    {
        bool isSaved = false;
        try
        {
            if (saveOption == SaveOption.spSaveChanges)
            {
                MathcadPrimeComServerManager.Instance.DisableMessageBoxManager(isUiInteractiveModeDisabled: true);
            }

            IEngineeringDocument engineeringDocument = GetAssociatedEngineeringDocument();
            if (engineeringDocument != null)
            {
                isSaved = MathcadPrimeComServerManager.Instance.PrimeWindow.SaveAndPushEngNote(engineeringDocument);
            }
        }
        catch
        {
            isSaved = false;
        }
        finally
        {
            MathcadPrimeComServerManager.Instance.EnableMessageBoxManager(_isUiInteractiveModeDisabled);
        }

        return isSaved;
    }
}
