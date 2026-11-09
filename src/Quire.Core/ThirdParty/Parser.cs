#nullable disable
using System.Reflection;
using System.Reflection;
using Quire.Mirror;
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
        [Argument(ArgumentType.AtMostOnce, ShortName = "?")]
        public bool help;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        public HelpArgument() { }
    }

    private partial struct COORD
    {
        internal short x;
        internal short y;
    }

    private partial struct SMALL_RECT
    {
        internal short Left;
        internal short Top;
        internal short Right;
        internal short Bottom;
    }

    private partial struct CONSOLE_SCREEN_BUFFER_INFO
    {
        internal COORD dwSize;
        internal COORD dwCursorPosition;
        internal short wAttributes;
        internal SMALL_RECT srWindow;
        internal COORD dwMaximumWindowSize;
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

        private Type elementType;
        private ArgumentType flags;
        private global::System.Collections.ArrayList collectionValues;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        private bool isDefault;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        public bool IsDefault => isDefault;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        private string validValues;

        [MirrorVerified("PtcThirdPartyParserMechanicalDifferentialTests")]
        public string ValidValues { get => validValues; set => validValues = value; }
    }

    [MirrorVerified("Release IL")]
    private void ReportUnrecognizedArgument(string argument) => ReportError(string.Format("Unrecognized command line argument '{0}'", argument));

    [MirrorVerified("Release IL")]
    private static ArgumentHelpStrings GetHelpStrings(Argument arg) => new ArgumentHelpStrings(arg.SyntaxHelp, arg.FullHelpText);

    public const string NewLine = "\r\n";
    private const int STD_OUTPUT_HANDLE = -11;
    private const int spaceBeforeParam = 2;
    private global::System.Collections.ArrayList arguments;
    private global::System.Collections.Hashtable argumentMap;

    [MirrorVerified("Release IL")]
    private Argument defaultArgument;

    private ErrorReporter reporter;

    [MirrorVerified("Release IL")]
    public bool HasDefaultArgument => defaultArgument != null;

    partial class Argument
    {
        [MirrorVerified("Release IL")]
        public bool IsRequired => (uint)(flags & ArgumentType.Required) > 0u;

        [MirrorVerified("Release IL")]
        public bool Unique => (uint)(flags & ArgumentType.Unique) > 0u;

        [MirrorVerified("Release IL")]
        public bool AllowMultiple => (uint)(flags & ArgumentType.Multiple) > 0u;

        [MirrorVerified("Release IL")]
        public Type ValueType
        {
            get
            {
                if (!IsCollection)
                {
                    return Type;
                }
                return elementType;
            }
        }

        [MirrorVerified("Release IL")]
        private ErrorReporter reporter;

        [MirrorVerified("Release IL")]
        private void ReportDuplicateArgumentValue(string value) => reporter.Invoke(string.Format("Duplicate '{0}' argument '{1}'", LongName, value));

        [MirrorVerified("Release IL")]
        private void ReportBadArgumentValue(string value) => reporter.Invoke(string.Format("'{0}' is not a valid value for the '{1}' command line option", value, LongName));

        [MirrorVerified("Release IL")]
        private void ReportInvalidValue(string value) => reporter.Invoke(string.Format("'{0}' is not a valid value for the '{1}' command line option. Acceptable values are {2}", value, LongName, ValidValues.Replace("|", ", ")));

        [MirrorVerified("Release IL")]
        public void ClearShortName() => shortName = null;

        [MirrorVerified("Release IL")]
        public bool HasShortName => shortName != null;

        [MirrorVerified("Release IL")]
        public bool HasDefaultValue => defaultValue != null;

        [MirrorVerified("Release IL")]
        public bool IsCollection => IsCollectionType(Type);
    }
}
