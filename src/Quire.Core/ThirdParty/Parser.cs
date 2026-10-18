#nullable disable
using System.Reflection;
using Quire.Mirror;

namespace Quire.ThirdParty;

public partial class Parser
{
    [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
    private static void NullErrorReporter(string message) { }

    [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
    private static string ValidValues(ArgumentAttribute attribute, FieldInfo fieldInfo) => attribute.ValidValues;

    [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
    private static bool IsCollectionType(Type type) => type.IsArray;

    partial class HelpArgument
    {
        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        public HelpArgument() { }
    }

    partial struct ArgumentHelpStrings
    {
        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        public string syntax;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        public string help;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        public ArgumentHelpStrings(string syntax, string help)
        {
            this.syntax = syntax;
            this.help = help;
        }
    }

    partial class Argument
    {
        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        private string longName;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        public string LongName => longName;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        private bool explicitShortName;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        public bool ExplicitShortName => explicitShortName;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        private string shortName;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        public string ShortName => shortName;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        private bool hasHelpText;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        public bool HasHelpText => hasHelpText;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        private string helpText;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        public string HelpText => helpText;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        private object defaultValue;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        public object DefaultValue => defaultValue;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        private bool seenValue;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        public bool SeenValue => seenValue;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        private FieldInfo fieldInfo;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        public Type Type => fieldInfo.FieldType;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        private bool isDefault;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        public bool IsDefault => isDefault;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        private string validValues;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        public string ValidValues { get => validValues; set => validValues = value; }
    }
}
