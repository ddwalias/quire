#nullable disable
using System.Collections.Generic;
using Quire.Controls.Core;
using Quire.Mirror;

namespace Quire.EquationEditor.Tree;

public partial class MathNodeDescriptorTable
{
    public IList<FunctionOperatorDescriptor> FunctionOperatorsList;

    public IDictionary<string, IdLabels> IDLabelMapDictionary;

    public IEnumerable<IMathNodeDescriptor> ValueDescriptors => new c(-2) { d = this };

    public IEnumerable<IMathNodeDescriptor> FormattedStringValueDescriptors => new b(-2) { d = this };

    public void Add(MathNodeDescriptor descriptor)
    {
        Add(descriptor.DescriptorName, descriptor);
    }

    public MathNodeDescriptor FindMatchingValueDescriptor(string value)
    {
        foreach (MathNodeDescriptor valueDescriptor in ValueDescriptors)
        {
            if (IsMatchingDescriptor(valueDescriptor, value))
            {
                return valueDescriptor;
            }
        }
        return null;
    }

    public MathNodeDescriptor FindMatchingValueDescriptor(FormattedString value)
    {
        string text = value.ToString();
        foreach (MathNodeDescriptor valueDescriptor in FormattedStringValueDescriptors)
        {
            if (IsMatchingDescriptor(valueDescriptor, text))
            {
                return valueDescriptor;
            }
        }
        return null;
    }

    public bool IsMatchingDescriptor(IMathNodeDescriptor descriptor, string value)
    {
        if (descriptor.CarriesValue && descriptor.MatchRegex != null)
        {
            return descriptor.MatchRegex.IsMatch(value);
        }
        return false;
    }

    [MirrorVerified("Release IL")]
    public MathNodeDescriptorTable() { }

    partial class a
    {
        [MirrorVerified("Release IL")]
        public a() { }
    }
}
