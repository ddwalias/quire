# Mcd.MatVal core constructors, lifecycle and pointer NaN members

- **Original:** mplrun.dll 12.0.1.0, SHA-256 `15274355cb262d96ada1295d4949f7f30ee750926d31e117019671de6b3efb52`, MVID `17a0933b-4988-4c18-963d-563998321cc8`.
- **Reference:** ilspycmd 11.1.0.9782 (`tools/reference/reference.py`); inspection record `ev_c8ca49d8286456aa09639a00185113080927a4662dda8cb0c122b48acc2f6af8`, managed-member inspection.
- **Quire:** `src/Quire.Mpl/Mcd/MatVal.cs`; tests: `MatValDifferentialTests`.
- Contracts recorded before implementation. Units conversion is a representation cast, not physical scaling. Original unexpected mutations and indexing are intentional fidelity requirements.

| Member | Token | Contract | Confidence / limits | Status |
|---|---|---|---|---|
| Mcd.MatVal..ctor() | 0x06000481 | Parameterless constructor: delegates to this(null, null, 0, 0, 0, null, Flags.MatrixTag, 0). | Direct cases: parameterless construction and default state verification. Exact Release IL match. | Verified |
| Mcd.MatVal..ctor(i4) | 0x06000477 | Single-dimension constructor: delegates to this(r, 1, false, r). | Direct cases: positive and negative dimensions. Exact Release IL match. | Verified |
| Mcd.MatVal..ctor(i4, i4) | 0x06000478 | Two-dimension constructor: delegates to this(r, c, false, r). | Direct cases: rectangular dimensions. Exact Release IL match. | Verified |
| Mcd.MatVal..ctor(i4, i4, bool) | 0x06000479 | Sized constructor with imaginary flag: delegates to this(r, c, imag, r). | Direct cases: real and complex allocations. Exact Release IL match. | Verified |
| Mcd.MatVal..ctor(i4, i4, bool, i4) | 0x06000476 | Designated sized constructor: allocates real backing of size c * anr, conditional imaginary backing of size c * anr, initializes null pointer table, offset 0, and flags MatrixTag. | Direct cases: various shapes and stride configurations. Exact Release IL match. | Verified |
| Mcd.MatVal..ctor(r8[], r8[], i4, i4, i4, Hashtable, Flags, i4) | 0x0600047a | Full designated constructor: initializes m_dim to identity, assigns backing arrays, pointer table, offset, rows, cols, stride, and flags without validation. | Direct cases: raw buffer and flag wrapping. Exact Release IL match. | Verified |
| Mcd.MatVal..ctor(r8[], r8[], i4, i4, i4, Hashtable) | 0x0600047b | Wrapping constructor with pointer table: delegates to full ctor with Flags.MatrixTag and offset 0. | Direct cases: pointer table wrapping. Exact Release IL match. | Verified |
| Mcd.MatVal..ctor(r8[], r8[], i4, i4, i4) | 0x0600047c | Wrapping constructor with stride: delegates to full ctor with null table, Flags.MatrixTag and offset 0. | Direct cases: strided arrays. Exact Release IL match. | Verified |
| Mcd.MatVal..ctor(r8[], r8[], i4, i4) | 0x0600047d | Wrapping constructor: delegates to full ctor with anr = r, null table, Flags.MatrixTag and offset 0. | Direct cases: rectangular array wrapping. Exact Release IL match. | Verified |
| Mcd.MatVal..ctor(r8[], r8[], i4) | 0x0600047e | Column vector wrapping constructor: delegates to full ctor with c = 1, anr = r, null table, Flags.MatrixTag and offset 0. | Direct cases: column vector wrapping. Exact Release IL match. | Verified |
| Mcd.MatVal..ctor(r8[], r8[]) | 0x0600047f | Array wrapping constructor: delegates to full ctor with r = rarr.Length, c = 1, anr = rarr.Length, null table, Flags.MatrixTag and offset 0. | Direct cases: 1D array pair wrapping. Exact Release IL match. | Verified |
| Mcd.MatVal..ctor(r8[]) | 0x06000480 | Real array constructor: delegates to full ctor with null imaginary, r = rarr.Length, c = 1, anr = rarr.Length, null table, Flags.MatrixTag and offset 0. | Direct cases: 1D real array wrapping. Exact Release IL match. | Verified |
| Mcd.MatVal.get_Packed() | 0x06000433 | Tests whether Data.m_rows * Data.m_cols == m_rarr.Length. | Direct cases: packed and unpacked matrices. Exact Release IL match. | Verified |
| Mcd.MatVal.get_Immutable() | 0x06000460 | Tests CopyOnWrite flag bit (1). | Direct cases: mutable and immutable matrices. Exact Release IL match. | Verified |
| Mcd.MatVal.set_Immutable(bool) | 0x06000461 | Sets or clears CopyOnWrite flag bit (1) preserving other flag bits. | Direct cases: setting and clearing CopyOnWrite. Exact Release IL match. | Verified |
| Mcd.MatVal.get_Shared() | 0x06000462 | Tests Shared flag bit (4). | Direct cases: private and shared matrices. Exact Release IL match. | Verified |
| Mcd.MatVal.set_Shared(bool) | 0x06000463 | Sets or clears Shared flag bit (4) preserving other flag bits. | Direct cases: setting and clearing Shared. Exact Release IL match. | Verified |
| Mcd.MatVal.GetHashCode() | 0x06000474 | Returns Rows.GetHashCode() * 64 + Cols.GetHashCode(). | Direct cases: distinct shapes. Exact Release IL match. | Verified |
| Mcd.MatVal.ThePtrNan() | 0x0600052e | Returns double sentinel with all 64 bits set via raw pointer indexing. | Direct cases: bit-level double validation. Structure verified match. | Verified |
| Mcd.MatVal.IsPtrNan(r8&) | 0x0600052d | Checks whether upper 32 bits of pinned double reference equal 0xffffffff via raw pointer indexing. | Direct cases: PtrNan, normal NaN, and finite doubles. Exact Release IL match. | Verified |
| Mcd.MatVal.Index(i4) | 0x060004cc | Returns r + m_offset. | Direct cases: various offsets and row indices. Exact Release IL match. | Verified |
| Mcd.MatVal.Index(i4, i4) | 0x060004cd | Returns m_offset + r + Anr * c. | Direct cases: multidimensional indexing with strides. Exact Release IL match. | Verified |

## Test cases

Direct comparisons in `MatValDifferentialTests` invoke comprehensive suites across all 22 core constructors, flags, lifecycle, indexing, and pointer NaN members on both artifacts. Comparisons verify return values, exception behaviors, shape, stride, offset, flags, dimension, backing arrays, and bit-level pointer NaN behaviors across receivers and arguments. Conclusions are limited to the inputs and branches exercised in the current focused validation run.

## Structure mismatches observed in the focused run

None. All constructors, indexing methods, flag accessors, and `IsPtrNan` achieve 100% exact Release IL matching against Mathcad Prime 12.0.1.0 `mplrun.dll`. `ThePtrNan` achieves full opcode/structure parity with Mathcad.
