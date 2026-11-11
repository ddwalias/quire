using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Quire.Controls.Core;
using Quire.EquationEditor.Tree.Navigation;
using Quire.EquationEditor.Tree.Parsing;
using Quire.Native.PtcEquationEditor;

namespace Quire.EquationEditor.Tree;

public partial class MathNode
{
    public static partial class Utilities
    {
        public static string ToString(object obj)
        {
            if (obj != null)
            {
                return obj.ToString();
            }

            return "null";
        }

        public static bool MoveChildren(MathNode srcNode, MathNode tgtNode)
        {
            Fixity fixity = tgtNode.Fixity;
            if (fixity == Fixity.Special)
            {
                fixity = aw.a(tgtNode);
            }

            if (srcNode.Fixity != fixity)
            {
                return false;
            }

            switch (fixity)
            {
                case Fixity.Prefix:
                    tgtNode[tgtNode.ChildCount - 1] = srcNode[0];
                    break;
                case Fixity.Suffix:
                    tgtNode[0] = srcNode[0];
                    break;
                case Fixity.Infix:
                    tgtNode[0] = srcNode[0];
                    tgtNode[tgtNode.ChildCount - 1] = srcNode[1];
                    break;
                case Fixity.Outfix:
                    tgtNode[0] = srcNode[0];
                    break;
            }

            if (srcNode.NodeInfo != null && tgtNode.NodeInfo != null && srcNode.NodeInfo.GetType() == tgtNode.NodeInfo.GetType())
            {
                tgtNode.NodeInfo = (INodeInfo)srcNode.NodeInfo.Clone();
            }

            return true;
        }

        public static bool CopyChildren(MathNode srcNode, MathNode tgtNode)
        {
            if (srcNode.ChildCount > tgtNode.ChildCount)
            {
                return false;
            }

            for (int i = 0; i < srcNode.ChildCount; i++)
            {
                tgtNode[i] = srcNode[i].Clone();
            }

            if (srcNode.NodeInfo != null && tgtNode.NodeInfo != null && srcNode.NodeInfo.GetType() == tgtNode.NodeInfo.GetType())
            {
                tgtNode.NodeInfo = (INodeInfo)srcNode.NodeInfo.Clone();
            }

            return true;
        }

        public static void ValidateSubTree(MathNode subTreeRoot)
        {
            if (subTreeRoot == null)
            {
                return;
            }

            if (!subTreeRoot.IsRoot && subTreeRoot.IndexInParent == -1)
            {
                throw new q(p.b);
            }

            foreach (MathNode child in subTreeRoot.Children)
            {
                if (child.Parent == null)
                {
                    throw new q(p.c);
                }

                if (child.Parent != subTreeRoot)
                {
                    throw new q(p.b);
                }

                ValidateSubTree(child);
            }
        }

        public static IList<MathNode> PathsToNodes(MathNode root, IList<IList<int>> listOfPaths)
        {
            IList<MathNode> nodes = new List<MathNode>(listOfPaths.Count);
            foreach (IList<int> path in listOfPaths)
            {
                nodes.Add(root.DescendentFromPath(path));
            }

            return nodes;
        }

        public static IList<IList<int>> NodesToPaths(MathNode root, IList<MathNode> nodes)
        {
            List<IList<int>> paths = new List<IList<int>>(nodes.Count);
            foreach (MathNode node in nodes)
            {
                paths.Add(node.PathFromAncestor(root));
            }

            return paths;
        }

        public static MathNode FindCommonAncestor(MathNode node1, MathNode node2)
        {
            IEnumerator<MathNode> path1 = b(node1).GetEnumerator();
            IEnumerator<MathNode> path2 = b(node2).GetEnumerator();
            MathNode common = null;
            while (path1.MoveNext() && path2.MoveNext())
            {
                if (path1.Current != path2.Current)
                {
                    break;
                }

                common = path1.Current;
            }

            if (common == null)
            {
                throw new q(p.e);
            }

            return common;
        }

        private static IEnumerable<MathNode> b(MathNode node)
        {
            LinkedList<MathNode> path = new LinkedList<MathNode>();
            while (node != null)
            {
                path.AddFirst(node);
                node = node.Parent;
            }

            return path;
        }

        public static MathNode FindNearestCommonContainerRoot(MathNode node1, MathNode node2)
        {
            if (node1.RootNode != node2.RootNode)
            {
                throw new ArgumentException("Error: node1 and node2 must be in the same tree");
            }

            List<MathNode> roots = new List<MathNode>();
            roots.Add(node1.ContainerRoot);
            while (!roots.Last().IsEquationRoot)
            {
                roots.Add(roots.Last().Parent.ContainerRoot);
            }

            MathNode containerRoot = node2.ContainerRoot;
            while (!roots.Contains(containerRoot))
            {
                containerRoot = containerRoot.Parent.ContainerRoot;
            }

            return containerRoot;
        }

        public static bool HasParent(MathNode node, string descriptorName)
        {
            if (node == null)
            {
                throw new ArgumentNullException("node");
            }

            for (MathNode parent = node.Parent; parent != null; parent = parent.Parent)
            {
                if (parent.DescriptorName == descriptorName)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool AreTreesEqual(MathNode leftTree, MathNode rightTree)
        {
            return leftTree.IsEquivalentTo(rightTree);
        }

        public static IdLabels? IdLabelOfNode(MathNode node)
        {
            if (!(node.NodeInfo is IdentifierNodeInfo info))
            {
                return null;
            }

            return info.IdLabel;
        }

        public static IdLabels? DisplayIdLabelOfNode(MathNode node)
        {
            if (!(node.NodeInfo is IdentifierNodeInfo info))
            {
                return null;
            }

            return info.IdLabelForDisplay;
        }

        internal static MathNode a(MathNode valueNode, int start, int length, r factory)
        {
            if (!valueNode.CarriesValue)
            {
                throw new ArgumentException("Cannot extract a value from a node which doesn't carry a value", "valueNode");
            }

            if (valueNode.NodeInfo is StringValueNodeInfo stringInfo)
            {
                string text = stringInfo.Value.Substring(start, length);
                return factory.b(text);
            }

            IdentifierNodeInfo identifierInfo = (IdentifierNodeInfo)valueNode.NodeInfo;
            FormattedString value = identifierInfo.Value.Substring(start, length);
            IdentifierNodeInfo clone = identifierInfo.Clone();
            return factory.a(value, clone);
        }

        public static NavigatorState MergeWithPreviousProgram(MathNode current, Direction cursorDirection)
        {
            if (current.Family != OperatorFamily.Program)
            {
                return NavigatorState.Empty;
            }

            MathNode parent = current.Parent;
            int indexInParent = current.IndexInParent;
            if (parent.Family == OperatorFamily.Program && indexInParent > 0)
            {
                MathNode target = null;
                MathNode previous = parent[indexInParent - 1];
                if (previous.Family == OperatorFamily.Program)
                {
                    target = previous;
                }
                else if (previous.ChildCount > 0)
                {
                    MathNode last = previous[previous.ChildCount - 1];
                    if (last.Family == OperatorFamily.Program && last.IsInherentChildOfParent)
                    {
                        target = last;
                    }
                }

                if (target != null)
                {
                    int caretIndex = target.ChildCount;
                    Direction opposite = DirectionOps.Opposite(cursorDirection);
                    if (cursorDirection == Direction.Left)
                    {
                        caretIndex--;
                    }

                    target.MergeNode(current);
                    parent.RemoveChildren(indexInParent, 1);
                    return bq.c(opposite, target[caretIndex]).f();
                }
            }

            return NavigatorState.Empty;
        }

        internal static MathNode a(MathNode application, int argumentIndex)
        {
            if (application.DescriptorName != "APPLY" && application.DescriptorName != "FUNCTION_DEFINITION")
            {
                throw new InvalidOperationException("Logic Error: Can't get the function argument of a node which isn't a function application or definition");
            }

            MathNode node = application[1][0];
            Stack<MathNode> arguments = new Stack<MathNode>();
            while (node.DescriptorName == "ARGUMENT_SEPARATOR")
            {
                arguments.Push(node[1]);
                node = node[0];
            }

            arguments.Push(node);
            return arguments.ElementAtOrDefault(argumentIndex);
        }

        public static IList<MathNode> GetEvaluationNodes(MathNode rootNode)
        {
            if (rootNode == null)
            {
                return null;
            }

            List<MathNode> nodes = null;
            if (rootNode.IsEvaluation || (rootNode.DescriptorName == "ScriptableControl" && rootNode.NodeInfo is InputScriptableWithInputNodeInfo { IsInputsExists: not false }))
            {
                nodes = new List<MathNode> { rootNode };
            }
            else
            {
                foreach (MathNode child in rootNode.Children)
                {
                    if (child.IsEvaluation || child.DescriptorName == "ScriptableControl")
                    {
                        nodes = new List<MathNode> { child };
                        break;
                    }
                }
            }

            if (!nodes.IsNullOrEmpty())
            {
                while (nodes.Last()[0].IsEvaluation)
                {
                    nodes.Add(nodes.Last()[0]);
                }

                nodes.Reverse();
            }

            return nodes;
        }

        internal static MathNode a(string mantissa, int exponent, r factory)
        {
            MathNode mantissaNode = factory.b(mantissa);
            if (double.Parse(mantissa, NumberFormatInfo.InvariantInfo) == 0.0)
            {
                return mantissaNode;
            }

            if (exponent == 0)
            {
                return mantissaNode;
            }

            if (exponent == 1)
            {
                return factory.a("MULTIPLICATION", mantissaNode, factory.b("10"));
            }

            MathNode exponentNode;
            if (exponent > 1)
            {
                exponentNode = factory.b(exponent.ToString(NumberFormatInfo.InvariantInfo));
            }
            else
            {
                exponentNode = factory.a("NEGATION", factory.b((-exponent).ToString(NumberFormatInfo.InvariantInfo)));
            }

            return factory.a("MULTIPLICATION", mantissaNode, factory.a("EXPONENTIATION", factory.b("10"), exponentNode));
        }

        // Mathcad's method has .initlocals False.
        [System.Runtime.CompilerServices.SkipLocalsInit]
        internal static MathNode a(MathNode valueNode, string suffix, NumberFormat.ExponentStyles style, r factory, bool keepZero)
        {
            if (!keepZero && NodeValueIsZero(valueNode))
            {
                return valueNode;
            }

            if (style == NumberFormat.ExponentStyles.MultiplyByBaseToAPower)
            {
                if (valueNode.ChildCount == 0)
                {
                    return factory.b(valueNode.NodeInfoValueAsString + suffix);
                }

                return factory.a("MULTIPLICATION", factory.b(valueNode[0].NodeInfoValueAsString + suffix), valueNode[1]);
            }

            return factory.b(valueNode.NodeInfoValueAsString + suffix);
        }

        public static bool NodeValueIsZero(MathNode node)
        {
            if (!node.CarriesValue)
            {
                return false;
            }

            if (double.TryParse(node.NodeInfoValueAsString, out var result))
            {
                return result == 0.0;
            }

            return false;
        }

        internal static bool a(MathNode node)
        {
            if (!(node.DescriptorName == "IDENTIFIER"))
            {
                return node.DescriptorName == "FUNCTION_DEFINITION";
            }

            return true;
        }

        public static bool IsDefiniteIntegral(MathNode node)
        {
            if (!node[0].IsPlaceholder)
            {
                return !node[1].IsPlaceholder;
            }

            return false;
        }
    }
}
