using System.Reflection.PortableExecutable;
using System.Text;

internal static class MetadataRootEditor
{
    private sealed record StreamEntry(string Name, int HeaderOffset, byte[] Content);

    public static byte[] AppendStandaloneSignatures(byte[] image, PEHeaders headers,
        IReadOnlyList<byte[]> signatures)
    {
        var metadataOffset = RvaToOffset(headers, headers.CorHeader!.MetadataDirectory.RelativeVirtualAddress);
        var metadataSize = headers.CorHeader.MetadataDirectory.Size;
        var root = image.AsSpan(metadataOffset, metadataSize).ToArray();
        var entries = ReadStreams(root);
        var tables = entries.Single(entry => entry.Name is "#~" or "#-");
        var blobs = entries.Single(entry => entry.Name == "#Blob");
        var expandedTables = tables.Content;
        var expandedBlobs = blobs.Content;
        foreach (var signature in signatures)
        {
            expandedTables = AddStandaloneSignatureRow(expandedTables, expandedBlobs.Length, out _);
            expandedBlobs = AppendBlob(expandedBlobs, signature);
        }
        var outputEntries = entries.Select(entry => entry.Name switch
        {
            "#~" or "#-" => entry with { Content = expandedTables },
            "#Blob" => entry with { Content = expandedBlobs },
            _ => entry
        }).ToArray();
        return WriteRoot(root, outputEntries);
    }

    private static List<StreamEntry> ReadStreams(byte[] root)
    {
        var versionLength = ReadInt32(root, 12);
        var position = Align4(16 + versionLength);
        var streamCount = ReadUInt16(root, position + 2);
        position += 4;
        var entries = new List<StreamEntry>(streamCount);
        for (var index = 0; index < streamCount; index++)
        {
            var offset = ReadInt32(root, position);
            var size = ReadInt32(root, position + 4);
            var nameStart = position + 8;
            var nameEnd = Array.IndexOf(root, (byte)0, nameStart);
            if (nameEnd < 0 || offset < 0 || size < 0 || offset + size > root.Length)
                throw new BadImageFormatException("invalid metadata stream header");
            var name = Encoding.ASCII.GetString(root, nameStart, nameEnd - nameStart);
            var content = root.AsSpan(offset, size).ToArray();
            entries.Add(new StreamEntry(name, position, content));
            position = Align4(nameEnd + 1);
        }
        return entries;
    }

    private static byte[] WriteRoot(byte[] oldRoot, StreamEntry[] entries)
    {
        var versionLength = ReadInt32(oldRoot, 12);
        var streamHeaderStart = Align4(16 + versionLength) + 4;
        var headerEnd = streamHeaderStart;
        foreach (var entry in entries)
        {
            var nameEnd = Array.IndexOf(oldRoot, (byte)0, entry.HeaderOffset + 8);
            headerEnd += Align4(nameEnd + 1) - entry.HeaderOffset;
        }
        var dataOffset = Align4(headerEnd);
        var offsets = new int[entries.Length];
        var cursor = dataOffset;
        for (var index = 0; index < entries.Length; index++)
        {
            cursor = Align4(cursor);
            offsets[index] = cursor;
            cursor = checked(cursor + entries[index].Content.Length);
        }

        var root = new byte[cursor];
        oldRoot.AsSpan(0, streamHeaderStart).CopyTo(root);
        var headerPosition = streamHeaderStart;
        for (var index = 0; index < entries.Length; index++)
        {
            var entry = entries[index];
            WriteInt32(root, headerPosition, offsets[index]);
            WriteInt32(root, headerPosition + 4, entry.Content.Length);
            var oldNameEnd = Array.IndexOf(oldRoot, (byte)0, entry.HeaderOffset + 8);
            var oldHeaderSize = Align4(oldNameEnd + 1) - entry.HeaderOffset;
            oldRoot.AsSpan(entry.HeaderOffset + 8, oldHeaderSize - 8)
                .CopyTo(root.AsSpan(headerPosition + 8));
            entry.Content.CopyTo(root.AsSpan(offsets[index]));
            headerPosition += oldHeaderSize;
        }
        return root;
    }

    private static byte[] AddStandaloneSignatureRow(byte[] oldTables, int blobOffset, out int rowSize)
    {
        if (oldTables.Length < 24)
            throw new BadImageFormatException("truncated metadata tables stream");
        var heapSizes = oldTables[6];
        var valid = ReadUInt64(oldTables, 8);
        const int standaloneSignatureTable = 17;
        if ((valid & (1UL << standaloneSignatureTable)) == 0)
            throw new BadImageFormatException("candidate has no StandaloneSig table");

        var rowCounts = new int[64];
        var countPosition = 24;
        for (var table = 0; table < 64; table++)
            if ((valid & (1UL << table)) != 0)
            {
                rowCounts[table] = checked((int)ReadUInt32(oldTables, countPosition));
                countPosition += 4;
            }

        var rowCountsBeforeStandalone = 0;
        for (var table = 0; table < standaloneSignatureTable; table++)
            if ((valid & (1UL << table)) != 0)
                rowCountsBeforeStandalone++;
        var rowCountPosition = 24 + rowCountsBeforeStandalone * 4;
        WriteUInt32(oldTables, rowCountPosition,
            checked((uint)(rowCounts[standaloneSignatureTable] + 1)));

        var tablesStart = countPosition;
        var insertionOffset = tablesStart;
        for (var table = 0; table <= standaloneSignatureTable; table++)
            insertionOffset = checked(insertionOffset + rowCounts[table] * RowSize(table, rowCounts, heapSizes));

        var blobIndexSize = (heapSizes & 0x04) != 0 ? 4 : 2;
        if (blobIndexSize == 2 && blobOffset >= 0x10000)
            throw new BadImageFormatException("adding a StandaloneSig would widen the Blob index");
        rowSize = blobIndexSize;
        var row = new byte[rowSize];
        if (blobIndexSize == 2)
            WriteUInt16(row, 0, checked((ushort)blobOffset));
        else
            WriteUInt32(row, 0, checked((uint)blobOffset));

        var tables = new byte[oldTables.Length + rowSize];
        oldTables.AsSpan(0, insertionOffset).CopyTo(tables);
        row.CopyTo(tables.AsSpan(insertionOffset));
        oldTables.AsSpan(insertionOffset).CopyTo(tables.AsSpan(insertionOffset + rowSize));
        return tables;
    }

    private static int RowSize(int table, int[] rows, byte heapSizes)
    {
        int tableIndex(int id) => rows[id] < 0x10000 ? 2 : 4;
        int stringIndex() => (heapSizes & 0x01) != 0 ? 4 : 2;
        int guidIndex() => (heapSizes & 0x02) != 0 ? 4 : 2;
        int blobIndex() => (heapSizes & 0x04) != 0 ? 4 : 2;
        int codedIndex(int tagBits, params int[] ids)
            => ids.Max(id => rows[id]) < (1 << (16 - tagBits)) ? 2 : 4;

        return table switch
        {
            0 => 2 + stringIndex() + 3 * guidIndex(),
            1 => codedIndex(2, 0, 26, 35, 1) + 2 * stringIndex(),
            2 => 4 + 2 * stringIndex() + codedIndex(2, 2, 1, 27) + tableIndex(4) + tableIndex(6),
            3 => tableIndex(4),
            4 => 2 + stringIndex() + blobIndex(),
            5 => tableIndex(6),
            6 => 8 + stringIndex() + blobIndex() + tableIndex(8),
            7 => tableIndex(8),
            8 => 4 + stringIndex(),
            9 => tableIndex(2) + codedIndex(2, 2, 1, 27),
            10 => codedIndex(3, 2, 1, 26, 6, 27) + stringIndex() + blobIndex(),
            11 => 2 + codedIndex(2, 4, 8, 23) + blobIndex(),
            12 => codedIndex(5, 6, 4, 1, 2, 8, 9, 10, 0, 14, 23, 20, 17, 26, 27, 32, 35, 38, 39, 40, 42, 44, 43)
                + codedIndex(3, 6, 10) + blobIndex(),
            13 => codedIndex(1, 4, 8) + blobIndex(),
            14 => 2 + codedIndex(2, 2, 6, 32) + blobIndex(),
            15 => 6 + tableIndex(2),
            16 => 4 + tableIndex(4),
            17 => blobIndex(),
            _ => throw new NotSupportedException($"unexpected table {table} before StandaloneSig")
        };
    }

    private static byte[] AppendBlob(byte[] oldBlob, byte[] payload)
    {
        var prefix = new List<byte>();
        WriteCompressedUInt(prefix, payload.Length);
        var blob = new byte[oldBlob.Length + prefix.Count + payload.Length];
        oldBlob.CopyTo(blob, 0);
        prefix.CopyTo(blob, oldBlob.Length);
        payload.CopyTo(blob.AsSpan(oldBlob.Length + prefix.Count));
        return blob;
    }

    private static void WriteCompressedUInt(List<byte> bytes, int value)
    {
        if ((uint)value <= 0x7f)
            bytes.Add((byte)value);
        else if ((uint)value <= 0x3fff)
        {
            bytes.Add((byte)((value >> 8) | 0x80));
            bytes.Add((byte)value);
        }
        else if ((uint)value <= 0x1fffffff)
        {
            bytes.Add((byte)((value >> 24) | 0xc0));
            bytes.Add((byte)(value >> 16));
            bytes.Add((byte)(value >> 8));
            bytes.Add((byte)value);
        }
        else
            throw new BadImageFormatException("Blob entry exceeds the compressed integer range");
    }

    private static int RvaToOffset(PEHeaders headers, int rva)
    {
        foreach (var section in headers.SectionHeaders)
        {
            var size = Math.Max(section.VirtualSize, section.SizeOfRawData);
            if (rva >= section.VirtualAddress && rva < section.VirtualAddress + size)
                return checked(section.PointerToRawData + rva - section.VirtualAddress);
        }
        throw new BadImageFormatException($"metadata RVA 0x{rva:x} is outside PE sections");
    }

    private static int Align4(int value) => (value + 3) & ~3;
    private static int ReadInt32(byte[] bytes, int offset) => BitConverter.ToInt32(bytes, offset);
    private static ushort ReadUInt16(byte[] bytes, int offset) => BitConverter.ToUInt16(bytes, offset);
    private static uint ReadUInt32(byte[] bytes, int offset) => BitConverter.ToUInt32(bytes, offset);
    private static ulong ReadUInt64(byte[] bytes, int offset) => BitConverter.ToUInt64(bytes, offset);
    private static void WriteInt32(byte[] bytes, int offset, int value) => BitConverter.TryWriteBytes(bytes.AsSpan(offset, 4), value);
    private static void WriteUInt16(byte[] bytes, int offset, ushort value) => BitConverter.TryWriteBytes(bytes.AsSpan(offset, 2), value);
    private static void WriteUInt32(byte[] bytes, int offset, uint value) => BitConverter.TryWriteBytes(bytes.AsSpan(offset, 4), value);
}
