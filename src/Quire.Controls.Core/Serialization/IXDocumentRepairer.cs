#nullable disable
using System.Xml.Linq;

namespace Quire.Controls.Core.Serialization;

public partial interface IXDocumentRepairer
{
    void RepairXDocument(XDocument xDocument);
}
