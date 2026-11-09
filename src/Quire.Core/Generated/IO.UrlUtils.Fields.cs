#nullable disable
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace Quire.IO;

public static partial class UrlUtils
{
    // Compile order places fields in Generated/ ahead of IO/UrlUtils.cs to preserve Mathcad closure ordinals.
    private static readonly string mathcad15Extenstion = ".xmcd";
    private const string TemplatePrimeExtenstion = ".mctx";
    private const string InternalLinkPrefix = "#";
    private const string PrimeExtenstion = ".mcdx";
    private static readonly List<UriProtocolCreationData> _schemeDetectionList = new List<UriProtocolCreationData>
    {
        new UriProtocolCreationData(Uri.UriSchemeMailto + ":", new Regex("^(\\S+)@(\\S+)\\.(\\S+)", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(10L))),
        new UriProtocolCreationData(Uri.UriSchemeHttp + Uri.SchemeDelimiter, new Regex("^www\\.(\\S+)", RegexOptions.IgnoreCase, TimeSpan.FromSeconds(10L)))
    };
}
