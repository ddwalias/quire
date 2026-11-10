using System;

namespace Quire.Controls;

public partial class InsertFunctionCommandParameterComparer
{
    public bool Equals(object x, object y)
    {
        return Equals(x as InsertFunctionCommandParameter, y as InsertFunctionCommandParameter);
    }

    public int GetHashCode(object obj)
    {
        return GetHashCode(obj as InsertFunctionCommandParameter);
    }

    public bool Equals(InsertFunctionCommandParameter x, InsertFunctionCommandParameter y)
    {
        if (object.Equals(x.Name, y.Name))
        {
            var xLabel = x.Label;
            var yLabel = y.Label;
            if (xLabel.GetValueOrDefault() == yLabel.GetValueOrDefault() & xLabel.HasValue == yLabel.HasValue)
            {
                if (x.ArgsCount == y.ArgsCount)
                {
                    if (x.Args != y.Args)
                    {
                        // Release IL converts each array to ReadOnlySpan<string> with op_Implicit and calls SequenceEqual directly.
                        if (x.Args != null && y.Args != null)
                        {
                            return MemoryExtensions.SequenceEqual<string>(x.Args, y.Args);
                        }

                        return false;
                    }

                    return true;
                }
            }
        }

        return false;
    }

    public int GetHashCode(InsertFunctionCommandParameter obj)
    {
        return (obj.Name.ToString() + obj.Label.ToString() + obj.ArgsCount.ToString() + (obj.Args == null ? string.Empty : string.Concat(obj.Args))).GetHashCode();
    }
}
