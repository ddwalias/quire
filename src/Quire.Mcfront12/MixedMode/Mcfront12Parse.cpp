#include <stdio.h>
#include <string>
#include <vcclr.h>

// sscanf is the original's unchecked CRT call.
#pragma warning(disable: 4996)

namespace utl
{
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
namespace Engine {

    unsigned int ParseHex(System::String^ ss)
    {
        std::string s = utl::ToStrng(ss);
        unsigned int value = 0;
        sscanf(s.c_str(), "0x%x", &value);
        return value;
    }

}

#pragma comment(linker, "/include:?ParseHex@Engine@@$$FYMIPE$AAVString@System@@@Z")