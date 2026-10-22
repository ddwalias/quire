#nullable disable
#pragma warning disable

namespace Quire;

partial struct TableCoordinate
{
    public int Row { get => _coordinate.First; set => _coordinate.First = value; }
    public int Column { get => _coordinate.Second; set => _coordinate.Second = value; }
}
