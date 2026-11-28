using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Quire.Properties;
using Quire.Services;
using Quire.Wpf;

namespace Quire;

internal partial class MathcadPrimeComServerManager
{
    private bool _isRuntimeKeyExist;

    private static readonly object _syncRoot = new object();

    private static MathcadPrimeComServerManager _instance = null;

    private App _primeApp;

    private SpiritMainWindow _primeWindow;

    private static bool Succeeded(int hResult)
    {
        return hResult == 0;
    }

    private string RuntimeKey { get; set; }

    internal string RuntimeKeyHex { get; private set; }

    [global::Quire.Mirror.MirrorStub]
    [global::Quire.Mirror.MirrorBlocked("Release IL: Mathcad body has two more instructions than the idiomatic form; unresolved")]
    private void GenerateRuntimeKey() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    internal bool ValidateEngNotebookLicKey(string receivedLicKey)
    {
        if (string.IsNullOrEmpty(receivedLicKey) || receivedLicKey.Length > 36)
        {
            return false;
        }

        if (!_isRuntimeKeyExist || string.IsNullOrEmpty(RuntimeKeyHex))
        {
            return false;
        }

        int digitSum = 0;
        foreach (char character in RuntimeKey)
        {
            digitSum += character - 48;
        }

        int multiplier = digitSum % 100;
        long checksum = 1L;
        foreach (char character in RuntimeKey)
        {
            checksum = (checksum * multiplier + (character ^ 0x5A)) % 1000000000000L;
        }

        string expectedKey = checksum.ToString("X").PadLeft(12, '0');
        if (receivedLicKey.Equals(expectedKey, StringComparison.InvariantCultureIgnoreCase))
        {
            return true;
        }

        return false;
    }

    private MathcadPrimeComServerManager()
    {
    }

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

    public bool IsInitialized()
    {
        return PrimeApp != null && PrimeWindow != null;
    }

    public void ClosePrimeWindow()
    {
        Delegate0 closePrimeWindow = () =>
        {
            Instance.PrimeWindow.Close();
        };
        Instance.InvokeDelegate(closePrimeWindow, DispatcherPriority.Normal);
    }

    private static uint _cookie = 0u;

    private static bool _isRegistered = false;

    private static bool _isComServerReady = false;

    private readonly IDictionary<string, string> _listOfControlledWorksheets = new Dictionary<string, string>();

    public ulong CallsCounter { get; private set; }

    public object InvokeDelegate(Delegate methodDelegate, DispatcherPriority priority = DispatcherPriority.Normal)
    {
        CallsCounter++;
        if (!_isComServerReady)
        {
            int attempt = 0;
            bool isScriptingRunWorksheetOpen = Settings.Default.IsScriptingRunWorksheetOpen;
            while (!_isComServerReady)
            {
                Thread.Sleep(100);
                if (!isScriptingRunWorksheetOpen)
                {
                    continue;
                }

                if (attempt > 10)
                {
                    try
                    {
                        if (Application.Current.Dispatcher.Thread == Thread.CurrentThread)
                        {
                            bool? hasScriptableControls = PrimeWindow?.OpenDocumentsList?[0]?.Worksheet?.HasScriptableControls;
                            if (hasScriptableControls.HasValue && hasScriptableControls.Value)
                            {
                                break;
                            }
                        }
                    }
                    catch
                    {
                    }
                }

                attempt++;
            }
        }

        object result = PrimeApp.Dispatcher.Invoke(priority, methodDelegate);
        CallsCounter--;
        return result;
    }

    public void Initialize(bool isComServer, bool isApplicationObsolete, bool isEmbeddingEngNote, bool isEmbeddingEngNotePlus)
    {
        if (!isComServer)
        {
            return;
        }

        if (_isRegistered)
        {
            return;
        }

        if (isEmbeddingEngNote | isEmbeddingEngNotePlus)
        {
            GenerateRuntimeKey();
        }

        RegisterMathcadPrimeComServer(isApplicationObsolete, isEmbeddingEngNote, isEmbeddingEngNotePlus);
        PrimeApp.Exit += OnApplicationExit;
    }

    private void OnApplicationExit(object sender, ExitEventArgs e)
    {
        PrimeApp.Exit -= OnApplicationExit;
        UnRegisterMathcadPrimeComServer();
    }

    private static void RegisterMathcadPrimeComServer(bool isApplicationObsolete, bool isEmbeddingEngNote, bool isEmbeddingEngNotePlus)
    {
        if (_isRegistered)
        {
            return;
        }

        if (isEmbeddingEngNote)
        {
            int hResult = ComNativeExternalApi.CoRegisterClassObject(new Guid("BFF864AC-81A8-4D77-A37F-0682640F1335"), new MathcadPrimeEngNoteClassFactory(), ComInteropEnums.CLSCTX.CLSCTX_LOCAL_SERVER, ComInteropEnums.REGCLS.REGCLS_SINGLEUSE, out _cookie);
            if (!Succeeded(hResult))
            {
                throw new ApplicationException("CoRegisterClassObject failed with error 0x" + hResult.ToString("X"));
            }
        }
        else if (isEmbeddingEngNotePlus)
        {
            int hResult = ComNativeExternalApi.CoRegisterClassObject(new Guid("CD723889-9B5D-4F8E-ADDA-7E25DFB86EBA"), new MathcadPrimeEngNotePlusClassFactory(), ComInteropEnums.CLSCTX.CLSCTX_LOCAL_SERVER, ComInteropEnums.REGCLS.REGCLS_SINGLEUSE, out _cookie);
            if (!Succeeded(hResult))
            {
                throw new ApplicationException("CoRegisterClassObject failed with error 0x" + hResult.ToString("X"));
            }
        }
        else
        {
            Guid classId = new Guid(isApplicationObsolete ? "A3E4E622-5CFF-4973-96EB-1E3EAB5151C8" : "A00E8B95-D415-433F-A04E-D298A54A7BB7");
            IClassFactory classFactory;
            if (!isApplicationObsolete)
            {
                classFactory = new MathcadPrimeClassFactory();
            }
            else
            {
                classFactory = new MathcadPrimeClassFactoryObsolete();
            }

            int hResult = ComNativeExternalApi.CoRegisterClassObject(classId, classFactory, ComInteropEnums.CLSCTX.CLSCTX_LOCAL_SERVER, ComInteropEnums.REGCLS.REGCLS_MULTIPLEUSE, out _cookie);
            if (!Succeeded(hResult))
            {
                throw new ApplicationException("CoRegisterClassObject failed with error 0x" + hResult.ToString("X"));
            }
        }

        _isRegistered = true;
    }

    private static void UnRegisterMathcadPrimeComServer()
    {
        if (!_isRegistered)
        {
            return;
        }

        if (_cookie != 0)
        {
            try
            {
                int hResult = ComNativeExternalApi.CoRevokeClassObject(_cookie);
                if (!Succeeded(hResult))
                {
                    DebugTools.FailAssertion("Mathcad Prime COM Server unregistration failed with error 0x" + hResult.ToString("X"));
                }
            }
            catch
            {
                DebugTools.FailAssertion("Mathcad Prime COM Server unregistration failed with exception.");
            }
        }

        _isRegistered = false;
    }

    public void SetComServerReady()
    {
        _isComServerReady = true;
    }

    public void EnableMessageBoxManager(bool isUiInteractiveModeDisabled)
    {
        if (isUiInteractiveModeDisabled)
        {
            IMessageBoxManager messageBoxManager = AppServiceRegistry.GetService<IMessageBoxManager>();
            if (messageBoxManager != null)
            {
                messageBoxManager.IsDisabled = false;
            }
        }
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

    public IDictionary<string, string> ListOfControlledWorksheets
    {
        get
        {
            return _listOfControlledWorksheets;
        }
    }

    public void AddWorksheet(string name, string fullName)
    {
        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        KeyValuePair<string, string> worksheet = new KeyValuePair<string, string>(name, fullName);
        if (!_listOfControlledWorksheets.Contains(worksheet))
        {
            _listOfControlledWorksheets.Add(worksheet);
        }
    }

    public void RemoveWorksheet(string name, string fullName)
    {
        if (string.IsNullOrEmpty(name))
        {
            return;
        }

        KeyValuePair<string, string> worksheet = new KeyValuePair<string, string>(name, fullName);
        if (_listOfControlledWorksheets.Contains(worksheet))
        {
            _listOfControlledWorksheets.Remove(worksheet);
        }
    }
}
