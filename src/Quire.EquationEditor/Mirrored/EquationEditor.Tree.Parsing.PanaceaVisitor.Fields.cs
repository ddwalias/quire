using Quire.EquationEditor.Tree.Parsing.Panceaux;

namespace Quire.EquationEditor.Tree.Parsing;

public partial class PanaceaVisitor
{
    public Delegate1<Panacea> PanaceaHandler;
    public Delegate1<ReparentChild> ReparentChildHandler;
    public Delegate1<ReparentNode> ReparentNodeHandler;
    public Delegate1<ReplaceChildDescriptor> ReplaceChildDescriptorHandler;
    public Delegate1<TransformTo> TransformToHandler;
    public Delegate1<BinaryMergeValueChildren> BinaryMergeValueChildrenHandler;
    public Delegate1<ReplaceWithDefaultNode> ReplaceWithDefaultNodeHandler;
    public Delegate1<ReplaceWithChild> ReplaceWithChildHandler;
    public Delegate1<ReplaceWith> ReplaceWithHandler;
    public Delegate1<AutoLabelIdDescendant> AutoLabelIdDescendantHandler;
    public Delegate1<AutoLabelIdChild> AutoLabelIdChildHandler;
    public Delegate1<ReparentNodeAndPullOutChild> ReparentNodeAndPullOutChildHandler;
    public Delegate1<PullOutAdjacentDescendantFromChild> PullOutAdjacentDescendantFromChildHandler;
    public Delegate1<InsertChildrenIntoParent> InsertChildrenIntoParentHandler;
}
