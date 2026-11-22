#include "prolog.h"

namespace Prolog {

Program *Ruleset::GetProgram() const {
    return m_program;
}

Program *Ruleset::operator->() const {
    return m_program;
}

}
