#include "EngutilsMgd.h"
#include "EngutilsNative.h"

using namespace System;
using namespace System::Text;

__declspec(dllimport) bool parse_int(const char* text, unsigned int maxDigits, int& value);

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
        int stringLength = s->Length;
        for (int index = 0; index < stringLength; index++)
        {
            wchar_t c = s[index];
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
