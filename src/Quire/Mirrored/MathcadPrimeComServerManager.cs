using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using Quire.Services;
using Quire.Wpf;

namespace Quire;

internal partial class MathcadPrimeComServerManager
{
    private static bool Succeeded(int hResult)
    {
        return hResult == 0;
    }

    private string RuntimeKey { get; set; }

    internal string RuntimeKeyHex { get; private set; }

    public static MathcadPrimeComServerManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (_syncRoot)
                {
                    if (_instance == null)
                    {
                        _instance = new MathcadPrimeComServerManager();
                    }
                }
            }

            return _instance;
        }
    }

    public Application PrimeApp
    {
        get
        {
            return _primeApp ??= Application.Current as App;
        }
    }

    public SpiritMainWindow PrimeWindow
    {
        get
        {
            return _primeWindow ??= PrimeApp.MainWindow as SpiritMainWindow;
        }
    }

    public ulong CallsCounter { get; private set; }

    public IDictionary<string, string> ListOfControlledWorksheets
    {
        get
        {
            return _listOfControlledWorksheets;
        }
    }

    public bool IsInitialized()
    {
        return PrimeApp != null && PrimeWindow != null;
    }

    public IDialogManager GetDialogManager()
    {
        return AppServiceRegistry.GetService<IDialogManager>();
    }

    public IMessageBoxManager GetMessageBoxManager()
    {
        return AppServiceRegistry.GetService<IMessageBoxManager>();
    }

    public void DisableMessageBoxManager(bool isUiInteractiveModeDisabled)
    {
        if (isUiInteractiveModeDisabled)
        {
            IMessageBoxManager messageBoxManager = AppServiceRegistry.GetService<IMessageBoxManager>();
            if (messageBoxManager != null)
            {
                messageBoxManager.IsDisabled = true;
            }
        }
    }

    public void AddWorksheet(string name, string fullName)
    {
        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        KeyValuePair<string, string> worksheet = new KeyValuePair<string, string>(name, fullName);
        if (!((ICollection<KeyValuePair<string, string>>)_listOfControlledWorksheets).Contains(worksheet))
        {
            ((ICollection<KeyValuePair<string, string>>)_listOfControlledWorksheets).Add(worksheet);
        }
    }

    public void RemoveWorksheet(string name, string fullName)
    {
        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        KeyValuePair<string, string> worksheet = new KeyValuePair<string, string>(name, fullName);
        if (((ICollection<KeyValuePair<string, string>>)_listOfControlledWorksheets).Contains(worksheet))
        {
            ((ICollection<KeyValuePair<string, string>>)_listOfControlledWorksheets).Remove(worksheet);
        }
    }
}
