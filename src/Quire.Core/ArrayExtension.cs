using Quire.Mirror;

namespace Quire;

public static partial class ArrayExtension
{
    [MirrorVerified("PtcArrayExtensionTierAMechanicalDifferentialTests")]
    public static void Fill<T>(this T[] array, T value, int startIndex = 0, int endIndex = -1)
    {
        endIndex = endIndex < 0 || endIndex > array.Length ? array.Length : endIndex;
        startIndex = startIndex < 0 || startIndex > array.Length ? 0 : startIndex;
        for (int i = startIndex; i < endIndex; i++)
        {
            array[i] = value;
        }
    }
}
