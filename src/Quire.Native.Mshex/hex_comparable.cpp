#include "hex_comparable.h"

// The original instantiates each base for Signature.
template struct Hex::equality_comparable<Hex::Signature>;
template struct Hex::less_than_comparable<Hex::Signature>;
template struct Hex::totally_ordered<Hex::Signature>;
