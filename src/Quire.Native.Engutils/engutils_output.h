#pragma once

#include <deque>
#include <ostream>

template <typename Char>
class Substring {
public:
    Substring(Char *begin, Char *end) : begin_(begin), end_(end) {}
    Char *begin() const { return begin_; }
    Char *end() const { return end_; }
    size_t size() const { return static_cast<size_t>(end_ - begin_); }

private:
    Char *begin_;
    Char *end_;
};

namespace Engine {

class Output {
public:
    std::ostream &GetStream();
    void IndentDown();
    void Linesep(bool check_col);
    void Put(Substring<const char> str);

private:
    void indent();

    std::ostream *m_stream;
    std::deque<size_t> m_indents;
    size_t m_col;
    bool m_need_indent;

    static const bool s_log;
};

}  // namespace Engine
