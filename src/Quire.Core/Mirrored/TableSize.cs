#nullable disable
#pragma warning disable

namespace Quire;

partial struct TableSize
{
    public int Rows { get => _coordinate.First; set => _coordinate.First = value; }
    public int Columns { get => _coordinate.Second; set => _coordinate.Second = value; }
}
