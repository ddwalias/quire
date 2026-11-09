#nullable disable
using System.Windows;

namespace Quire;

public abstract partial class TextStyleBase
{
    // Compile order places fields in Generated/ ahead of TextStyleBase.cs to preserve Mathcad closure ordinals.
    protected FontStretch _fontStretch;
}
