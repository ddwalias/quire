#include "EngutilsMgd.h"
#include <string>

using namespace System;
using namespace System::Text;

__declspec(dllimport) bool parse_int(const char* text, unsigned int maxDigits, int& value);

template <class _Elem, class _Traits, class _Alloc>
std::basic_string<_Elem, _Traits, _Alloc> to_string(const std::basic_string<_Elem, _Traits, _Alloc>& rs)
{
    return std::basic_string<_Elem, _Traits, _Alloc>(rs);
}

namespace utl {
    std::string ToStrng(System::Object^ o)
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

namespace Quire { namespace Engine {

    int^ StringUtils::ParseInt(String^ s)
    {
        std::string str = utl::ToStrng(s);
        int value;
        bool flag = parse_int(to_string(str).c_str(), 9, value);
        if (flag)
        {
            return value;
        }
        return nullptr;
    }

    String^ StringUtils::MangleForXAML(String^ s)
    {
        StringBuilder^ stringBuilder = gcnew StringBuilder();
        int num2 = s->Length;
        for (int num = 0; num < num2; num++)
        {
            wchar_t c = s[num];
            switch (c)
            {
            default:
                stringBuilder->Append(c);
                break;
            case L'_':
                stringBuilder->Append("%_");
                break;
            case L'>':
                stringBuilder->Append("%gt");
                break;
            case L'<':
                stringBuilder->Append("%lt");
                break;
            case L'%':
                stringBuilder->Append("%%");
                break;
            case L' ':
                stringBuilder->Append(L'_');
                break;
            }
        }
        return stringBuilder->ToString();
    }

}}
