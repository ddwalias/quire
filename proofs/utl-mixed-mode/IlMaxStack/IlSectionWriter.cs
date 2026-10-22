using System.Reflection.Metadata;
using System.Reflection.Metadata.Ecma335;
using System.Reflection.PortableExecutable;
using System.Text;

internal sealed class IlSectionWriter
{
    private readonly int _fileOffset;
    private readonly int _rva;
    private readonly int _capacity;
    public bool AlreadyPatched { get; }
    private int _used;

    private IlSectionWriter(int fileOffset, int rva, int capacity, bool alreadyPatched = false)
    {
        _fileOffset = fileOffset;
        _rva = rva;
        _capacity = capacity;
        AlreadyPatched = alreadyPatched;
    }

    public static (byte[] Bytes, IlSectionWriter Writer) Reserve(byte[] image, int capacity)
    {
        using var stream = new MemoryStream(image, writable: false);
        using var pe = new PEReader(stream);
        var headers = pe.PEHeaders;
        if (headers.SectionHeaders.Any(section => section.Name == ".ilnew"))
            return (image, new IlSectionWriter(0, 0, 0, alreadyPatched: true));
        var coffOffset = ReadInt32(image, 0x3c);
        var sectionHeaderOffset = checked(coffOffset + 4 + 20 + headers.CoffHeader.SizeOfOptionalHeader);
        var newSectionHeaderOffset = checked(sectionHeaderOffset + headers.CoffHeader.NumberOfSections * 40);
        var firstRawOffset = headers.SectionHeaders.Min(section => section.PointerToRawData);
        if (newSectionHeaderOffset + 40 > firstRawOffset || newSectionHeaderOffset + 40 > headers.PEHeader!.SizeOfHeaders)
            throw new BadImageFormatException("PE headers have no room for an additional IL section header");

        var fileAlignment = checked((int)headers.PEHeader.FileAlignment);
        var sectionAlignment = checked((int)headers.PEHeader.SectionAlignment);
        var rawSize = Align(capacity, fileAlignment);
        var rawOffset = Align(image.Length, fileAlignment);
        var lastSectionEnd = headers.SectionHeaders.Max(section =>
            checked(section.VirtualAddress + Math.Max(section.VirtualSize, section.SizeOfRawData)));
        var sectionRva = Align(lastSectionEnd, sectionAlignment);
        var expanded = new byte[checked(rawOffset + rawSize)];
        image.CopyTo(expanded, 0);

        Encoding.ASCII.GetBytes(".ilnew\0").CopyTo(expanded, newSectionHeaderOffset);
        WriteInt32(expanded, newSectionHeaderOffset + 8, capacity);
        WriteInt32(expanded, newSectionHeaderOffset + 12, sectionRva);
        WriteInt32(expanded, newSectionHeaderOffset + 16, rawSize);
        WriteInt32(expanded, newSectionHeaderOffset + 20, rawOffset);
        WriteInt32(expanded, newSectionHeaderOffset + 36, 0x40000040); // initialized, readable data

        WriteUInt16(expanded, coffOffset + 6, checked((ushort)(headers.CoffHeader.NumberOfSections + 1)));
        var optionalOffset = coffOffset + 4 + 20;
        WriteInt32(expanded, optionalOffset + 8,
            checked(ReadInt32(expanded, optionalOffset + 8) + rawSize));
        WriteInt32(expanded, optionalOffset + 56, Align(checked(sectionRva + capacity), sectionAlignment));
        return (expanded, new IlSectionWriter(rawOffset, sectionRva, capacity));
    }

    public void PlaceBody(byte[] image, MetadataReader reader, PEHeaders headers,
        MethodDefinitionHandle method, ReadOnlySpan<byte> body)
    {
        var offset = Align(_used, 4);
        if (offset + body.Length > _capacity)
            throw new InvalidOperationException("reserved IL section is full");
        body.CopyTo(image.AsSpan(_fileOffset + offset, body.Length));
        var rva = checked(_rva + offset);
        var rvaField = MethodDefinitionRvaOffset(image, headers, reader, method);
        WriteInt32(image, rvaField, rva);
        _used = offset + body.Length;
    }

    private static int MethodDefinitionRvaOffset(byte[] image, PEHeaders headers,
        MetadataReader reader, MethodDefinitionHandle method)
    {
        var metadataRva = headers.CorHeader!.MetadataDirectory.RelativeVirtualAddress;
        var metadataOffset = RvaToOffset(headers, metadataRva);
        var versionLength = ReadInt32(image, metadataOffset + 12);
        var cursor = Align(checked(metadataOffset + 16 + versionLength), 4);
        var streamCount = ReadUInt16(image, cursor + 2);
        cursor += 4;
        var tablesOffset = -1;
        for (var i = 0; i < streamCount; i++)
        {
            var streamOffset = ReadInt32(image, cursor);
            cursor += 8;
            var nameStart = cursor;
            while (image[cursor] != 0)
                cursor++;
            var name = Encoding.ASCII.GetString(image, nameStart, cursor - nameStart);
            cursor = Align(cursor + 1, 4);
            if (name is "#~" or "#-")
                tablesOffset = checked(metadataOffset + streamOffset);
        }
        if (tablesOffset < 0)
            throw new BadImageFormatException("metadata has no tables stream");

        var heapSizes = image[tablesOffset + 6];
        var valid = ReadUInt64(image, tablesOffset + 8);
        var rows = new int[64];
        cursor = tablesOffset + 24;
        for (var table = 0; table < rows.Length; table++)
        {
            if ((valid & (1UL << table)) == 0)
                continue;
            rows[table] = checked((int)ReadUInt32(image, cursor));
            cursor += 4;
        }

        var dataOffset = cursor;
        var methodTableOffset = dataOffset;
        for (var table = 0; table < 6; table++)
        {
            methodTableOffset = checked(methodTableOffset + rows[table] * TableRowSize(table, rows, heapSizes));
        }
        var methodRowSize = TableRowSize(6, rows, heapSizes);
        var row = MetadataTokens.GetRowNumber(method);
        if (row <= 0 || row > rows[6])
            throw new BadImageFormatException("method handle is outside the MethodDef table");
        return checked(methodTableOffset + (row - 1) * methodRowSize);
    }

    private static int TableRowSize(int table, int[] rows, byte heapSizes)
    {
        var stringIndex = (heapSizes & 0x1) != 0 ? 4 : 2;
        var guidIndex = (heapSizes & 0x2) != 0 ? 4 : 2;
        var blobIndex = (heapSizes & 0x4) != 0 ? 4 : 2;
        int Simple(int target) => rows[target] < 0x10000 ? 2 : 4;
        int Coded(int[] targets, int tagBits) => targets.Max(target => rows[target]) < (1 << (16 - tagBits)) ? 2 : 4;
        return table switch
        {
            0 => 2 + stringIndex + 3 * guidIndex,
            1 => Coded([0, 26, 35, 1], 2) + 2 * stringIndex,
            2 => 4 + 2 * stringIndex + Coded([2, 1, 27], 2) + Simple(4) + Simple(6),
            3 => Simple(4),
            4 => 2 + stringIndex + blobIndex,
            5 => Simple(6),
            6 => 4 + 2 + 2 + stringIndex + blobIndex + Simple(8),
            _ => throw new ArgumentOutOfRangeException(nameof(table))
        };
    }

    private static int RvaToOffset(PEHeaders headers, int rva)
    {
        foreach (var section in headers.SectionHeaders)
        {
            var length = Math.Max(section.VirtualSize, section.SizeOfRawData);
            if (rva >= section.VirtualAddress && rva < section.VirtualAddress + length)
                return checked(section.PointerToRawData + rva - section.VirtualAddress);
        }
        throw new BadImageFormatException($"RVA 0x{rva:x} is outside PE sections");
    }

    private static int ReadInt32(byte[] bytes, int offset) => BitConverter.ToInt32(bytes, offset);
    private static uint ReadUInt32(byte[] bytes, int offset) => BitConverter.ToUInt32(bytes, offset);
    private static ushort ReadUInt16(byte[] bytes, int offset) => BitConverter.ToUInt16(bytes, offset);
    private static ulong ReadUInt64(byte[] bytes, int offset) => BitConverter.ToUInt64(bytes, offset);
    private static void WriteInt32(byte[] bytes, int offset, int value) => BitConverter.TryWriteBytes(bytes.AsSpan(offset, 4), value);
    private static void WriteUInt16(byte[] bytes, int offset, ushort value) => BitConverter.TryWriteBytes(bytes.AsSpan(offset, 2), value);
    private static int Align(int value, int alignment) => checked((value + alignment - 1) / alignment * alignment);
}
