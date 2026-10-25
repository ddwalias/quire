#nullable disable
using System;
using System.ComponentModel;

namespace Quire.License;

public partial interface ILicenseManager
{
    bool HasValidLicense { get; }

    int LicenseRefreshInterval { get; set; }

    LicenseType LicenseType { get; }

    DateTime? IssueDate { get; }

    DateTime? ExpirationDate { get; }

    string HostId { get; }

    LicenseSource LicenseSourceInfo { get; }

    string LicenseMessageText { get; }

    LicenseBorrowStatus LicenseBorrowStatus { get; }

    DateTime? BorrowedUntil { get; }

    bool IsEducationalLicense { get; }

    int? DaysLeft { get; }

    string ProductCode { get; }

    string SalesOrderNumber { get; }

    string CustomerNumber { get; }

    bool IsEngNbPlus { get; }

    bool IsEngNb { get; }

    bool IsEngNotebookLostConnection { get; }

    event Action LicenseStatusChanged;

    event PropertyChangedEventHandler PropertyChanged;

    event EventHandler LicenseTypeChanged;

    event Action EngNotebookConnectionStatusChanged;

    void UpdateApplicationVariant();

    bool IsUsingReleaseFeature();

    bool SetLicenseFile(string fileName);

    bool SetLicenseSourceList(string licenseSourceList);

    bool SetEnterpriseLicenseServer(string host, uint port);

    void SetNoLicense();

    bool BorrowLicense(DateTime returnDate);

    bool ReturnLicense();

    void InitLicense();

    void ReleaseLicense(bool isClosingWindow);

    void ResetLicenseSettings();

    bool RefreshLicense();

    void LaunchLicenseWizard();

    void LaunchLicenseWizardAtStartupIfNeeded();

    byte[] GetEncryptionKey(EncryptorVersion encryptorVersion);

    byte[] GetEncryptionInitVector(EncryptorVersion encryptorVersion);
}
