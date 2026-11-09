namespace Quire.EquationEditor.Tree;

public static partial class NodeInfoExtensions
{
    public static int ValueLength(INodeInfo nodeInfo, NodeValueType nodeValueType)
    {
        switch (nodeInfo)
        {
            case StringValueNodeInfo stringValueNodeInfo:
                return stringValueNodeInfo.Value.Length;
            case IdentifierNodeInfo identifierNodeInfo:
                return identifierNodeInfo.Value.Length;
            default:
                DebugTools.DevAssert(nodeValueType != NodeValueType.None, nodeInfo?.GetType().Name);
                return 0;
        }
    }

    public static string ValueAsString(INodeInfo nodeInfo, NodeValueType nodeValueType)
    {
        switch (nodeInfo)
        {
            case StringValueNodeInfo stringValueNodeInfo:
                return stringValueNodeInfo.Value;
            case IdentifierNodeInfo identifierNodeInfo:
                return identifierNodeInfo.Value.ToString();
            default:
                DebugTools.DevAssert(nodeValueType != NodeValueType.None, nodeInfo?.GetType().Name);
                return string.Empty;
        }
    }

    public static void ClearValue(this INodeInfo nodeInfo)
    {
        switch (nodeInfo)
        {
            case StringValueNodeInfo stringValueNodeInfo:
                stringValueNodeInfo.Value = string.Empty;
                break;
            case IdentifierNodeInfo identifierNodeInfo:
                identifierNodeInfo.Value = identifierNodeInfo.Value.IsEditable ? FormattedString.CreateEditable() : FormattedString.CreateReadOnly();
                break;
            case ValueNodeInfo valueNodeInfo:
                valueNodeInfo.Value = null;
                break;
            default:
                DebugTools.FailAssertion(nodeInfo?.GetType().Name);
                break;
        }
    }

    public static void SetValue(this INodeInfo nodeInfo, string newValue)
    {
        switch (nodeInfo)
        {
            case StringValueNodeInfo stringValueNodeInfo:
                _ = stringValueNodeInfo.Value;
                stringValueNodeInfo.Value = newValue;
                break;
            case IdentifierNodeInfo identifierNodeInfo:
                _ = identifierNodeInfo.Value;
                identifierNodeInfo.Value = newValue;
                break;
            default:
                DebugTools.FailAssertion(nodeInfo?.GetType().Name);
                break;
        }
    }
}
