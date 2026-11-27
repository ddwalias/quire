using System.Runtime.InteropServices;

namespace Quire;

public partial class Addon
{
    private nint _hModule = nint.Zero;

    public void UnloadAddon()
    {
        if (_hModule != nint.Zero)
        {
            CallAddonCleanup();
            try
            {
                AddonsLoaderHelper.FreeLibrary(_hModule);
                _hModule = nint.Zero;
            }
            catch
            {
            }
        }
    }

    public void CallAddonOnIdle()
    {
        try
        {
            nint functionAddress = GetAddressOfFunction(FnOnIdleTime);
            if (functionAddress != nint.Zero)
            {
                OnIdle onIdle = Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(OnIdle)) as OnIdle;
                if (onIdle != null)
                {
                    onIdle.Invoke();
                }
            }
        }
        catch
        {
        }
    }

    public void CallAddonWhenDocumentOpened(string documentPath)
    {
        try
        {
            nint functionAddress = GetAddressOfFunction(FnDocumentOpened);
            if (functionAddress != nint.Zero)
            {
                DocumentOpened documentOpened = Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(DocumentOpened)) as DocumentOpened;
                if (documentOpened != null)
                {
                    documentOpened.Invoke(documentPath);
                }
            }
        }
        catch
        {
        }
    }

    public void CallAddonWhenDocumentClosing(string documentPath)
    {
        try
        {
            nint functionAddress = GetAddressOfFunction(FnDocumentClosing);
            if (functionAddress != nint.Zero)
            {
                DocumentClosing documentClosing = Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(DocumentClosing)) as DocumentClosing;
                if (documentClosing != null)
                {
                    documentClosing.Invoke(documentPath);
                }
            }
        }
        catch
        {
        }
    }

    public void CallAddonWhenDocumentSaving(string documentPath)
    {
        try
        {
            nint functionAddress = GetAddressOfFunction(FnDocumentSaving);
            if (functionAddress != nint.Zero)
            {
                DocumentSaving documentSaving = Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(DocumentSaving)) as DocumentSaving;
                if (documentSaving != null)
                {
                    documentSaving.Invoke(documentPath);
                }
            }
        }
        catch
        {
        }
    }

    public void CallAddonWhenDocumentSaved(string documentPath)
    {
        try
        {
            nint functionAddress = GetAddressOfFunction(FnDocumentSaved);
            if (functionAddress != nint.Zero)
            {
                DocumentSaved documentSaved = Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(DocumentSaved)) as DocumentSaved;
                if (documentSaved != null)
                {
                    documentSaved.Invoke(documentPath);
                }
            }
        }
        catch
        {
        }
    }

    public void CallAddonOnCommand(int cmdID)
    {
        try
        {
            nint functionAddress = GetAddressOfFunction(FnOnCommand);
            if (functionAddress != nint.Zero)
            {
                OnCommand onCommand = Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(OnCommand)) as OnCommand;
                if (onCommand != null)
                {
                    onCommand.Invoke(cmdID);
                }
            }
        }
        catch
        {
        }
    }

    private nint GetAddressOfFunction(string functionName)
    {
        if (_hModule == nint.Zero)
        {
            return nint.Zero;
        }

        return AddonsLoaderHelper.GetProcAddress(_hModule, functionName);
    }

    private void CallAddonCleanup()
    {
        try
        {
            nint functionAddress = GetAddressOfFunction(FnCleanup);
            if (functionAddress != nint.Zero)
            {
                Cleanup cleanup = Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(Cleanup)) as Cleanup;
                if (cleanup != null)
                {
                    cleanup.Invoke();
                }
            }
        }
        catch
        {
        }
    }

    internal Addon(string addOnId)
    {
        AddOnId = addOnId;
    }

    public string AddOnId { get; private set; }

    public bool LoadAddon(string pathToAddonLoader, global::Quire.Automation.IMathcadPrimeApplication mathcadPrimeApplicationPtr)
    {
        bool loaded = false;
        try
        {
            _hModule = AddonsLoaderHelper.LoadLibrary(pathToAddonLoader);
            if (_hModule != nint.Zero)
            {
                loaded = CallAddonInitialize(mathcadPrimeApplicationPtr);
            }
        }
        catch
        {
        }

        return loaded;
    }

    private bool CallAddonInitialize(global::Quire.Automation.IMathcadPrimeApplication cMathcadPrimeApplicationPtr)
    {
        bool initialized = false;
        try
        {
            nint functionAddress = GetAddressOfFunction(FnInitializeAddOnForPrime);
            if (functionAddress != nint.Zero)
            {
                Initialize initialize = Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(Initialize)) as Initialize;
                if (initialize != null)
                {
                    initialize.Invoke(cMathcadPrimeApplicationPtr);
                }

                initialized = true;
            }
        }
        catch
        {
        }

        return initialized;
    }

    public string CallAddonToGetResolvedFileNameForOpenAlloc(string documentPath)
    {
        string resolvedFileName = null;
        try
        {
            nint functionAddress = GetAddressOfFunction(FnGetResolvedFileNameAlloc);
            if (functionAddress != nint.Zero)
            {
                GetResolvedFileNameAlloc getResolvedFileNameAlloc = Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(GetResolvedFileNameAlloc)) as GetResolvedFileNameAlloc;
                if (getResolvedFileNameAlloc != null)
                {
                    resolvedFileName = Marshal.PtrToStringAuto(getResolvedFileNameAlloc.Invoke(0, documentPath));
                }
            }
        }
        catch
        {
        }

        return resolvedFileName;
    }

    public string CallAddonToGetDefaultDirectory(int dlgType)
    {
        string defaultDirectory = null;
        try
        {
            nint functionAddress = GetAddressOfFunction(FnGetDefaultDirectoryForPrime);
            if (functionAddress != nint.Zero)
            {
                GetDefaultDirectory getDefaultDirectory = Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(GetDefaultDirectory)) as GetDefaultDirectory;
                if (getDefaultDirectory != null)
                {
                    defaultDirectory = Marshal.PtrToStringAuto(getDefaultDirectory.Invoke(dlgType));
                }
            }
        }
        catch
        {
        }

        return defaultDirectory;
    }

    public bool CallAddonUpdateMenuItem(int cmdID)
    {
        bool updated = false;
        try
        {
            nint functionAddress = GetAddressOfFunction(FnOnUpdateUIForPrime);
            if (functionAddress != nint.Zero)
            {
                UpdateMenuItem updateMenuItem = Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(UpdateMenuItem)) as UpdateMenuItem;
                if (updateMenuItem != null)
                {
                    updated = updateMenuItem.Invoke(cmdID) != 0;
                }
            }
        }
        catch
        {
        }

        return updated;
    }

    public bool CallAddonToCheckIsAddOnActive()
    {
        bool isActive = false;
        try
        {
            nint functionAddress = GetAddressOfFunction(FnIsAddOnActive);
            if (functionAddress != nint.Zero)
            {
                IsAddOnActive isAddOnActive = Marshal.GetDelegateForFunctionPointer(functionAddress, typeof(IsAddOnActive)) as IsAddOnActive;
                if (isAddOnActive != null)
                {
                    isActive = isAddOnActive.Invoke() != 0;
                }
            }
        }
        catch
        {
        }

        return isActive;
    }
}
