#nullable disable
namespace Quire.Controls.Core;

public partial interface IPropertyAggregate
{
    object GetValueByName(string propertyName);
    void SetValueByName(string propertyName, object value);
}
