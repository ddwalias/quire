#include "engutils_output.h"

namespace Engine {

std::ostream &Output::GetStream() {
    indent();
    return *m_stream;
}

void Output::IndentDown() {
    if (s_log) {
        m_indents.pop_back();
    }
}

void Output::Linesep(bool check_col) {
    if (s_log) {
        if (check_col && m_col == m_indents.back()) {
            return;
        }
        m_stream->put('\n');
        m_need_indent = true;
        m_col = m_indents.back();
    } else {
        m_stream->put(' ');
    }
}

void Output::Put(Substring<const char> str) {
    if (s_log) {
        indent();
    }
    m_stream->write(str.begin(), static_cast<std::streamsize>(str.size()));
    if (s_log) {
        m_col += str.size();
        m_need_indent = false;
    }
}

void Output::indent() {
    if (!s_log || !m_need_indent) {
        return;
    }
    const size_t col = m_indents.back();
    for (unsigned int i = 0; i < m_indents.back() / 8; ++i) {
        m_stream->put('\t');
    }
    m_stream->write("       ", static_cast<std::streamsize>(col % 8));
}

}  // namespace Engine
