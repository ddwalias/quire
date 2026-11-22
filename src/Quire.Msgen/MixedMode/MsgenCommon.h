#pragma once

#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <sdkddkver.h>
#include <cstddef>
#include <cstdint>
#include <cstring>

template <typename Char>
class Substring
{
public:
    Substring() : m_begin(nullptr), m_end(nullptr) {}
    Substring(Char* begin, Char* end) : m_begin(begin), m_end(end) {}
    Substring(Char* begin, std::size_t len) : m_begin(begin), m_end(begin + len) {}
    Substring(Char* text) : Substring(text, std::strlen(text)) {}

    Char* begin() const { return m_begin; }
    Char* end() const { return m_end; }
    std::size_t size() const { return m_end - m_begin; }

private:
    Char* m_begin;
    Char* m_end;
};

namespace ms {

    template <typename T>
    class Range
    {
    public:
        T* begin() const { return m_begin; }
        T* end() const { return m_end; }
        std::size_t size() const { return m_end - m_begin; }
        T& operator[](std::size_t index) const { return m_begin[index]; }

    private:
        T* m_begin;
        T* m_end;
    };
}

namespace Hex {

    class Tag
    {
    public:
        __declspec(dllimport) unsigned int GetHandle() const;

    private:
        unsigned int m_handle;
    };
}

namespace gen {

    System::String^ ToString(Substring<const char> text);

}

namespace Quire { namespace gen {

    enum Key {};
    enum Decoration {};
    enum CallConv
    {
        CallConv_Static = 0,
        CallConv_Instance = 1,
        CallConv_Default = 2,
    };
    enum DataTypeImplementationCategory {};
    enum ftype {};

}}
