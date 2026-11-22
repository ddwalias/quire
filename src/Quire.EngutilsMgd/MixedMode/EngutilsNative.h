#pragma once

#include <string>
#include <type_traits>

namespace ms
{
    template <typename T>
    struct Range
    {
        T* first;
        T* last;
        Range() = default;
        Range(T* f, T* l) : first(f), last(l) {}
    };
}

template <typename T>
class Substring : public ms::Range<T>
{
public:
    Substring(T* p, size_t n) : ms::Range<T>(p, p + n) {}
    Substring(T* s) : Substring(s, std::char_traits<std::remove_const_t<T>>::length(s)) {}
    Substring(std::basic_string<std::remove_const_t<T>>& s)
    {
        auto* p = s.data();
        this->first = p;
        this->last = p + s.size();
    }
    Substring(const std::basic_string<std::remove_const_t<T>>& s) : Substring(s.c_str(), s.size()) {}
    Substring(const ms::Range<T>& r) : ms::Range<T>(r) {}
};

template <class _Elem, class _Traits, class _Alloc>
std::basic_string<_Elem, _Traits, _Alloc> to_string(const std::basic_string<_Elem, _Traits, _Alloc>& rs)
{
    return std::basic_string<_Elem, _Traits, _Alloc>(rs);
}

namespace utl
{
    inline std::string ToStrng(System::Object^ o)
    {
        if (o == nullptr)
        {
            return std::string("<null>");
        }
        System::String^ s = o->ToString();
        if (s->Length > 0)
        {
            cli::array<System::Byte>^ bytes = System::Text::Encoding::UTF8->GetBytes(s);
            pin_ptr<System::Byte> ptr = &bytes[0];
            return std::string((const char*)ptr);
        }
        return std::string("");
    }
}
