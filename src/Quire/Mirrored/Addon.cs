using System.Runtime.InteropServices;

namespace Quire;

public partial class Addon
{
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
}
