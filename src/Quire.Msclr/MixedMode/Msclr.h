#pragma once

#define WIN32_LEAN_AND_MEAN
#define NOMINMAX
#include <sdkddkver.h>
#include <cstddef>
#include <cstdint>
#include <string>

// A view of a character range, as Mathcad's native code passes text: a begin and an end pointer.
template <typename Char>
class Substring
{
public:
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

    class Heap;

    class Signature
    {
    public:
        __declspec(dllimport) Substring<const char> GetName() const;
        __declspec(dllimport) Substring<const char> GetType() const;

    private:
        std::uint16_t m_name_end;
        std::uint16_t m_module_start;
        std::string m_text;
    };

    class Module
    {
    public:
        struct State
        {
            void* m_vtable;
            void* m_unknown[2];
        };

        __declspec(dllimport) State* GetState() const;

    private:
        void* m_unknown[19];
    };
}

#pragma make_public(Hex::Module)

namespace Mpl {

    struct Table
    {
    };

    struct Loaded : Hex::Module::State
    {
        void* GetLoader() const { return m_loader; }
        ms::Range<const Hex::Signature> GetImportSignatures() const { return m_imports; }
        ms::Range<const Hex::Signature> GetExportSignatures() const { return m_exports; }

        Hex::Heap* m_heap;
        Table* m_table;
        void* m_loader;
        ms::Range<const Hex::Signature> m_imports;
        ms::Range<const Hex::Signature> m_exports;
        void* m_unknown[2];
    };

    Loaded* GetModuleState(const Hex::Module& module);

    inline System::String^ ToString(Substring<const char> text)
    {
        return gcnew System::String(text.begin(), 0, static_cast<int>(text.size()), System::Text::Encoding::UTF8);
    }

    template <typename T>
    T^ unpack(void* handle)
    {
        return dynamic_cast<T^>(System::Runtime::InteropServices::GCHandle::FromIntPtr(System::IntPtr(handle)).Target);
    }
}
