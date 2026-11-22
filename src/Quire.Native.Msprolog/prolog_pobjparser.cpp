#include "prolog.h"

namespace Prolog {

PObjParser &PObjParser::operator=(PObjParser &&) {
    return *this;
}

PObjParser &PObjParser::operator=(PObjParser const &) {
    return *this;
}

}
