#nullable disable
#pragma warning disable

namespace Quire;

partial interface IVisitable<T>
{
    void Accept(T visitor);
}
