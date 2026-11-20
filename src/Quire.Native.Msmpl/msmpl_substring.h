#pragma once

template <typename Char>
class Substring {
public:
    Substring(Char *begin, Char *end) : begin_(begin), end_(end) {}

    Char *begin() const { return begin_; }
    Char *end() const { return end_; }

private:
    Char *begin_;
    Char *end_;
};
