#nullable disable
using Quire.Mirror;

namespace Quire;

public partial class WpfClipboard
{
    [MirrorVerified("Release IL")]
    public void SetSExpression(string sExpression)
    {
        System.Windows.DataObject dataObject = new System.Windows.DataObject();
        CopySExpressionToDataObject(dataObject, sExpression);
        dataObject.SetText(sExpression);
        SetDataObjectToClipboard(dataObject, false);
    }
}
