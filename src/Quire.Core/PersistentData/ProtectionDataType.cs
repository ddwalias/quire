#nullable disable
using Quire.Mirror;

namespace Quire.PersistentData;

public partial class ProtectionDataType
{
    public DateTime? TimestampWrapper
    {
        get
        {
            if (!TimestampSpecified)
            {
                return null;
            }
            return Timestamp;
        }
        set
        {
            if (value.HasValue)
            {
                Timestamp = DateTime.SpecifyKind(value.Value, DateTimeKind.Unspecified);
                TimestampSpecified = true;
            }
            else
            {
                TimestampSpecified = false;
            }
        }
    }

    [MirrorVerified("PtcPersistentDataProtectionDataTypeMechanicalDifferentialTests")]
    private bool showTimestampField;

    [MirrorVerified("PtcPersistentDataProtectionDataTypeMechanicalDifferentialTests")]
    public ProtectionDataType()
    {
        showTimestampField = false;
    }

    [MirrorVerified("PtcPersistentDataProtectionDataTypeMechanicalDifferentialTests")]
    private byte[] passwordField;

    [MirrorVerified("PtcPersistentDataProtectionDataTypeMechanicalDifferentialTests")]
    public byte[] Password { get => passwordField; set => passwordField = value; }

    [MirrorVerified("PtcPersistentDataProtectionDataTypeMechanicalDifferentialTests")]
    public bool ShowTimestamp { get => showTimestampField; set => showTimestampField = value; }

    [MirrorVerified("PtcPersistentDataProtectionDataTypeMechanicalDifferentialTests")]
    private DateTime timestampField;

    [MirrorVerified("PtcPersistentDataProtectionDataTypeMechanicalDifferentialTests")]
    public DateTime Timestamp { get => timestampField; set => timestampField = value; }

    [MirrorVerified("PtcPersistentDataProtectionDataTypeMechanicalDifferentialTests")]
    private bool timestampFieldSpecified;

    [MirrorVerified("PtcPersistentDataProtectionDataTypeMechanicalDifferentialTests")]
    public bool TimestampSpecified { get => timestampFieldSpecified; set => timestampFieldSpecified = value; }

    [MirrorVerified("PtcPersistentDataProtectionDataTypeMechanicalDifferentialTests")]
    private ProtectionLevelType[] protectionLevelField;

    [MirrorVerified("PtcPersistentDataProtectionDataTypeMechanicalDifferentialTests")]
    public ProtectionLevelType[] ProtectionLevel { get => protectionLevelField; set => protectionLevelField = value; }
}
