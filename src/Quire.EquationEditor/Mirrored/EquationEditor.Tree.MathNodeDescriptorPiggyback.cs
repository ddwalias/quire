namespace Quire.EquationEditor.Tree;

public partial class MathNodeDescriptorPiggyback
{
    private string b3;
    private bool b4;

    public MathNodeDescriptorPiggyback(string piggyBackName, string baseDescriptorName) : base(baseDescriptorName)
    {
        b3 = piggyBackName;
    }

    public override string DescriptorName => b3;
}
