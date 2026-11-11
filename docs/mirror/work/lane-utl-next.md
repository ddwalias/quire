# Lane utl-next, WordData and WordChoice: utl.dll native exports (QUI-15)

Owner: utl-wordata worker. Branch: `feature/utl-wordata`. Project: `Quire.Native.Utl`.

## Matched exports

| Export | RVA | Source |
| --- | --- | --- |
| `WordData::WordData()` | 0xCB00 | `= default` |
| `WordData::WordData(WordData &&)` | 0xCBC0 | `= default` |
| `WordData::~WordData()` | 0xCB40 | `= default` |
| `WordData::operator=(const WordData &)` | 0xCC20 | `= default` |
| `WordData::operator=(WordData &&)` | 0xCC50 | `= default` |
| `WordData::Add(WordDatum *)` | 0xD850 | `data.push_front(datum)` |
| `WordData::Count()` | 0xDA20 | `data.size()` |
| `WordChoice::MatchChar(char, std::string, int)` | 0xD770 | `index >= 0 && index < size && text[index] == c` |

## How the bytes were reached

- **`WordData` is one `std::list<WordDatum *>`** (head node pointer at +0, size at +8). The special members are the list's:
  the default constructor allocates the 0x18-byte sentinel, the move constructor swaps with a fresh one, the destructor
  frees the nodes and then the sentinel, and the copy assignment is the list's `_Assign_unchecked` behind a
  `this != &other` test. All of them match as `= default`.
- **`Add` pushes at the front.** The new node is linked between `head` and `head->_Next` (`[rbx+8]`, `[rcx]`), which is
  `push_front`, not `push_back`.
- **`MatchChar` takes its string by value** and the callee destroys it, so the inlined `~basic_string` (size reset,
  capacity 15, NUL) ends the function.
- **Flags** for `utl_word.cpp` and `utl_worddata.cpp` (a per-file copy of the timer's, plus two):
  - `/EHa` (the `rcx`/`rdx` homing and the `nop` after the allocation) and `/Ob3` (inlines the string destructor into
    `MatchChar`; `/Ob2` leaves a call to `~basic_string`).
  - `/std:c++20` (the `list` push and the C++17 library differ in register choice).
  - `/GS-`: the original has no cookie in `MatchChar`.
  - `/D_CRTIMP2=__declspec(dllimport)`: the original reaches `std::_Xlength_error` through `call [__imp_...]`
    (`FF 15`) and keeps the throw block inline after the `jne`; the pinned headers define `_CRTIMP2` empty, which gives
    a thunk call and moves the throw block to the end of the function.
- **Internal targets** added to `docs/mirror/native/internal-targets.json`: `??2@YAPEAX_K@Z` (sized operator new, supplied
  by the static CRT, so `toolchain`), the `"list too long"` literal at 0x18E28, and the `list<WordDatum *>`
  `_Assign_unchecked` instance at 0xDDC0.

## Not matched (declared in `utl.h`, not in the `.def`)

The pinned compiler (19.51.36246) reproduces the following bodies, but only with inlining decisions that differ from the
original's; those exports are left out until a shape is found. The original objects were built by 19.51.35721 as well
(Rich header), the same situation as the parked `_Emplace_reallocate` in `lane-utl-position.md`.

- `WordChoice::GetSuggestions(const char *, list<WordDatum *>)` (0xD040): the loop, `push_back(WordChoice(datum,
  GetScore(...)))` and the final `sort(CompareScores)` match, but the original calls `list::_Sort` (0xDC20) out of line,
  and the pinned compiler inlines its first level at every `/Ob` setting (558 bytes against 353).
- `WordChoice::BestGuess`, `WordChoice::BestGuesses`, `WordData::GetSuggestions`, `WordData::BestGuesses`: byte-exact
  against the original when `GetSuggestions` and the list copy constructor are reachable (`BestGuess` needs
  `if (threshold > front.score) return "";`, which is the original's `comisd threshold, score; jbe` shape). They cannot
  be landed alone because they call `GetSuggestions`.
- `WordChoice::GetScore` (0xD1B0) is written (exact match +1.0, neighbour match +0.5, containment ratio with a +0.25
  prefix bonus, `max(containment, matched / length)`), but the original out-of-lines `_Construct` and inlines
  `MatchChar` at the first call site only; the pinned compiler gives 1524 (`/Ob1`) to 2326 (`/Ob3`) bytes against 1464.
- `WordData::BestGuess` (66 bytes) needs `WordChoice::BestGuess` out of line, i.e. its own translation unit, and the list
  copy constructor (0xDAF0) stays out of line only when it has several callers (four in the original).
- `WordData::WordData(const WordData &)`: `= default` inlines the list copy (302 bytes against 28) until another caller
  of the list copy constructor exists.
