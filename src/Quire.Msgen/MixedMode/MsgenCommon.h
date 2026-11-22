#pragma once

#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <sdkddkver.h>
#include <cstddef>
#include <cstdint>
#include <cstring>

namespace ms {

    template <typename T>
    class Range
    {
    public:
        Range() = default;
        Range(T* begin, T* end) : m_begin(begin), m_end(end) {}

        T* begin() const { return m_begin; }
        T* end() const { return m_end; }
        std::size_t size() const { return m_end - m_begin; }
        T& operator[](std::size_t index) const { return m_begin[index]; }

    protected:
        T* m_begin;
        T* m_end;
    };
}

template <typename Char>
class Substring : public ms::Range<Char>
{
public:
    Substring() : ms::Range<Char>() {}
    Substring(ms::Range<Char> range) : ms::Range<Char>(range) {}
    explicit Substring(Char& ch) : ms::Range<Char>(&ch, &ch + 1) {}
    Substring(Char* begin, Char* end) : ms::Range<Char>(begin, end) {}
    Substring(Char* begin, std::size_t len) : ms::Range<Char>(begin, begin + len) {}
    Substring(Char* text) : Substring(text, std::strlen(text)) {}
};

namespace Hex {

    class Tag
    {
    public:
        __declspec(dllimport) unsigned int GetHandle() const;

    private:
        unsigned int m_handle;
    };

    // Match Mathcad's Hex::OnError(Hex::Error(code) << arg) temporary-to-ref idiom under /W4 /WX.
#pragma warning(disable: 4239)
    class Error
    {
    public:
        __declspec(dllimport) Error(unsigned int code);
        __declspec(dllimport) ~Error();

    private:
        std::int64_t m_data[8];
    };

    __declspec(dllimport) Error& operator<<(Error& err, const char* text);
    __declspec(dllimport) Error& operator<<(Error& err, Substring<const char> text);
    __declspec(dllimport) __declspec(noreturn) void OnError(Error& err);
}

namespace Quire { namespace gen {
    ref class IlAsm;
}}

namespace gen {

    struct AstInfo
    {
        // MSVC groups same-name virtual overloads in reverse declaration order in the vtable.
        virtual const char* Name(void*) = 0;            // vtable offset 16
        virtual const char* Name(void**) = 0;           // vtable offset 8
        virtual const char* Name(void***) = 0;          // vtable offset 0
        virtual const char* ModuleName(void**) = 0;     // vtable offset 24
        virtual std::size_t GetSpan(void***) = 0;       // vtable offset 32
        virtual std::size_t GetTag(void**) = 0;         // vtable offset 40
        virtual void*** GetDataType(void**) = 0;        // vtable offset 48
        virtual std::size_t Arity(void**) = 0;          // vtable offset 56
        virtual void** GetConstructor(void***, std::size_t) = 0; // vtable offset 64
        virtual unsigned int Exported(void*) = 0;       // vtable offset 72
        virtual std::size_t Slot80(void*) = 0;          // vtable offset 80
        virtual Substring<const char> GetType(void**) = 0; // vtable offset 96
        virtual Substring<const char> GetType(void*) = 0;  // vtable offset 88
    };

    System::String^ ToString(Substring<const char> text);

}

[returnvalue: System::Runtime::InteropServices::MarshalAs(System::Runtime::InteropServices::UnmanagedType::U1)]
bool IsDyn(System::String^ TyName);

[returnvalue: System::Runtime::InteropServices::MarshalAs(System::Runtime::InteropServices::UnmanagedType::U1)]
bool IsTuple(System::String^ TyName);

[returnvalue: System::Runtime::InteropServices::MarshalAs(System::Runtime::InteropServices::UnmanagedType::U1)]
bool IsStrVal(System::String^ TyName);

namespace Quire { namespace gen {

    enum Key {};
    enum Decoration {};
    enum CallConv
    {
        CallConv_Static = 0,
        CallConv_Instance = 1,
        CallConv_Default = 2,
    };
    enum DataTypeImplementationCategory
    {
        DTIC_Box = 0,
        DTIC_Null = 1,
        DTIC_Bool = 2,
        DTIC_Enum = 3,
        DTIC_Unknown = 4,
        DTIC_Maybe = 5,
        DTIC_Record = 6,
        DTIC_Untagged = 7,
        DTIC_Tagged = 8,
        DTIC_Void = 9,
    };
    enum ftype
    {
        ftype_Delegate = 0,
        ftype_Function = 1,
    };

}}
