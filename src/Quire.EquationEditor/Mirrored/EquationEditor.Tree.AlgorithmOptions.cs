using System;
using System.Collections.Generic;

namespace Quire.EquationEditor.Tree;

public partial class AlgorithmOptions
{
    private bool b;

    public AlgorithmOptions()
    {
        IsMultiStart = false;
        IsOptimize = false;
    }

    public AlgorithmOptions(AlgorithmOptions other)
    {
        if (other != null)
        {
            IsMultiStart = other.IsMultiStart;
            IsOptimize = other.IsOptimize;
        }
    }

    public bool IsOptimize
    {
        get => b;
        set => b = value;
    }

    public bool HasAnyOption => IsMultiStart || IsOptimize;

    public void ToggleMultiStart()
    {
        IsMultiStart = !IsMultiStart;
    }

    public void ToggleOptimize()
    {
        IsOptimize = !IsOptimize;
    }

    public AlgorithmOptions Clone()
    {
        return new AlgorithmOptions(this);
    }

    object ICloneable.Clone()
    {
        return Clone();
    }

    public void Reset()
    {
        IsMultiStart = false;
        IsOptimize = false;
    }

    public string ToSExpression()
    {
        if (!HasAnyOption)
        {
            return string.Empty;
        }
        var options = new List<string>();
        if (IsMultiStart)
        {
            options.Add(MultiStart);
        }
        if (IsOptimize)
        {
            options.Add(Optimize);
        }
        return "-" + string.Join(OptionSeparator.ToString(), options);
    }
}
