#include "EngutilsMgd.h"

namespace Quire { namespace Engine {

    String^ Tags::StripTag(String^ s)
    {
        int num = s->LastIndexOf(':');
        return (num == -1) ? s : s->Substring(0, num);
    }

    String^ Tags::GetTag(String^ s)
    {
        if (s != nullptr)
        {
            int num = s->LastIndexOf(':');
            if (num != -1)
                return s->Substring(num + 1);
        }
        return nullptr;
    }

    bool Tags::EqualsNoTags(Object^ x, Object^ y)
    {
        if (x != nullptr && y != nullptr)
        {
            String^ text = x->ToString();
            int num = text->LastIndexOf(':');
            String^ obj = (num == -1) ? text : text->Substring(0, num);
            String^ yWithoutTag = y->ToString();
            int yTagIndex = yWithoutTag->LastIndexOf(':');
            String^ yNameWithoutTag = (yTagIndex == -1) ? yWithoutTag : yWithoutTag->Substring(0, yTagIndex);
            return obj->Equals(yNameWithoutTag);
        }
        return x == y;
    }

    String^ Tags::StripFontTag(String^ s)
    {
        if (s != nullptr && s->StartsWith("mc__2D_FONT"))
        {
            int num = s->IndexOf('_', 11);
            if (num != -1)
            {
                String^ text = s->Substring(num + 4);
                return "mc_" + text;
            }
        }
        return s;
    }

    String^ Tags::EncodeRegionID(int id, String^ tag)
    {
        if (tag == nullptr)
            throw gcnew Exception("null tag");

        if (id >= 0 && id <= 65535)
        {
            if (tag->Length > 1 && tag[0] == '0' && tag[1] == 'x')
                tag = tag->Substring(2);
            if (tag->Length > 3 && tag[0] == '0' && tag[1] == '0' && tag[2] == '0' && tag[3] == '0')
                tag = tag->Substring(4);
            if (tag->Length != 4)
                throw gcnew Exception("bad tag: " + tag);

            for (int num = 0; num < 4; num++)
            {
                Char c = tag[num];
                if (!is_hex(c))
                    throw gcnew Exception("bad tag: " + tag);
            }

            System::Text::StringBuilder^ stringBuilder = gcnew System::Text::StringBuilder(10);
            stringBuilder->Append("0x");
            for (int idDigitIndex = 0; idDigitIndex < 4; idDigitIndex++)
            {
                int idDigit = (id >> 12) & 0xF;
                stringBuilder->Append(to_hex(idDigit));
                id <<= 4;
            }
            for (int tagIndex = 0; tagIndex < 4; tagIndex++)
                stringBuilder->Append(to_hex(tag[tagIndex]));
            return stringBuilder->ToString();
        }
        throw gcnew Exception("bad region ID: " + id);
    }

    wchar_t Tags::to_hex(wchar_t c)
    {
        if ((unsigned int)(c - 'a') <= 5)
            return (wchar_t)(c - 32);
        return c;
    }

    wchar_t Tags::to_hex(int d)
    {
        if (d < 10)
            return (wchar_t)(d + 48);
        return (wchar_t)(d + 55);
    }

    bool Tags::is_hex(wchar_t c)
    {
        return (c >= '0' && c <= '9') || (c >= 'A' && c <= 'F') || (c >= 'a' && c <= 'f');
    }

}}
