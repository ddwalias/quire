using System;

internal static class InlineHelpers
{
    internal static void WriteFour(string format, object a, object b, object c, object d)
    {
        char[] commandSpecialChars = new char[] { '\\', '"', '\'', '\t', ' ' };
        char[] charMappings = new char[] {
            'e', '∈', 'h', 'ℏ', 'o', '°', 'p', 'π', 'w', 'ω',
            'A', '∀', 'C', 'ℂ', 'E', '∃', 'O', 'Ω', 'Q', 'ℚ',
            'R', 'ℝ', 'V', '∇', 'Y', '¥', 'Z', 'ℤ', '$', '¤',
            '8', '∞', '#', '≠', '<', '≤', '>', '≥', '^', '∧',
            'v', '∨', '!', '¬', '~', '≈', '=', '≡' };
        _ = commandSpecialChars.Length + charMappings.Length;
        _ = string.Format(format, a, b, c, d);
        Console.WriteLine(format, a, b, c, d);
        TextReader reader = new StringReader(string.Empty);
        reader.Dispose();
    }
}
