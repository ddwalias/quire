#pragma once

namespace Prolog {

// Abstract program. The destructor is the first virtual slot; the five pure virtual slots after it are not in the
// export list, and their names do not affect the emitted code.
class Program {
public:
    Program();
    Program(Program const &other);
    virtual ~Program();

    Program &operator=(Program const &other);

    virtual void Slot1() = 0;
    virtual void Slot2() = 0;
    virtual void Slot3() = 0;
    virtual void Slot4() = 0;
    virtual void Slot5() = 0;
};

// Owns the program it names. Its destructor and vtable wait on the slot-1 function, which is not yet mirrored.
class Ruleset {
public:
    virtual ~Ruleset();

    Program *GetProgram() const;
    Program *operator->() const;

private:
    Program *m_program;
};

class PObjParser {
public:
    PObjParser &operator=(PObjParser &&other);
    PObjParser &operator=(PObjParser const &other);
};

}
