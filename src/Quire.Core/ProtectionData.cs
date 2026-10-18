#nullable disable
using System.ComponentModel;
using Quire.Mirror;

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
}
