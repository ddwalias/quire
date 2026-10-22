#nullable disable
#pragma warning disable

namespace Quire.Core;

partial interface IHelpSettings
{
    bool DocOptionsEnabled { get; set; }
    bool IsHelpDefault { get; set; }
    string HelpPath { get; set; }
}
