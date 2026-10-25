#nullable disable

namespace Quire.License;

public partial class ProductCode
{
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
}
