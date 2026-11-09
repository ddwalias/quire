#nullable disable

namespace Quire.License;

public partial class LicenseConfigProvider
{
    // Compile order places XmlNodeNames in License/ after 00_LicenseConfigProvider.Ordinal to preserve Mathcad closure ordinals.
    private static partial class XmlNodeNames
    {
        public const string LicenseConfigElemName = "LicenseConfig";
        public const string LicenseCodesElemName = "LicenseCodes";
        public const string HashCodeElemName = "HashCode";
        public const string ProductCodeElemName = "ProductCode";
    }
}
