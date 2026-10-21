#nullable disable
using System.Runtime.InteropServices;
using Quire.Mirror;

namespace Quire.Calculation.Core.Support;

public partial struct RegexForRL
{
    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string NodeTag = "?::0x[0-9a-fA-F]{8}";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string OptNodeTag = "(?::0x[0-9a-fA-F]{8})?";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string Arg = "(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string DefinedArg = "(?<defined>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-defined))";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string ExpressionArg = "(?<expression>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-expression))";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string ScaleFactorArg = "(?<scaleFactor>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-scaleFactor))";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string KeywordsArg = "(?<keywords>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-keywords))";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string ResultIdArg = "(?<resultId>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-resultId))";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string EvalOperator = "(?<operator>eval)(?<operatorTag>(?::0x[0-9a-fA-F]{8})?)";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string DefineOperator = "(?<operator>define)(?<operatorTag>(?::0x[0-9a-fA-F]{8})?)";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string DefEvalOperator = "(?<operator>define-eval)(?<operatorTag>(?::0x[0-9a-fA-F]{8})?)";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string DefSymEvalOperator = "(?<operator>define-symeval)(?<operatorTag>(?::0x[0-9a-fA-F]{8})?)";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string SymEvalOperator = "(?<operator>define-only-symbolic)(?<operatorTag>(?::0x[0-9a-fA-F]{8})?)";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string EvalMatch = "\\((?<operator>eval)(?<operatorTag>(?::0x[0-9a-fA-F]{8})?)\\s+(?<expression>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-expression))\\s*(?:\\s(?<scaleFactor>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-scaleFactor)))?\\s*\\)";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string DefineMatch = "\\((?<operator>define)(?<operatorTag>(?::0x[0-9a-fA-F]{8})?)\\s+(?<defined>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-defined))\\s+(?<expression>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-expression))\\s*\\)";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string DefEvalMatch = "\\((?<operator>define-eval)(?<operatorTag>(?::0x[0-9a-fA-F]{8})?)\\s+(?<defined>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-defined))\\s+(?<expression>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-expression))\\s*(?:\\s(?<scaleFactor>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-scaleFactor)))?\\s*\\)";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string SymbolicEvalMatch = "\\((?<operator>define-only-symbolic)(?<operatorTag>(?::0x[0-9a-fA-F]{8})?)\\s+(?<resultId>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-resultId))\\s+(?<expression>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-expression))\\s*(?:\\s(?<keywords>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-keywords)))?\\s*\\)";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string DefineSymbolicEvalMatch = "\\((?<operator>define-symeval)(?<operatorTag>(?::0x[0-9a-fA-F]{8})?)\\s+(?<resultId>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-resultId))\\s+(?<expression>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-expression))\\s*(?:\\s(?<keywords>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-keywords)))?\\s*\\)";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string DefinitionAndDefined = "(?<operator>define(?:-eval)?)(?::0x[0-9a-fA-F]{8})?\\s+(?<defined>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-defined))";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string DefineSymEval = "(?<operator>define-symeval)(?::0x[0-9a-fA-F]{8})?\\s+(?<defined>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-defined))";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string Eval = "(?<operator>eval)(?::0x[0-9a-fA-F]{8})?";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string SymbolicEval = "(?<operator>define-only-symbolic)(?::0x[0-9a-fA-F]{8})?";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string EvalOrDefinition = "(?:(?<operator>eval)(?::0x[0-9a-fA-F]{8})?)|(?:(?<operator>define(?:-eval)?)(?::0x[0-9a-fA-F]{8})?\\s+(?<defined>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-defined)))|(?:(?<operator>define-only-symbolic)(?::0x[0-9a-fA-F]{8})?)|(?:(?<operator>define-symeval)(?::0x[0-9a-fA-F]{8})?\\s+(?<defined>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-defined)))(?#after eval or definition)";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string SolverName = "(?<solver_name>[fF]ind|[mM]inerr|[mM]inimize|[mM]aximize|[oO]desolve|[pP]desolve)";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string SolverCase = "(?<solver_case>KEYWORD|VARIABLE)";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string ApplyOfSolver = "(?:\\(op_apply(?::0x[0-9a-fA-F]{8})?)\\s+mc_(?<solver_name>[fF]ind|[mM]inerr|[mM]inimize|[mM]aximize|[oO]desolve|[pP]desolve)\\{(?<solver_case>KEYWORD|VARIABLE)\\}(?<solver_tag>(?::0x[0-9a-fA-F]{8})?(?#endof_solver_tag))(?:\\s+(?<solver_arg>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof_solver_arg))(?#endof_arg_with_space))+\\s*\\)";

    [MirrorVerified("PtcCalculationCoreSupportRegexForRLMechanicalDifferentialTests")]
    public const string SolverExpression = "^(?<left_of_solver>\\((?:(?:(?<operator>eval)(?::0x[0-9a-fA-F]{8})?)|(?:(?<operator>define(?:-eval)?)(?::0x[0-9a-fA-F]{8})?\\s+(?<defined>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-defined)))|(?:(?<operator>define-only-symbolic)(?::0x[0-9a-fA-F]{8})?)|(?:(?<operator>define-symeval)(?::0x[0-9a-fA-F]{8})?\\s+(?<defined>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-defined)))(?#after eval or definition)).*(?#endof_left_of_solver))(?:\\(op_apply(?::0x[0-9a-fA-F]{8})?)\\s+mc_(?<solver_name>[fF]ind|[mM]inerr|[mM]inimize|[mM]aximize|[oO]desolve|[pP]desolve)\\{(?<solver_case>KEYWORD|VARIABLE)\\}(?<solver_tag>(?::0x[0-9a-fA-F]{8})?(?#endof_solver_tag))(?:\\s+(?<solver_arg>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof_solver_arg))(?#endof_arg_with_space))+\\s*\\)(?<right_of_solver>.*\\))";
}
