#nullable disable
using System;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Documents;

namespace Quire.Serialization;

public static partial class TextRangeExtensionMethods
{
    public static void LoadWithCustomContext(this global::System.Windows.Documents.TextRange range, Stream packageStream, bool export = false) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static PackagePart GetXamlPart(Package package)
    {
        return package.GetParts().FirstOrDefault((PackagePart p) => (p.ContentType != null && p.ContentType.IndexOf("xaml", StringComparison.OrdinalIgnoreCase) >= 0) || p.Uri.ToString().EndsWith(".xaml", StringComparison.OrdinalIgnoreCase));
    }

    public static void LoadElementFromClipboard(this global::System.Windows.Documents.TextRange textRange, string pasteXaml)
    {
        using MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(pasteXaml));
        textRange.Load(stream, DataFormats.Xaml);
        textRange.Select(textRange.End, textRange.End);
    }

    public static void LoadElementFromFile(this global::System.Windows.Documents.TextRange textRange, object content)
    {
        Block firstInsertedBlock = null;
        Block lastInsertedBlock = null;
        if (content is Block block)
        {
            InsertBlock(textRange, block, ref firstInsertedBlock, ref lastInsertedBlock);
        }
        if (firstInsertedBlock != null && lastInsertedBlock != null)
        {
            textRange.Select(firstInsertedBlock.ElementStart, lastInsertedBlock.ElementEnd);
        }
    }

    public static void LoadElementToExport(this global::System.Windows.Documents.TextRange textRange, object content) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

    private static void InsertBlock(global::System.Windows.Documents.TextRange textRange, Block block, ref Block firstInsertedBlock, ref Block lastInsertedBlock)
    {
        TextPointer start = textRange.Start;
        BlockCollection blockCollection = null;
        FlowDocument flowDocument;
        Section section;
        Paragraph paragraph;
        FlowDocument flowDocument2;
        Section section2;
        Section section3;
        Block block2;
        flowDocument = start.Parent as FlowDocument;
        if (flowDocument != null)
        {
            blockCollection = flowDocument.Blocks;
        }
        else
        {
            section = start.Parent as Section;
            if (section != null)
            {
                blockCollection = section.Blocks;
            }
            else
            {
                paragraph = start.Parent as Paragraph;
                if (paragraph != null)
                {
                    flowDocument2 = paragraph.Parent as FlowDocument;
                    if (flowDocument2 != null)
                    {
                        blockCollection = flowDocument2.Blocks;
                    }
                    else
                    {
                        section2 = paragraph.Parent as Section;
                        if (section2 != null)
                        {
                            blockCollection = section2.Blocks;
                        }
                    }
                }
            }
        }
        if (blockCollection != null)
        {
            section3 = block as Section;
            if (section3 != null)
            {
                block2 = blockCollection.FirstBlock;
                foreach (Block item in section3.Blocks.ToList())
                {
                    section3.Blocks.Remove(item);
                    if (block2 != null)
                    {
                        blockCollection.InsertAfter(block2, item);
                        block2 = item;
                    }
                    else
                    {
                        blockCollection.Add(item);
                    }
                    if (firstInsertedBlock == null)
                    {
                        firstInsertedBlock = item;
                    }
                    lastInsertedBlock = item;
                }
            }
        }
    }

}
