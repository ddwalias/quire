#nullable disable
namespace Quire.Controls.Core;

public partial interface IWorksheetPropertiesTarget
{
    void OnWorksheetPropertiesChanged(string[] propertyNames, IWorksheetProperties oldProperties, IWorksheetProperties newProperties);
}
