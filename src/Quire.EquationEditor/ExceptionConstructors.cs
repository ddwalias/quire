namespace Quire.EquationEditor
{
    public partial class InvalidEditorCommandException
    {
        public InvalidEditorCommandException()
        {
        }

        public InvalidEditorCommandException(string s) : base(s)
        {
        }
    }
}

namespace Quire.EquationEditor.Tree.Transform
{
    public partial class InvalidSExpression
    {
        public InvalidSExpression(string expression) : base(expression)
        {
        }

        public InvalidSExpression(string expression, global::System.Exception inner) : base(expression, inner)
        {
        }
    }
}

namespace Quire.EquationEditor.Tree
{
    public partial class MathNodeInvalidLabelException
    {
        public MathNodeInvalidLabelException(string message, string paramName) : base(message, paramName)
        {
        }

        public MathNodeInvalidLabelException(string message) : base(message)
        {
        }

        public MathNodeInvalidLabelException()
        {
        }

        public MathNodeInvalidLabelException(string message, global::System.Exception innerException) : base(message, innerException)
        {
        }

        public MathNodeInvalidLabelException(string message, string paramName, global::System.Exception innerException) : base(message, paramName, innerException)
        {
        }
    }
}
