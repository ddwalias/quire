#nullable disable
using System.Text.RegularExpressions;
using Quire.Mirror;

namespace Quire.IO;

public static partial class UrlUtils
{
    partial class UriProtocols
    {
        [MirrorVerified("PtcIOUrlUtilsMechanicalDifferentialTests")]
        private static readonly HashSet<string> Protocols;

        [MirrorVerified("PtcIOUrlUtilsMechanicalDifferentialTests")]
        public static bool Contains(string element) => Protocols.Contains(element);
    }

    partial struct UriProtocolCreationData
    {
        [MirrorVerified("PtcIOUrlUtilsMechanicalDifferentialTests")]
        public UriProtocolCreationData(string schemeWithDelimiter, Regex matchPattern)
        {
            SchemeWithDelimiter = schemeWithDelimiter;
            MatchPattern = matchPattern;
        }

        [MirrorVerified("PtcIOUrlUtilsMechanicalDifferentialTests")]
        public Regex MatchPattern { get; }

        [MirrorVerified("PtcIOUrlUtilsMechanicalDifferentialTests")]
        public string SchemeWithDelimiter { get; }
    }
}
