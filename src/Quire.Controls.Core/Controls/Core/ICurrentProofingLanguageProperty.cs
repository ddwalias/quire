#nullable disable
using System.Windows.Markup;

namespace Quire.Controls.Core;

public partial interface ICurrentProofingLanguageProperty
{
    XmlLanguage CurrentProofingLanguage { get; }
}
