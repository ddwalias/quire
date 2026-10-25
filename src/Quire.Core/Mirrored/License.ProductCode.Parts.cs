#nullable disable

namespace Quire.License;

public partial class ProductCode
{
    private string _part1;
    private string _part2;
    private string _part3;

    public string Code
    {
        get => string.Concat(_part1, _part2, _part3);
        set
        {
            char[] separator = ['-'];
            string[] parts = value.Split(separator, 3);
            int partCount = parts.Length;
            if (partCount != 2)
            {
                if (partCount == 3)
                {
                    _part1 = SubstringHelper(parts[0], 0, 14);
                    _part2 = SubstringHelper(parts[1], 0, 4);
                    _part3 = SubstringHelper(parts[2], 0, 4);
                    return;
                }
            }
            else
            {
                _part1 = SubstringHelper(parts[0], 0, 14);
                _part2 = SubstringHelper(parts[1], 0, 4);
                _part3 = SubstringHelper(parts[1], 4, 4);
                return;
            }

            _part1 = SubstringHelper(value, 0, 14);
            _part2 = SubstringHelper(value, 14, 4);
            _part3 = SubstringHelper(value, 18, 4);
        }
    }

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

    private static string SubstringHelper(string value, int start, int length)
    {
        start = global::System.Math.Min(value.Length, start);
        length = global::System.Math.Min(value.Length - start, length);
        return value.Substring(start, length).ToUpperInvariant();
    }
}
