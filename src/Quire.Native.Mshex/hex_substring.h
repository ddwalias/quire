#pragma once

// A view of a character range, as Mathcad's native code passes text: a begin and an end pointer.
template <typename Char>
class Substring {
public:
    Substring(Char *begin, Char *end) : begin_(begin), end_(end) {}

private:
    Char *begin_;
    Char *end_;
};
