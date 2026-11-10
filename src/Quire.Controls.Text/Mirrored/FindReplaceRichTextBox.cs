#nullable disable
#pragma warning disable

using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;

namespace Quire;

public partial class FindReplaceRichTextBox
{

    private void PreventTextWrapping()
    {
        Document.PageWidth = 1000;
    }

    public void GetSubscriptPositions(List<Pair<int, int>> subscriptPositions)
    {
        subscriptPositions.Clear();
        BuildSubscriptPositions(TextRange, subscriptPositions);
    }

    private void RemoveAdditionalParagraphs()
    {
        if (Document == null)
        {
            return;
        }

        if (Document.Blocks.Count > 1)
        {
            if (!(Document.Blocks.FirstBlock is Paragraph))
            {
                return;
            }

            foreach (Paragraph paragraph in Document.Blocks.Skip(1).OfType<Paragraph>().ToList())
            {
                Document.Blocks.Remove(paragraph);
            }
        }
    }

    private void InsertPlainText(string formattedText, TextPointer insertPosition)
    {
        Selection.Text = string.Empty;
        Run run = new Run(formattedText, insertPosition);
        CaretPosition = run.ContentEnd;
    }

    private void PasteFormattedContent(string formattedText, string dataFormat)
    {
        if (dataFormat == global::System.Windows.DataFormats.Xaml || dataFormat == global::System.Windows.DataFormats.Rtf)
        {
            SetFormattedContent(formattedText, dataFormat, CurrentProofingLanguage, null);
            foreach (global::Quire.Controls.Text.SuperscriptRun run in global::Quire.Wpf.TextRangeExtension.EnumerateRunsInRange(TextRange).OfType<global::Quire.Controls.Text.SuperscriptRun>())
            {
                global::Quire.Controls.Text.CustomScriptRunBase.ToggleRangeText(new global::System.Windows.Documents.TextRange(run.ContentStart, run.ContentEnd), global::Quire.Controls.Text.ScriptState.None, double.NaN);
            }
        }
    }
}
