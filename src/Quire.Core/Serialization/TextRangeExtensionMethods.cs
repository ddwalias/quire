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

    private static void InsertBlock(global::System.Windows.Documents.TextRange textRange, Block block, ref Block firstInsertedBlock, ref Block lastInsertedBlock) => throw global::Quire.Mirror.MirrorStub.NotImplemented();

}
