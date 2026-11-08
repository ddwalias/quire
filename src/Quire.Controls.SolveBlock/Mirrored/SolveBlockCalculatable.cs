namespace Quire.Controls;

internal partial class SolveBlockCalculatable
{
    public static int FindMatchingParenthesis(int startParenPos, string str)
    {
        int nesting = 1;
        for (int i = startParenPos + 1; i < str.Length; i++)
        {
            if (str[i] == ')')
                nesting--;
            else if (str[i] == '(')
                nesting++;

            if (nesting == 0)
                return i;
        }

        return -1;
    }
}
