namespace Quire.Native.PtcEquationEditor;

internal partial class a7
{
    public string CleanValue(string value)
    {
        int num = 0;
        while (num < value.Length - 1)
        {
            if (value[num] != '0' || value[num + 1] == '.')
            {
                break;
            }

            num++;
        }

        return value.Substring(num);
    }

    public a7()
    {
    }
}
