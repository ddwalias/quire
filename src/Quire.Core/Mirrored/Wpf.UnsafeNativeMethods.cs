#nullable disable
#pragma warning disable

namespace Quire.Wpf;

partial class UnsafeNativeMethods
{
    public partial struct POINT
    {
        public int x;
        public int y;
    }

    public partial struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;
    }

    public partial struct TS_STATUS
    {
        public DynamicStatusFlags dynamicFlags;
        public StaticStatusFlags staticFlags;
    }

    public partial struct TS_SELECTIONSTYLE
    {
        public TsActiveSelEnd ase;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public bool interimChar;
    }

    public partial struct TS_SELECTION_ACP
    {
        public int start;
        public int end;
        public TS_SELECTIONSTYLE style;
    }

    public partial struct TS_RUNINFO
    {
        public int count;
        public TsRunType type;
    }

    public partial struct TS_TEXTCHANGE
    {
        public int start;
        public int oldEnd;
        public int newEnd;
    }

    public partial struct TF_PRESERVEDKEY
    {
        public int vKey;
        public int modifiers;
    }

    public partial struct TF_DA_COLOR
    {
        public TF_DA_COLORTYPE type;
        public int indexOrColorRef;
    }

    public partial struct TF_DISPLAYATTRIBUTE
    {
        public TF_DA_COLOR crText;
        public TF_DA_COLOR crBk;
        public TF_DA_LINESTYLE lsStyle;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        public bool fBoldLine;
        public TF_DA_COLOR crLine;
        public TF_DA_ATTR_INFO bAttr;
    }

    public partial struct TF_RENDERINGMARKUP
    {
        public ITfRange range;
        public TF_DISPLAYATTRIBUTE tfDisplayAttr;
    }

    public partial struct TF_LANGUAGEPROFILE
    {
        internal global::System.Guid clsid;
        internal short langid;
        internal global::System.Guid catid;
        [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        internal bool fActive;
        internal global::System.Guid guidProfile;
    }

    public partial struct COMPOSITIONFORM
    {
        public uint dwStyle;
        public POINT ptCurrentPos;
        public RECT rcArea;
    }

    public const int TF_CLIENTID_NULL = 0;
    public const char TS_CHAR_EMBEDDED = '\uFFFC';
    public const char TS_CHAR_REGION = '\u0000';
    public const char TS_CHAR_REPLACEMENT = '\uFFFD';
    public const int TS_DEFAULT_SELECTION = -1;
    public const int TS_S_ASYNC = 262912;
    public const int TS_E_NOSELECTION = -2147220987;
    public const int TS_E_NOLAYOUT = -2147220986;
    public const int TS_E_INVALIDPOINT = -2147220985;
    public const int TS_E_SYNCHRONOUS = -2147220984;
    public const int TS_E_READONLY = -2147220983;
    public const int TS_E_FORMAT = -2147220982;
    public const int TF_INVALID_COOKIE = -1;
    public const int TF_DICTATION_ON = 1;
    public const int TF_COMMANDING_ON = 8;
    public static readonly global::System.Guid IID_ITextStoreACPSink = new global::System.Guid(584338580u, 42009, 17730, 162, 114, 174, 38, 9, 62, 206, 207);
    public static readonly global::System.Guid IID_ITfThreadFocusSink = new global::System.Guid(3237075724u, 14880, 16476, 163, 3, 150, 182, 1, 10, 136, 95);
    public static readonly global::System.Guid IID_ITfTextEditSink = new global::System.Guid(2166871049u, 52435, 18051, 150, 122, 180, 61, 91, 72, 43, 247);
    public static readonly global::System.Guid IID_ITfLanguageProfileNotifySink = new global::System.Guid(1137311253u, 62612, 19479, 157, 226, 184, 164, 172, 53, 10, 168);
    public static readonly global::System.Guid IID_ITfCompartmentEventSink = new global::System.Guid(1950006623u, 62061, 18655, 140, 197, 35, 132, 146, 65, 155, 100);
    public static readonly global::System.Guid IID_ITfTransitoryExtensionSink = new global::System.Guid(2786396527u, 7255, 18451, 138, 21, 85, 238, 110, 90, 131, 156);
    public static readonly global::System.Guid GUID_TFCAT_TIP_KEYBOARD = new global::System.Guid(880041059u, 45808, 18308, 139, 103, 94, 18, 200, 112, 26, 49);
    public static readonly global::System.Guid GUID_PROP_ATTRIBUTE = new global::System.Guid(884233840, 29990, 4562, 161, 71, 0, 16, 90, 39, 153, 181);
    public static readonly global::System.Guid GUID_PROP_LANGID = new global::System.Guid(847302176u, 32818, 4562, 182, 3, 0, 16, 90, 39, 153, 181);
    public static readonly global::System.Guid GUID_PROP_READING = new global::System.Guid(1415837632u, 36401, 4562, 191, 70, 0, 16, 90, 39, 153, 181);
    public static readonly global::System.Guid GUID_PROP_INPUTSCOPE = new global::System.Guid(387177818, 26855, 19035, 154, 246, 89, 42, 89, 92, 119, 141);
    public static readonly global::System.Guid GUID_COMPARTMENT_KEYBOARD_DISABLED = new global::System.Guid(1906684499, 6481, 18027, 159, 188, 156, 136, 8, 250, 132, 242);
    public static global::System.Guid GUID_COMPARTMENT_KEYBOARD_OPENCLOSE = new global::System.Guid(1478965933, 443, 16740, 149, 198, 117, 91, 160, 181, 22, 45);
    public static readonly global::System.Guid GUID_COMPARTMENT_HANDWRITING_OPENCLOSE = new global::System.Guid(4188941419u, 6246, 17249, 175, 114, 122, 163, 9, 72, 137, 14);
    public static readonly global::System.Guid GUID_COMPARTMENT_SPEECH_DISABLED = new global::System.Guid(1455801863, 1795, 20057, 142, 82, 203, 200, 78, 139, 190, 53);
    public static readonly global::System.Guid GUID_COMPARTMENT_SPEECH_OPENCLOSE = new global::System.Guid(1414359651u, 58088, 18258, 187, 209, 0, 9, 96, 188, 160, 131);
    public static readonly global::System.Guid GUID_COMPARTMENT_SPEECH_GLOBALSTATE = new global::System.Guid(710213262, 3336, 17932, 167, 93, 135, 3, 95, 244, 54, 197);
    public static readonly global::System.Guid GUID_COMPARTMENT_KEYBOARD_INPUTMODE_CONVERSION = new global::System.Guid(3438304728u, 19079, 4567, 166, 226, 0, 6, 91, 132, 67, 92);
    public static readonly global::System.Guid GUID_COMPARTMENT_KEYBOARD_INPUTMODE_SENTENCE = new global::System.Guid(3438304729u, 19079, 4567, 166, 226, 0, 6, 91, 132, 67, 92);
    public static readonly global::System.Guid GUID_COMPARTMENT_TRANSITORYEXTENSION = new global::System.Guid(2346928117u, 51104, 4567, 180, 8, 0, 6, 91, 132, 67, 92);
    public static readonly global::System.Guid GUID_COMPARTMENT_TRANSITORYEXTENSION_DOCUMENTMANAGER = new global::System.Guid(2346928119u, 51104, 4567, 180, 8, 0, 6, 91, 132, 67, 92);
    public static readonly global::System.Guid GUID_COMPARTMENT_TRANSITORYEXTENSION_PARENT = new global::System.Guid(2346928120u, 51104, 4567, 180, 8, 0, 6, 91, 132, 67, 92);
    public static readonly global::System.Guid Clsid_SpeechTip = new global::System.Guid(3703402408u, 815, 4563, 181, 177, 0, 192, 79, 195, 36, 161);
    public static readonly global::System.Guid Guid_Null = new global::System.Guid(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0);
    public static readonly global::System.Guid IID_ITfFnCustomSpeechCommand = new global::System.Guid(4238787401u, 41263, 17315, 141, 214, 90, 90, 66, 130, 87, 123);
    public static readonly global::System.Guid IID_ITfFnReconversion = new global::System.Guid("4cea93c0-0a58-11d3-8df0-00105a2799b5");
    public static readonly global::System.Guid IID_ITfFnConfigure = new global::System.Guid(2297784262u, 5975, 18936, 161, 178, 137, 35, 76, 30, 239, 249);
    public static readonly global::System.Guid IID_ITfFnConfigureRegisterWord = new global::System.Guid(3147137162u, 28047, 19402, 132, 0, 83, 144, 181, 134, 174, 223);
    public static readonly global::System.Guid TSATTRID_Font_FaceName = new global::System.Guid(3040259766u, 1339, 20152, 182, 90, 80, 218, 30, 129, 231, 46);
    public static readonly global::System.Guid TSATTRID_Font_SizePts = new global::System.Guid(3360240386u, 42473, 17773, 175, 4, 128, 5, 228, 19, 15, 3);
    public static readonly global::System.Guid TSATTRID_Font_Style_Height = new global::System.Guid(2123592823, 4838, 17803, 146, 106, 31, 164, 78, 232, 243, 145);
    public static readonly global::System.Guid TSATTRID_Text_VerticalWriting = new global::System.Guid(1807384981, 1135, 20137, 179, 17, 151, 253, 102, 196, 39, 75);
    public static readonly global::System.Guid TSATTRID_Text_Orientation = new global::System.Guid(1806397567u, 34693, 19513, 139, 82, 150, 248, 120, 48, 63, 251);
    public static readonly global::System.Guid TSATTRID_Text_ReadOnly = new global::System.Guid(2239981079u, 56882, 19197, 165, 15, 162, 219, 17, 14, 110, 77);
    public static readonly global::System.Guid GUID_SYSTEM_FUNCTIONPROVIDER = new global::System.Guid("9a698bb0-0f21-11d3-8df1-00105a2799b5");
    public const uint CFS_DEFAULT = 0u;
    public const uint CFS_RECT = 1u;
    public const uint CFS_POINT = 2u;
    public const uint CFS_FORCE_POSITION = 32u;
    public const uint WM_IME_COMPOSITION = 271u;

    [System.Runtime.InteropServices.DllImport("msctf.dll")]
    [System.Security.SuppressUnmanagedCodeSecurity]
    [System.Security.SecurityCritical]
    public static extern int TF_CreateThreadMgr(out ITfThreadMgr threadManager);

    [System.Runtime.InteropServices.DllImport("msctf.dll")]
    [System.Security.SecurityCritical]
    [System.Security.SuppressUnmanagedCodeSecurity]
    public static extern int TF_CreateInputProcessorProfiles(out ITfInputProcessorProfiles profiles);

    [System.Runtime.InteropServices.DllImport("msctf.dll")]
    [System.Security.SuppressUnmanagedCodeSecurity]
    [System.Security.SecurityCritical]
    public static extern int TF_CreateDisplayAttributeMgr(out ITfDisplayAttributeMgr dam);

    [System.Runtime.InteropServices.DllImport("msctf.dll")]
    [System.Security.SuppressUnmanagedCodeSecurity]
    [System.Security.SecurityCritical]
    public static extern int TF_CreateCategoryMgr(out ITfCategoryMgr catmgr);

    [System.Runtime.InteropServices.DllImport("user32.dll", CharSet = System.Runtime.InteropServices.CharSet.Auto, ExactSpelling = true, SetLastError = true)]
    public static extern int GetKeyboardLayoutList(int size, [System.Runtime.InteropServices.Out][System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.LPArray)] nint[] hkls);

    [System.Runtime.InteropServices.DllImport("imm32.dll")]
    public static extern nint ImmGetContext(nint hWnd);

    [System.Runtime.InteropServices.DllImport("imm32.dll")]
    public static extern int ImmReleaseContext(nint hWnd, nint hIMC);

    [System.Runtime.InteropServices.DllImport("imm32.dll")]
    public static extern bool ImmSetCompositionWindow(nint hIMC, ref COMPOSITIONFORM lpCompForm);

    [System.Runtime.InteropServices.DllImport("imm32.dll")]
    public static extern bool ImmGetOpenStatus(nint hIMC);

    partial interface ITfMessagePump
    {
        void PeekMessageA(ref global::System.Windows.Interop.MSG msg, int hwnd, int msgFilterMin, int msgFilterMax, int removeMsg, out int result);
        void GetMessageA(ref global::System.Windows.Interop.MSG msg, int hwnd, int msgFilterMin, int msgFilterMax, out int result);
        void PeekMessageW(ref global::System.Windows.Interop.MSG msg, int hwnd, int msgFilterMin, int msgFilterMax, int removeMsg, out int result);
        void GetMessageW(ref global::System.Windows.Interop.MSG msg, int hwnd, int msgFilterMin, int msgFilterMax, out bool result);
    }

    partial interface ITfProperty
    {
        void GetType(out global::System.Guid type);
        int EnumRanges(int editcookie, out global::Quire.Wpf.UnsafeNativeMethods.IEnumTfRanges ranges, global::Quire.Wpf.UnsafeNativeMethods.ITfRange targetRange);
        void GetValue(int editCookie, global::Quire.Wpf.UnsafeNativeMethods.ITfRange range, out object value);
        void GetContext(out global::Quire.Wpf.UnsafeNativeMethods.ITfContext context);
        void FindRange(int editCookie, global::Quire.Wpf.UnsafeNativeMethods.ITfRange inRange, out global::Quire.Wpf.UnsafeNativeMethods.ITfRange outRange, global::Quire.Wpf.UnsafeNativeMethods.TfAnchor position);
        void stub_SetValueStore();
        void SetValue(int editCookie, global::Quire.Wpf.UnsafeNativeMethods.ITfRange range, object value);
        void Clear(int editCookie, global::Quire.Wpf.UnsafeNativeMethods.ITfRange range);
    }

    partial interface ITfContext
    {
        int stub_RequestEditSession();
        void InWriteSession(int clientId, out bool inWriteSession);
        void stub_GetSelection();
        void stub_SetSelection();
        void GetStart(int ec, out global::Quire.Wpf.UnsafeNativeMethods.ITfRange range);
        void stub_GetEnd();
        void stub_GetActiveView();
        void stub_EnumViews();
        void stub_GetStatus();
        void GetProperty(ref global::System.Guid guid, out global::Quire.Wpf.UnsafeNativeMethods.ITfProperty property);
        void stub_GetAppProperty();
        void stub_TrackProperties();
        void stub_EnumProperties();
        void stub_GetDocumentMgr();
        void stub_CreateRangeBackup();
    }

    partial interface ITfDocumentMgr
    {
        void CreateContext(int clientId, global::Quire.Wpf.UnsafeNativeMethods.CreateContextFlags flags, object obj, out global::Quire.Wpf.UnsafeNativeMethods.ITfContext context, out int editCookie);
        void Push(global::Quire.Wpf.UnsafeNativeMethods.ITfContext context);
        void Pop(global::Quire.Wpf.UnsafeNativeMethods.PopFlags flags);
        void GetTop(out global::Quire.Wpf.UnsafeNativeMethods.ITfContext context);
        void GetBase(out global::Quire.Wpf.UnsafeNativeMethods.ITfContext context);
        void EnumContexts(out object enumContexts);
    }

    partial interface ITfFunctionProvider
    {
        void GetType(out global::System.Guid guid);
        void GetDescription(out string desc);
        int GetFunction(ref global::System.Guid guid, ref global::System.Guid riid, out object obj);
    }

    partial interface ITfFunction
    {
        void GetDisplayName(out string funcName);
    }

    partial interface ITfCandidateString
    {
        void GetString(out string funcName);
        void GetIndex(out int nIndex);
    }

    partial interface ITfCandidateList
    {
        void EnumCandidates(out object enumCand);
        void GetCandidate(int nIndex, out global::Quire.Wpf.UnsafeNativeMethods.ITfCandidateString candstring);
        void GetCandidateNum(out int nCount);
        void SetResult(int nIndex, global::Quire.Wpf.UnsafeNativeMethods.TfCandidateResult result);
    }

    partial interface ITfFnReconversion
    {
        void GetDisplayName(out string funcName);
        int QueryRange(global::Quire.Wpf.UnsafeNativeMethods.ITfRange range, out global::Quire.Wpf.UnsafeNativeMethods.ITfRange newRange, out bool isConvertable);
        int GetReconversion(global::Quire.Wpf.UnsafeNativeMethods.ITfRange range, out global::Quire.Wpf.UnsafeNativeMethods.ITfCandidateList candList);
        int Reconvert(global::Quire.Wpf.UnsafeNativeMethods.ITfRange range);
    }

    partial interface ITfFnConfigure
    {
        void GetDisplayName(out string funcName);
        int Show(nint hwndParent, short langid, ref global::System.Guid guidProfile);
    }

    partial interface ITfFnConfigureRegisterWord
    {
        void GetDisplayName(out string funcName);
        int Show(nint hwndParent, short langid, ref global::System.Guid guidProfile, string bstrRegistered);
    }

    partial interface ITfCompartment
    {
        int SetValue(int tid, ref object varValue);
        void GetValue(out object varValue);
    }

    partial interface ITfCompartmentEventSink
    {
        void OnChange(ref global::System.Guid rguid);
    }

    partial interface ITfCompartmentMgr
    {
        void GetCompartment(ref global::System.Guid guid, out global::Quire.Wpf.UnsafeNativeMethods.ITfCompartment comp);
        void ClearCompartment(int tid, global::System.Guid guid);
        void EnumCompartments(out object enumGuid);
    }

    partial interface ITfThreadMgr
    {
        void Activate(out int clientId);
        void Deactivate();
        void CreateDocumentMgr(out global::Quire.Wpf.UnsafeNativeMethods.ITfDocumentMgr docMgr);
        void EnumDocumentMgrs(out global::Quire.Wpf.UnsafeNativeMethods.IEnumTfDocumentMgrs enumDocMgrs);
        void GetFocus(out global::Quire.Wpf.UnsafeNativeMethods.ITfDocumentMgr docMgr);
        void SetFocus(global::Quire.Wpf.UnsafeNativeMethods.ITfDocumentMgr docMgr);
        void AssociateFocus(nint hwnd, global::Quire.Wpf.UnsafeNativeMethods.ITfDocumentMgr newDocMgr, out global::Quire.Wpf.UnsafeNativeMethods.ITfDocumentMgr prevDocMgr);
        void IsThreadFocus(out bool isFocus);
        int GetFunctionProvider(ref global::System.Guid classId, out global::Quire.Wpf.UnsafeNativeMethods.ITfFunctionProvider funcProvider);
        void EnumFunctionProviders(out global::Quire.Wpf.UnsafeNativeMethods.IEnumTfFunctionProviders enumProviders);
        void GetGlobalCompartment(out global::Quire.Wpf.UnsafeNativeMethods.ITfCompartmentMgr compartmentMgr);
    }

    partial interface ITextStoreACPSink
    {
        void OnTextChange(global::Quire.Wpf.UnsafeNativeMethods.OnTextChangeFlags flags, ref global::Quire.Wpf.UnsafeNativeMethods.TS_TEXTCHANGE change);
        void OnSelectionChange();
        void OnLayoutChange(global::Quire.Wpf.UnsafeNativeMethods.TsLayoutCode lcode, int viewCookie);
        void OnStatusChange(global::Quire.Wpf.UnsafeNativeMethods.DynamicStatusFlags flags);
        void OnAttrsChange(int start, int end, int count, global::System.Guid[] attributes);
        int OnLockGranted(global::Quire.Wpf.UnsafeNativeMethods.LockFlags flags);
        void OnStartEditTransaction();
        void OnEndEditTransaction();
    }

    partial interface ITfThreadFocusSink
    {
        void OnSetThreadFocus();
        void OnKillThreadFocus();
    }

    partial interface ITfSource
    {
        void AdviseSink(ref global::System.Guid riid, object obj, out int cookie);
        void UnadviseSink(int cookie);
    }

    partial interface ITfKeystrokeMgr
    {
        void AdviseKeyEventSink(int clientId, object obj, bool fForeground);
        void UnadviseKeyEventSink(int clientId);
        void GetForeground(out global::System.Guid clsid);
        void TestKeyDown(int wParam, int lParam, out bool eaten);
        void TestKeyUp(int wParam, int lParam, out bool eaten);
        void KeyDown(int wParam, int lParam, out bool eaten);
        void KeyUp(int wParam, int lParam, out bool eaten);
        void GetPreservedKey(global::Quire.Wpf.UnsafeNativeMethods.ITfContext context, ref global::Quire.Wpf.UnsafeNativeMethods.TF_PRESERVEDKEY key, out global::System.Guid guid);
        void IsPreservedKey(ref global::System.Guid guid, ref global::Quire.Wpf.UnsafeNativeMethods.TF_PRESERVEDKEY key, out bool registered);
        void PreserveKey(int clientId, ref global::System.Guid guid, ref global::Quire.Wpf.UnsafeNativeMethods.TF_PRESERVEDKEY key, char[] desc, int descCount);
        void UnpreserveKey(ref global::System.Guid guid, ref global::Quire.Wpf.UnsafeNativeMethods.TF_PRESERVEDKEY key);
        void SetPreservedKeyDescription(ref global::System.Guid guid, char[] desc, int descCount);
        void GetPreservedKeyDescription(ref global::System.Guid guid, out string desc);
        void SimulatePreservedKey(global::Quire.Wpf.UnsafeNativeMethods.ITfContext context, ref global::System.Guid guid, out bool eaten);
    }

    partial interface ITfRange
    {
        void GetText(int ec, int flags, char[] text, int countMax, out int count);
        void SetText(int ec, int flags, char[] text, int count);
        void GetFormattedText(int ec, out object data);
        void GetEmbedded(int ec, ref global::System.Guid guidService, ref global::System.Guid iid, out object obj);
        void InsertEmbedded(int ec, int flags, object data);
        void ShiftStart(int ec, int count, out int result, int ZeroForNow);
        void ShiftEnd(int ec, int count, out int result, int ZeroForNow);
        void ShiftStartToRange(int ec, global::Quire.Wpf.UnsafeNativeMethods.ITfRange range, global::Quire.Wpf.UnsafeNativeMethods.TfAnchor position);
        void ShiftEndToRange(int ec, global::Quire.Wpf.UnsafeNativeMethods.ITfRange range, global::Quire.Wpf.UnsafeNativeMethods.TfAnchor position);
        void ShiftStartRegion(int ec, global::Quire.Wpf.UnsafeNativeMethods.TfShiftDir dir, out bool noRegion);
        void ShiftEndRegion(int ec, global::Quire.Wpf.UnsafeNativeMethods.TfShiftDir dir, out bool noRegion);
        void IsEmpty(int ec, out bool empty);
        void Collapse(int ec, global::Quire.Wpf.UnsafeNativeMethods.TfAnchor position);
        void IsEqualStart(int ec, global::Quire.Wpf.UnsafeNativeMethods.ITfRange with, global::Quire.Wpf.UnsafeNativeMethods.TfAnchor position, out bool equal);
        void IsEqualEnd(int ec, global::Quire.Wpf.UnsafeNativeMethods.ITfRange with, global::Quire.Wpf.UnsafeNativeMethods.TfAnchor position, out bool equal);
        void CompareStart(int ec, global::Quire.Wpf.UnsafeNativeMethods.ITfRange with, global::Quire.Wpf.UnsafeNativeMethods.TfAnchor position, out int result);
        void CompareEnd(int ec, global::Quire.Wpf.UnsafeNativeMethods.ITfRange with, global::Quire.Wpf.UnsafeNativeMethods.TfAnchor position, out int result);
        void AdjustForInsert(int ec, int count, out bool insertOk);
        void GetGravity(out global::Quire.Wpf.UnsafeNativeMethods.TfGravity start, out global::Quire.Wpf.UnsafeNativeMethods.TfGravity end);
        void SetGravity(int ec, global::Quire.Wpf.UnsafeNativeMethods.TfGravity start, global::Quire.Wpf.UnsafeNativeMethods.TfGravity end);
        void Clone(out global::Quire.Wpf.UnsafeNativeMethods.ITfRange clone);
        void GetContext(out global::Quire.Wpf.UnsafeNativeMethods.ITfContext context);
    }

    partial interface ITfRangeACP
    {
        void GetText(int ec, int flags, char[] text, int countMax, out int count);
        void SetText(int ec, int flags, char[] text, int count);
        void GetFormattedText(int ec, out object data);
        void GetEmbedded(int ec, ref global::System.Guid guidService, ref global::System.Guid iid, out object obj);
        void InsertEmbedded(int ec, int flags, object data);
        void ShiftStart(int ec, int count, out int result, int ZeroForNow);
        void ShiftEnd(int ec, int count, out int result, int ZeroForNow);
        void ShiftStartToRange(int ec, global::Quire.Wpf.UnsafeNativeMethods.ITfRange range, global::Quire.Wpf.UnsafeNativeMethods.TfAnchor position);
        void ShiftEndToRange(int ec, global::Quire.Wpf.UnsafeNativeMethods.ITfRange range, global::Quire.Wpf.UnsafeNativeMethods.TfAnchor position);
        void ShiftStartRegion(int ec, global::Quire.Wpf.UnsafeNativeMethods.TfShiftDir dir, out bool noRegion);
        void ShiftEndRegion(int ec, global::Quire.Wpf.UnsafeNativeMethods.TfShiftDir dir, out bool noRegion);
        void IsEmpty(int ec, out bool empty);
        void Collapse(int ec, global::Quire.Wpf.UnsafeNativeMethods.TfAnchor position);
        void IsEqualStart(int ec, global::Quire.Wpf.UnsafeNativeMethods.ITfRange with, global::Quire.Wpf.UnsafeNativeMethods.TfAnchor position, out bool equal);
        void IsEqualEnd(int ec, global::Quire.Wpf.UnsafeNativeMethods.ITfRange with, global::Quire.Wpf.UnsafeNativeMethods.TfAnchor position, out bool equal);
        void CompareStart(int ec, global::Quire.Wpf.UnsafeNativeMethods.ITfRange with, global::Quire.Wpf.UnsafeNativeMethods.TfAnchor position, out int result);
        void CompareEnd(int ec, global::Quire.Wpf.UnsafeNativeMethods.ITfRange with, global::Quire.Wpf.UnsafeNativeMethods.TfAnchor position, out int result);
        void AdjustForInsert(int ec, int count, out bool insertOk);
        void GetGravity(out global::Quire.Wpf.UnsafeNativeMethods.TfGravity start, out global::Quire.Wpf.UnsafeNativeMethods.TfGravity end);
        void SetGravity(int ec, global::Quire.Wpf.UnsafeNativeMethods.TfGravity start, global::Quire.Wpf.UnsafeNativeMethods.TfGravity end);
        void Clone(out global::Quire.Wpf.UnsafeNativeMethods.ITfRange clone);
        void GetContext(out global::Quire.Wpf.UnsafeNativeMethods.ITfContext context);
        void GetExtent(out int start, out int count);
        void SetExtent(int start, int count);
    }

    partial interface ITfCompositionView
    {
        void GetOwnerClsid(out global::System.Guid clsid);
        void GetRange(out global::Quire.Wpf.UnsafeNativeMethods.ITfRange range);
    }

    partial interface ITfContextOwnerCompositionSink
    {
        void OnStartComposition(global::Quire.Wpf.UnsafeNativeMethods.ITfCompositionView view, out bool ok);
        void OnUpdateComposition(global::Quire.Wpf.UnsafeNativeMethods.ITfCompositionView view, global::Quire.Wpf.UnsafeNativeMethods.ITfRange rangeNew);
        void OnEndComposition(global::Quire.Wpf.UnsafeNativeMethods.ITfCompositionView view);
    }

    partial interface ITfContextComposition
    {
        void StartComposition(int ecWrite, global::Quire.Wpf.UnsafeNativeMethods.ITfRange range, object sink, out object composition);
        void EnumCompositions(out global::Quire.Wpf.UnsafeNativeMethods.IEnumITfCompositionView enumView);
        void FindComposition(int ecRead, global::Quire.Wpf.UnsafeNativeMethods.ITfRange testRange, out object enumView);
        void TakeOwnership(int ecWrite, global::Quire.Wpf.UnsafeNativeMethods.ITfCompositionView view, object sink, out object composition);
    }

    partial interface ITfContextOwnerCompositionServices
    {
        void StartComposition(int ecWrite, global::Quire.Wpf.UnsafeNativeMethods.ITfRange range, object sink, out object composition);
        void EnumCompositions(out object enumView);
        void FindComposition(int ecRead, global::Quire.Wpf.UnsafeNativeMethods.ITfRange testRange, out object enumView);
        void TakeOwnership(int ecWrite, global::Quire.Wpf.UnsafeNativeMethods.ITfCompositionView view, object sink, out object composition);
        int TerminateComposition(global::Quire.Wpf.UnsafeNativeMethods.ITfCompositionView view);
    }

    partial interface IEnumITfCompositionView
    {
        void Clone(out global::Quire.Wpf.UnsafeNativeMethods.IEnumTfRanges ranges);
        int Next(int count, global::Quire.Wpf.UnsafeNativeMethods.ITfCompositionView[] compositionview, out int fetched);
        void Reset();
        int Skip(int count);
    }

    partial interface IEnumTfRanges
    {
        void Clone(out global::Quire.Wpf.UnsafeNativeMethods.IEnumTfRanges ranges);
        int Next(int count, global::Quire.Wpf.UnsafeNativeMethods.ITfRange[] ranges, out int fetched);
        void Reset();
        int Skip(int count);
    }

    partial interface ITfEditRecord
    {
        void GetSelectionStatus(out bool selectionChanged);
        void GetTextAndPropertyUpdates(int flags, ref nint properties, int count, out global::Quire.Wpf.UnsafeNativeMethods.IEnumTfRanges ranges);
    }

    partial interface ITfTextEditSink
    {
        void OnEndEdit(global::Quire.Wpf.UnsafeNativeMethods.ITfContext context, int ecReadOnly, global::Quire.Wpf.UnsafeNativeMethods.ITfEditRecord editRecord);
    }

    partial interface IEnumTfRenderingMarkup
    {
        void Clone(out global::Quire.Wpf.UnsafeNativeMethods.IEnumTfRenderingMarkup clone);
        int Next(int count, global::Quire.Wpf.UnsafeNativeMethods.TF_RENDERINGMARKUP[] markup, out int fetched);
        void Reset();
        int Skip(int count);
    }

    partial interface ITfContextRenderingMarkup
    {
        void GetRenderingMarkup(int editCookie, global::Quire.Wpf.UnsafeNativeMethods.GetRenderingMarkupFlags flags, global::Quire.Wpf.UnsafeNativeMethods.ITfRange range, out global::Quire.Wpf.UnsafeNativeMethods.IEnumTfRenderingMarkup enumMarkup);
        void FindNextRenderingMarkup(int editCookie, global::Quire.Wpf.UnsafeNativeMethods.FindRenderingMarkupFlags flags, global::Quire.Wpf.UnsafeNativeMethods.ITfRange queryRange, global::Quire.Wpf.UnsafeNativeMethods.TfAnchor queryAnchor, out global::Quire.Wpf.UnsafeNativeMethods.ITfRange foundRange, out global::Quire.Wpf.UnsafeNativeMethods.TF_RENDERINGMARKUP foundMarkup);
    }

    partial interface ITfInputProcessorProfiles
    {
        void stub_Register();
        void stub_Unregister();
        void stub_AddLanguageProfile();
        void stub_RemoveLanguageProfile();
        void stub_EnumInputProcessorInfo();
        void stub_GetDefaultLanguageProfile();
        void stub_SetDefaultLanguageProfile();
        void ActivateLanguageProfile(ref global::System.Guid clsid, short langid, ref global::System.Guid guidProfile);
        int GetActiveLanguageProfile(ref global::System.Guid clsid, out short langid, out global::System.Guid profile);
        void stub_GetLanguageProfileDescription();
        void GetCurrentLanguage(out short langid);
        int ChangeCurrentLanguage(short langid);
        int GetLanguageList(out nint langids, out int count);
        void EnumLanguageProfiles(short langid, out global::Quire.Wpf.UnsafeNativeMethods.IEnumTfLanguageProfiles enumIPP);
        void stub_EnableLanguageProfile();
        void stub_IsEnabledLanguageProfile();
        void stub_EnableLanguageProfileByDefault();
        void stub_SubstituteKeyboardLayout();
    }

    partial interface IEnumTfLanguageProfiles
    {
        void Clone(out global::Quire.Wpf.UnsafeNativeMethods.IEnumTfLanguageProfiles enumIPP);
        int Next(int count, global::Quire.Wpf.UnsafeNativeMethods.TF_LANGUAGEPROFILE[] profiles, out int fetched);
        void Reset();
        void Skip(int count);
    }

    partial interface ITfLanguageProfileNotifySink
    {
        void OnLanguageChange(short langid, out bool bAccept);
        void OnLanguageChanged();
    }

    partial interface ITfDisplayAttributeMgr
    {
        void OnUpdateInfo();
        void stub_EnumDisplayAttributeInfo();
        void GetDisplayAttributeInfo(ref global::System.Guid guid, out global::Quire.Wpf.UnsafeNativeMethods.ITfDisplayAttributeInfo info, out global::System.Guid clsid);
    }

    partial interface ITfDisplayAttributeInfo
    {
        void stub_GetGUID();
        void stub_GetDescription();
        void GetAttributeInfo(out global::Quire.Wpf.UnsafeNativeMethods.TF_DISPLAYATTRIBUTE attr);
        void stub_SetAttributeInfo();
        void stub_Reset();
    }

    partial interface ITfCategoryMgr
    {
        void stub_RegisterCategory();
        void stub_UnregisterCategory();
        void stub_EnumCategoriesInItem();
        void stub_EnumItemsInCategory();
        void stub_FindClosestCategory();
        void stub_RegisterGUIDDescription();
        void stub_UnregisterGUIDDescription();
        void stub_GetGUIDDescription();
        void stub_RegisterGUIDDWORD();
        void stub_UnregisterGUIDDWORD();
        void stub_GetGUIDDWORD();
        void stub_RegisterGUID();
        int GetGUID(int guidatom, out global::System.Guid guid);
        void stub_IsEqualTfGuidAtom();
    }

    partial interface ITfContextOwner
    {
        void GetACPFromPoint(ref global::Quire.Wpf.UnsafeNativeMethods.POINT point, global::Quire.Wpf.UnsafeNativeMethods.GetPositionFromPointFlags flags, out int position);
        void GetTextExt(int start, int end, out global::Quire.Wpf.UnsafeNativeMethods.RECT rect, out bool clipped);
        void GetScreenExt(out global::Quire.Wpf.UnsafeNativeMethods.RECT rect);
        void GetStatus(out global::Quire.Wpf.UnsafeNativeMethods.TS_STATUS status);
        void GetWnd(out nint hwnd);
        void GetValue(ref global::System.Guid guidAttribute, out object varValue);
    }

    partial interface ITfContextOwnerServices
    {
        void stub_OnLayoutChange();
        void stub_OnStatusChange();
        void stub_OnAttributeChange();
        void stub_Serialize();
        void stub_Unserialize();
        void stub_ForceLoadProperty();
        void CreateRange(int acpStart, int acpEnd, out global::Quire.Wpf.UnsafeNativeMethods.ITfRangeACP range);
    }

    partial interface ITfTransitoryExtensionSink
    {
        void OnTransitoryExtensionUpdated(global::Quire.Wpf.UnsafeNativeMethods.ITfContext context, int ecReadOnly, global::Quire.Wpf.UnsafeNativeMethods.ITfRange rangeResult, global::Quire.Wpf.UnsafeNativeMethods.ITfRange rangeComposition, out bool fDeleteResultRange);
    }

    partial interface ITfInputScope
    {
        void GetInputScopes(out nint ppinputscopes, out int count);
        int GetPhrase(out nint ppbstrPhrases, out int count);
        int GetRegularExpression(out string desc);
        int GetSRGC(out string desc);
        int GetXML(out string desc);
    }

    partial interface ITfMouseTrackerACP
    {
        int AdviceMouseSink(global::Quire.Wpf.UnsafeNativeMethods.ITfRangeACP range, global::Quire.Wpf.UnsafeNativeMethods.ITfMouseSink sink, out int dwCookie);
        int UnadviceMouseSink(int dwCookie);
    }

    partial interface ITfMouseSink
    {
        int OnMouseEvent(int edge, int quadrant, int btnStatus, out bool eaten);
    }
}
