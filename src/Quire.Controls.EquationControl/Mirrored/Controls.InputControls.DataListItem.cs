namespace Quire.Controls.InputControls;

public partial class DataListItem
{
    public string Name { get; set; }

    public string Value { get; set; }

    public string GroupName { get; set; }

    public override string ToString() => Name;
}
