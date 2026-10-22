#nullable disable
#pragma warning disable

namespace Quire;

partial interface IVisitableFunc<T, R>
{
    R Accept(T visitor);
}
