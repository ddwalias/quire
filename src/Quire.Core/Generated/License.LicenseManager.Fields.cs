#nullable disable
using System;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

namespace Quire.License;

public sealed partial class LicenseManager
{
    // Compile order places these declarations in Generated/ ahead of Mirrored/ to preserve Mathcad closure ordinals.
    private const string ENG_NB_VARIANT_ENV = "ENG_NB_VARIANT";
    private const int MaxNumMessages = 30;
    private const int DefaultLicenseRefreshInterval = 3600000;
    private static LicenseManager _instance;
    private readonly Dispatcher _uiThreadDispatcher;
    private Timer _refreshTimer;
    private readonly ResourceDictionary _licenseMessageDictionary;
    private bool? _hasValidLicense;
    private LicenseType _licenseType;
    private DateTime? _issueDate;
    private DateTime? _expirationDate;
    private string _hostId;
    private LicenseBorrowStatus _licenseBorrowStatus;
    private DateTime? _borrowedUntil;
    private bool _isEducationalLicense;
    private bool _isUsingReleaseFeature;
    private string _productCode;
    private string _salesOrderNumber;
    private string _customerNumber;
    private bool _isEngNbPlus;
    private bool _isEngNb;
    private bool _isEngNotebookLostConnection;
    private static readonly char[] semicolon = new char[1] { ';' };
    private static readonly Regex validServerPortRegex = new Regex(@"\d{4}@[^;@]+$", RegexOptions.CultureInvariant);
    private static readonly TimeSpan _oneMonth = new TimeSpan(31, 0, 0, 0);

    public static partial class PropertyNames
    {
        public const string HasValidLicense = "HasValidLicense";
        public const string DaysLeft = "DaysLeft";
        public const string LicenseType = "LicenseType";
        public const string IssueDate = "IssueDate";
        public const string ExpirationDate = "ExpirationDate";
        public const string HostId = "HostId";
        public const string ProductCode = "ProductCode";
        public const string SalesOrderNumber = "SalesOrderNumber";
        public const string CustomerNumber = "CustomerNumber";
        public const string IsEngNbPlus = "IsEngNbPlus";
        public const string IsEngNb = "IsEngNb";
        public const string IsEngNotebookLostConnection = "IsEngNotebookLostConnection";
        public const string LicenseMessageText = "LicenseMessageText";
        public const string LicenseBorrowStatus = "LicenseBorrowStatus";
        public const string BorrowedUntil = "BorrowedUntil";
    }
}
