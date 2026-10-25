#nullable disable

namespace Quire.License;

public partial class ProductCode
{
    // Keep these members before the lambda so it emits Mathcad's <get_IsValid>b__18_0 ordinal.
    private string _part1 = "";
    private string _part2 = "";
    private string _part3 = "";

    public override string ToString() => throw global::Quire.Mirror.MirrorStub.NotImplemented();

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

    public string SON
    {
        get
        {
            if (!StartsWithX)
            {
                return null;
            }

            long number = long.Parse(SubstringHelper(Part1, 1, 14));
            return string.Format("{0}", number);
        }
    }

    public string CustomerNumber
    {
        get
        {
            if (!StartsWithX)
            {
                return null;
            }

            long number = long.Parse(string.Concat(Part2, Part3));
            return string.Format("{0}", number);
        }
    }

    public bool IsValid
    {
        get
        {
            bool hasExpectedLength = Code.Length == 22;
            if (!hasExpectedLength)
            {
                return false;
            }

            return Code.ToCharArray().All(character => "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".Contains(character));
        }
    }

    public bool StartsWithX => Code.StartsWith("X");

    private static string SubstringHelper(string value, int start, int length)
    {
        start = global::System.Math.Min(value.Length, start);
        length = global::System.Math.Min(value.Length - start, length);
        return value.Substring(start, length).ToUpperInvariant();
    }

    public ProductCode()
    {
    }
}
