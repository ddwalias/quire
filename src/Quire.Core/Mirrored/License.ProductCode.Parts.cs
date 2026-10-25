#nullable disable

namespace Quire.License;

public partial class ProductCode
{
    private string _part1;
    private string _part2;
    private string _part3;

    public string Part1
    {
        get => _part1;
        set => _part1 = value.ToUpperInvariant();
    }

    public string Part2
    {
        get => _part2;
        set => _part2 = value.ToUpperInvariant();
    }

    public string Part3
    {
        get => _part3;
        set => _part3 = value.ToUpperInvariant();
    }
}
