#nullable disable
#pragma warning disable
using System;

namespace Quire.Controls.Include;

public partial class BackupData
{
    public bool originalMissingCacheUsed { get; set; }
    public bool circularInclusion { get; set; }
    public RegionsList regions { get; set; }
    public bool regionsLoaded { get; set; }
    public DateTime lastSavedDate { get; set; }
    public string lastSavedFilePath { get; set; }
    public bool cacheValid { get; set; }
    public bool cacheCorrupted { get; set; }
    public ErrorsList errorsList { get; set; }
    public WarningsList warningsList { get; set; }
    public global::Quire.Controls.CalculationVariables includedFileCalculationVariables { get; set; }
    public global::Quire.Controls.CalculationOptions includedFileCalculationOptions { get; set; }

    public BackupData() { }
}
