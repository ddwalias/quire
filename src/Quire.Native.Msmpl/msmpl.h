#pragma once

#include <string>
#include <streambuf>
#include "msmpl_substring.h"

namespace Hex {
    class Interpreter;
}

namespace Mpl {

    enum module_attribute {
        attribute_name = 0,
        attribute_text = 1,
        attribute_interface = 2
    };

    // Imported from msclr.dll
    __declspec(dllimport) std::string GetModuleAttribute(Substring<char const> sub, module_attribute attr, bool flag);
    __declspec(dllimport) std::string GetModuleAttribute(Substring<wchar_t const> sub, module_attribute attr, bool flag);

    // Module functions
    bool IsModule(Substring<char const> sub);
    std::string GetModuleName(Substring<char const> sub);
    std::string GetModuleText(Substring<char const> sub);
    std::string GetModuleInterface(Substring<char const> sub);
    std::string GetModuleDependencies(Substring<char const> sub);

    bool IsModule(Substring<wchar_t const> sub);
    std::string GetModuleName(Substring<wchar_t const> sub);
    std::string GetModuleText(Substring<wchar_t const> sub);
    std::string GetModuleInterface(Substring<wchar_t const> sub);
    std::string GetModuleDependencies(Substring<wchar_t const> sub);

    bool IsModule(std::basic_streambuf<char, std::char_traits<char>>& sb);
    std::string GetModuleName(std::basic_streambuf<char, std::char_traits<char>>& sb);
    std::string GetModuleDependencies(std::basic_streambuf<char, std::char_traits<char>>& sb);

    Hex::Interpreter* NewInterpreter();

}
