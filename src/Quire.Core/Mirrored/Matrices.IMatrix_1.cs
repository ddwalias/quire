#nullable disable
#pragma warning disable

namespace Quire.Matrices;

partial interface IMatrix<T>
{
    global::Quire.TableSize Size { get; }
    int Rows { get; }
    int Columns { get; }
    T this[int r, int c] { get; set; }
    T this[global::Quire.TableCoordinate position] { get; set; }
}
