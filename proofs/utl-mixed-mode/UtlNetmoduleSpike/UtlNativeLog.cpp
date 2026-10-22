#include <Windows.h>
#include <ostream>

template <typename T>
class Substring
{
public:
    T* begin;
    T* end;
    Substring(T* first, T* last) : begin(first), end(last) {}
};

namespace utl
{
    __declspec(dllimport) std::ostream& __cdecl Log(Substring<const char> name);
}

extern "C" __declspec(dllexport) void __cdecl UtlNativeLogUtf8(
    const char* nameBegin, const char* nameEnd,
    const char* messageBegin, const char* messageEnd,
    BOOL appendNewline)
{
    if (nameBegin == nullptr || nameEnd == nullptr || messageBegin == nullptr || messageEnd == nullptr
        || nameEnd < nameBegin || messageEnd < messageBegin)
        return;

    std::ostream& stream = utl::Log(Substring<const char>(nameBegin, nameEnd));
    for (auto cursor = messageBegin; cursor != messageEnd; ++cursor)
        stream.put(*cursor);
    if (appendNewline)
        stream.put('\n').flush();
}
