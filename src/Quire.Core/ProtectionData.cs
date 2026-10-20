#nullable disable
using System.ComponentModel;
using System.ComponentModel;
using Quire.Mirror;
using Quire.Mirror;
using Quire.PersistentData;
using Quire.Services;

namespace Quire;

public partial class ProtectionData
{
    [MirrorVerified("PtcProtectionDataMechanicalDifferentialTests")]
    private readonly byte[] _EncryptedPassowrd;

    [MirrorVerified("PtcProtectionDataMechanicalDifferentialTests")]
    public bool HasPassword => EnumerableExtensions.IsNotEmpty<byte>(_EncryptedPassowrd);

    [MirrorVerified("PtcProtectionDataMechanicalDifferentialTests")]
    private readonly Nullable<DateTime> _timestamp;

    [MirrorVerified("PtcProtectionDataMechanicalDifferentialTests")]
    public Nullable<DateTime> Timestamp => _timestamp;

    [MirrorVerified("PtcProtectionDataMechanicalDifferentialTests")]
    public bool ShowTimestamp { get; private set; }

    [MirrorVerified("PtcProtectionDataMechanicalDifferentialTests")]
    private readonly ProtectionLevel _protectionLevel;

    [MirrorVerified("PtcProtectionDataMechanicalDifferentialTests")]
    public ProtectionLevel ProtectionLevel => _protectionLevel;

    [MirrorVerified("PtcProtectionDataMechanicalDifferentialTests")]
    public byte[] EncryptedPassword => _EncryptedPassowrd;

    [MirrorVerified("PtcProtectionDataMechanicalDifferentialTests")]
    public event PropertyChangedEventHandler PropertyChanged;

    [MirrorVerified("Release IL")]
    public ProtectionData(ProtectionDataType persistentData)
    {
        _EncryptedPassowrd = persistentData.Password;
        _timestamp = new DateTime?(persistentData.Timestamp);
        ShowTimestamp = persistentData.ShowTimestamp;
        _protectionLevel = persistentData.ProtectionLevelWrapper;
    }

    [MirrorVerified("Release IL")]
    public ProtectionData(ProtectionData protectionData, Version formatVersion)
    {
        _EncryptedPassowrd = AppServiceRegistry.GetService<IEncryptor>().UpdateEncryption(protectionData.EncryptedPassword, formatVersion);
        _timestamp = protectionData.Timestamp;
        ShowTimestamp = protectionData.ShowTimestamp;
        _protectionLevel = protectionData.ProtectionLevel;
    }
}
