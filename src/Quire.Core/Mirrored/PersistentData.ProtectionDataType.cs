#nullable disable
#pragma warning disable

namespace Quire.PersistentData;

partial class ProtectionDataType
{
    public global::System.Nullable<global::System.DateTime> TimestampWrapper
    {
        get
        {
            if (TimestampSpecified)
            {
                goto HasTimestamp;
            }
            return default(global::System.Nullable<global::System.DateTime>);

        HasTimestamp:
            return Timestamp;
        }
        set
        {
            if (value.HasValue)
            {
                Timestamp = global::System.DateTime.SpecifyKind(value.Value, global::System.DateTimeKind.Unspecified);
                TimestampSpecified = true;
            }
            else
            {
                TimestampSpecified = false;
            }
        }
    }
    public global::Quire.ProtectionLevel ProtectionLevelWrapper
    {
        get
        {
            global::Quire.ProtectionLevel protectionLevel;
            global::Quire.EnumHelper.EnumHelperSettings settings;
            global::Quire.EnumHelper.EnumHelperSettings defaultSettings = default;
            defaultSettings.IgnoreCase = true;
            defaultSettings.AssertOnParseFailure = true;
            settings = defaultSettings;
            string value = global::System.String.Join<global::Quire.PersistentData.ProtectionLevelType>(", ", ProtectionLevel);
            global::Quire.EnumHelper.TryParse<global::Quire.ProtectionLevel>(value, out protectionLevel, settings);
            return protectionLevel;
        }
        set
        {
            global::Quire.EnumHelper.EnumHelperSettings settings;
            global::Quire.EnumHelper.EnumHelperSettings defaultSettings = default;
            defaultSettings.IgnoreCase = true;
            defaultSettings.AssertOnParseFailure = true;
            settings = defaultSettings;
            ProtectionLevel = global::Quire.EnumHelper.TryConvertFlagsToList<global::Quire.ProtectionLevel, global::Quire.PersistentData.ProtectionLevelType>(value, settings)
                ?? new global::Quire.PersistentData.ProtectionLevelType[1];
        }
    }
}
