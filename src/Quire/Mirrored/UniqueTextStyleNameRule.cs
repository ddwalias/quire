#nullable disable

namespace Quire;

public partial class UniqueTextStyleNameRule
{
    private UniqueTextStyleNameHelper _helper;

    public string StyleNames { get; set; }

    public UniqueTextStyleNameHelper DependencyPropertyHelper
    {
        get
        {
            return _helper;
        }
        set
        {
            _helper = value;
            value?.SetValidator(this);
        }
    }
}
