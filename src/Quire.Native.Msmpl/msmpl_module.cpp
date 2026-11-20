#include "msmpl.h"

namespace Mpl {

bool IsModule(Substring<char const> sub) {
    try {
        return !GetModuleName(sub).empty();
    } catch (...) {
        return false;
    }
}

std::string GetModuleName(Substring<char const> sub) {
    return GetModuleAttribute(sub, attribute_name, true);
}

std::string GetModuleText(Substring<char const> sub) {
    return GetModuleAttribute(sub, attribute_text, true);
}

std::string GetModuleInterface(Substring<char const> sub) {
    return GetModuleAttribute(sub, attribute_interface, true);
}

bool IsModule(Substring<wchar_t const> sub) {
    try {
        return !GetModuleName(sub).empty();
    } catch (...) {
        return false;
    }
}

std::string GetModuleName(Substring<wchar_t const> sub) {
    return GetModuleAttribute(sub, attribute_name, true);
}

std::string GetModuleText(Substring<wchar_t const> sub) {
    return GetModuleAttribute(sub, attribute_text, true);
}

std::string GetModuleInterface(Substring<wchar_t const> sub) {
    return GetModuleAttribute(sub, attribute_interface, true);
}

}
