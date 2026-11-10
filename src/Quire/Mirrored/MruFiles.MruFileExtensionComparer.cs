#nullable disable

using System.Collections.Generic;
using System.IO;

namespace Quire.MruFiles;

public partial class MruFileExtensionComparer
{
    public new bool Equals(object x, object y)
    {
        if (!IsMctxExtension(x as string))
        {
            return !IsMctxExtension(y as string);
        }

        return IsMctxExtension(y as string);
    }

    public int GetHashCode(object obj)
    {
        if (!IsMctxExtension(obj as string))
        {
            return ".*".GetHashCode();
        }

        return ".mctx".GetHashCode();
    }

    private static bool IsMctxExtension(string path)
    {
        string extension = Path.GetExtension(path);
        return ".mctx".Equals(extension, System.StringComparison.OrdinalIgnoreCase);
    }
}
