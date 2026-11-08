using System.Text.RegularExpressions;

namespace Quire.Controls;

internal partial class SolveBlockCalculatable
{
    private static Regex _definitionMatch = new Regex("^\\(\\s*(define|define-eval)[:\\s]");
    private static Regex _constraintMatch = new Regex("^\\(\\s*(op_(lt|gt|eq|leq|geq|neq)|\\(\\s*relation)[:\\s]");
    protected static Regex _solverMatch = new Regex("^(?<left_of_solver>\\((?:(?:(?<operator>eval)(?::0x[0-9a-fA-F]{8})?)|(?:(?<operator>define(?:-eval)?)(?::0x[0-9a-fA-F]{8})?\\s+(?<defined>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-defined)))|(?:(?<operator>define-only-symbolic)(?::0x[0-9a-fA-F]{8})?)|(?:(?<operator>define-symeval)(?::0x[0-9a-fA-F]{8})?\\s+(?<defined>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof-defined)))(?#after eval or definition)).*(?#endof_left_of_solver))(?:\\(op_apply(?::0x[0-9a-fA-F]{8})?)\\s+mc_(?<solver_name>[fF]ind|[mM]inerr|[mM]inimize|[mM]aximize|[oO]desolve|[pP]desolve)\\{(?<solver_case>KEYWORD|VARIABLE)\\}(?<solver_tag>(?::0x[0-9a-fA-F]{8})?(?#endof_solver_tag))(?:\\s+(?<solver_arg>(?(\\()(?:\\((?:(?:[^()]*(?<open>\\())+(?:[^()]*(?<close-open>\\)))+)*[^()]*\\)(?(open)(?!)))|[^\\s()]+(?#endof-argmatch))(?#endof_solver_arg))(?#endof_arg_with_space))+\\s*\\)(?<right_of_solver>.*\\))");

}
